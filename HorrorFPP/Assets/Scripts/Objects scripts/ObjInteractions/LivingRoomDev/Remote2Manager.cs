using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remote2Manager : ItemBase
{
    [SerializeField] private InventoryScript inventory;
    public enum enFoodCondition { withBattClosed, withBattOpened, noBattClosed, noBattOpen };
    public enFoodCondition itemMode;

    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    private Animator animator;

    [SerializeField] private List<GameObject> batteries;
    [SerializeField] private GameObject batteries1;
    [SerializeField] private Transform inventoryParent;

    private void Start()
    {
        animator = GetComponent<Animator>();
        itemMode = enFoodCondition.withBattClosed;

        isRotationVertical = false;

        foreach(GameObject el in batteries)
        {
            el.SetActive(true);
        }

        titleTxt = "BROKEN REMOTE CONTROL";

        pressTxt = "PRESS F TO INSPECT";
        pressTxt1 = "PRESS F TO OPEN";
        pressTxt2 = "PRESS F TO REMOVE BATTERIES";

        descriptionTxt = "OLD RERMOTE CONTROL. NOT SUITABLE FOR PRIMARY OPERATION...";
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
        if (itemMode == enFoodCondition.withBattClosed || itemMode == enFoodCondition.noBattClosed)
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

        else if (itemMode == enFoodCondition.withBattOpened)
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

    public override string ShowInfoFront()
    {
        if (itemMode == enFoodCondition.withBattClosed || itemMode == enFoodCondition.noBattClosed)
        {
            if (discardBack)
            {
                if (!discardBackFirstTime)
                {
                    discardBackFirstTime = true;
                    OpenAnim();
                }
            }
        }

        else if (itemMode == enFoodCondition.withBattOpened)
        {
            if (discardBack)
            {
                if (!discardBackFirstTime)
                {
                    discardBackFirstTime = true;
                    RemoveBatteries();
                }
            }
        }
        return null;
    }

    private void OpenAnim()
    {
        animator.SetBool("isOpen", true);
    }

    private void RemoveBatteries()
    {
        animator.SetBool("remove", true);
    }

    public void lockRotation()
    {
        pickUpManager.freezeInspectRotationFlag = true;
    }

    public void unlockRotation()
    {
        if (itemMode == enFoodCondition.withBattClosed)
        {
            itemMode = enFoodCondition.withBattOpened;
            discardBack = false;
            discardBackFirstTime = false;
        }  
        else if (itemMode == enFoodCondition.noBattClosed)
        {
            itemMode = enFoodCondition.noBattOpen;
            discardBack = false;
            discardBackFirstTime = false;
        }
           
        else if (itemMode == enFoodCondition.withBattOpened)
        {
            itemMode = enFoodCondition.noBattOpen;
            discardBack = false;
            discardBackFirstTime = false;
            foreach(GameObject el in batteries)
            {
                el.SetActive(false);
            }

            inventory.AddToInventory(batteries1);
            batteries1.transform.SetParent(inventoryParent);
            batteries1.transform.position = inventoryParent.position;
            batteries1.SetActive(true);
        }
            

        pickUpManager.freezeInspectRotationFlag = false;
    }
}
