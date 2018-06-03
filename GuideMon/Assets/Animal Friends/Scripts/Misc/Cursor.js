#pragma strict

var sceneCamera : Camera;

var offset : Vector2;

var defaultTexture : Texture;
var clickTexture : Texture;

var clickImageDuration : float = 0.05;

private var lastClickTime : float;

function Start () {

}

function Update () {
	transform.position = sceneCamera.ScreenToWorldPoint(Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.0)) + sceneCamera.transform.TransformDirection(offset.x, offset.y,0);
	
	if(Input.GetMouseButtonDown(0)){
		lastClickTime = Time.time;
	}
	
	if(Time.time > lastClickTime + clickImageDuration){
		GetComponent.<Renderer>().material.SetTexture("_MainTex", defaultTexture);
	}
	else{
		GetComponent.<Renderer>().material.SetTexture("_MainTex", clickTexture);
	}
}