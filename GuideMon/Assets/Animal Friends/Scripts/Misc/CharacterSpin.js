#pragma strict

var sceneCamera : Transform;

var speed : float = 25.0;

var bias : float = 1.0;

private var facingCamera : float;

function Start () {

}

function Update () {
	facingCamera = Mathf.Abs(Quaternion.Dot(transform.rotation, sceneCamera.rotation));
	
	bias = Mathf.Max(bias, 0.01);
	
	transform.Rotate(Vector3.up * speed * Time.deltaTime * (facingCamera + (1 / bias)));
}