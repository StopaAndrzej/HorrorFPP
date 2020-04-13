using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadSlicer : InteractableObjectBase
{
    [SerializeField] private Animator animator;
    private bool isActive = false;
    [SerializeField] private BreadSlicerBlade blade;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Interact()
    {
        if (!isActive)
        {
            interactText = "Activate";
            animator.SetBool("isActive", true);
            isActive = true;
            animator.SetBool("isFinished", false);
            blade.TurnOnBlade();
        }
    }

    void Deactivate()
    {
        isActive = false;
        animator.SetBool("isFinished", true);
        animator.SetBool("isActive", false);
    }

}
