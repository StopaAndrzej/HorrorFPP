using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : InteractableObjectBase
{
    public bool isOpen { private set;  get; }
    [SerializeField] private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        isOpen = false;
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
