﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MopScript : ItemBase
{
    private enum enItemCondition { clear, dirty};
    private enItemCondition itemMode;

    [SerializeField] private Material dirtyTexture;
    private Material cleanTexture;

    [SerializeField] private List<GameObject> cloth;

    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    private int mopConditon = 0;        //+1 after destroying footprint, 4 - change state to dirty

    private void Start()
    {
        itemMode = enItemCondition.clear;

        cleanTexture = cloth[0].GetComponent<MeshRenderer>().material;

        titleTxt = "MOP";
        titleTxt1 = "DIRTY MOP";

        pressTxt = "PRESS F TO INSPECT";

        descriptionTxt = "IT CAN BE USED TO WASH DIRTY SURFACES.\nDIRTY SHOULD BE WASHED OFF IN A BUCKET OF WATER...";
        descriptionTxt1 = "IT SHOULD BE RINSED IN A BUCKET OF WATER TO BE ABLE TO CONTINUE USING IT FOR CLEANING";

        inspectModeDirInteractionFlags[0] = false;
        inspectModeDirInteractionFlags[1] = true;
        inspectModeDirInteractionFlags[2] = false;
        inspectModeDirInteractionFlags[3] = false;

        delay = delayNormal;
        pressPos1 = -182.0f;
        pressPos2 = 182.0f;
    }

    public override string InteractionDown()
    {

        if (!discardDown)
        {
            if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
            {
                discardDown = true;
            }
            return pressTxt;
        }

        return null;
    }

    public override string ShowInfoDown()
    {
        if (discardDown)
        {
            if (discardUp)
            {
                if (!discardUpFirstTime)
                {
                    discardUpFirstTime = true;
                    discardUpFirstTimeFinished = false;
                    delay = delayNormal;
                    fullTextToRead = descriptionTxt;

                    pickUpManager.pressOffsetFlag = true;
                    pickUpManager.freezeInspectRotationFlag = true;

                    StartCoroutine(ShowText(discardUpFirstTimeFinished));
                }

                if (discardUpFirstTimeFinished)
                {
                    return descriptionTxt;
                }

                return currentTextToRead;
            }
        }
        
        return null;
    }

    public void ClearMop()
    {
        foreach(GameObject el in cloth)
        {
            el.GetComponent<MeshRenderer>().material = cleanTexture;
        }
        pickUpManager.title.text = titleTxt;
    }

    public void GetMopDirty()
    {
        foreach (GameObject el in cloth)
        {
            el.GetComponent<MeshRenderer>().material = dirtyTexture;
        }
        pickUpManager.title.text = titleTxt1;
    }

    public bool Use()
    {
        mopConditon++;

        if(mopConditon>=4)
        {
            GetMopDirty();
            return false;
        }

        return true;
    }
}
