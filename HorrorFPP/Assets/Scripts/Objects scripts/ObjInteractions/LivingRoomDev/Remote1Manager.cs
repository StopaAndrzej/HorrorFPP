using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remote1Manager : ItemBase
{
    public enum enFoodCondition { emptyClosed, emptyOpened, loadedOpen, loadedClosed };
    public enFoodCondition itemMode;

    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    private Animator animator;

    [SerializeField] private GameObject batteries;

    private void Start()
    {
        animator = GetComponent<Animator>();

        itemMode = enFoodCondition.emptyClosed;
        isRotationVertical = false;

        batteries.SetActive(false);

        titleTxt = "REMOTE CONTROL";

        pressTxt = "PRESS F TO INSPECT";
        pressTxt1 = "PRESS F TO OPEN";
        pressTxt2 = "PRESS F TO LOAD BATTERIES";

        descriptionTxt = "WHEN IT IS POINTED AT THE RECEIVER,\nIT IS USED TO CHANGE TV STATIONS...";

        controlText = "...PRESS (MOUSE1) TO GRAB IT...\n...PRESS(ESC) TO PUT IT BACK.....";

        inspectModeDirInteractionFlags[0] = false;
        inspectModeDirInteractionFlags[1] = false;
        inspectModeDirInteractionFlags[2] = true;
        inspectModeDirInteractionFlags[3] = true;

        delay = delayNormal;
        pressPos1 = -182.0f;
        pressPos2 = 182.0f;
    }

    public override string InteractionFront()
    {
        if (itemMode == enFoodCondition.emptyClosed || itemMode == enFoodCondition.loadedClosed)
        {
            if (!discardBack)
            {
                if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
                {
                    discardBack = true;
                }
                return pressTxt1;
            }

        }
        else if (itemMode == enFoodCondition.emptyOpened || itemMode == enFoodCondition.loadedOpen)
        {
            if (!discardBack)
            {
                if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
                {
                    discardBack = true;
                }
                return pressTxt2;
            }

        }

        return null;
    }

    public override string InteractionBack()
    {
        if (!discardFront)
        {
            if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
            {
                discardFront = true;
                return null;
            }
            return pressTxt;
        }

        return null;
    }

    public override string ShowInfoFront()
    {
        if (itemMode == enFoodCondition.emptyClosed)
        {
            if (discardBack)
            {
                if (!discardBackFirstTime)
                {
                    discardBackFirstTime = true;
                    OpenLidAnim();
                }
            }
        }

        else if (itemMode == enFoodCondition.emptyOpened)
        {
            if (discardBack)
            {
                if (!discardBackFirstTime)
                {
                    discardBackFirstTime = true;
                    LoadBatteriesAnim();
                }
            }
        }
        return null;
    }

    public override string ShowInfoBack()
    {
        if (discardFront)
        {
            if (!discardFrontFirstTime)
            {
                discardFrontFirstTime = true;
                discardFrontFirstTimeFinished = false;
                delay = delayNormal;
                fullTextToRead = descriptionTxt;

                pickUpManager.pressOffsetFlag = true;
                pickUpManager.freezeInspectRotationFlag = true;

                StartCoroutine(ShowText(discardFrontFirstTimeFinished));
            }

            if (discardFrontFirstTimeFinished)
            {
                return descriptionTxt;
            }

            return currentTextToRead;
        }

        return null;
    }

    private void OpenLidAnim()
    {
        animator.SetBool("isOpen", true);
    }

    private void CloseLidAnim()
    {
        animator.SetBool("isOpen", false);
    }

    private void LoadBatteriesAnim()
        
    {
        batteries.transform.localScale = new Vector3(1, 1, 0.6f);
        animator.SetBool("reloadBatteries", true);
    }

    public void lockRotation()
    {
        pickUpManager.freezeInspectRotationFlag = true;
    }

    public void lockDownBatteries()
    {
        animator.SetBool("isLoad", true);
        animator.SetBool("isOpen", false);
    }

    public void makeBatteriesVisible()
    {
        batteries.SetActive(true);
    }

    public void unlockRotation()
    {
        if (itemMode == enFoodCondition.emptyClosed)
            itemMode = enFoodCondition.emptyOpened;
        else if (itemMode == enFoodCondition.emptyOpened)
            itemMode = enFoodCondition.emptyClosed;
        else if (itemMode == enFoodCondition.loadedClosed)
            itemMode = enFoodCondition.loadedOpen;
        else if (itemMode == enFoodCondition.loadedOpen)
            itemMode = enFoodCondition.loadedClosed;

        pickUpManager.freezeInspectRotationFlag = false;
        discardBack = false;
        discardBackFirstTime = false;
        discardFront = false;
        discardFrontFirstTime = false;

    }
}
