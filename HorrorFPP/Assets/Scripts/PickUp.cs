using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : InteractableObjectBase
{
    public Transform destinationPos;
    public Transform inspectPos;

    public bool stopFlag = false;

    [SerializeField] private KeyCode inspectKey;
    [SerializeField] private KeyCode dropKey;

    [SerializeField] public PlayerMove playerController;
    [SerializeField] private FocusSwitcher focus;
    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private Transform camera;
    [SerializeField] private LayerMask layerMaskInteract;

    //fix
    [SerializeField] private Vector3 objScale;
    public bool isGrabbed = false;

    //additinal list of children's colliders to switch on/off
    public List<GameObject> additionalColliders;

    private void Start()
    {
        //objScale = transform.localScale;
        playerEquipment = GameObject.Find("Player").GetComponent<PlayerEquipment>();
    }

    void Grab()
    {
        GetComponent<Rigidbody>().useGravity = false;
        this.transform.position = destinationPos.position;
        this.transform.localRotation = new Quaternion(0, 0, 0, 0);
        this.transform.parent = GameObject.Find("Destination").transform;

        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<BoxCollider>().enabled = false;
        foreach (GameObject element in additionalColliders)
            element.GetComponent<BoxCollider>().enabled = false;
    }


    public override void Interact()
    {
        if(!playerEquipment.grabInHand)
        {
            playerEquipment.grabInHand = true;
            isGrabbed = true;
            Grab();
        }
    }

    private void Update()
    {
        //inspectItem
        if ((Input.GetKey(inspectKey) && playerEquipment.grabInHand && isGrabbed) || (stopFlag && isGrabbed))
        {
            //turn a blur on
            focus.SetFocused(gameObject);

            playerController.inspectMode = true;
            this.transform.position = new Vector3(Mathf.Lerp(this.transform.position.x, inspectPos.position.x, Time.deltaTime * 5.0f), Mathf.Lerp(this.transform.position.y, inspectPos.position.y, Time.deltaTime * 5.0f), Mathf.Lerp(this.transform.position.z, inspectPos.position.z, Time.deltaTime * 5.0f));
            Inspect();
        }
        else if(isGrabbed)
        {
            //turn a blur off
            focus.SetFocused(null);

            playerController.inspectMode = false;

          //  if (isGrabbed)
         //   {
                this.transform.position = new Vector3(Mathf.Lerp(this.transform.position.x, destinationPos.position.x, Time.deltaTime * 5.0f), Mathf.Lerp(this.transform.position.y, destinationPos.position.y, Time.deltaTime * 5.0f), Mathf.Lerp(this.transform.position.z, destinationPos.position.z, Time.deltaTime * 5.0f));
                //this.transform.localRotation = new Quaternion(Mathf.Lerp(this.transform.localRotation.x, 0.0f, Time.deltaTime), Mathf.Lerp(this.transform.localRotation.y, 0.0f, Time.deltaTime), Mathf.Lerp(this.transform.localRotation.z, 0.0f, Time.deltaTime), Mathf.Lerp(this.transform.localRotation.w, 0.0f, Time.deltaTime));
                this.transform.localRotation = new Quaternion(0, 0, 0, 0);
           // }
        }

        //drop item
        if (Input.GetKey(dropKey) && playerEquipment.grabInHand && !Input.GetKey(inspectKey) && isGrabbed)
        {
            //show drop points
            foreach(GameObject element in playerEquipment.kitchenDrops)
            {
                element.GetComponent<MeshRenderer>().enabled = true;
            }

            RaycastHit hit;
            Vector3 fwd = camera.TransformDirection(Vector3.forward);

            if (Physics.Raycast(camera.position, fwd, out hit, 10.0f, layerMaskInteract.value))
            {
                if (hit.collider.CompareTag("DropArea"))
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        playerEquipment.grabInHand = false;
                        isGrabbed = false;
                        this.transform.position = hit.transform.position;
                        this.transform.rotation = hit.transform.localRotation;
                        this.transform.parent = hit.transform;
                       // this.transform.localScale = objScale;
                        GetComponent<BoxCollider>().enabled = true;
                        GetComponent<Rigidbody>().useGravity = true;

                        foreach (GameObject element in additionalColliders)
                            element.GetComponent<BoxCollider>().enabled = true;
                    }

                }
            }
        }
        else if(!Input.GetKey(dropKey) && playerEquipment.grabInHand && isGrabbed)
        {
            foreach (GameObject element in playerEquipment.kitchenDrops)
            {
                element.GetComponent<MeshRenderer>().enabled = false;
            }

        }
    }
    public void Inspect()
    {

        float mouseX = Input.GetAxis("Mouse X") * 150.0f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * 150.0f * Time.deltaTime;

        this.transform.Rotate(mouseY, 0, 0);
    }

}
