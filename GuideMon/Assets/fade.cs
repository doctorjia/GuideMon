using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fade : MonoBehaviour {
    Camera mainCamera;// import camera
    private GameObject gameObject;
    public bool Enter = false;
	public bool Visit = false;
	public Color targetCol;
	private float DistanceVisit = 7f;
	private float DistanceEnter = 5f;
	List<Material> mList = new List<Material>();
	Color currCol;
	Renderer[] renderers;
	// Use this for initialization
	void Start () {
        gameObject = GameObject.Find("Main Camera");
        renderers = GetComponentsInChildren<Renderer>();
		foreach (Renderer cop in renderers){
			foreach (Material mat in cop.materials){
				mList.Add(mat);
			}
		}
	}

	// Update is called once per frame
	void Update () {

        float distance = Vector3.Distance(gameObject.transform.position, this.transform.position);

        if ((DistanceVisit-1)<= distance && distance <= DistanceVisit)
            Visit = true;

        if ((DistanceEnter-1)<= distance && distance <= DistanceEnter)
            Enter = true;



        if (!Visit){
			targetCol = new Color(0,0,0,1);
		}

        else if (Enter)
        {
            targetCol = new Color(0, 0, 0, 1);
        }


        else
        {
            targetCol = new Color(1, 1, 1, 1);
        }
		foreach (Material mat in mList){
			currCol = mat.GetColor("_TintColor");
			currCol = Color.Lerp(currCol, targetCol, 0.1f);
			mat.SetColor("_TintColor", currCol);
		}
	}
}
