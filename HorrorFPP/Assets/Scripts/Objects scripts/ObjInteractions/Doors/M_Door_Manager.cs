using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class M_Door_Manager : InteractableObjectBase
{
    [SerializeField] private FindInteraction findInteraction;
    [SerializeField] private List<Canvas> canvases;

    public KeyCode handleButton;
    public KeyCode judasButton;
    public KeyCode lockButton;
    public KeyCode exitButton;

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
    private Vector3 destinationPos;
    private Quaternion destinationRot;

    private Vector3  originalPlayerCameraPos;
    private Quaternion originalPlayerCameraRot;

    [SerializeField] private GameObject playerCamera;
    [SerializeField] private PlayerMove playerMove;

    private bool lerpCameraMovement = false;
    private bool cameraMovementMotionReverse = false;

    public float smoothMoveSpeed = 0.125f;
    public float smoothRotationSpeed = 0.125f;
    public float fadeSpeed = 0.2f;

    [SerializeField] private PostProcessVolume cameraPostProcVolume;
    [SerializeField] private PostProcessVolume peepHolePostProcVolume;

    void Start()
    {
        animator = GetComponent<Animator>();
        interactText = "DOOR";

        foreach (Canvas element in canvases)
        {
            element.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if(lerpCameraMovement)
        {
            cameraPostProcVolume.profile.TryGetSettings(out ColorGrading colorGradingLayer);

            if (!cameraMovementMotionReverse)
            {
                destinationPos = cameraJudasPos.position;
                destinationRot = cameraJudasPos.rotation;

                colorGradingLayer.postExposure.value = Mathf.Lerp(colorGradingLayer.postExposure.value, -50.0f, Time.deltaTime * fadeSpeed);
            }
            else
            {
                destinationPos = originalPlayerCameraPos;
                destinationRot = originalPlayerCameraRot;

                cameraPostProcVolume.enabled = true;
                peepHolePostProcVolume.enabled = false;

                colorGradingLayer.postExposure.value = 0;
            }


            Vector3 smoothedPos = Vector3.Lerp(playerCamera.GetComponent<Transform>().position, destinationPos, smoothMoveSpeed);
            Quaternion smoothedRot = Quaternion.Lerp(playerCamera.GetComponent<Transform>().rotation, destinationRot, smoothRotationSpeed);

            if (destinationPos == playerCamera.GetComponent<Transform>().position)
            {
                lerpCameraMovement = false;
               // animationInProgress = false;

                if(cameraMovementMotionReverse)
                {
                    playerMove.disablePlayerController = false;
                    findInteraction.peepHoleMode = false;
                }
                else
                {
                    colorGradingLayer.postExposure.value = -50.0f;
                    playerCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("IgnoreLights"));
                    cameraPostProcVolume.enabled = false;
                    peepHolePostProcVolume.enabled = true;
                    findInteraction.peepHoleMode = true;
                }
            }
            else
            {
                playerCamera.GetComponent<Transform>().position = smoothedPos;
                playerCamera.GetComponent<Transform>().rotation = smoothedRot;
            }
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
            if ((Input.GetKeyDown(handleButton) || kidID==1) && !isJudas)
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
            else if ((Input.GetKeyDown(judasButton) || kidID == 2) && !isJudas)
            {
                playerMove.disablePlayerController = true;
                lerpCameraMovement = true;

                isJudas = true;
                cameraMovementMotionReverse = false; 

                originalPlayerCameraPos = playerCamera.GetComponent<Transform>().position;
                originalPlayerCameraRot = playerCamera.GetComponent<Transform>().rotation;

                kidID = 0;
            }
            else if ((Input.GetKeyDown(lockButton) || kidID == 3)  && !isJudas)
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
            else if(Input.GetKeyDown(exitButton) && isJudas)
            {
                isJudas = false;
                cameraMovementMotionReverse = true;
                lerpCameraMovement = true;
                //animationInProgress = true;
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
