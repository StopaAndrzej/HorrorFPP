using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TVManager : InteractableObjectBase
{
    [SerializeField] private VideoPlayer videoClipManager;

    [SerializeField] private VideoClip tv_snow;
    [SerializeField] private VideoClip tv_standBy;
    [SerializeField] private VideoClip tv_creepyMovie1;

    [SerializeField] private Canvas screenCanvas;
    [SerializeField] private GameObject offScreen;

    public KeyCode button = KeyCode.Mouse0;

    public bool tvSwitchOn = false;

    private void Start()
    {
        offScreen.SetActive(true);
        screenCanvas.enabled = false;
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(button))
        {
            if(tvSwitchOn)
            {
                offScreen.SetActive(true);
                screenCanvas.enabled = false;
            }
            else
            {
                offScreen.SetActive(false);
                videoClipManager.clip = tv_snow;
                screenCanvas.enabled = true;

            }

            tvSwitchOn = !tvSwitchOn;
        }
    }

    public void SetStandBy()
    {
        videoClipManager.isLooping = false;
        videoClipManager.clip = tv_standBy;
        videoClipManager.loopPointReached += SetMovie1;
    }

    void SetMovie1(UnityEngine.Video.VideoPlayer vp)
    {
        vp.isLooping = false;
        vp.clip = tv_creepyMovie1;
    }
}
