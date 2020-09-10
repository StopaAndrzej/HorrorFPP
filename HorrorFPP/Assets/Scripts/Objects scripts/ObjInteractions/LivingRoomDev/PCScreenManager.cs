using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PCScreenManager : InteractableObjectBase
{
    [SerializeField] private VideoPlayer videoClipManager;

    [SerializeField] private VideoClip pc_start;
    [SerializeField] private VideoClip pc_noConnection;
    [SerializeField] private VideoClip pc_boot;
    [SerializeField] private VideoClip pc_desktop;

    [SerializeField] private VideoClip mailBoxTransition;
    [SerializeField] private VideoClip mailBox;
    [SerializeField] private VideoClip updateMailbox;
    [SerializeField] private VideoClip mailBoxTransition1;
    private enum enPCCondition {noSignal, bootSystem, mailBoxWithUpdate, mailBox};
    private enPCCondition pcVideoMode;


    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;
    private KeyCode downKey = KeyCode.S;
    private KeyCode upKey = KeyCode.W;

    public bool screenOn;
    public bool interactiveScreenMode;
    public bool desktopDisplay;
    [SerializeField] private GameObject screen;
    [SerializeField] private Canvas screenCanvas;

    [SerializeField] private List<SpriteRenderer> icons;
    [SerializeField] private GameObject iconSelector;
    private Vector3 selectorOriginPos;
    private float selectorOffset = 0.09f;
    private int iconSelectorId;

    public bool animationInProgress = false;

    private void Start()
    {
        iconSelector.GetComponent<SpriteRenderer>().enabled = false;
        foreach (SpriteRenderer el in icons)
        {
            el.enabled = false;
        }

        selectorOriginPos = iconSelector.transform.position;
        screenOn = false;
        desktopDisplay = false;
        interactiveScreenMode = false;
        screen.SetActive(true);
        screenCanvas.enabled = false;
        interactText = "SWITCH ON";
        pcVideoMode = enPCCondition.noSignal;
    }

    private void Update()
    {
        if(iconSelector.GetComponent<SpriteRenderer>().enabled)
        {
            if (Input.GetKeyDown(downKey) && iconSelectorId<icons.Count-1)
            {
                if(iconSelectorId!=0 && iconSelectorId%3==0)
                    iconSelector.transform.localPosition = new Vector3(selectorOriginPos.x + 0.8f, selectorOriginPos.y, selectorOriginPos.z);
                else
                    iconSelector.transform.position -= new Vector3(0, selectorOffset, 0);

                iconSelectorId++;
            }
            else if(Input.GetKeyDown(upKey) && iconSelectorId>0)
            {
                if (iconSelectorId != 0 && iconSelectorId % 4 == 0)
                    iconSelector.transform.localPosition = new Vector3(selectorOriginPos.x - 0.8f, selectorOriginPos.y, selectorOriginPos.z);
                else
                    iconSelector.transform.position += new Vector3(0, selectorOffset, 0);

                iconSelectorId--;
            }
            else if(Input.GetKeyDown(interactionKey))
            {
                switch(iconSelectorId)
                {
                    case 1:
                        SetScreenMailBox();
                        break;
                }
            }
        }
    }

    public override void Interact()
    {
        if ((Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton)) && !animationInProgress)
        {
            if(screenOn)
            {
                screen.SetActive(true);
                screenCanvas.enabled = false;
                interactText = "SWITCH ON";
                iconSelector.GetComponent<SpriteRenderer>().enabled = false;
                foreach (SpriteRenderer el in icons)
                {
                    el.enabled = false;
                }
            }
            else
            {
                screen.SetActive(false);
                if(desktopDisplay)
                {
                    foreach (SpriteRenderer el in icons)
                    {
                        el.enabled = true;
                    }
                }
                if(interactiveScreenMode)
                {
                    iconSelector.GetComponent<SpriteRenderer>().enabled = true;
                }
                videoClipManager.clip = pc_start;
                videoClipManager.loopPointReached += SetClip;
                
                screenCanvas.enabled = true;
                interactText = "SWITCH OFF";

            }

            screenOn = !screenOn;
        }
    }

    void SetClip(UnityEngine.Video.VideoPlayer vp)
    {
        switch(pcVideoMode)
        {
            case enPCCondition.noSignal:
                vp.isLooping = true;
                vp.clip = pc_noConnection;
                break;

            case enPCCondition.bootSystem:
                vp.isLooping = true;
                vp.clip = pc_desktop;
                foreach (SpriteRenderer el in icons)
                {
                    el.enabled = true;
                }
                desktopDisplay = true;
                break;

            case enPCCondition.mailBoxWithUpdate:
                vp.clip = updateMailbox;
                pcVideoMode = enPCCondition.mailBox;
                vp.loopPointReached += SetClip;
                break;

            case enPCCondition.mailBox:
                vp.isLooping = true;
                vp.clip = mailBox;
                break;

        }
    }


    public void SetScreenBoot()
    {
        videoClipManager.isLooping = false;
        videoClipManager.clip = pc_boot;
        pcVideoMode = enPCCondition.bootSystem;
        videoClipManager.loopPointReached += SetClip;
    }

    public void KillConnection()
    {
        iconSelector.GetComponent<SpriteRenderer>().enabled = false;
        foreach (SpriteRenderer el in icons)
        {
            el.enabled = false;
        }
        screenOn = false;
        desktopDisplay = false;
        screen.SetActive(true);
        screenCanvas.enabled = false;
        interactText = "SWITCH ON";
        pcVideoMode = enPCCondition.noSignal;
    }

    public void SetScreenMailBox()
    {
        desktopDisplay = false;
        videoClipManager.clip = mailBoxTransition;
        videoClipManager.isLooping = false;
        iconSelector.GetComponent<SpriteRenderer>().enabled = false;
        foreach (SpriteRenderer el in icons)
        {
            el.enabled = false;
        }
        pcVideoMode = enPCCondition.mailBoxWithUpdate;
        videoClipManager.loopPointReached += SetClip;
    }

    public void InteractiveDesktop()
    {
        iconSelector.transform.position = selectorOriginPos;
        iconSelector.GetComponent<SpriteRenderer>().enabled = true;
        iconSelectorId = 0;
    }
}
