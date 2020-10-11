﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItemManager : InteractableObjectBase
{
    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] private PickUpManager pickUpManager;
    [SerializeField] private FoodScript foodScript;

    public Vector3 originPos;
    public Quaternion originRot;
    public Transform parent;
    public Transform originLocation;

    [SerializeField] private float objInspectModeOffset = 0;
    [SerializeField] private float txtTitleOffset = 0;

    private void Start()
    {
        if (pickUpManager == null)
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

            if(pickUpManager.itemMode == PickUpManager.enManagerItemMode.inHand && pickUpManager.lastSelectedObj.GetComponent<ItemBase>().titleTxt == "CUTLERY")
            {
                interactText = "ATTACH";
            }
            else
            {
                interactText = "LUNCH";
            }
        }

        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if (pickUpManager.itemMode == PickUpManager.enManagerItemMode.clear)
            {
                parent = this.transform.parent;
                originPos = this.GetComponent<Transform>().transform.position;
                originRot = this.GetComponent<Transform>().transform.rotation;
                pickUpManager.PickUp(this.gameObject, objInspectModeOffset, txtTitleOffset);

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
            else if(pickUpManager.itemMode == PickUpManager.enManagerItemMode.inHand)
            {

            }
        }
    }
}
