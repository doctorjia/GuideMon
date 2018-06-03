using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace InsightAR.Internal
{
    public struct InsightARCameraPose
    {
        //CV右手坐标系
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public float[] rotation;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] translation;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] quaternion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] center;

        //OpenGL坐标系
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] quaternion_opengl;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] center_opengl;

        //U3D坐标系

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] quaternion_u3d;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] center_u3d;

		//只使用陀螺仪计算出来的相机pose
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public float[] quaternion_imu;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] center_imu;
    }
}
