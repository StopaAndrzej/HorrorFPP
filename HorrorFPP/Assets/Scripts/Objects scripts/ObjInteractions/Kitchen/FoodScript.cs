﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FoodScript : ItemBase
{
    public enum enFoodCondition { cold, heated, unapckCold, spoiled };
    public enFoodCondition itemMode;

    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] private List<GameObject> foilBag;
    

    private float fadeValue;
    [SerializeField] private float maxFadeValue;
    [SerializeField] private float fadeSpeed =0.1f;
    [SerializeField] private PostProcessVolume cameraPostProcVolume;

    [SerializeField] private GameObject pork;
    [SerializeField] private GameObject potato;
    [SerializeField] private GameObject salad;
    [SerializeField] private GameObject plate;

    [SerializeField] private Texture porkSpoiled;
    [SerializeField] private Texture porkNoSpoiled;
    [SerializeField] private Texture potatoSpoiled;
    [SerializeField] private Texture potatoNoSpoiled;
    [SerializeField] private Texture saladSpoiled;
    [SerializeField] private Texture saladNoSpoiled;

    [SerializeField] private Texture plateClear;
    [SerializeField] private Texture plateDirty;


    private void Start()
    {
        itemMode = enFoodCondition.cold;

        titleTxt = "PACKED LUNCH";
        titleTxt1 = "COLD LUNCH";
        titleTxt2 = "HEATED LUNCH";
        titleTxt3 = "SPOILED LUNCH";

        pressTxt = "PRESS F TO INSPECT";
        pressTxt1 = "PRESS F TO UNPACK";

        descriptionTxt = "COLD DINNER PLATE PACKED IN FOIL PAPER.\nTRHERE IS A STICKY NOTE ON IT.\n- UNPACK AND HEAT ME UP...";
        descriptionTxt1 = "SPOILED DINNER PLATE PACKED IN FOIL PAPER.\n IT SHOULD NOT BE HEATED WITH FOIL BAG.\nONLY SUITABLE FOR DISPOSAL...";
        descriptionTxt2 = "A PLATE OF POTATOES WITH PORK CHOP\nAND SALAD. READY TO HEAT...";
        descriptionTxt3 = "A WARM AND WELL-PREPARED MEAL./n TO EAT IT YOU NEED CUTLERY AND A COMFORTABLE SEAT. /n YOU CAN ALSO HAVE A DRINK.";
        descriptionTxt4 = "A SPOILED MEAL. IT SHOULD NOT BE HEATED THAT LONG./nONLY SUITABLE FOR DISPOSAL.";

        inProgressBarTxt = "...UNPACKING...";

        pickUpManager.title.text = titleTxt;
        pickUpManager.press.text = pressTxt;
        pickUpManager.description.text = descriptionTxt;
        pickUpManager.inProgressBar.text = inProgressBarTxt;

        pork.GetComponent<MeshRenderer>().material.SetTexture(1,porkNoSpoiled);
        potato.GetComponent<MeshRenderer>().material.SetTexture(1, potatoNoSpoiled);
        salad.GetComponent<MeshRenderer>().material.SetTexture(1, saladNoSpoiled);
        plate.GetComponent<MeshRenderer>().material.SetTexture(1, plateClear);

        inspectModeDirInteractionFlags[0] = true;
        inspectModeDirInteractionFlags[1] = true;
        inspectModeDirInteractionFlags[2] = false;
        inspectModeDirInteractionFlags[3] = false;

        delay = delayNormal;
        pressPos1 = -182.0f;
        pressPos2 = 182.0f;

        foreach (GameObject element in foilBag)
            element.SetActive(true);
    }


    public override string InteractionUp()
    {

        if(!discardUp)
        {
            if(Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
            {
                discardUp = true;
                return null;
            }
            return pressTxt;
        }
     
        return null;
    }

    public override string InteractionDown()
    {
        if (itemMode == enFoodCondition.cold)
        {
            if (!discardDown)
            {
                if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
                {
                    discardDown = true;
                }
                return pressTxt1;
            }

            return null;
        }
        else if ((itemMode == enFoodCondition.unapckCold))
        {
            if (!discardUp)
            {
                return pressTxt;
            }
            else
            {
                return null;
            }
        }
        else if ((itemMode == enFoodCondition.heated))
        {
            if (!discardUp)
            {
                return pressTxt;
            }
            else
            {
                return null;
            }
        }
        else if ((itemMode == enFoodCondition.spoiled))
        {
            if (!discardUp)
            {
                return pressTxt;
            }
            else
            {
                return null;
            }
        }

        return null;
    }

    public override string ShowInfoUp()
    {
        if (itemMode == enFoodCondition.cold)
        {
            if(discardUp)
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

                if(discardUpFirstTimeFinished)
                {
                    return descriptionTxt;
                }

                return currentTextToRead;
            }
        }
        else if(itemMode == enFoodCondition.unapckCold)
        {
            if (discardUp)
            {
                if (!discardUpFirstTime)
                {
                    discardUpFirstTime = true;
                    discardUpFirstTimeFinished = false;
                    delay = delayNormal;
                    fullTextToRead = descriptionTxt2;
                    pickUpManager.pressOffsetFlag = true;
                    pickUpManager.freezeInspectRotationFlag = true;

                    StartCoroutine(ShowText(discardUpFirstTimeFinished));
                }

                if(discardUpFirstTimeFinished)
                {
                    return descriptionTxt2;
                }

                return currentTextToRead;
            }
        }

        return null;
    }

    public override string ShowInfoDown()
    {
        if (itemMode == enFoodCondition.cold)
        {
            if (discardDown)
            {
                if (!discardDownFirstTime)
                {
                    discardDownFirstTime = true;
                    discardDownFirstTimeFinished = false;
                    StartCoroutine(OpenFadeOut(discardDownFirstTimeFinished));
                }
            }
        }
        return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(itemDrop&& itemMode != enFoodCondition.cold && collision.collider.tag == "Surface")
        {
            itemDrop = false;
            Debug.Log("Crash it!");
            this.transform.rotation = new Quaternion(180, 0, 0, 0);
            pork.transform.localPosition += new Vector3(-0.2f, 0, 0);
            potato.transform.localPosition += new Vector3(0.3f, 0, 0);
            salad.transform.localPosition += new Vector3(0, 0, -0.3f);
            plate.transform.localPosition += new Vector3(0, -0.08f, 0);
        }
    }

    public IEnumerator OpenFadeOut(bool valueFlag)
    {
        if (!valueFlag)
        {
            pickUpManager.inProgressBar.text = inProgressBarTxt;
            pickUpManager.inProgressBar.enabled = true;
            StartCoroutine(pickUpManager.FadeOutTextLoop(pickUpManager.title));
            StartCoroutine(pickUpManager.FadeOutTextLoop(pickUpManager.description));
            StartCoroutine(pickUpManager.FadeOutTextLoop(pickUpManager.press));
            StartCoroutine(pickUpManager.FadeOutTextLoop(pickUpManager.controlInfo));
            StartCoroutine(pickUpManager.FadeInTextLoop(pickUpManager.inProgressBar));


            while (fadeValue > -10f)
            {
                fadeValue -= fadeSpeed;
                cameraPostProcVolume.profile.TryGetSettings(out ColorGrading colorGradingLayer);
                colorGradingLayer.postExposure.value = fadeValue;
                yield return null;
            }

            valueFlag = true;
            foreach (GameObject element in foilBag)
                element.SetActive(false);
        }

        pickUpManager.pulseTextLoopFlag = false;

        pickUpManager.title.text = titleTxt1;
        pickUpManager.description.text = descriptionTxt2;

        if (valueFlag)
        {

            pickUpManager.title.color = new Vector4(pickUpManager.title.color.r, pickUpManager.title.color.g, pickUpManager.title.color.b, 0);
            pickUpManager.controlInfo.color = new Vector4(pickUpManager.controlInfo.color.r, pickUpManager.controlInfo.color.g, pickUpManager.controlInfo.color.b, 0);
            pickUpManager.description.color = new Vector4(pickUpManager.description.color.r, pickUpManager.description.color.g, pickUpManager.description.color.b, 0);

            StartCoroutine(pickUpManager.FadeInTextLoop(pickUpManager.title));
            StartCoroutine(pickUpManager.FadeInTextLoop(pickUpManager.controlInfo));
            StartCoroutine(pickUpManager.FadeOutTextLoop(pickUpManager.inProgressBar));

            while (fadeValue < 0f)
            {
                fadeValue += fadeSpeed;
                cameraPostProcVolume.profile.TryGetSettings(out ColorGrading colorGradingLayer);
                colorGradingLayer.postExposure.value = fadeValue;
                yield return null;
            }

            pickUpManager.press.color = new Vector4(pickUpManager.press.color.r, pickUpManager.press.color.g, pickUpManager.press.color.b, 1);
            pickUpManager.description.color = new Vector4(pickUpManager.description.color.r, pickUpManager.description.color.g, pickUpManager.description.color.b, 1);
        }

        valueFlag = false;

        //next state
        itemMode = enFoodCondition.unapckCold;
        pickUpManager.pressOffsetFlag = false;

        inspectModeDirInteractionFlags[0] = true;
        inspectModeDirInteractionFlags[1] = false;
        inspectModeDirInteractionFlags[2] = false;
        inspectModeDirInteractionFlags[3] = false;

        ResetValues();


    }

    public void ChangeState()
    {
        if(itemMode == enFoodCondition.heated)
        {
            pickUpManager.title.text = titleTxt3;
            pickUpManager.description.text = descriptionTxt4;

            pickUpManager.pressOffsetFlag = false;

            pork.GetComponent<MeshRenderer>().material.SetTexture(1, porkSpoiled);
            potato.GetComponent<MeshRenderer>().material.SetTexture(1, potatoSpoiled);
            salad.GetComponent<MeshRenderer>().material.SetTexture(1, saladSpoiled);
            itemMode = enFoodCondition.spoiled;

            inspectModeDirInteractionFlags[0] = true;
            inspectModeDirInteractionFlags[1] = false;
            inspectModeDirInteractionFlags[2] = false;
            inspectModeDirInteractionFlags[3] = false;

            ResetValues();

            itemMode = enFoodCondition.spoiled;

        }
        else if (itemMode == enFoodCondition.unapckCold)
        {
            pickUpManager.title.text = titleTxt2;
            pickUpManager.description.text = descriptionTxt3;

            pickUpManager.pressOffsetFlag = false;

            inspectModeDirInteractionFlags[0] = true;
            inspectModeDirInteractionFlags[1] = false;
            inspectModeDirInteractionFlags[2] = false;
            inspectModeDirInteractionFlags[3] = false;

            ResetValues();

            itemMode = enFoodCondition.heated;
        }
        else if(itemMode == enFoodCondition.cold)
        {
            pickUpManager.title.text = titleTxt3;
            pickUpManager.description.text = descriptionTxt1;

            pork.GetComponent<MeshRenderer>().material.SetTexture(1, porkSpoiled);
            potato.GetComponent<MeshRenderer>().material.SetTexture(1, potatoSpoiled);
            salad.GetComponent<MeshRenderer>().material.SetTexture(1, saladSpoiled);

            inspectModeDirInteractionFlags[0] = true;
            inspectModeDirInteractionFlags[1] = false;
            inspectModeDirInteractionFlags[2] = false;
            inspectModeDirInteractionFlags[3] = false;

            ResetValues();

            itemMode = enFoodCondition.spoiled;
        }
    }
}
//descriptionTxt1