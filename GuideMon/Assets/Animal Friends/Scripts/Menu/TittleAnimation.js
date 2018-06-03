#pragma strict

private var letters : Transform[];

private var letterStartPosition : Vector3 [];

var waveLength : float = 1.0;
var waveSpeed : float =  1.0;
var waveAmplitude : float = 1.0;

function Start () {
	letters = GetComponentsInChildren.<Transform>();
	
	letterStartPosition = new Vector3[letters.Length];
	
	for (var i = 0; i < letters.Length; i++){
		letterStartPosition[i] = letters[i].position;
	}
}

function Update () {
	waveLength = Mathf.Max(waveLength, 0.01);
	for(var i = 0; i < letters.Length; i++){
		letters[i].position = letterStartPosition[i] + Vector3.up * (Mathf.Sin(Time.time * waveSpeed +  (letters[i].position.x  / waveLength) ) * waveAmplitude);
	}
}