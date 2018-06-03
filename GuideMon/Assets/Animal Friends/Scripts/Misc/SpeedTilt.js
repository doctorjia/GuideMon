#pragma strict

var multiplier : float = 1.0;
var tiltDamp : float = .1;

private var startingRotation : Quaternion;
private var currentTilt : float;
private var targetTilt : float;
private var velocityTilt : float;

private var currentXspeed : float;
private var previousXPosition : float;

function Start () {
	startingRotation = transform.rotation;
}

function Update () {
	currentXspeed = transform.position.x - previousXPosition;
	targetTilt = Mathf.Clamp(currentXspeed * multiplier,-90,90);
	currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, velocityTilt, tiltDamp);
	previousXPosition = transform.position.x;
	transform.rotation = startingRotation;
	transform.RotateAround(transform.position, Vector3.forward, currentTilt); 
}
