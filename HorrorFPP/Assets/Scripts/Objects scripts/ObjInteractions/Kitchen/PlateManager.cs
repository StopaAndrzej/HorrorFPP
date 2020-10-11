using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateManager : ItemBase
{
    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;
    public enum enFoodCondition { clean, dirty, soaped, food, broken };
    public enFoodCondition itemMode = enFoodCondition.clean;

    public DishSoapManager dishSoap;

    [SerializeField] private GameObject solidPlate;
    [SerializeField] private GameObject solidPlateDirty;
    [SerializeField] private GameObject solidPlateDirtySoap;

    [SerializeField] private GameObject brokenPlate;
    [SerializeField] private List<GameObject> brokenPlateDirty;
    [SerializeField] private List<GameObject> brokenPlateDirtySoap;

    [SerializeField] public GameObject foam;

    void Start()
    {
        delay = delayNormal;
        pressPos1 = -182.0f;
        pressPos2 = 182.0f;

        pressTxt = "PRESS F TO INSPECT";

        descriptionTxt = "CLEAN PLATE. CAN BE COMBINED WITH FOOD TO CREATE A MEAL. IT CAN BREAK EASLY...";
        descriptionTxt1 = "DIRTY PLATE. IT MUST BE WASHED IN THE SINK TO BE USED AGAIN. FIRST USE WASHINH UP LIQUID, THEN RINSE WITH WATER...";

        inspectModeDirInteractionFlags[0] = false;
        inspectModeDirInteractionFlags[1] = false;
        inspectModeDirInteractionFlags[2] = false;
        inspectModeDirInteractionFlags[3] = true;

        foam.SetActive(false);

        solidPlate.SetActive(true);
        if (itemMode == enFoodCondition.dirty)
        {
            solidPlateDirty.SetActive(true);
            foreach (GameObject el in brokenPlateDirty)
                el.SetActive(true);
        }
        else
        {
            solidPlateDirty.SetActive(false);
            foreach (GameObject el in brokenPlateDirty)
                el.SetActive(false);
        }

        solidPlateDirtySoap.SetActive(false);
        foreach (GameObject el in brokenPlateDirtySoap)
            el.SetActive(false);

    }

    public override string InteractionBack()
    {
        if (!discardBack)
        {
            if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
            {
                discardBack = true;
                return null;
            }
            return pressTxt;
        }
        return null;
    }

    public override string ShowInfoBack()
    {
        if (discardBack)
        {
            if (!discardBackFirstTime)
            {
                discardBackFirstTime = true;
                discardBackFirstTimeFinished = false;
                delay = delayNormal;

                if (itemMode == enFoodCondition.dirty)
                    fullTextToRead = descriptionTxt1;
                else
                    fullTextToRead = descriptionTxt;

                pickUpManager.pressOffsetFlag = true;
                pickUpManager.freezeInspectRotationFlag = true;

                StartCoroutine(ShowText(discardBackFirstTimeFinished));
            }

            if (discardBackFirstTimeFinished)
            {
                if (itemMode == enFoodCondition.dirty)
                    return descriptionTxt1;

                return descriptionTxt;
            }


            return currentTextToRead;
        }

        return null;
    }


    public void ChangeState(enFoodCondition state)
    {
        itemMode = state;
        ResetValues();
        if (state == enFoodCondition.dirty)
            titleTxt = "DIRTY_PLATE";
        else if (state == enFoodCondition.clean)
            titleTxt = "PLATE";
    }

    public override void SwitchToBreakMode()
    {
        solidPlate.SetActive(false);
        brokenPlate.SetActive(true);
    }

    public void ChangeSolidDirty()
    {
        solidPlateDirty.SetActive(true);
        solidPlateDirtySoap.SetActive(false);

        foreach (GameObject el in brokenPlateDirty)
            el.SetActive(true);

        foreach (GameObject el in brokenPlateDirtySoap)
            el.SetActive(false);

        ChangeState(enFoodCondition.dirty);
    }

    public void ChangeSolidDirtySoap()
    {
        solidPlateDirty.SetActive(false);
        solidPlateDirtySoap.SetActive(true);

        foreach (GameObject el in brokenPlateDirty)
            el.SetActive(false);

        foreach (GameObject el in brokenPlateDirtySoap)
            el.SetActive(true);

        ChangeState(enFoodCondition.soaped);
    }

    public void ChangeSolidClear()
    {
        solidPlateDirty.SetActive(false);
        solidPlateDirtySoap.SetActive(false);

        foreach (GameObject el in brokenPlateDirty)
            el.SetActive(false);

        foreach (GameObject el in brokenPlateDirtySoap)
            el.SetActive(false);

        ChangeState(enFoodCondition.clean);
    }

}
