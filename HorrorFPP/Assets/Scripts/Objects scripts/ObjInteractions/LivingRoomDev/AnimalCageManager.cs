using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalCageManager : InteractableObjectBase
{
    [SerializeField] private PickUpManager pickUpManager;
    public bool isOpen = false;
    public bool isFed = false;
    [SerializeField] private MeshRenderer foodPot;

    //posses
    public Transform destinationPoint;

    private Vector3 closedPos;
    private Quaternion closeRot;

    private Vector3 openPos;
    private Quaternion openRot;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] private GameObject door;
    [SerializeField] Animator animalAnimator;

    private void Start()
    {
        foodPot.enabled = false;
        closedPos = door.GetComponent<Transform>().position;
        closeRot = door.GetComponent<Transform>().rotation;

        openPos = destinationPoint.position;
        openRot = destinationPoint.rotation;

        interactText = "OPEN";
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if (!isOpen)
            {
                door.GetComponent<Transform>().position = openPos;
                door.GetComponent<Transform>().rotation = openRot;
                interactText = "CLOSE";
                isOpen = !isOpen;
                WaitForEat();
                if (pickUpManager.itemMode == PickUpManager.enManagerItemMode.inHand && pickUpManager.lastSelectedObj != null && pickUpManager.lastSelectedObj.GetComponent<InteractableObjectBase>().interactText == "PET'S FOOD")
                {
                    interactText = "FEED";
                }
            }
            else
            {
                if(pickUpManager.itemMode == PickUpManager.enManagerItemMode.inHand && pickUpManager.lastSelectedObj != null && pickUpManager.lastSelectedObj.GetComponent<InteractableObjectBase>().interactText == "PET'S FOOD")
                {
                    foodPot.enabled = true ;
                    Eat();
                }
                else
                {
                    door.GetComponent<Transform>().position = closedPos;
                    door.GetComponent<Transform>().rotation = closeRot;
                    interactText = "OPEN";
                    isOpen = !isOpen;
                }
            }
        }
    }

    public void Warning()
    {
        animalAnimator.Play("warning_stand");
    }

    public void CageBite()
    {
        animalAnimator.Play("warning_stand 0");
    }

    private void WaitForEat()
    {
        animalAnimator.Play("cage_exit 0");
    }

    private void Eat()
    {
        animalAnimator.Play("eat 0");
    }
}
