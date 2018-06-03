
using UnityEngine;

namespace InsightAR.Internal
{
	public struct InsightARVector2
	{
		public float x;
		public float y;
	}

	public struct InsightARVector3
	{
		public float x;
		public float y;
		public float z;
	}

	public struct InsightARVector4
	{
		public float x;
		public float y;
		public float z;
		public float w;

		public string ToString(){
			return "("+x+","+y+","+z+","+w+")";
		}

		public string ToString(string format){
			return "("+x.ToString(format)+","+y.ToString(format)+","+z.ToString(format)+","+w.ToString(format)+")";
		}
	}

	public struct InsightARMatrix4x4
	{
		public InsightARVector4 column0;
		public InsightARVector4 column1;
		public InsightARVector4 column2;
		public InsightARVector4 column3;
	}

	public class InsightARMath{
		

		public static void Cvt_GLPose_UnityPose(float[] cent,float[] quat,out Vector3 position,out Quaternion rotation){
			
			Matrix4x4 glWorld_T_glLocal =
				Matrix4x4.TRS (
					new Vector3 (cent [0], cent [1], cent [2]),
					new Quaternion (quat [0], quat [1], quat [2], quat [3]), Vector3.one
				);
			Matrix4x4 unityWorld_T_glWorld = Matrix4x4.Scale (new Vector3 (1, 1, -1));
			Matrix4x4 unityWorld_T_unityLocal = unityWorld_T_glWorld * glWorld_T_glLocal * unityWorld_T_glWorld.inverse;
			position = unityWorld_T_unityLocal.GetColumn (3);
			rotation = Quaternion.LookRotation (unityWorld_T_unityLocal.GetColumn (2), unityWorld_T_unityLocal.GetColumn (1));

		}
	}



}
