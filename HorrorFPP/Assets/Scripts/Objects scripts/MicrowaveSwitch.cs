using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrowaveSwitch : InteractableObjectBase
{
    public bool isActive = false;
    [SerializeField] private Animator animator;

    [SerializeField] private MicrowaveManager manager;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public override void Interact()
    {
        if(!manager.doorOpen)
        {
            if(!isActive)
            {
                interactText = "Active";
                animator.SetBool("isActive", true);
                manager.stwichOn = true;
                isActive = true;
            }
        }
    }

    void DeactivateMicrowave()
    {
        animator.SetBool("isActive", false);
    }
}

