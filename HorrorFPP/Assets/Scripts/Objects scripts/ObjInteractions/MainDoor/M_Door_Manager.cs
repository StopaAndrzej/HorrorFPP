using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class M_Door_Manager : InteractableObjectBase
{
    [SerializeField] private List<Canvas> canvases;

    public KeyCode handleButton;
    public KeyCode judasButton;
    public KeyCode lockButton;

    [SerializeField] private Text handleTxt;
    [SerializeField] private Text judasTxt;
    [SerializeField] private Text lockTxt;

    private bool isHandle = false;
    private bool isJudas = false;
    private bool isLock = false;

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

        if(!animationInProgress)
        {
            //check statements
            if (Input.GetKeyDown(handleButton))
            {
                if (isHandle)
                {
                    if (!isLock)
                    {
                        //simple transform rotation
                        this.GetComponent<Transform>().Rotate(new Vector3(0, 90, 0));
                    }
                    else
                    {
                        //animation
                        handleTxt.text = "OPEN";
                        animator.SetBool("isOpen", false);
                    }
                }
                else
                {
                    if (!isLock)
                    {
                        //simple transform rotation
                        this.GetComponent<Transform>().Rotate(new Vector3(0, -90, 0));
                    }
                    else
                    {
                        //animation
                        handleTxt.text = "CLOSE";
                        animator.SetBool("isOpen", true);
                    }
                }

                isHandle = !isHandle;
            }
            else if (Input.GetKeyDown(judasButton))
            {

            }
            else if (Input.GetKeyDown(lockButton))
            {
                if (!isHandle)
                {
                    if (isLock)
                    {
                        lockTxt.text = "SET\nLOCK";
                        animator.SetBool("isLock", false);
                        isLock = false;
                    }
                    else
                    {
                        lockTxt.text = "RELEASE\nLOCK";
                        animator.SetBool("isLock", true);
                        isLock = true;
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
