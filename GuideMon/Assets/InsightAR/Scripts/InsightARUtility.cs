using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using InsightAR.Internal;

namespace InsightAR
{
	
	public enum InsightARPlaneAnchorAlignment
	{
		HorizontalUp,
		HorizontalDown,
		Vertical,
		Unknown
	}

	public enum InsightARAnchorType
	{
		Plane,
		UserHitTest,
		Marker_2D,
		Object_3D
	}



	public struct InsightARSettings
	{
		public string configPath;
		public string appKey;
		public string appSecret;
	}

    public class InsightARUtility
    {       

		public static void SetColumn(Matrix4x4 mat, int i, InsightARVector4 ivect)
		{
			Vector4 vect = new Vector4(ivect.x, ivect.y, ivect.z, ivect.w);
			mat.SetColumn(i, vect);
		}

		/// <summary>
		/// 从4*4 矩阵获取pos 
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="matrix">Matrix.</param>
		public static Vector3 GetPosition(Matrix4x4 matrix)
		{
			Vector3 position = matrix.GetColumn(3); 
			return position;
		}

		/// <summary>
		/// 从mat 获取四元数  
		/// </summary>
		/// <returns>The rotation.</returns>
		/// <param name="matrix">Matrix.</param>
		public static Quaternion GetRotation(Matrix4x4 matrix)
		{
			Quaternion rotation = QuaternionFromMatrix(matrix);
			return rotation;
		}

		/// <summary>
		/// 4*4 转到四元数 
		/// </summary>
		/// <returns>The from matrix.</returns>
		/// <param name="m">M.</param>
		static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			// Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
			Quaternion q = new Quaternion();
			q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2; 
			q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2; 
			q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2; 
			q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2; 
			q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
			q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
			q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
			return q;
		}

		public static InsightARPlaneAnchor GetPlaneAnchorFromAnchorData(InsightARAnchorData anchor)
		{
			InsightARPlaneAnchor arPlaneAnchor = new InsightARPlaneAnchor();
			arPlaneAnchor.identifier = string.Copy( anchor.identifier);
			#if UNITY_ANDROID
			if (InsightARNative.isUseHWAR()) {
				float[] tran = new float[]{ anchor.center.x, anchor.center.y, anchor.center.z };
				float[] quat = new float[]{anchor.rotation.x,anchor.rotation.y,anchor.rotation.z,anchor.rotation.w};
				Matrix4x4 glWorld_T_glLocal =	Matrix4x4.TRS (
					new Vector3 (tran [0], tran [1], tran [2]),
					new Quaternion (quat [0], quat [1], quat [2], quat [3]),Vector3.one
				);
				Matrix4x4 unityWorld_T_glWorld = Matrix4x4.Scale (new Vector3 (1, 1, -1));
				Matrix4x4 unityWorld_T_unityLocal = unityWorld_T_glWorld * glWorld_T_glLocal * unityWorld_T_glWorld.inverse;
				arPlaneAnchor.center = unityWorld_T_unityLocal.GetColumn (3);

				arPlaneAnchor.rotation = Quaternion.LookRotation(unityWorld_T_unityLocal.GetColumn(2),unityWorld_T_unityLocal.GetColumn(1));
				arPlaneAnchor.extent = new Vector3 (anchor.extent.x,1.0f,anchor.extent.z);

			} else 
			#endif
			{
				Matrix4x4 matrix = new Matrix4x4 ();
				matrix.SetColumn (0, new Vector4 (anchor.transform.column0.x, anchor.transform.column0.y, anchor.transform.column0.z, anchor.transform.column0.w));
				matrix.SetColumn (1, new Vector4 (anchor.transform.column1.x, anchor.transform.column1.y, anchor.transform.column1.z, anchor.transform.column1.w));
				matrix.SetColumn (2, new Vector4 (anchor.transform.column2.x, anchor.transform.column2.y, anchor.transform.column2.z, anchor.transform.column2.w));
				matrix.SetColumn (3, new Vector4 (anchor.transform.column3.x, anchor.transform.column3.y, anchor.transform.column3.z, anchor.transform.column3.w));
				arPlaneAnchor.rotation = GetRotation (matrix);
				arPlaneAnchor.extent = new Vector3 (anchor.extent.x, anchor.extent.y, anchor.extent.z);
				#if UNITY_ANDROID
					arPlaneAnchor.center = new Vector3 (anchor.center.x, anchor.center.y, anchor.center.z);
            	#elif UNITY_IOS
               		arPlaneAnchor.center =  GetPosition(matrix);
               		//Debug.Log(arPlaneAnchor.center.ToString("f3"));
            	#endif
				arPlaneAnchor.isValid = anchor.isValid;
			}
			return arPlaneAnchor;
		}

		public static InsightARUserHitAnchor GetUserHitAnchorFromAnchorData(InsightARAnchorData anchor)
		{
			InsightARUserHitAnchor arUserHitAnchor = new InsightARUserHitAnchor();
			arUserHitAnchor.identifier = string.Copy( anchor.identifier);
			#if UNITY_ANDROID
			if (InsightARNative.isUseHWAR ()) {

				float[] cent = new float[]
				{ anchor.center.x, anchor.center.y, anchor.center.z };
				float[] quat = new float[]
				{ anchor.rotation.x, anchor.rotation.y, anchor.rotation.z, anchor.rotation.w };
				Vector3 camPos;
				Quaternion camRot;
				InsightARMath.Cvt_GLPose_UnityPose(cent,quat,out camPos,out camRot);
				
				arUserHitAnchor.position = camPos;
				arUserHitAnchor.rotation = camRot;

			} else 
			#endif
			{
				arUserHitAnchor.position = new Vector3 (anchor.transform.column3.x, anchor.transform.column3.y, anchor.transform.column3.z);
			}
			arUserHitAnchor.isValid = anchor.isValid;

			return arUserHitAnchor;
		}
    }
}
