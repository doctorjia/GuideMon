#pragma strict

var eulerAngles : Vector3;
var relativeTo : Space;

function Start () {

}

function Update () {
	transform.Rotate(eulerAngles * Time.deltaTime, relativeTo);
}