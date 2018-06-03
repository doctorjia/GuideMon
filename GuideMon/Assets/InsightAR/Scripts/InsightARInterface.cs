using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using AOT;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using InsightAR.Internal;

namespace InsightAR
{

    public class InsightARInterface
    {

        private static InsightARInterface m_Interface;

        public static InsightARInterface GetInterface()
        {
            if (m_Interface == null)
            {
                m_Interface = new InsightARInterface();
            }

            return m_Interface;
        }


        public InsightARState CurrentState
        {
            get
            { 
                return (InsightARState)trackResult.state;
            }
        }

        private Camera mARCamera;
        private InsightARSettings mCurrentSetting;
        private bool mARCameraConfiged = false;
        private Material m_BackgroundMaterial = null;

        public static Action<InsightARPlaneAnchor> planeAddedAction;
        public static Action<InsightARPlaneAnchor> planeUpdatedAction;
        public static Action<InsightARPlaneAnchor> planeRemovedAction;

        private static InsightARResult trackResult = new InsightARResult();

        private static List<InsightARPlaneAnchor> listAnchorsToUpdate;
        private static List<InsightARPlaneAnchor> listAnchorsToAdd;
        private static List<InsightARPlaneAnchor> listAnchorsToRemove;
        private float[] uvcoords = new float[8]{ 0f, 1f, 0f, 0f, 1f, 1f, 1f, 0f };

        #if UNITY_IOS
        private Texture2D _videoTextureY = null;
        private Texture2D _videoTextureCbCr = null;
        #elif UNITY_ANDROID
		private Texture2D _videoTextureRGBA = null;
		#endif

        private CommandBuffer m_VideoCommandBuffer;

        [MonoPInvokeCallback(typeof(InsightARNative.Internal_FrameUpdate))]
        private static void onFrameUpdate(InsightARResult insightResult, IntPtr pHandler)
        {
			InsightARState arState = (InsightARState)insightResult.state;
//			Debug.Log("-ar- onFrameUpdate:"+insightResult.state);
            if (arState == InsightARState.Initing || arState == InsightARState.Init_Fail
                || arState == InsightARState.Detect_Fail || arState == InsightARState.Track_Fail
                || arState == InsightARState.Track_Stop || arState == InsightARState.Uninitialized)
            {
                trackResult.state = insightResult.state;
            }
            else
            {
                trackResult = insightResult;
            }

        }

        [MonoPInvokeCallback(typeof(InsightARNative.Internal_AnchorAdded))]
        private static void onAnchorAdded(InsightARAnchorData anchor, IntPtr pHandler)
        {
            if (anchor.type == InsightARAnchorType.Plane)
            {
                InsightARPlaneAnchor arPlaneAnchor = InsightARUtility.GetPlaneAnchorFromAnchorData(anchor);
                listAnchorsToAdd.Add(arPlaneAnchor);
            }
        }

        [MonoPInvokeCallback(typeof(InsightARNative.Internal_AnchorUpdated))]
        private static void onAnchorUpdated(InsightARAnchorData anchor, IntPtr pHandler)
        {
            if (anchor.type == InsightARAnchorType.Plane)
            {
                InsightARPlaneAnchor arPlaneAnchor = InsightARUtility.GetPlaneAnchorFromAnchorData(anchor);
                listAnchorsToUpdate.Add(arPlaneAnchor);
            }
        }

        [MonoPInvokeCallback(typeof(InsightARNative.Internal_AnchorRemoved))]
        private static void onAnchorRemoved(InsightARAnchorData anchor, IntPtr pHandler)
        {
            if (anchor.type == InsightARAnchorType.Plane)
            {
                InsightARPlaneAnchor arPlaneAnchor = InsightARUtility.GetPlaneAnchorFromAnchorData(anchor);
                listAnchorsToRemove.Add(arPlaneAnchor);
            }
        }

        [MonoPInvokeCallback(typeof(InsightARNative.Internal_FaceUpdate))]
        private static void onFaceUpdate(IntPtr jsonRes, IntPtr pHandler)
        {
        }


        #region PUBLIC_API

        public void StartAR(InsightARSettings settings)
        {
            checkARSupport(settings);
            mCurrentSetting = settings;
            mARCameraConfiged = false;
            registerInsightAR(settings);
            startInsightAR(settings);	
        }

		public void StopAR()
		{ 
			#if UNITY_ANDROID || UNITY_IOS
			if (mARCameraConfiged)
			{
				if (mARCamera != null)
				{
					CommandBuffer[] cbs = mARCamera.GetCommandBuffers(CameraEvent.BeforeForwardOpaque);
					if (cbs != null && cbs.Length > 0)
					{
						mARCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
					}
				}
			}
			listAnchorsToAdd.Clear();
			listAnchorsToUpdate.Clear();
			listAnchorsToRemove.Clear();
			InsightARNative.iarStop();
			#endif
		}

        public void ResetAR(string path)
        {
			#if UNITY_ANDROID || UNITY_IOS
			Debug.Log("-ar- ResetAR:"+path);
            InsightARNative.iarReset(path);
			#endif
        }

        public void Update()
        {

            UpdateInsightAR();
            updateInsightARPlanes();
        }

        public void UpdateARBackground()
        {
			#if UNITY_ANDROID
            if (InsightARNative.isUseHWAR())
            {
                updateHWARBackground();
            }
            else
			#endif
            {
                updateInsightARBackground();
            }
        }

        public void SetupCamera(Camera camera)
        {
            mARCamera = camera;
        }

        public void SwitchCamera()
        {
//            InsightARNative.iarSwitchCameras();
        }

		public InsightARUserHitAnchor GetHitTestResult(Touch touchPoint)
		{
			InsightARUserHitAnchor anchor = new InsightARUserHitAnchor()
			{
				identifier = "",
				position = Vector3.zero,
				isValid = 0,
			};
			#if UNITY_ANDROID || UNITY_IOS
			if (CurrentState != InsightARState.Tracking)
			{
				return anchor;
			}
			InsightARVector2 poi = new InsightARVector2
			{
				x = touchPoint.position.x,
				y = touchPoint.position.y
			};
			#if UNITY_ANDROID
			if (InsightARNative.isUseHWAR())
			{
				poi.y = Screen.height - touchPoint.position.y;
			}
			else
			#endif
			{
				var screenPosition = Camera.main.ScreenToViewportPoint(touchPoint.position);
				poi.x = screenPosition.x;
				poi.y = screenPosition.y;

			}
			InsightARAnchorData anchorData = InsightARNative.iarGetLastHitTestResult(poi);
			anchor = InsightARUtility.GetUserHitAnchorFromAnchorData(anchorData);
			#endif
			return anchor;
		}

		public InsightARUserHitAnchor GetHitTestResult(float pointX, float pointY)
		{
			InsightARUserHitAnchor anchor = new InsightARUserHitAnchor()
			{
				identifier = "",
				position = Vector3.zero,
				isValid = 0,
			};
			#if UNITY_ANDROID || UNITY_IOS
			if (CurrentState != InsightARState.Tracking)
			{
				return anchor;
			}
			InsightARVector2 poi = new InsightARVector2();
			#if UNITY_ANDROID
			if (InsightARNative.isUseHWAR())
			{
				poi.y = Screen.height * pointY;
				poi.x = Screen.width * pointX;
			}
			else
			#endif
			{
				var screenPosition = Camera.main.ScreenToViewportPoint(new Vector3(pointX, pointY, 0f));
				poi.x = screenPosition.x;
				poi.y = screenPosition.y;
			}
			InsightARAnchorData anchorData = InsightARNative.iarGetLastHitTestResult(poi);
			anchor = InsightARUtility.GetUserHitAnchorFromAnchorData(anchorData);
			#endif
			return anchor;
		}


        #endregion //PUBLIC_API




        #region InsightAR

        private void configARCamera()
        {
            m_VideoCommandBuffer = new CommandBuffer();
            #if UNITY_IOS
            m_BackgroundMaterial = new Material(Shader.Find("Unlit/ARCameraShader"));
            #elif UNITY_ANDROID
			if(InsightARNative.isUseHWAR()){
				m_BackgroundMaterial = new Material (Shader.Find ("HuaweiAR/ARBackground"));
			}
			else{
				m_BackgroundMaterial = new Material (Shader.Find ("VideoPlaneNoLight"));	
			}
            #endif

            m_VideoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, m_BackgroundMaterial);
            mARCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);

            mARCameraConfiged = true;
        }

        private void checkARSupport(InsightARSettings settings)
        {
			#if UNITY_ANDROID || UNITY_IOS
			//check InsightAR support
			if (!InsightARNative.iarSupport())
			{

			}
			#endif
        }

        private void updateHWARBackground()
        {
            #if UNITY_ANDROID
//			InsightARState arState = (InsightARState)trackResult.state;

//			if (arState < InsightARState.Init_OK || arState >= InsightARState.Track_Stop) {
//				return;
//			}
			InsightARTextureHandles handles = InsightARNative.iarGetVideoTextureHandles();

			if (handles.textureY == System.IntPtr.Zero)
			{
				return;
			}

			if (!mARCameraConfiged)
			{
				configARCamera ();
			}
			Resolution currentResolution = Screen.currentResolution;

			if(_videoTextureRGBA == null || _videoTextureRGBA.GetNativeTexturePtr().ToInt32() != handles.textureY.ToInt32()){
				_videoTextureRGBA = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height,
					TextureFormat.RGBA32, false, false, (System.IntPtr)handles.textureY);
				_videoTextureRGBA.filterMode = FilterMode.Bilinear;
				_videoTextureRGBA.wrapMode = TextureWrapMode.Repeat;
			}
			_videoTextureRGBA.UpdateExternalTexture(handles.textureY);
			m_BackgroundMaterial.SetTexture("_MainTex", _videoTextureRGBA);

			const string topLeft = "_UvTopLeftRight";
			const string botLeft = "_UvBottomLeftRight";
			InsightARNative.getHWDisplayUVCoords (uvcoords);

			m_BackgroundMaterial.SetVector (topLeft, new Vector4 (uvcoords [0], uvcoords [1], uvcoords [4], uvcoords [5]));
			m_BackgroundMaterial.SetVector (botLeft, new Vector4 (uvcoords [2], uvcoords [3], uvcoords [6], uvcoords [7]));
            #endif

        }

        private void updateInsightARBackground()
        {
			
            if (CurrentState < InsightARState.Init_OK || CurrentState >= InsightARState.Track_Stop)
            {
                return;
            }
			#if UNITY_ANDROID || UNITY_IOS
            #if UNITY_ANDROID
			GL.IssuePluginEvent(InsightARNative.iarUpdateCameraTexture(),1);
            #endif
            InsightARTextureHandles handles = InsightARNative.iarGetVideoTextureHandles();
//			Debug.Log ("-ar- updateInsightARBackground:handles :" + handles.textureY);
            if (handles.textureY == System.IntPtr.Zero)
            {
                return;
            }
            #if UNITY_IOS
            if (handles.textureCbCr == System.IntPtr.Zero)
            {
                return;
            }
            #endif
            if (!mARCameraConfiged)
            {
                configARCamera();
            }
            Resolution currentResolution = Screen.currentResolution;

            #if UNITY_IOS
            // Texture Y
            _videoTextureY = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height,
                TextureFormat.R8, false, false, (System.IntPtr)handles.textureY);
            _videoTextureY.filterMode = FilterMode.Bilinear;
            _videoTextureY.wrapMode = TextureWrapMode.Repeat;
            _videoTextureY.UpdateExternalTexture(handles.textureY);

            // Texture CbCr
            _videoTextureCbCr = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height,
                TextureFormat.RG16, false, false, (System.IntPtr)handles.textureCbCr);
            _videoTextureCbCr.filterMode = FilterMode.Bilinear;
            _videoTextureCbCr.wrapMode = TextureWrapMode.Repeat;
            _videoTextureCbCr.UpdateExternalTexture(handles.textureCbCr);

            m_BackgroundMaterial.SetTexture("_textureY", _videoTextureY);
            m_BackgroundMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);

            #elif UNITY_ANDROID
			if(_videoTextureRGBA == null 
				|| _videoTextureRGBA.GetNativeTexturePtr().ToInt32() != handles.textureY.ToInt32())
			{
				_videoTextureRGBA = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height,
					TextureFormat.RGBA32, false, false, (System.IntPtr)handles.textureY);
				_videoTextureRGBA.filterMode = FilterMode.Bilinear;
				_videoTextureRGBA.wrapMode = TextureWrapMode.Repeat;
				m_BackgroundMaterial.SetTexture("_MainTex", _videoTextureRGBA);
			}
			_videoTextureRGBA.UpdateExternalTexture(handles.textureY);
            #endif

            int isPortrait = 0;
            float rotation = 0;
            if (Screen.orientation == ScreenOrientation.Portrait)
            {
                rotation = -90;
                isPortrait = 1;
            }
            else if (Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                rotation = 90;
                isPortrait = 1;
            }
            else if (Screen.orientation == ScreenOrientation.LandscapeRight || Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                isPortrait = 0;
            } 

            Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, rotation), Vector3.one);
            m_BackgroundMaterial.SetMatrix("_TextureRotation", m);

            float imageAspect = (float)trackResult.param.width / (float)trackResult.param.height;
            float screenAspect = (float)currentResolution.width / (float)currentResolution.height;
            float ratio = screenAspect > 1 ? imageAspect / screenAspect : imageAspect * screenAspect;

            float s_ShaderScaleX = 1.0f;
            float s_ShaderScaleY = 1.0f;
            if (isPortrait == 1)
            {
                if (ratio < 1)
                {
                    s_ShaderScaleX = ratio;
                }
                else if (ratio > 1)
                {
                    s_ShaderScaleY = 1f / ratio;
                }
            }
            else if (isPortrait == 0)
            {
                if (ratio < 1f)
                {
                    s_ShaderScaleY = ratio;
                }
                else if (ratio > 1f)
                {
                    s_ShaderScaleX = 1f / ratio;
                }
            }

            m_BackgroundMaterial.SetFloat("_texCoordScaleX", s_ShaderScaleX);
            m_BackgroundMaterial.SetFloat("_texCoordScaleY", s_ShaderScaleY); 
            m_BackgroundMaterial.SetInt("_isPortrait", isPortrait); 
			#endif
        }

        private InsightARPlaneAnchor DuplicatePlane(InsightARPlaneAnchor anchor)
        {
            InsightARPlaneAnchor temp = new InsightARPlaneAnchor()
            {
                identifier = anchor.identifier,
                rotation = anchor.rotation,
                center = anchor.center,
                extent = anchor.extent,
                isValid = anchor.isValid
            };
            return temp;
        }

        private void UpdateInsightAR()
        {

            InsightARState arState = (InsightARState)trackResult.state; 

            if (arState <= InsightARState.Uninitialized)
            {
                return;
            }
            if (arState == InsightARState.Init_Fail)
            {
                StopAR();
                return;
            }
			#if UNITY_ANDROID
			if (InsightARNative.isUseHWAR ()) {
				float[] arr = new float[16];
				InsightARNative.getHWARProjectionMatrix(arr, mARCamera.nearClipPlane, mARCamera.farClipPlane);

				Vector4[] cols = new Vector4[4];
				for (int i = 0; i < 4; i++)
				{
					cols[i].x = arr[i * 4 + 0];
					cols[i].y = arr[i * 4 + 1];
					cols[i].z = arr[i * 4 + 2];
					cols[i].w = arr[i * 4 + 3];
				}
			
				Matrix4x4 mat = Matrix4x4.identity;
				mat.SetColumn(0, cols[0]);
				mat.SetColumn(1, cols[1]);
				mat.SetColumn(2, cols[2]);
				mat.SetColumn(3, cols[3]);
				mARCamera.projectionMatrix = mat;
			
			}
			else
			#endif
			{
				if (arState > InsightARState.Uninitialized && mARCamera.fieldOfView != trackResult.param.fov [1]) {
					mARCamera.fieldOfView = trackResult.param.fov [1];
				}
			}
			if (arState == InsightARState.Initing || arState == InsightARState.Init_Fail
				|| arState == InsightARState.Detect_Fail || arState == InsightARState.Track_Fail
				|| arState == InsightARState.Track_Stop || arState == InsightARState.Uninitialized)
				return;
			#if UNITY_ANDROID
			if (InsightARNative.isUseHWAR())
			{
				Vector3 camPos;
				Quaternion camRot;
				InsightARMath.Cvt_GLPose_UnityPose(
					trackResult.camera.center,trackResult.camera.quaternion,out camPos,out camRot);

				trackResult.camera.center_u3d[0] = camPos.x;
				trackResult.camera.center_u3d[1] = camPos.y;
				trackResult.camera.center_u3d[2] = camPos.z;
				trackResult.camera.quaternion_u3d[0] = camRot.x;
				trackResult.camera.quaternion_u3d[1] = camRot.y;
				trackResult.camera.quaternion_u3d[2] = camRot.z;
				trackResult.camera.quaternion_u3d[3] = camRot.w;

				mARCamera.transform.position = camPos;
				mARCamera.transform.rotation = camRot;
			}
			else
			#endif
			{
				mARCamera.transform.localPosition = new Vector3(
					trackResult.camera.center_u3d[0], 
					trackResult.camera.center_u3d[1], 
					trackResult.camera.center_u3d[2]);
				mARCamera.transform.localRotation = new Quaternion(
					trackResult.camera.quaternion_u3d[0],
					trackResult.camera.quaternion_u3d[1],
					trackResult.camera.quaternion_u3d[2],
					trackResult.camera.quaternion_u3d[3]
				);
			}
        }

        private void registerInsightAR(InsightARSettings settings)
        {
			#if UNITY_ANDROID || UNITY_IOS
            InsightARNative.iarRegisterAppKey(settings.appKey, settings.appSecret);
			#endif
        }

        private void startInsightAR(InsightARSettings settings)
        {
			#if UNITY_ANDROID || UNITY_IOS
            listAnchorsToUpdate = new List<InsightARPlaneAnchor>();
            listAnchorsToAdd = new List<InsightARPlaneAnchor>();
            listAnchorsToRemove = new List<InsightARPlaneAnchor>();
            InsightARNative.iarInit(settings.configPath, onFrameUpdate, onAnchorAdded, onAnchorUpdated, onAnchorRemoved, onFaceUpdate, IntPtr.Zero);
			#endif
        }

        private void updateInsightARPlanes()
        {
            foreach (InsightARPlaneAnchor tPlane in listAnchorsToAdd)
            {
                if (planeAddedAction != null)
                    planeAddedAction(tPlane);
            }
            listAnchorsToAdd.Clear();

            foreach (InsightARPlaneAnchor tPlane in listAnchorsToUpdate)
            {
                if (planeUpdatedAction != null)
                    planeUpdatedAction(tPlane);
            }
            listAnchorsToUpdate.Clear();

            foreach (InsightARPlaneAnchor tPlane in listAnchorsToRemove)
            {
                if (planeRemovedAction != null)
                    planeRemovedAction(tPlane);
            }
            listAnchorsToRemove.Clear();
        }

        #endregion //InsightAR


    }
}