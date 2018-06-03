#pragma strict

function Start () {
	UnityEngine.Cursor.visible = false;
}

function Update () {
	if(Input.GetKeyDown(KeyCode.Escape)){
		UnityEngine.Cursor.visible = true;
	}
}