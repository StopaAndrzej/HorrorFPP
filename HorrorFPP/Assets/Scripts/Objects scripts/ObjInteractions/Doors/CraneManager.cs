using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneManager : InteractableObjectBase
{
    [SerializeField] private PickUpManager pickUpManager;
    [SerializeField] private TapHandle tapHandleManager;

    [SerializeField] private Transform craneSlot;
    private Vector3 dishSlotPos;
    private Quaternion dishSlotRot;

    private BoxCollider trigger;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    public bool resetText = false;

    private float timeToWait;

    private void Start()
    {
        interactText = "WASH_THE_DISH";

        dishSlotPos = craneSlot.localPosition;
        dishSlotRot = craneSlot.localRotation;

        foreach(BoxCollider box in this.GetComponentsInChildren<BoxCollider>())
        {
            if(box.isTrigger == true)
            {
                trigger = box;
            }
        }

        trigger.enabled = false;
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if (pickUpManager.lastSelectedObj != null)
            {
                if (pickUpManager.lastSelectedObj.GetComponent<PlateManager>())
                {
                    if (pickUpManager.lastSelectedObj.GetComponent<PlateManager>().itemMode == PlateManager.enFoodCondition.soaped && tapHandleManager.waterOn)
                    {
                        // pickUpManager.
                        pickUpManager.lastSelectedObj.transform.parent = this.transform;


                        pickUpManager.originGrabbedItemPos = dishSlotPos;
                        pickUpManager.originGrabbedItemRot = dishSlotRot;

                        pickUpManager.distance = pickUpManager.UpdateDistance(pickUpManager.destinationPosInspect.position);

                        pickUpManager.itemMode = PickUpManager.enManagerItemMode.getToPos;

                        pickUpManager.lastSelectedObj.GetComponent<BoxCollider>().enabled = true;
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

    private void DeniedText()
    {
        interactText = "REQUIRED_DIRTY_DISH";
        resetText = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlateManager>())
        {
            if(other.GetComponent<PlateManager>().itemMode == PlateManager.enFoodCondition.soaped)
            {
                pickUpManager.lastSelectedObj.GetComponent<PlateManager>().foam.SetActive(true);
                timeToWait = pickUpManager.lastSelectedObj.GetComponent<PlateManager>().foam.GetComponent<ParticleSystem>().duration + pickUpManager.lastSelectedObj.GetComponent<PlateManager>().foam.GetComponent<ParticleSystem>().startLifetime;
                StartCoroutine(CleanDish());

                pickUpManager.lastSelectedObj.GetComponent<BoxCollider>().enabled = false;
                trigger.enabled = false;
            }      
        }
    }

    private IEnumerator CleanDish()
    {
        yield return new WaitForSeconds(timeToWait);
        pickUpManager.lastSelectedObj.GetComponent<PlateManager>().foam.SetActive(false);
        pickUpManager.lastSelectedObj.GetComponent<PlateManager>().ChangeSolidClear();
        pickUpManager.PickUp(pickUpManager.lastSelectedObj);
    }

    public override void DeInteract()
    {
        if (resetText)
        {
            interactText = "WASH_THE_DISH";
            resetText = false;
        }
    }
}
