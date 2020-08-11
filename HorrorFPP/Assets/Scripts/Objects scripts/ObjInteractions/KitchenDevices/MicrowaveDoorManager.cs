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

    public bool isCooking = false;
    private bool readyToCook = false;
    private bool isOpen = false;

    //wait until animation finish to interact again
    public bool animationInProgress = false;

    [SerializeField] private Animator animator;

    //canvas offsets
    [SerializeField] private Vector3 offsetText;
    [SerializeField] private Vector3 offsetButtonIcon;

    [SerializeField] private GameObject openColiderObj;
    [SerializeField] private Vector3 offsetCanvas;

    //bool check to offset canvace only once when door is closed
    private bool OpenTextOffsetOne = false;

    [SerializeField] private BoxCollider cookObj;
    


    void Start()
    {
        animator = GetComponent<Animator>();

        for(int i=0;i<canvases.Length; i++)
        {
            if (canvases[i] == null)
                break;

            canvases[i].gameObject.SetActive(false);
        }
        
        //cook canvas collider deactive;
        cookObj.enabled = false;
    }

    public override void InteractMulti()
    {
        if(!isCooking)
        {
            canvases[0].gameObject.SetActive(true);
            canvases[2].gameObject.SetActive(true);
        }
        
        if(microwaveManager.itemInside && !isOpen && !isCooking)
        {
            canvases[1].gameObject.SetActive(true);
            readyToCook = true;
            cookObj.enabled = true;
        }
        else
        {
            canvases[1].gameObject.SetActive(false);
            readyToCook = false;
            cookObj.enabled = false;
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

                openColiderObj.GetComponent<Transform>().localPosition += offsetCanvas;
                OpenTextOffsetOne = false;

                microwaveManager.TurnOnMicrowave();
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

        if(!OpenTextOffsetOne)
        {
            openColiderObj.GetComponent<Transform>().localPosition -= offsetCanvas;
            OpenTextOffsetOne = true;
        }

        openTxt.GetComponent<RectTransform>().localPosition -= offsetText;
        openIcon.GetComponent<RectTransform>().localPosition -= offsetButtonIcon;
        openIcon2.GetComponent<RectTransform>().localPosition -= offsetButtonIcon;
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
