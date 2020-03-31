using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : InteractableObjectBase
{
    public Transform destinationPos;
    public Transform inspectPos;



    private bool isGrabed = false;

    [SerializeField] private KeyCode inspectKey;

    [SerializeField] private PlayerMove playerController;

    void Grab()
    {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        this.transform.position = destinationPos.position;
        this.transform.parent = GameObject.Find("Destination").transform;
    }

    public override void Interact()
    {
        if(!isGrabed)
        {
            isGrabed = true;
            Grab();
        }
    }

    private void Update()
    {

        if (Input.GetKey(inspectKey) && isGrabed)
        {
            playerController.inspectMode = true;
            this.transform.position = new Vector3(Mathf.Lerp(this.transform.position.x, inspectPos.position.x, Time.deltaTime), Mathf.Lerp(this.transform.position.y, inspectPos.position.y, Time.deltaTime), Mathf.Lerp(this.transform.position.z, inspectPos.position.z, Time.deltaTime));
            Inspect();
        }
        else 
        {
            playerController.inspectMode = false;

            if(isGrabed)
                this.transform.position = new Vector3(Mathf.Lerp(this.transform.position.x, destinationPos.position.x, Time.deltaTime), Mathf.Lerp(this.transform.position.y, destinationPos.position.y, Time.deltaTime), Mathf.Lerp(this.transform.position.z, destinationPos.position.z, Time.deltaTime));
        }
    }
    public void Inspect()
    {
        float mouseX = Input.GetAxis("Mouse X") * 150.0f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * 150.0f * Time.deltaTime;

        this.transform.Rotate(mouseX, mouseY, 0);
    }

}
