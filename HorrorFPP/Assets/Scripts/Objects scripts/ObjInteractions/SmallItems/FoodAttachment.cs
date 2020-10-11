using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodAttachment : AttachmentScript
{
    [SerializeField] private FoodScript foodScript;

    private void Start()
    {
        foodScript = this.GetComponent<FoodScript>();
    }

    public override bool Interact()
    {
        if(pickUpManager.itemMode == PickUpManager.enManagerItemMode.inHand && pickUpManager.lastSelectedObj.GetComponent<CutleryManager>() != null)
        {
            return true;
        }
        return false;
    }

    public override void Interact1()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            pickUpManager.originGrabbedItemPos = foodScript.forkSlot.position;
            pickUpManager.itemMode = PickUpManager.enManagerItemMode.returnToPos;
        }
    }

}
