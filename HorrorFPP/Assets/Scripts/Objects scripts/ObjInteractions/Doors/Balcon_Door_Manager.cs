using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Balcon_Door_Manager : InteractableObjectBase
{
    [SerializeField] private List<Canvas> canvases;

    public KeyCode handleButton;
    public KeyCode overrideButton;

    [SerializeField] private Text handleTxt;
    [SerializeField] private Text overrideTxt;

    private bool isFullOpen = false;
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
            if (Input.GetKeyDown(handleButton) || kidID == 1)
            {
                if (isOverride)
                {
                    //cant open when is override! COMMUNICATE
                }
                else
                {
                    if (isFullOpen)
                    {
                        animator.SetBool("isFullOpen", false);
                        StartCoroutine(Opening());
                    }
                    else
                    {
                        animator.SetBool("isFullOpen", true);
                        handleTxt.text = "CLOSE";
                        StartCoroutine(Opening());
                    }

                    isFullOpen = !isFullOpen;
                    kidID = 0;
                }
            }
            else if(Input.GetKeyDown(overrideButton) || kidID == 2)
            {
                if(!isFullOpen)
                {
                    if (isOverride)
                    {
                        overrideTxt.text = "OVERRIDE";
                        animator.SetBool("isOpen", false);
                    }
                    else
                    {
                        overrideTxt.text = "CLOSE";
                        animator.SetBool("isOpen", true);
                    }

                    isOverride = !isOverride;
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

    IEnumerator Opening()
    {
        if (!isFullOpen)
        {
            yield return new WaitForSeconds(0.6f);
            this.GetComponent<Transform>().Rotate(new Vector3(0, -90, 0));
        }
        else
        {
            this.GetComponent<Transform>().Rotate(new Vector3(0, 90, 0));
            yield return new WaitForSeconds(0.6f);
            handleTxt.text = "OPEN";
        }
    }
}
