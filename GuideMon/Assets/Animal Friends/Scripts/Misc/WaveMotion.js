#pragma strict

var speed : float = 1.0;
var multiplier : float = 1.0;
var offset : float = 0.0;

private var startingRotation : Quaternion;
private var waveAngle : float;


function Start () {
	startingRotation = transform.rotation;
}

function Update () {
	waveAngle = Mathf.Sin(Time.time * speed + offset) * multiplier;
	transform.rotation = startingRotation;
	transform.RotateAround(transform.position, Vector3.forward, waveAngle); 
}