using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace InsightAR.Internal
{
    /// <summary>
    /// From Native Plugin 
    /// </summary>
    public struct InsightARAnchorData
    {
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string identifier;
		public InsightARAnchorType type;
		public InsightARMatrix4x4 transform;
        public InsightARPlaneAnchorAlignment alignment;
		public InsightARVector4 center;
		public InsightARVector4 rotation;
		public InsightARVector4 extent;
		public int isValid;
		public InsightARMatrix4x4 corners;
    }
}
