using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugDoor : InteractableObjectBase
{
    public bool isOpen = false;
    [SerializeField] private Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
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
