#pragma strict

var translation : Vector3;
var relativeTo : Space;

function Start () {

}

function Update () {
	transform.Translate(translation * Time.deltaTime, relativeTo);
}