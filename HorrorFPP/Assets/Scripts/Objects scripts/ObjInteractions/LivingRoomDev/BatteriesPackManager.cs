using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteriesPackManager : ItemBase
{
    [SerializeField] private InventoryScript inventory;

    public enum enPackCondition { withBatteries, withouutBatteries};
    public enPackCondition itemMode;

    private Animator animator;
    [SerializeField] GameObject batteriesObj;

    [SerializeField] private PlayerEquipment playerEq;
    [SerializeField] private Transform playerEqParent;

    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] private GameObject batteries1;
    [SerializeField] private Transform inventoryParent;

    void Start()
    {
        animator = GetComponent<Animator>();
        isRotationVertical = false;

        itemMode = enPackCondition.withBatteries;

        titleTxt = "BATTERY PACK";

        pressTxt = "PRESS (F) TO INSPECT";
        pressTxt1 = "PRESS (F) TO GRAB";

        descriptionTxt = "PLASTIC AA BATTERY PACK. THEY CAN BE USED TO POWER SOME SMALL ELECTRICAL APPLIANCES...";
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
            }
            return pressTxt;
        }

        return null;
    }

    public override string InteractionFront()
    {
        if(itemMode == enPackCondition.withBatteries)
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
        if (itemMode == enPackCondition.withBatteries )
        {
            if (discardFront)
            {
                if (!discardFrontFirstTime)
                {
                    discardFrontFirstTime = true;
                    GrabAnim();
                }
            }
        }
        return null;
    }

    private void GrabAnim()
    {
        animator.SetBool("isGrabbed", true);
    }

    public void lockRotation()
    {
        pickUpManager.freezeInspectRotationFlag = true;
    }

    public void unlockRotation()
    {
        if (itemMode == enPackCondition.withBatteries)
        {
            itemMode = enPackCondition.withouutBatteries;
            HideCD();

            inventory.AddToInventory(batteries1);
            batteries1.transform.SetParent(inventoryParent);
            batteries1.transform.position = inventoryParent.position;
            batteries1.SetActive(true);
        }

        pickUpManager.freezeInspectRotationFlag = false;
    }

    public void HideCD()
    {
        batteriesObj.SetActive(false);
        playerEq.iteamInventory.Add(batteriesObj);
        batteriesObj.transform.parent = playerEqParent;
    }
}
