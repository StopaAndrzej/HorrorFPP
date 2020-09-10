using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypicalItemManager : ItemBase
{
    [SerializeField] private KeyCode keyboardButton = KeyCode.F;
    [SerializeField] private KeyCode mouseButton = KeyCode.Mouse0;

    void Start()
    {
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

    public override string InteractionUp()
    {
        if (!discardUp)
        {
            if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
            {
                discardUp = true;
                return null;
            }
            return pressTxt;
        }
        return null;
    }

    public override string InteractionDown()
    {
        if (!discardDown)
        {
            if (Input.GetKeyDown(keyboardButton) || Input.GetKeyUp(mouseButton))
            {
                discardDown = true;
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

    public override string ShowInfoUp()
    {
        if (discardUp)
        {
            if (!discardUpFirstTime)
            {
                discardUpFirstTime = true;
                discardUpFirstTimeFinished = false;
                delay = delayNormal;
                fullTextToRead = descriptionTxt;

                pickUpManager.pressOffsetFlag = true;
                pickUpManager.freezeInspectRotationFlag = true;

                StartCoroutine(ShowText(discardUpFirstTimeFinished));
            }

            if (discardUpFirstTimeFinished)
            {
                return descriptionTxt;
            }

            return currentTextToRead;
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
}
