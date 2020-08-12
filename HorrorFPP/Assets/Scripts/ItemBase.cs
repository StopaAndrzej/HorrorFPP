using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBase : MonoBehaviour
{
    public PickUpManager pickUpManager;

    public string titleTxt;
    public string titleTxt1;
    public string titleTxt2;
    public string titleTxt3;

    public string descriptionTxt;
    public string descriptionTxt1;
    public string descriptionTxt2;
    public string descriptionTxt3;
    public string descriptionTxt4;

    public string inProgressBarTxt;

    public string pressTxt;
    public string pressTxt1;

    public bool[] inspectModeDirInteractionFlags = new bool[4]; //up down front back

    [SerializeField] private KeyCode interactionKey = KeyCode.F;
    [SerializeField] private KeyCode interactionKey1 = KeyCode.Mouse0;

    public bool discardUp = false;      //check if text is discard
    public bool discardDown = false;
    public bool discardFront = false;       
    public bool discardBack = false;

    public bool discardUpFirstTime = false;     //when first time then start typing coroutine
    public bool discardDownFirstTime = false;
    public bool discardFrontFirstTime = false;
    public bool discardBackFirstTime = false;

    public bool discardUpFirstTimeFinished = false;         //when coroutine finished set flag to not start corouting ever again
    public bool discardDownFirstTimeFinished = false;
    public bool discardFrontFirstTimeFinished = false;
    public bool discardBackFirstTimeFinished = false;

    public bool changeToUpPos = false;
    public float pressPos1, pressPos2;

    public int letterID = 0;

    public string fullTextToRead;
    public string currentTextToRead;
    public float delayNormal = 0.1f;
    public float delayPause = 0.5f;
    public float delay;

    public bool itemDrop = false;
    public bool actualStateItemDescriptinShowed = false;

    public virtual string InteractionUp()
    {
        return null;
    }

    public virtual string InteractionDown()
    {
        return null;
    }
    public virtual string InteractionFront()
    {
        return null;
    }
    public virtual string InteractionBack()
    {
        return null;
    }

    public virtual string ShowInfoUp()
    {
        return null;
    }

    public virtual string ShowInfoDown()
    {
        return null;
    }

    public virtual IEnumerator ShowText(bool value)
    {
        for (int i = 0; i < fullTextToRead.Length; i++)
        {
            currentTextToRead = fullTextToRead.Substring(0, i);

            if (i == fullTextToRead.Length-1)
            {
                pickUpManager.freezeInspectRotationFlag = false;
                pickUpManager.freezeDescriptionOnScreen = true;
                value = true;
            }

            yield return new WaitForSeconds(delay);

            if (fullTextToRead[i] == '.')
                delay = delayPause;
            else
                delay = delayNormal;
        }
    }


    public void ResetValues()
    {
        discardUp = false;
        discardDown = false;
        discardFront = false;
        discardBack = false;

        discardUpFirstTime = false;
        discardDownFirstTime = false;
        discardFrontFirstTime = false;
        discardBackFirstTime = false;

        discardUpFirstTimeFinished = false;
        discardDownFirstTimeFinished = false;
        discardFrontFirstTimeFinished = false;
        discardBackFirstTimeFinished = false;

        pickUpManager.freezeDescriptionOnScreen = false;

        changeToUpPos = false;

        for (int i = 0; i < inspectModeDirInteractionFlags.Length; i++)
        {
           pickUpManager.inspectModeDirInteractionFlags[i] = inspectModeDirInteractionFlags[i];
        }
    }
}
