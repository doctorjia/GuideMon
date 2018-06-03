using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

namespace InsightAR.Internal
{
    
    public static class InsightARNative
    {

        #if UNITY_ANDROID
		private const string Dll_Name = "AREngine";
		#elif UNITY_IOS
        private const string Dll_Name = "__Internal";
        #endif

        public delegate void Internal_FrameUpdate(InsightARResult result,IntPtr pHandler);

        public delegate void Internal_AnchorAdded(InsightARAnchorData anchorData,IntPtr pHandler);

        public delegate void Internal_AnchorUpdated(InsightARAnchorData anchorData,IntPtr pHandler);

        public delegate void Internal_AnchorRemoved(InsightARAnchorData anchorData,IntPtr pHandler);

        public delegate void Internal_FaceUpdate(IntPtr jsonRes,IntPtr pHandler);

		#if UNITY_ANDROID || UNITY_IOS
        [DllImport(Dll_Name)]
        public static extern void iarRegisterAppKey(string key, string secret);

        [DllImport(Dll_Name)]
        public static extern void iarInit(
            string path, 
            Internal_FrameUpdate frameUpdate, 
            Internal_AnchorAdded anchorAdded,
            Internal_AnchorUpdated anchorUpdated, 
            Internal_AnchorRemoved anchorRemoved, 
            Internal_FaceUpdate faceUpdate,
            IntPtr pHandler);

        [DllImport(Dll_Name)]
        public static extern void iarReset(string configFilePath = "");

        [DllImport(Dll_Name)]
        public static extern void iarStop();

        [DllImport(Dll_Name)]
        public static extern InsightARTextureHandles iarGetVideoTextureHandles();

        [DllImport(Dll_Name)]
        public static extern InsightARAnchorData  iarGetLastHitTestResult(InsightARVector2 point);
	
        [DllImport(Dll_Name)]
        public static extern bool iarSupport();

        [DllImport(Dll_Name)]
        public static extern int iarIsARKIT();
   

        #if UNITY_ANDROID
        [DllImport(Dll_Name)]
        public static extern bool isUseHWAR(); 

        [DllImport(Dll_Name)]
        public static extern void getHWARProjectionMatrix(
        [In,Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 16)]
        float[] matrix, float nearPlane, float farPlane);

        [DllImport(Dll_Name)]
        public static extern void getHWDisplayUVCoords(
        [In,Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 8)]
        float[] uvCoords);

        [DllImport(Dll_Name)]
        public static extern IntPtr iarUpdateCameraTexture();

        [DllImport(Dll_Name)]
        public static extern bool checkARCoreServiceInstalled(StringBuilder fatalResult, int length);	


#elif UNITY_IOS
        public static  bool isUseHWAR()
        {
            return false;
        }

        public static  void getHWARProjectionMatrix(
            [In,Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 16)]
            float[] matrix, float nearPlane, float farPlane)
        {

        }
        #endif
       
		#endif


      
    }
}

