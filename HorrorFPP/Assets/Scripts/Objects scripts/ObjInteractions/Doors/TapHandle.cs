using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapHandle : InteractableObjectBase
{
    public bool isOpen;
    public bool isLocked;
    public bool waterOn;

    //posses
    public Transform destinationPoint;

    private Vector3 closedPos;
    private Quaternion closeRot;

    private Vector3 openPos;
    private Quaternion openRot;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] Animator animatorWaterFall;

    [SerializeField] private GameObject waterTap;
    [SerializeField] private float lowestWaterLevel;
    [SerializeField] private float highestWaterLevel = 1.707f;

    //additional for specific obj
    public GameObject objInTap;

    private void Start()
    {
        closedPos = this.GetComponent<Transform>().position;
        closeRot = this.GetComponent<Transform>().rotation;

        openPos = destinationPoint.position;
        openRot = destinationPoint.rotation;

        interactText = "TURN ON";

        lowestWaterLevel = waterTap.GetComponent<Transform>().localPosition.y;

        isLocked = true;
        isOpen = false;
        waterOn = false;
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if(!isOpen)
            {
                this.GetComponent<Transform>().position = openPos;
                this.GetComponent<Transform>().rotation = openRot;
                interactText = "TURN_OFF";
            }
            else
            {
                this.GetComponent<Transform>().position = closedPos;
                this.GetComponent<Transform>().rotation = closeRot;
                interactText = "TURN_ON";
            }

            isOpen = !isOpen;
            CheckWaterInPipe();
        }
    }


    public void CheckWaterInPipe()
    {
        if(isOpen && !isLocked)
        {
            animatorWaterFall.SetBool("isActiveBBBBBBB", true);
            waterOn = true;
        }
        else
        {
            animatorWaterFall.SetBool("isActiveBBBBBBB", false);
            waterOn = false;
        }
    }

    private void Update()
    {
        if (waterOn)
        {
            if(objInTap!=null)
            {
                if(objInTap.GetComponent<BucketScript>())
                {
                    objInTap.GetComponent<BucketScript>().waterOn = true;
                }
            }
            else
            {
                waterTap.GetComponent<Transform>().localPosition = new Vector3(waterTap.GetComponent<Transform>().localPosition.x, Mathf.Lerp(waterTap.GetComponent<Transform>().localPosition.y, highestWaterLevel, Time.deltaTime * 0.1f), waterTap.GetComponent<Transform>().localPosition.z);
            }
        }
        else
        {
            if (objInTap != null)
            {
                if (objInTap.GetComponent<BucketScript>())
                {
                    objInTap.GetComponent<BucketScript>().waterOn = false;
                }
            }
            else
            {
                waterTap.GetComponent<Transform>().localPosition = new Vector3(waterTap.GetComponent<Transform>().localPosition.x, Mathf.Lerp(waterTap.GetComponent<Transform>().localPosition.y, lowestWaterLevel, Time.deltaTime * 0.1f), waterTap.GetComponent<Transform>().localPosition.z);
            }
        }
    }
}
