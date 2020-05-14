using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathroomHandle : InteractableObjectBase
{
    private bool isOpen = false;

    [SerializeField] private Animator HandleAnimator;
    [SerializeField] private Animator DoorAnimator;
    [SerializeField] private Animator LockAnimator;

    // Start is called before the first frame update
    void Start()
    {
        HandleAnimator = GetComponent<Animator>();
    }

    public override void Interact()
    {
        if(!isOpen)
        {
            interactText = "Open";
            HandleAnimator.SetBool("isOpen", true);
            LockAnimator.SetBool("isOpen", true);
        }
        else
        {
            DoorOpen();
        }
    }

    public void DoorOpen()
    {
        if (!isOpen)
        {
            DoorAnimator.SetBool("isOpen", true);
            HandleAnimator.SetBool("isOpen", false);
            LockAnimator.SetBool("isOpen", false);
            isOpen = true;
        }  
        else
        {
            DoorAnimator.SetBool("isOpen", false);
            isOpen = false;
        }
           
    }

}
