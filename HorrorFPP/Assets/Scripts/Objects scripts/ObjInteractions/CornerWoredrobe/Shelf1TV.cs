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

    [SerializeField] private List<Canvas> canvases;
    [SerializeField] private Text text;
    [SerializeField] private Text buttonIcon;
    [SerializeField] private Text buttonIcon1;

    [SerializeField] private Vector3 offsetText;
    [SerializeField] private Vector3 offsetButtonIcon;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] private List<Light> lights;

    private void Start()
    {
        closedPos = this.GetComponent<Transform>().position;
        closeRot = this.GetComponent<Transform>().rotation;

        openPos = destinationPoint.position;
        closeRot = destinationPoint.rotation;

        interactText = "OPEN";

        foreach (Canvas element in canvases)
        {
            element.gameObject.SetActive(false);
        }

        foreach(Light li in lights)
        {
            li.enabled = false;
        }
    }

    public override void Interact()
    {
        foreach (Canvas element in canvases)
        {
            element.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if (!isOpen && !isBlocked)
            {
                this.GetComponent<Transform>().position = openPos;
                this.GetComponent<Transform>().rotation = closeRot;
                interactText = "CLOSE";
                text.text = "CLOSE";
                text.GetComponent<RectTransform>().localPosition += offsetText;
                buttonIcon.GetComponent<RectTransform>().localPosition += offsetButtonIcon;
                buttonIcon1.GetComponent<RectTransform>().localPosition += offsetButtonIcon;

                isOpen = !isOpen;

                foreach (Light li in lights)
                {
                    li.enabled = true;
                }

                //for double doors
                if (twin != null)
                {
                    twin.GetComponent<Transform>().position = twin.openPos;
                    twin.GetComponent<Transform>().rotation = twin.closeRot;
                    twin.interactText = "CLOSE";

                    twin.GetComponent<Shelf1TV>().text.text = "CLOSE";
                    twin.GetComponent<Shelf1TV>().text.GetComponent<RectTransform>().localPosition += twin.GetComponent<Shelf1TV>().offsetText;
                    twin.GetComponent<Shelf1TV>().buttonIcon.GetComponent<RectTransform>().localPosition += twin.GetComponent<Shelf1TV>().offsetButtonIcon;
                    twin.GetComponent<Shelf1TV>().buttonIcon1.GetComponent<RectTransform>().localPosition += twin.GetComponent<Shelf1TV>().offsetButtonIcon;

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
            else
            {
                this.GetComponent<Transform>().position = closedPos;
                this.GetComponent<Transform>().rotation = openRot;
                interactText = "OPEN";
                text.text = "OPEN";
                text.GetComponent<RectTransform>().localPosition -= offsetText;
                buttonIcon.GetComponent<RectTransform>().localPosition -= offsetButtonIcon;
                buttonIcon1.GetComponent<RectTransform>().localPosition -= offsetButtonIcon;

                isOpen = !isOpen;

                foreach (Light li in lights)
                {
                    li.enabled = false;
                }

                //for double doors
                if (twin != null)
                {
                    twin.GetComponent<Transform>().position = twin.closedPos;
                    twin.GetComponent<Transform>().rotation = twin.openRot;
                    twin.interactText = "OPEN";

                    twin.GetComponent<Shelf1TV>().text.text = "OPEN";
                    twin.GetComponent<Shelf1TV>().text.GetComponent<RectTransform>().localPosition -= twin.GetComponent<Shelf1TV>().offsetText;
                    twin.GetComponent<Shelf1TV>().buttonIcon.GetComponent<RectTransform>().localPosition -= twin.GetComponent<Shelf1TV>().offsetButtonIcon;
                    twin.GetComponent<Shelf1TV>().buttonIcon1.GetComponent<RectTransform>().localPosition = twin.GetComponent<Shelf1TV>().offsetButtonIcon;

                    twin.GetComponent<Shelf1TV>().isOpen = !twin.GetComponent<Shelf1TV>().isOpen;
                }
            }
        } 
    }

    public override void DeInteractMulti()
    {
        foreach (Canvas element in canvases)
        {
            element.gameObject.SetActive(false);
        }
    }

}
