using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KettlePour : InteractableObjectBase
{
    public bool isActive = false;
    [SerializeField] private Animator animator;

  
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Interact()
    {
        if (isActive)
        {
            interactText = "isActive";
            animator.SetBool("isActive", false);
        }
        else
        {
            interactText = "isActive";
            animator.SetBool("isActive", true);
        }
        isActive = !isActive;
    }
}
