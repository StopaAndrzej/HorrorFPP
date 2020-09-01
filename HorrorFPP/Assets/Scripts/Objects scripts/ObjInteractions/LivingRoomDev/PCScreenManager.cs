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

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    public bool screenOn;
    [SerializeField] private GameObject screen;
    [SerializeField] private Canvas screenCanvas;

    public bool animationInProgress = false;

    private void Start()
    {
        screenOn = false;
        screen.SetActive(true);
        screenCanvas.enabled = false;
        interactText = "SWITCH ON";
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
            }
            else
            {
                videoClipManager.clip = pc_start;
                videoClipManager.loopPointReached += SetScreenOffline;
                screen.SetActive(false);
                screenCanvas.enabled = true;
                interactText = "SWITCH OFF";
            }

            screenOn = !screenOn;
        }
    }

    void SetScreenOffline(UnityEngine.Video.VideoPlayer vp)
    {
        vp.isLooping = true;
        vp.clip = pc_noConnection;
    }

    void SetScreenDesktop(UnityEngine.Video.VideoPlayer vp)
    {
        vp.isLooping = true;
        vp.clip = pc_desktop;
    }

    public void SetScreenBoot()
    {
        videoClipManager.isLooping = false;
        videoClipManager.clip = pc_boot;
        videoClipManager.loopPointReached += SetScreenDesktop;
    }
}
