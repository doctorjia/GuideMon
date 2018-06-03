using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Math;


public class Walk_rot : MonoBehaviour
{
    public float i=0;
    public float moveSpeed = 0.5f;
    public float rotateSpeed = 0.1f;
    public float start = 0;
    private float DistanceClose = 3;
    public float followspeed=2f;
    private float DistanceStopwalk = 2;
    private float DistanceRotate = 1;
	  private float DegreesPerSecond = 10f;
	  private float DegreesAmount = 270.0f;
	  private float totalRotation = 0;
	  private Vector3 offset;
    public Vector3 IniLocation = new Vector3(0,0,0);
    Camera mainCamera;// import camera
    Animator animator;//this animator

    private GameObject gameObject;
    public GameObject Owl;

    // Use this for initialization
    void Start()
    {

        Debug.Log("hello unity");
        gameObject = GameObject.Find("Main Camera");
        Owl = GameObject.Find("Owl");
        animator = GetComponent<Animator>();//获取当前对象的Animator
        //Owl.transform.position=IniLocation;
    }
    void rotate(){
        print(DegreesAmount);
        float currentAngle = transform.rotation.eulerAngles.y;
        this.transform.RotateAround(gameObject.transform.position,Vector3.up,DegreesPerSecond);
        totalRotation += DegreesPerSecond;
        offset=this.transform.position-gameObject.transform.position;
}
void follow(){
//Vector from owl to camera
Vector3 currentoffset=this.transform.position-gameObject.transform.position;
if (currentoffset.magnitude>offset.magnitude){
this.transform.Translate(currentoffset*(-1)*followspeed/currentoffset.magnitude);
}
//
//Vector3 owlfacing(,);
  //this.transform.position=offset+gameObject.transform.position;
print("debug: following");
}
    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("times",++i);
        animator.SetBool("close_enough", true);
        animator.SetBool("stop_walking",true);
        animator.SetBool("stop_rotate",true);
       if (i <= 150)
        this.transform.localScale = new Vector3(0,0,0);
        Vector3 x = new Vector3(0,0,0);
        // initialize every update
        float distance = Vector3.Distance(x, this.transform.position);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        print(distance);
        if (stateInfo.IsName("Base Layer.Appear"))
        {
            //if ((DistanceClose-1) <= distance && distance <= DistanceClose)
            if (i >= 150 )
            {
            animator.SetBool("close_enough", true);
            this.transform.localScale = new Vector3(1, 1, 1);
                start = 1;
            }

        }


        if (stateInfo.IsName("Base Layer.Roll and turn"))
        {
            Quaternion target = Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, 3.0f);
        }


        if (stateInfo.IsName("Base Layer.Walk 1") )
        {
            if ( distance <= DistanceStopwalk )
                animator.SetBool("stop_walking", false);
        }

        if (stateInfo.IsName("Base Layer.Success and turn"))
        {

            Quaternion target = Quaternion.Euler(0, -90, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, 3.0f);
        }
        //TODO Add a state machine and modify the last one.
        //TODO The distance for rotate is 2.5f.
        // If the total rotation is 180, it will stop rotate. This should be written in the state machine!!!
        if (stateInfo.IsName("Base Layer.Run to behind"))
        {
            if(Mathf.Abs(totalRotation) < Mathf.Abs(DegreesAmount)){
				rotate();
			}
            else
            {
                animator.SetBool("stop_rotate",true);
            }
        }
        //TODO Add motion!
        if (stateInfo.IsName("Base Layer.Follow behind") )
        {
           follow();
        }



    }
}
