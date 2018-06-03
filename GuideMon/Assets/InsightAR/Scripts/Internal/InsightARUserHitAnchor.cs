using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InsightAR
{
    /// <summary>
    /// Insight AR plane anchor For Unity
    /// </summary>
	public struct InsightARUserHitAnchor
    {
        public string identifier;
        public Vector3 position;
		public Quaternion rotation;
		public int isValid;
    }
}
