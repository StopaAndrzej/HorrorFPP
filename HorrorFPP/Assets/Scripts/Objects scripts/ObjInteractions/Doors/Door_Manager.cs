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
            if (Input.GetKeyDown(handleButton))
            {
                if(isOverride)
                {

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
            }
            else if(Input.GetKeyDown(holeButton))
            {

            }
            else if(Input.GetKeyDown(overrideButton))
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
                        isOverride = true;
                    }
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
