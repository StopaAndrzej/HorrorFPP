using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KettleDoor : InteractableObjectBase
{
    public bool isOpen = false;
    [SerializeField] private Animator animator;

    [SerializeField] private KettleButton button;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Interact()
    {
        if(!button.isActive)
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
}
