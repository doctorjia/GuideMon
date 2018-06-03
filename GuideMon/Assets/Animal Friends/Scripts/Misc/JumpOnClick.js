#pragma strict

var sceneCamera : Camera;

private var groundY : float;
private var ySpeed : float;
static var gravity : float = 0.3;
static var jumpSpeed : float = 0.03;

function Start () {
	groundY = transform.position.y;
}

function Update () {

	if(transform.position.y > groundY){
		ySpeed -= gravity * Time.deltaTime;
		transform.position.y += ySpeed;
	}
		
	if(transform.position.y <= groundY){
		transform.position.y = groundY;
		ySpeed = 0.0;
	}

	
	if(Input.GetMouseButtonDown(0)){
		var ray : Ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
		var hit : RaycastHit;
		if(Physics.Raycast(ray, hit)){
			if(hit.collider == GetComponent.<Collider>()){
				ySpeed = jumpSpeed;
				transform.position.y = groundY + jumpSpeed;
			}
		}
	}
}