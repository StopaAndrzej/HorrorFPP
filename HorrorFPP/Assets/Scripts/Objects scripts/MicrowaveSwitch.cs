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

    void ActivateMicrowave()
    {
        manager.door.material = manager.doorActive;
        manager.backDoor.material = manager.backDoorActive;
        manager.inside.material = manager.insideActive;
        manager.plate.material = manager.plateActive;
        manager.console.material = manager.consoleActive;
    }

    void DeactivateMicrowave()
    {
        animator.SetBool("isActive", false);
        isActive = false;
        manager.stwichOn = false;
        manager.Cook();
        manager.door.material = manager.doorNoActive;
        manager.backDoor.material = manager.backDoorNoActive;
        manager.inside.material = manager.insideNoActive;
        manager.plate.material = manager.plateNoActive;
        manager.console.material = manager.consoleNoActive;
    }
}

