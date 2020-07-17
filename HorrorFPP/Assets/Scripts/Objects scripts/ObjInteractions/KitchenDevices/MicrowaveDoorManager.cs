using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;


public class MicrowaveDoorManager : InteractableObjectBase
{
    [SerializeField] private MicrowaveManager microwaveManager;

    [SerializeField] private Canvas[] canvases = new Canvas[4];

    public KeyCode openButton;
    public KeyCode setButton;

    [SerializeField] private Text setText;

    [SerializeField] private Text openTxt;
    [SerializeField] private Text openIcon;
    [SerializeField] private Text openIcon2;

    private bool isCooking = false;
    private bool readyToCook = false;
    private bool isOpen = false;

    //wait until animation finish to interact again
    public bool animationInProgress = false;

    [SerializeField] private Animator animator;

    //canvas offsets
    [SerializeField] private Vector3 offsetText;
    [SerializeField] private Vector3 offsetButtonIcon;

    [SerializeField] private Vector3 offsetText2;
    [SerializeField] private Vector3 offsetButtonIcon2;

    void Start()
    {
        animator = GetComponent<Animator>();

        for(int i=0;i<canvases.Length; i++)
        {
            if (canvases[i] == null)
                break;

            canvases[i].gameObject.SetActive(false);
        }
    }

    public override void InteractMulti()
    {

        canvases[0].gameObject.SetActive(true);

        if(microwaveManager.itemInside && !isOpen && !isCooking)
        {
            canvases[1].gameObject.SetActive(true);
            readyToCook = true;
        }
        else
        {
            canvases[1].gameObject.SetActive(false);
            readyToCook = false;
        }

        if(!animationInProgress)
        {
            if (Input.GetKeyDown(openButton) || kidID == 1)
            {
                if(isOpen)
                {
                    if(microwaveManager.itemInside)
                    {
                        animator.SetBool("isFullOpen", true);

                    }
                    else
                    {
                        this.GetComponent<Transform>().Rotate(new Vector3(0, -90, 0));
                        openTxt.text = "OPEN";

                        openTxt.GetComponent<RectTransform>().localPosition -= offsetText;
                        openIcon.GetComponent<RectTransform>().localPosition -= offsetButtonIcon;
                        openIcon2.GetComponent<RectTransform>().localPosition -= offsetButtonIcon;
                    }
                    
                }
                else
                {
                    this.GetComponent<Transform>().Rotate(new Vector3(0, 90, 0));
                    openTxt.text = "CLOSE";

                    openTxt.GetComponent<RectTransform>().localPosition += offsetText;
                    openIcon.GetComponent<RectTransform>().localPosition += offsetButtonIcon;
                    openIcon2.GetComponent<RectTransform>().localPosition += offsetButtonIcon;
                }

                isOpen = !isOpen;
                kidID = 0;
            }

            else if((Input.GetKeyDown(setButton) || kidID == 2) && !isCooking && readyToCook)
            {
                isCooking = true;
                microwaveManager.stwichOn = true;

                openTxt.GetComponent<RectTransform>().localPosition += offsetText2;
                openIcon.GetComponent<RectTransform>().localPosition += offsetButtonIcon2;
                openIcon2.GetComponent<RectTransform>().localPosition += offsetButtonIcon2;
            }
        }

    }

    public override void DeInteractMulti()
    {
        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i] == null)
                break;

            canvases[i].gameObject.SetActive(false);
        }
    }

    public void AnimationWorkOn()
    {
        animationInProgress = true;
       
    }

    public void AnimationWorkOff()
    {
        animationInProgress = false;
        animator.SetBool("isFullOpen", false);
        openTxt.text = "OPEN";
        openTxt.GetComponent<RectTransform>().localPosition -= offsetText;
        openIcon.GetComponent<RectTransform>().localPosition -= offsetButtonIcon;
        openIcon2.GetComponent<RectTransform>().localPosition -= offsetButtonIcon;

        openTxt.GetComponent<RectTransform>().localPosition -= offsetText2;
        openIcon.GetComponent<RectTransform>().localPosition -= offsetButtonIcon2;
        openIcon2.GetComponent<RectTransform>().localPosition -= offsetButtonIcon2;
    }

    IEnumerator Opening()
    {
        if (!isOpen)
        {
            yield return new WaitForSeconds(0.6f);
            this.GetComponent<Transform>().Rotate(new Vector3(0, -90, 0));

        }
        else
        {
            this.GetComponent<Transform>().Rotate(new Vector3(0, 90, 0));

            yield return new WaitForSeconds(1.0f);
            openTxt.text = "OPEN";
        }
    }
}
