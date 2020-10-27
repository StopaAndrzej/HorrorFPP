﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishSoapManager : InteractableObjectBase
{
    [SerializeField] private PickUpManager pickUpManager;
    [SerializeField] private BoxCollider trigger;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    private Animator animator;

    public bool resetText = false;
    [SerializeField] private Transform dishSlot;
    private Vector3 dishSlotPos;
    private Quaternion dishSlotRot;

    void Start()
    {
        animator = GetComponent<Animator>();
        interactText = "DISH_SOAP";

        dishSlotPos = dishSlot.localPosition;
        dishSlotRot = dishSlot.localRotation;

        trigger.enabled = false;
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if(pickUpManager.lastSelectedObj != null)
            {
                if (pickUpManager.lastSelectedObj.GetComponent<PlateManager>())
                {
                    if(pickUpManager.lastSelectedObj.GetComponent<PlateManager>().itemMode == PlateManager.enFoodCondition.dirty)
                    {
                        // pickUpManager.
                        pickUpManager.lastSelectedObj.transform.parent = this.transform;

                        pickUpManager.originGrabbedItemPos = dishSlotPos;
                        pickUpManager.originGrabbedItemRot = dishSlotRot;
                        pickUpManager.distance = pickUpManager.UpdateDistance(pickUpManager.destinationPosInspect.position);

                        pickUpManager.lastSelectedObj.GetComponent<BoxCollider>().enabled = true;
                        pickUpManager.itemMode = PickUpManager.enManagerItemMode.getToPos;
                        trigger.enabled = true;
                    }
                    else
                    {
                        DeniedText();
                    }
                   
                }
                else
                {
                    DeniedText();
                }
            }
            else
            {
                DeniedText();
            }

        }
    }

    public void UseAnim()
    {
        animator.Play("Use", -1, 0f);
    }


    public void unlockPlayer()
    {
        pickUpManager.lastSelectedObj.GetComponent<PlateManager>().ChangeSolidDirtySoap();
        pickUpManager.PickUp(pickUpManager.lastSelectedObj);
    }

    public override void DeInteract()
    {
        if (resetText)
        {
            interactText = "DISH_SOAP";
            resetText = false;
        }
    }

    private void DeniedText()
    {
        interactText = "REQUIRED_DIRTY_DISH";
        resetText = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlateManager>())
        {
            UseAnim();
        }

        pickUpManager.lastSelectedObj.GetComponent<BoxCollider>().enabled = false;
        trigger.enabled = false;
    }
}
