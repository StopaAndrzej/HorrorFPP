using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetsFoodManager : ItemBase
{
    public enum enFoodCondition { packed, opened };
    public enFoodCondition itemMode;

    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    private Animator animator;

    [SerializeField] private PlayerEquipment playerEq;
    [SerializeField] private Transform playerEqParent;

    void Start()
    {
        animator = GetComponent<Animator>();
        isRotationVertical = true;

        itemMode = enFoodCondition.packed;

        titleTxt = "PET FOOD BOX";

        pressTxt = "PRESS F TO INSPECT";
        pressTxt1 = "PRESS F TO OPEN";

        descriptionTxt = "DRY FOOD FOR RODENTS. HIGH CONTENT OF DRIED FRUIT AND CEREALS...";
        controlText = "...PRESS (MOUSE1) TO GRAB IT...\n...PRESS(ESC) TO PUT IT BACK.....";

        inspectModeDirInteractionFlags[0] = true;
        inspectModeDirInteractionFlags[1] = false;
        inspectModeDirInteractionFlags[2] = false;
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

    public override string InteractionUp()
    {
        if (itemMode == enFoodCondition.packed)
        {
            if (!discardUp)
            {
                if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
                {
                    discardUp = true;
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

    public override string ShowInfoUp()
    {
        if (itemMode == enFoodCondition.packed)
        {
            if (discardUp)
            {
                if (!discardUpFirstTime)
                {
                    discardUpFirstTime = true;
                    OpenBoxAnim();
                }
            }
        }

        return null;
    }

    private void OpenBoxAnim()
    {
        animator.SetBool("isOpen", true);
    }

    public void lockRotation()
    {
        pickUpManager.freezeInspectRotationFlag = true;
    }

    public void unlockRotation()
    {
        if (itemMode == enFoodCondition.packed)
            itemMode = enFoodCondition.opened;

        pickUpManager.freezeInspectRotationFlag = false;
    }
}
