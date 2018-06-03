using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Walk_rot_old : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public float rotateSpeed = 0.5f;
    public int start = 0;
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

    }

    // Update is called once per frame
    void Update()
    {
        if (start == 0)
        this.transform.localScale = new Vector3(0,0,0);


        // initialize every update
        float distance = Vector3.Distance(gameObject.transform.position, this.transform.position);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Base Layer.Appear"))
        {
            if (distance == distance)
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

        if (stateInfo.IsName("Base Layer.Walking"))
        {
            Owl.SetActive(false);
            if (distance == distance)
                animator.SetBool("stop_walking", true);
        }

        if (stateInfo.IsName("Base Layer.Walk") )
        {
            if (distance == distance)
                animator.SetBool("stop_walking", true);
        }

        if (stateInfo.IsName("Base Layer.Success and turn"))
        {

            Quaternion target = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, 3.0f);
        }

        if (stateInfo.IsName("Base Layer.Run behind") )
        {
            this.transform.Rotate(Vector3.up * moveSpeed, Space.Self);
            this.transform.Translate(Vector3.forward * rotateSpeed, Space.Self);
        }



    }
}
