using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenSwitch : InteractableObjectBase
{
    private bool isActive = false;
    [SerializeField] private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public override void Interact()
    {
        if (isActive)
        {
            interactText = "NotActive";
            animator.SetBool("isActive", false);
        }
        else
        {
            interactText = "Active";
            animator.SetBool("isActive", true);
        }

        isActive = !isActive;
    }
}
