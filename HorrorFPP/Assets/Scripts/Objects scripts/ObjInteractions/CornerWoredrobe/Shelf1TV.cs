using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf1TV : InteractableObjectBase
{
    private bool isOpen = false;

    //posses
    private Transform closedPos;
    public Transform openPos;

    //twin objects for double doors
    [SerializeField] Shelf1TV twin;

    //addition flag if obj can be block by other opened object
    [SerializeField] List<Shelf1TV> blockingObj;
    private bool isBlockingFlag = false;

    private void Start()
    {
        closedPos = this.GetComponent<Transform>();
        interactText = "OPEN";
    }

    public override void Interact()
    {
        if(blockingObj!= null)
        {
            foreach(Shelf1TV element in blockingObj)
            {
                if(element.isOpen)
                {
                    isBlockingFlag = true;
                    break;
                }
                else
                {
                    isBlockingFlag = false;
                }
            }
        }

        if (!isOpen && !isBlockingFlag)
        {
            this.GetComponent<Transform>().position = openPos.position;
            this.GetComponent<Transform>().rotation = openPos.rotation;
            interactText = "CLOSE";

            //for double doors
            if (twin != null)
            {
                twin.GetComponent<Transform>().position = twin.openPos.position;
                twin.GetComponent<Transform>().rotation = twin.openPos.rotation;
                twin.interactText = "CLOSE";
            }
        }
        else if(!isOpen && isBlockingFlag)
        {
            interactText = "BLOCKED";
            if (twin != null)
            {
                twin.interactText = "BLOCKED";
            }
        }
        else
        {
            this.GetComponent<Transform>().position = closedPos.position;
            this.GetComponent<Transform>().rotation = closedPos.rotation;
            interactText = "OPEN";

            //for double doors
            if (twin != null)
            {
                twin.GetComponent<Transform>().position = twin.closedPos.position;
                twin.GetComponent<Transform>().rotation = twin.closedPos.rotation;
                twin.interactText = "OPEN";
            }
        }

        isOpen = !isOpen;
    }

}
