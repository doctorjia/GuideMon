#pragma strict

private var animationStart : float;

private var currentFrame : int;

private var animationTime : float;

private var textureOffset : float;

var animationSpeed : float = 1.0;

static var maxFrame : int = 5;



function Start () {

}

function Update () {
	animationTime = Time.time - animationStart;
	
	currentFrame  = Mathf.FloorToInt( Mathf.Clamp(animationTime * animationSpeed,0 ,maxFrame) );
	
	textureOffset = (1.0 / (maxFrame + 1)) * currentFrame;
	
	GetComponent.<Renderer>().material.SetTextureOffset("_MainTex", Vector2(textureOffset,0));
	
}

function SetAnimationStart(time : float){
	animationStart = time;
	currentFrame = 0;
}