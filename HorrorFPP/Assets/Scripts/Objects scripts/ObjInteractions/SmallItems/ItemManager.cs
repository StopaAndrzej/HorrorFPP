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
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if(pickUpManager.itemMode == PickUpManager.enManagerItemMode.clear)
            {
                parent = this.transform.parent;
                originPos = this.GetComponent<Transform>().transform.position;
                originRot = this.GetComponent<Transform>().transform.rotation;
                pickUpManager.PickUp(this.gameObject);
            }
        }
    }
}
