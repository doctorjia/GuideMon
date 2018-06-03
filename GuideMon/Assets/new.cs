using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    this.transform.Rotate(Vector3.up * 1, Space.Self);
        this.transform.Translate(Vector3.forward * 1, Space.Self);

    }
}





        