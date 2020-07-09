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
    [SerializeField] private Transform cameraJudasPos;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private PlayerMove playerMove;

    void Start()
    {
        animator = GetComponent<Animator>();
        interactText = "DOOR";

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
            if (Input.GetKeyDown(handleButton) || kidID==1)
            {
                if (isHandle)
                {
                    if (!isLock)
                    {
                        //simple transform rotation
                        animator.SetBool("isFullOpen", false);
                        StartCoroutine(Opening());
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
                        handleTxt.text = "CLOSE";
                        animator.SetBool("isFullOpen", true);
                        StartCoroutine(Opening());
                    }
                    else
                    {
                        //animation
                        handleTxt.text = "CLOSE";
                        animator.SetBool("isOpen", true);
                    }
                }

                isHandle = !isHandle;
                kidID = 0;
            }
            else if (Input.GetKeyDown(judasButton) || kidID == 2)
            {
                playerMove.disablePlayerController = true;
                playerCamera.GetComponent<Transform>().position = cameraJudasPos.position;
                playerCamera.GetComponent<Transform>().rotation = cameraJudasPos.rotation;

                kidID = 0;
            }
            else if (Input.GetKeyDown(lockButton) || kidID == 3)
            {
                if (!isHandle)
                {
                    if (isLock)
                    {
                        lockTxt.text = "SET\nLOCK";
                        animator.SetBool("isLock", false);
                    }
                    else
                    {
                        lockTxt.text = "RELEASE\nLOCK";
                        animator.SetBool("isLock", true);
                    }
                }

                isLock = !isLock;
                kidID = 0;
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
        if (!isHandle)
        {
            yield return new WaitForSeconds(1.2f);
            this.GetComponent<Transform>().Rotate(new Vector3(0, -90, 0));
        }
        else
        {
            this.GetComponent<Transform>().Rotate(new Vector3(0, 90, 0));
            yield return new WaitForSeconds(1.2f);
            handleTxt.text = "OPEN";
        }
    }
}
