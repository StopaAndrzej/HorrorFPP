using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketScript : ItemBase
{
    [SerializeField] private Animator animator;
    public enum enItemCondition { empty, water, dirtywater };
    public enItemCondition itemMode;

    [SerializeField] private Material dirtyWaterMaterial;
    [SerializeField] private Material clearWaterMaterial;

    [SerializeField] private GameObject water;

    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    private int waterConditon = 0;
    public bool waterOn;

    [SerializeField] private float lowestWaterLevel;
    [SerializeField] private float highestWaterLevel = 0f;
    private float crossValueLevel;

    [SerializeField] private TapHandle tap;
    [SerializeField] private GameObject tapSlot;


    private void Start()
    {
        animator = GetComponent<Animator>();
        itemMode = enItemCondition.empty;

        waterOn = false;
        lowestWaterLevel = water.GetComponent<Transform>().localPosition.y;
        crossValueLevel = highestWaterLevel - lowestWaterLevel;

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

    private void Update()
    {
        if (waterOn)
        {
            water.GetComponent<Transform>().localPosition = new Vector3(water.GetComponent<Transform>().localPosition.x, Mathf.Lerp(water.GetComponent<Transform>().localPosition.y, highestWaterLevel, Time.deltaTime * 0.1f), water.GetComponent<Transform>().localPosition.z);
            float tmpValue = highestWaterLevel - water.GetComponent<Transform>().localPosition.y;
            if (tmpValue < crossValueLevel/2)
            {
                itemMode = enItemCondition.water;
            }
        }
        else
        {
            if(itemMode == enItemCondition.empty)
            {
                water.GetComponent<Transform>().localPosition = new Vector3(water.GetComponent<Transform>().localPosition.x, Mathf.Lerp(water.GetComponent<Transform>().localPosition.y, lowestWaterLevel, Time.deltaTime * 0.1f), water.GetComponent<Transform>().localPosition.z);
            }
        }
    }

    public override void SpecialActionAfterGrab()
    {
        animator.Play("Grab", -1, 0f);

        foreach (GameObject el in dropSlots)
        {
            if (el.GetComponent<DropSlotScript>())
                el.GetComponent<DropSlotScript>().ActivateDrop();
        }

        if(tap.objInTap!= null)
        {
            if(tap.objInTap == this.gameObject)
            {
                tap.objInTap = null;
            }
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

        if(tapSlot.GetComponent<DropSlotScript>().slotEmpty==false)
        {
            tap.objInTap = this.gameObject;
        }
    }



}
