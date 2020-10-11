using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DryerManager : InteractableObjectBase
{
    [SerializeField] private PickUpManager pickUpManager;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;


    [SerializeField] private Transform[] slotsDryer = new Transform[6];
    private int id = -1; //rigid assignement 2dishes inside dryer, possibly change in future
    private int maxId = 6;

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if (pickUpManager.itemMode == PickUpManager.enManagerItemMode.inHand && pickUpManager.lastSelectedObj.GetComponent<PlateManager>())
            {
                if(pickUpManager.lastSelectedObj.GetComponent<PlateManager>().itemMode == PlateManager.enFoodCondition.clean)
                {
                    if(id+1<=maxId)
                    {
                        id++;
                        pickUpManager.lastSelectedObj.transform.parent = slotsDryer[id];
                        pickUpManager.lastSelectedObj.transform.position = slotsDryer[id].position;
                        pickUpManager.lastSelectedObj.transform.rotation = slotsDryer[id].rotation;
                        pickUpManager.ForceToClearHandNoGravity(pickUpManager.lastSelectedObj);
                    }
                    
                }
            }
        }
    }

}
