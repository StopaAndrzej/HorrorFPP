using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Door_Manager : InteractableObjectBase
{
    [SerializeField] private List<Canvas> canvases;

    public KeyCode handleButton;
    public KeyCode holeButton;
    public KeyCode overrideButton;

    [SerializeField] private Text handleTxt;
    [SerializeField] private Text holeTxt;
    [SerializeField] private Text overrideTxt;

    //other side
    [SerializeField] private Text handleTxt1;
    [SerializeField] private Text holeTxt1;
    [SerializeField] private Text overrideTxt1;

    private bool isFullOpen = false;
    private bool isHole = false;
    private bool isOverride = false;

    //wait until animation finish to interact again
    public bool animationInProgress = false;

    [SerializeField] private Animator animator;

    [SerializeField] private PlayerMove playerMove;


    void Start()
    {
        animator = GetComponent<Animator>();

        foreach (Canvas element in canvases)
        {
            element.gameObject.SetActive(false);
        }
    }


    public override void InteractMulti()
    {
        foreach (Canvas element in canvases)
        {
            element.gameObject.SetActive(true);
        }

        if (!animationInProgress)
        {
            if ((Input.GetKeyDown(handleButton) || kidID == 1) && !isHole)
            {
                if(isOverride)
                {
                    if(!isFullOpen)
                    {
                        this.GetComponent<Transform>().Rotate(new Vector3(0, -65, 0));
                        handleTxt.text = "CLOSE";
                        handleTxt1.text = "CLOSE";
                        isFullOpen = true;
                    }
                    else
                    {
                        this.GetComponent<Transform>().Rotate(new Vector3(0, 90, 0));
                        handleTxt.text = "OPEN";
                        handleTxt1.text = "OPEN";
                        isFullOpen = false;
                        animator.SetBool("restartPose", true);
                    }
                }
                else
                {
                    if(isFullOpen)
                    {
                        this.GetComponent<Transform>().Rotate(new Vector3(0, 90, 0));
                        handleTxt.text = "OPEN";
                        handleTxt1.text = "OPEN";
                    }
                    else
                    {
                        this.GetComponent<Transform>().Rotate(new Vector3(0, -90, 0));
                        handleTxt.text = "CLOSE";
                        handleTxt1.text = "CLOSE";
                    }

                    isFullOpen = !isFullOpen;
                }

                kidID = 0;
            }
            else if(Input.GetKeyDown(holeButton) || kidID == 2)
            {
                playerMove.disablePlayerController = true;
                kidID = 0;
            }
            else if((Input.GetKeyDown(overrideButton) || kidID == 3) && !isHole)
            {
                if(!isFullOpen)
                {
                    if (isOverride)
                    {
                        overrideTxt.text = "OVERRIDE";
                        overrideTxt1.text = "OVERRIDE";
                        animator.SetBool("isOpen", false);
                        isOverride = false;
                    }
                    else
                    {
                        overrideTxt.text = "CLOSE";
                        overrideTxt1.text = "CLOSE";
                        animator.SetBool("isOpen", true);
                        animator.SetBool("restartPose", false);
                        isOverride = true;
                    }

                    kidID = 0;
                }
            }
        }
    }

    public override void DeInteractMulti()
    {
        foreach (Canvas element in canvases)
        {
            element.gameObject.SetActive(false);
        }
    }

    public void AnimationWorkOn()
    {
        animationInProgress = true;
    }

    public void AnimationWorkOff()
    {
        animationInProgress = false;
    }
}
