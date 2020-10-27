using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketScript : ItemBase
{
    [SerializeField] private Animator animator;
    private enum enItemCondition { empty, water, dirtywater };
    private enItemCondition itemMode;

    [SerializeField] private Material dirtyWaterMaterial;
    [SerializeField] private Material clearWaterMaterial;

    [SerializeField] private GameObject water;

    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    private int waterConditon = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
        itemMode = enItemCondition.empty;

        titleTxt = "BUCKET";
        titleTxt1 = "A BUCKET OF WATER";
        titleTxt2 = "A BUCKET OF DIRTY WATER";

        pressTxt = "PRESS F TO INSPECT";

        descriptionTxt = "IT CAN BE POUR WITH WATER INTO THEM. IT IS USED TO CLEAN A DIRTY MOP...";
        descriptionTxt1 = "BUCKET FILED WITH CLEAN WATER. ONCE SET ASIDE, IT CAN BE USED TO CLEAN THE MOP AS LONG AS ";

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
           
            if (!discardDownFirstTime)
            {
                discardDownFirstTime = true;
                discardDownFirstTimeFinished = false;
                delay = delayNormal;
                fullTextToRead = descriptionTxt;

                pickUpManager.pressOffsetFlag = true;
                pickUpManager.freezeInspectRotationFlag = true;

                StartCoroutine(ShowText(discardDownFirstTimeFinished));
            }

            if (discardDownFirstTimeFinished)
            {
                return descriptionTxt;
            }

             return currentTextToRead;
           
        }

        return null;
    }

    public override void SpecialActionAfterGrab()
    {
        animator.Play("Grab", -1, 0f);

        foreach (GameObject el in dropSlots)
        {
            if (el.GetComponent<DropSlotScript>())
                el.GetComponent<DropSlotScript>().ActivateDrop();
        }
    }

    public override void SpecialActionAfterDrop()
    {
        animator.Play("Drop", -1, 0f);

        foreach (GameObject el in dropSlots)
        {
            if (el.GetComponent<DropSlotScript>())
                el.GetComponent<DropSlotScript>().DeactivateDrop();
        }
    }

}
