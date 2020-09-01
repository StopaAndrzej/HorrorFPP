using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : InteractableObjectBase
{

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] private PickUpManager pickUpManager;

    public Vector3 originPos;
    public Quaternion originRot;
    public Transform parent;
    public Transform originLocation;

    private void Start()
    {
        if(pickUpManager==null)
        {
            Debug.Log("PickUpManager doesn't set!!!");
        }

        
        originPos = this.GetComponent<Transform>().position;
        originRot = this.GetComponent<Transform>().rotation;
        parent = this.transform.parent;
    }

    public override void Interact()
    {

        if (pickUpManager.itemMode == PickUpManager.enManagerItemMode.clear)
        {
            /////////////highlight object//////////////////////
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
        }

            if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if(pickUpManager.itemMode == PickUpManager.enManagerItemMode.clear)
            {
                parent = this.transform.parent;
                originPos = this.GetComponent<Transform>().transform.position;
                originRot = this.GetComponent<Transform>().transform.rotation;
                pickUpManager.PickUp(this.gameObject);

                foreach (Transform child in transform)
                {
                    foreach (Transform childChild in child)
                    {
                        if (childChild.GetComponent<MeshRenderer>())
                        {
                            childChild.GetComponent<MeshRenderer>().material.SetColor("_Color", new Vector4(0, 0, 0, 1));
                            childChild.GetComponent<MeshRenderer>().material.SetColor("_SpecularColor", new Vector4(1, 1, 1, 1));
                        }
                    }
                }
            }
        }
    }
}
