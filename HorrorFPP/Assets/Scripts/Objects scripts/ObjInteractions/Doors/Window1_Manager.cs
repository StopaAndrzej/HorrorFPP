using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Window1_Manager : InteractableObjectBase
{
    [SerializeField] private List<Canvas> canvases;

    public KeyCode overrideButton;
    public KeyCode openButton;

    [SerializeField] private Text overrideTxt;
    [SerializeField] private Text openTxt;

    private bool isOverride = false;
    private bool isOpen = false;

    //wait until animation finish to interact again
    public bool animationInProgress = false;

    [SerializeField] private Animator animator;

    //when some objects must be move to open window
    public List<GameObject> additionalObjectsToMove;
    public Transform[] alternativeLocationObjects;
    private Vector3[] defaultLocationObjectsPos = new Vector3[10];
    private Quaternion[] defaultLocationObjectsRot = new Quaternion[10];

    private void Awake()
    {
        int i = 0;
        foreach (GameObject element in additionalObjectsToMove)
        {
            defaultLocationObjectsPos[i] = element.GetComponent<Transform>().localPosition;
            defaultLocationObjectsRot[i] = element.GetComponent<Transform>().localRotation;
            i++;
        }
    }
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
            if (Input.GetKeyDown(overrideButton) || kidID == 1)
            {
                if(!isOpen)
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
                }

                isOverride = !isOverride;
                kidID = 0;
            }
            else if(Input.GetKeyDown(openButton) || kidID == 2)
            {
                if(!isOverride)
                {
                    if (isOpen)
                    {
                        openTxt.text = "CLOSE";
                        animator.SetBool("isFullOpen", false);
                        StartCoroutine(Opening());
                    }
                    else
                    {
                        animator.SetBool("isFullOpen", true);
                        StartCoroutine(Opening());
                    }

                    isOpen = !isOpen;
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
        if (!isOpen)
        {
            yield return new WaitForSeconds(0.6f);
            this.GetComponent<Transform>().Rotate(new Vector3(0, -90, 0));

            //move additional objects
            int i = 0;
            foreach (GameObject element in additionalObjectsToMove)
            {
                if (alternativeLocationObjects[i] == null)
                    break;

                element.GetComponent<Transform>().localPosition = alternativeLocationObjects[i].localPosition;
                element.GetComponent<Transform>().localRotation = alternativeLocationObjects[i].localRotation;
                i++;
            }
        }
        else
        {
            this.GetComponent<Transform>().Rotate(new Vector3(0, 90, 0));
            
            //move additional objects
            int i = 0;
            foreach (GameObject element in additionalObjectsToMove)
            {
                if (defaultLocationObjectsPos[i] == null)
                    break;

                element.GetComponent<Transform>().localPosition = defaultLocationObjectsPos[i];
                element.GetComponent<Transform>().localRotation = defaultLocationObjectsRot[i];
                i++;
            }


            yield return new WaitForSeconds(1.0f);
            openTxt.text = "OPEN";
        }
    }
}
