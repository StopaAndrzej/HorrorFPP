using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenDoor : InteractableObjectBase
{
    private bool isOpen = false;
    [SerializeField] private Animator animator;
    [SerializeField] private OvenManager manager;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    public override void Interact()
    {
        if (!manager.stwichOn)
        {
            if (isOpen)
            {
                interactText = "Close";
                animator.SetBool("isOpen", false);
                manager.doorOpen = false;
            }
            else
            {
                interactText = "Open";
                animator.SetBool("isOpen", true);
                manager.doorOpen = true;
            }
            isOpen = !isOpen;
        }
    }
}
