using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenSwitch : InteractableObjectBase
{
    private bool isActive = false;
    [SerializeField] private Animator animator;
    [SerializeField] private OvenManager manager;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Interact()
    {
        if (!manager.doorOpen)
        {
            if (!isActive)
            {
                interactText = "Active";
                animator.SetBool("isActive", true);
                manager.stwichOn = true;
                isActive = true;
            }
        }
    }
    void ActivateOven()
    {
        manager.door.material = manager.doorActive;
        manager.plate.material = manager.inside;
    }

    void DeactivateOven()
    {
        animator.SetBool("isActive", false);
        isActive = false;
        manager.stwichOn = false;
        manager.Cook();
        manager.door.material = manager.doorNoActive;
        manager.plate.material = manager.inside;
    }
}

