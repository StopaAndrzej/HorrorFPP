using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDBoxManager : ItemBase
{
    public enum enFoodCondition { emptyClosed, emptyOpened, cdInsideOpen, cdInsideClosed };
    public enFoodCondition itemMode;

    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    private Animator animator;
    [SerializeField] GameObject CDObj;

    void Start()
    {
        animator = GetComponent<Animator>();
        isRotationVertical = false;

        itemMode = enFoodCondition.cdInsideClosed;

        titleTxt = "UNKNOWN DVD BOX";

        pressTxt = "PRESS F TO INSPECT";
        pressTxt1 = "PRESS F TO OPEN";
        pressTxt2 = "PRESS F TO GRAB DISC";

        descriptionTxt = "AVE AVE SATIAFANA AVE SAJMONELLA...";
        controlText = "...PRESS(ESC) TO PUT IT BACK.....";

       

        inspectModeDirInteractionFlags[0] = false;
        inspectModeDirInteractionFlags[1] = false;
        inspectModeDirInteractionFlags[2] = true;
        inspectModeDirInteractionFlags[3] = true;

        delay = delayNormal;
        pressPos1 = -182.0f;
        pressPos2 = 182.0f;
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

    public override string InteractionFront()
    {
        if (itemMode == enFoodCondition.cdInsideClosed || itemMode == enFoodCondition.emptyClosed)
        {
            if (!discardFront)
            {
                if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
                {
                    discardFront = true;
                }
                return pressTxt1;
            }

        }
        else if (itemMode == enFoodCondition.cdInsideOpen)
        {
            if (!discardFront)
            {
                if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
                {
                    discardFront = true;
                }
                return pressTxt2;
            }

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

    public override string ShowInfoFront()
    {
        if (itemMode == enFoodCondition.emptyClosed || itemMode == enFoodCondition.cdInsideClosed)
        {
            if (discardFront)
            {
                if (!discardFrontFirstTime)
                {
                    discardFrontFirstTime = true;
                    OpenLidAnim();
                }
            }
        }
        else if (itemMode == enFoodCondition.emptyOpened )
        {
            if (discardFront)
            {
                if (!discardFrontFirstTime)
                {
                    discardFrontFirstTime = true;
                    CloseLidAnim();
                }
            }
        }
        else if (itemMode == enFoodCondition.cdInsideOpen)
        {
            if (discardFront)
            {
                if (!discardFrontFirstTime)
                {
                    discardFrontFirstTime = true;
                    GrabCD(CDObj);
                }
            }
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

    public void lockRotation()
    {
        pickUpManager.freezeInspectRotationFlag = true;
    }

    public void unlockRotation()
    {
        if (itemMode == enFoodCondition.emptyClosed)
            itemMode = enFoodCondition.emptyOpened;
        else if (itemMode == enFoodCondition.cdInsideClosed)
            itemMode = enFoodCondition.cdInsideOpen;
        else if (itemMode == enFoodCondition.emptyOpened)
            itemMode = enFoodCondition.emptyClosed;

        pickUpManager.freezeInspectRotationFlag = false;
        discardFront = false;
        discardFrontFirstTime = false;

    }
    
    public void HideCD()
    {
        CDObj.SetActive(false);
        animator.SetBool("isGrabed", false);
        animator.SetBool("isOpen", false);
    }

    void GrabCD(GameObject obj)
    {
        itemMode = enFoodCondition.emptyOpened;

        if (obj!=null)
        {
            animator.SetBool("isGrabed", true);
        }
    }


}
