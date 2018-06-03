#pragma strict

var firstAnimation : AnimationClip;
var secondAnimation : AnimationClip;

var blend : float;

function Start () {

}

function Update () {
	blend = Mathf.Clamp01(blend);
	
	GetComponent.<Animation>().Blend(firstAnimation.name, blend, 0.05);
	GetComponent.<Animation>().Blend(secondAnimation.name, 1 - blend, .05);
}