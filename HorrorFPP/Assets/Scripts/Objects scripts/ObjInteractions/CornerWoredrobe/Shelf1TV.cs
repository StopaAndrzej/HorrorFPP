using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shelf1TV : InteractableObjectBase
{
    public bool isOpen = false;

    //posses
    public Transform destinationPoint;

    private Vector3 closedPos;
    private Quaternion closeRot;

    private Vector3 openPos;
    private Quaternion openRot;

    //twin objects for double doors
    [SerializeField] private Shelf1TV twin;

    //addition flag if obj can be block by other opened object
    [SerializeField] List<Shelf1TV> blockingObj;
    public bool isBlocked = false;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] private List<Light> lights;

    public bool valuableToInteract = false;
    private bool resetText = false;             //reset text when "nothing" text apprears

    private void Start()
    {
        closedPos = this.GetComponent<Transform>().position;
        closeRot = this.GetComponent<Transform>().rotation;

        openPos = destinationPoint.position;
        openRot = destinationPoint.rotation;

        interactText = "OPEN";

        foreach(Light li in lights)
        {
            li.enabled = false;
        }
    }

    public override void Interact()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform childChild in child)
            {
                if (childChild.GetComponent<MeshRenderer>())
                {
                    childChild.GetComponent<MeshRenderer>().material.SetColor("_Color", new Vector4(1, 1, 1, 1));
                    childChild.GetComponent<MeshRenderer>().material.SetColor("_SpecularColor", new Vector4(0.2f, 0.2f, 0.2f, 1));
                }
            }

        }

        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if (!isOpen && !isBlocked && valuableToInteract)
            {
                this.GetComponent<Transform>().position = openPos;
                this.GetComponent<Transform>().rotation = openRot;
                interactText = "CLOSE";

                isOpen = !isOpen;

                foreach (Light li in lights)
                {
                    li.enabled = true;
                }

                //for double doors
                if (twin != null)
                {
                    twin.GetComponent<Transform>().position = twin.openPos;
                    twin.GetComponent<Transform>().rotation = twin.openRot;
                    twin.interactText = "CLOSE";

                    twin.GetComponent<Shelf1TV>().isOpen = !twin.GetComponent<Shelf1TV>().isOpen;
                }
            }
            else if(isBlocked)
            {
                interactText = "BLOCKED";
                if (twin != null)
                {
                    twin.interactText = "BLOCKED";
                }
            }
            else if(!valuableToInteract && !isOpen)
            {
                interactText = "NOTHING";
                resetText = true;

                if (twin != null)
                {
                    twin.interactText = "NOTHING";
                    twin.resetText = true;
                }
            }
            else
            {
                this.GetComponent<Transform>().position = closedPos;
                this.GetComponent<Transform>().rotation = closeRot;
                interactText = "OPEN";

                isOpen = !isOpen;

                foreach (Light li in lights)
                {
                    li.enabled = false;
                }

                //for double doors
                if (twin != null)
                {
                    twin.GetComponent<Transform>().position = twin.closedPos;
                    twin.GetComponent<Transform>().rotation = twin.closeRot;
                    twin.interactText = "OPEN";

                    twin.GetComponent<Shelf1TV>().isOpen = !twin.GetComponent<Shelf1TV>().isOpen;
                }
            }
        } 
    }

    public override void DeInteract()
    {
        if(resetText)
        {
            interactText = "OPEN";
            resetText = false;
            if (twin != null)
            {
                twin.interactText = "OPEN";
                twin.resetText = false;
            }
        }

        foreach (Transform child in transform)
        {
            foreach (Transform childChild in child)
            {
                if (childChild.GetComponent<MeshRenderer>())
                {
                    childChild.GetComponent<MeshRenderer>().material.SetColor("_Color", new Vector4(0.5882352941176471f, 0.5882352941176471f, 0.5882352941176471f, 1));
                    childChild.GetComponent<MeshRenderer>().material.SetColor("_SpecularColor", new Vector4(0.0f, 0.0f, 0.0f, 1));
                }
            }

        }
    }


}
