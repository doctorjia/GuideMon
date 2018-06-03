using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InsightAR
{
    /// <summary>
    /// Insight AR plane anchor For Unity
    /// </summary>
    public struct InsightARPlaneAnchor
    {
        public string identifier;
        public Vector3 center;
        public Vector3 extent;
		public Quaternion rotation;
		public int isValid;
    }
}
