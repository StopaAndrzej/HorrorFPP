﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class KettleInspect : MonoBehaviour
{
    [SerializeField] private PlayerMove playerComponent;
    [SerializeField] private KettleDoor kettleDoor;

    [SerializeField] private Text objectName;
    [SerializeField] private Text press;
    [SerializeField] private Text describtion;
    [SerializeField] private Text inProgressBar;

    public string pressText1;
    public string pressText2;
    public string pressText3;

    public string objectNameTitle;
    public string description;
    public string inProgressInfo;

    private float pressPos1, pressPos2;
    private bool changeToUpPos = false;

    private float delay;
    public float delayNormal = 0.1f;
    public float delayPause = 0.5f;
    public string fullText;
    private string currentText = "";

    [SerializeField] private KeyCode interactionKey;

    [SerializeField] private float rayLength = 10.0f;
    [SerializeField] private LayerMask layerMaskInteract;

    private PickUp pickUpScript;

    private bool inspectModeFlag = false;
    private bool descriptionShowed = false;


    private bool doorOpen = false;

    //package meshes
    [SerializeField] private GameObject[] packageMeshes;


    private void Awake()
    {
        objectName.text = "KETTLE";
    }

    // Start is called before the first frame update
    void Start()
    {
        delay = delayNormal;
        fullText = describtion.text;

        objectName.text = objectNameTitle;
        describtion.text = description;
        inProgressBar.text = inProgressInfo;

        objectName.enabled = false;
        press.enabled = false;
        objectName.enabled = false;
        inProgressBar.enabled = false;

        pickUpScript = GetComponent<PickUp>();
        pressPos1 = -182.0f;
        pressPos2 = 182.0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (playerComponent.inspectMode && pickUpScript.isGrabbed)
        {
            if (!changeToUpPos)
            {
                press.GetComponent<RectTransform>().localPosition = new Vector3(press.GetComponent<RectTransform>().localPosition.x, pressPos1, press.GetComponent<RectTransform>().localPosition.z);
            }

            objectName.text = objectNameTitle;
            objectName.enabled = true;

            RaycastHit hit;
            Vector3 up = transform.up;
            Vector3 down = -transform.up;

            Debug.DrawRay(transform.position, up, Color.green);
            Debug.DrawRay(transform.position, down, Color.red);

            if (descriptionShowed)
            {
                describtion.enabled = true;
            }

            if (Physics.Raycast(transform.position, up, out hit, rayLength, layerMaskInteract.value))
            {
                if (hit.collider.CompareTag("Player") && inspectModeFlag == false)
                {
                    if(!doorOpen)
                        press.text = pressText1;
                    else
                        press.text = pressText2;

                    press.enabled = true;

                    if (Input.GetKeyDown(interactionKey))
                    {
                        //pickUpScript.stopFlag = true;
                        press.enabled = false;
                        inspectModeFlag = true;
                        kettleDoor.Interact();

                        if (!doorOpen)
                            doorOpen = true;
                        else
                            doorOpen = false;
                        
                    }
                }
            }
            else if(Physics.Raycast(transform.position, down, out hit, rayLength, layerMaskInteract.value))
            {
                if (hit.collider.CompareTag("Player") && inspectModeFlag == false)
                {
                    if(!descriptionShowed)
                    {
                        press.text = pressText3;
                        press.enabled = true;

                        if (Input.GetKeyDown(interactionKey))
                        {
                            pickUpScript.stopFlag = true;
                            inspectModeFlag = true;
                            press.enabled = false;
                            fullText = describtion.text;
                            describtion.text = description;
                            describtion.enabled = true;
                            StartCoroutine(ShowText());
                        }
                    }
                }
            }
            else
            {
                press.enabled = false;
            }
        }
        else if (pickUpScript.isGrabbed)
        {
            objectName.enabled = false;
            press.enabled = false;
            describtion.enabled = false;
            inspectModeFlag = false;
            StopCoroutine(ShowText());
        }
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);

            describtion.text = currentText;

            if (i == fullText.Length - 1)
            {
                pickUpScript.stopFlag = false;
                descriptionShowed = true;
                inspectModeFlag = false;
                changeToUpPos = true;
                press.GetComponent<RectTransform>().localPosition = new Vector3(press.GetComponent<RectTransform>().localPosition.x, pressPos2, press.GetComponent<RectTransform>().localPosition.z);
            }

            yield return new WaitForSeconds(delay);

            if (fullText[i] == '.')
                delay = delayPause;
            else
                delay = delayNormal;
        }
    }

}
