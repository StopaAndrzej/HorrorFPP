using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutleryManager : InteractableObjectBase
{
    [SerializeField] private PickUpManager pickUpManager;
    [SerializeField] private GameObject cutleryObj;
    private bool cutleryGrabbed = false;

    private Vector3 cutleryOriginPos;
    private Quaternion cutleryOriginRot;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    private void Start()
    {
        interactText = "GET_CUTLERY";
        cutleryOriginPos = cutleryObj.transform.position;
        cutleryOriginRot = cutleryObj.transform.rotation;
        cutleryObj.SetActive(false);
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if(pickUpManager.itemMode == PickUpManager.enManagerItemMode.clear)
            {
                if(!cutleryGrabbed)
                {
                    cutleryGrabbed = true;
                    cutleryObj.transform.position = cutleryOriginPos;
                    cutleryObj.transform.rotation = cutleryOriginRot;
                    cutleryObj.SetActive(true);
                    pickUpManager.PickUp(cutleryObj, 0, 0);
                }
            }
        }
    }
}
