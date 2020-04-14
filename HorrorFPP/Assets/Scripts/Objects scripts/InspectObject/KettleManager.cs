using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KettleManager : InteractableObjectBase
{
    private PickUp pickUp;
    private Transform kettleFirstPos;
    private PlayerEquipment playerEquipment;
    [SerializeField] private GameObject parent;

    [SerializeField] List<GameObject> additionalColliders;

    private void Start()
    {
        pickUp = GetComponent<PickUp>();
        kettleFirstPos = GetComponent<Transform>();
    }

    public override void Interact()
    {
        if(pickUp.isGrabbed)
        {
            this.GetComponent<Transform>().position = kettleFirstPos.position;
            this.GetComponent<Transform>().localRotation = kettleFirstPos.localRotation;
            pickUp.isGrabbed = false;
            playerEquipment.grabInHand = false;

            this.transform.parent = parent.transform;
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<Rigidbody>().useGravity = true;

            foreach (GameObject element in pickUp.additionalColliders)
                element.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
