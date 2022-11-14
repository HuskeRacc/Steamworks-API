using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKControl : NetworkBehaviour
{
    protected Animator animator;

    public bool ikActive = false;
    public Transform headObj;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetLookAtPosition(headObj.position);
        animator.SetLookAtWeight(0.35f);
    }
}
