using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour
{

    public float DirectionDampTime = .25f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (animator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Base Layer.rotate and walking"))
        {
            this.transform.Rotate(Vector3.up * 1, Space.Self);
            this.transform.Translate(Vector3.forward * 1, Space.Self);
        }
    }
}