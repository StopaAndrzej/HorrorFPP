using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KettleManager1 : ItemBase
{
    public enum enKettleCondition { empty, waterCold, waterBoiled, waterBoiling};
    public enKettleCondition itemMode;

    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    private Animator animator;
    public bool animationInProgress = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        itemMode = enKettleCondition.empty;
        isRotationVertical = true;

        titleTxt = "KETTLE";
        titleTxt1 = "KETTLE - EMPTY";
        titleTxt2 = "KETTLE - COLD WATER";
        titleTxt3 = "KETTLE - BOILED WATER";

        pressTxt = "PRESS F TO INSPECT";
        pressTxt1 = "PRESS F TO OPEN";

        descriptionTxt = "AN ELECTRIC KETTLE IS USED TO BOIL WATER. IT MUST BE CONNEECTED TO A POWER STATION TO FUNCTION PROPERLY. THE WATER HEATING TIME IS APPROXIMATELY ONE MINUTE...";

        pickUpManager.title.text = titleTxt;

        inspectModeDirInteractionFlags[0] = true;
        inspectModeDirInteractionFlags[1] = false;
        inspectModeDirInteractionFlags[2] = false;
        inspectModeDirInteractionFlags[3] = true;

        delay = delayNormal;
        pressPos1 = -182.0f;
        pressPos2 = 182.0f;
    }

    public override string InteractionUp()
    {
        if (!discardUp && !animationInProgress)
        {
            if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
            {
                discardUp = true;
                return null;
            }
            return pressTxt1;
        }

        return null;
    }

    public override string InteractionBack()
    {
        if (!discardBack && !animationInProgress)
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


    public override string ShowInfoUp()
    {
        if (!discardUpFirstTime && discardUp)
        {
            discardFrontFirstTime = true;
            OpenDoor();
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
                fullTextToRead = descriptionTxt;

                pickUpManager.pressOffsetFlag = true;
                pickUpManager.freezeInspectRotationFlag = true;

                StartCoroutine(ShowText(discardBackFirstTimeFinished));
            }

            if (discardBackFirstTimeFinished)
            {
                return descriptionTxt;
            }

            return currentTextToRead;
        }

        return null;
    }

    public void OpenDoor()
    {
        animator.SetBool("isOpen", true);
    }
    public void lockRotation()
    {
        pickUpManager.freezeInspectRotationFlag = true;
        animationInProgress = true;
    }

    public void unlockRotation()
    {
        pickUpManager.freezeInspectRotationFlag = false;
        animationInProgress = false;

        if(itemMode == enKettleCondition.empty)
        {
            pickUpManager.title.text = titleTxt1;
        }
        else if(itemMode == enKettleCondition.waterCold)
        {
            pickUpManager.title.text = titleTxt2;
        }
        else if(itemMode == enKettleCondition.waterBoiled)
        {
            pickUpManager.title.text = titleTxt3;
        }
    }

}
