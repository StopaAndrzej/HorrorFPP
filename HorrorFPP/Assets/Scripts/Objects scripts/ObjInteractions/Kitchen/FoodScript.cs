using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FoodScript : ItemBase
{
    private enum enFoodCondition { cold, heated, unapckCold, spoiled };
    private enFoodCondition itemMode;

    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] private List<GameObject> foilBag;

    private float fadeValue;
    [SerializeField] private float maxFadeValue;
    [SerializeField] private float fadeSpeed =0.1f;
    [SerializeField] private PostProcessVolume cameraPostProcVolume;

    private void Start()
    {
        itemMode = enFoodCondition.cold;

        titleTxt = "FOIL-WRAPPED LUNCH";
        titleTxt1 = "COLD LUNCH";
        titleTxt2 = "HEATED LUNCH";
        titleTxt2 = "SPOILED LUNCH";

        pressTxt = "PRESS F TO INSPECT";
        pressTxt1 = "PRESS F TO UNPACK";

        descriptionTxt = "COLD DINNER PLATE PACKED IN FOIL PAPER. \nTRHERE IS A STICKY NOTE ON IT. \n- UNPACK AND HEAT ME UP..";
        descriptionTxt1 = "A FOIL BAG TIED WITH A KNOT";
        descriptionTxt2 = "A PLATE OF POTATOES WITH PORK CHOP AND SALAD.\n READY TO HEAT.";
        descriptionTxt3 = "A WARM AND WELL-PREPARED MEAL./n TO EAT IT YOU NEED CUTLERY AND A COMFORTABLE SEAT. /n YOU CAN ALSO HAVE A DRINK.";
        descriptionTxt4 = "A SPOILED MEAL. IT SHOULD NOT BE HEATED THAT LONG./n TO EAT IT YOU NEED CUTLERY AND A COMFORTABLE SEAT. /n YOU CAN ALSO HAVE A DRINK.";

        inProgressBarTxt = "...UNPACKING...";

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
        if(itemMode == enFoodCondition.cold)
        {
            if(!discardUp)
            {
                if(Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
                {
                    discardUp = true;
                }
                return pressTxt;
            }

                return null;
        }
        else if((itemMode == enFoodCondition.unapckCold))
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


    public IEnumerator OpenFadeOut(bool valueFlag)
    {
        pickUpManager.inProgressBar.text = inProgressBarTxt;
        pickUpManager.pulseTextLoopFlag = true;
        pickUpManager.InProgressShowTextPulseLoop();

        if (!valueFlag)
        {
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

        if (valueFlag)
        {
            while (fadeValue < 0f)
            {
                fadeValue += fadeSpeed;
                cameraPostProcVolume.profile.TryGetSettings(out ColorGrading colorGradingLayer);
                colorGradingLayer.postExposure.value = fadeValue;
                yield return null;
            }
        }
    }
}
