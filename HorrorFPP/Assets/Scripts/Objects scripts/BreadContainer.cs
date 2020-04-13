using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadContainer : InteractableObjectBase
{
    [SerializeField] private Animator animator;
    private bool isOpen = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Interact()
    {

        if (isOpen)
        {
            interactText = "Close";
            animator.SetBool("isOpen", false);
        }
        else
        {
            interactText = "Open";
            animator.SetBool("isOpen", true);
        }
        isOpen = !isOpen;
        
    }
}
