using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FocusSwitcher : MonoBehaviour
{
    public string FocusedLayer = "Focused";

    private GameObject currentlyFocused;
    private int previousLayer;

    //display fx to on/off for diff modes
    [SerializeField] private PostProcessVolume defaultCameraEffect;
    [SerializeField] private PostProcessVolume inspectCameraEffect;

    public void SetFocused(GameObject obj)
    {
        // enables this camera and the postProcessingVolume which is the child
        //gameObject.SetActive(true);
        inspectCameraEffect.enabled = true;
        defaultCameraEffect.enabled = false;

        // if something else was focused before reset it
        if (currentlyFocused) currentlyFocused.layer = previousLayer;

        // store and focus the new object
        currentlyFocused = obj;

        if (currentlyFocused)
        {
            previousLayer = currentlyFocused.layer;
            currentlyFocused.layer = LayerMask.NameToLayer(FocusedLayer);
        }
        else
        {
            // if no object is focused disable the FocusCamera
            // and PostProcessingVolume for not wasting rendering resources
            //gameObject.SetActive(false);
            inspectCameraEffect.enabled = false;
            defaultCameraEffect.enabled = true;
        }
    }

    // On disable make sure to reset the current object
    private void OnDisable()
    {
        if (currentlyFocused) currentlyFocused.layer = previousLayer;

        currentlyFocused = null;
    }
}
