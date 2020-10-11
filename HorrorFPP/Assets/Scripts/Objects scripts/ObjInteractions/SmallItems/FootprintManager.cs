using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootprintManager : InteractableObjectBase
{
    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] private PickUpManager pickUpManager;
    private bool resetText = false;

    private void Start()
    {
        pickUpManager = GameObject.Find("PlayerCamera").GetComponent<PickUpManager>();
        interactText = "FOOTPRINT";
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if (pickUpManager.itemMode == PickUpManager.enManagerItemMode.inHand && pickUpManager.lastSelectedObj.GetComponent<MopScript>())
            {

                if(!pickUpManager.lastSelectedObj.GetComponent<MopScript>().Use())
                {
                    interactText = "MOP_IS_DIRTY";
                }
            }
        }
    }

    public override void DeInteract()
    {
        if (resetText)
        {
            interactText = "FOOTPRINT";
            resetText = false;
        }
    }
}
