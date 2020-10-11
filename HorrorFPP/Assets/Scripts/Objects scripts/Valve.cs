using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : InteractableObjectBase
{
    [SerializeField] private TapHandle tapHandle;

    public bool isOpen { private set;  get; }
    [SerializeField] private Animator animator;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    public bool animationInProgress = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        isOpen = false;
    }

    public override void Interact()
    {
        if ((Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton)) && !animationInProgress)
        {
            if (isOpen)
            {
                interactText = "TURN_ON";
                animator.SetBool("isOpen", false);
            }
            else
            {
                interactText = "TURN_OFF";
                animator.SetBool("isOpen", true);
            }

            isOpen = !isOpen;
        }
    }

    public void ActivateWaterInPipe()
    {
        tapHandle.isLocked = false;
        tapHandle.CheckWaterInPipe();
    }

    public void DeActivateWaterInPipe()
    {
        tapHandle.isLocked = true;
        tapHandle.CheckWaterInPipe();
    }

    public void AnimationWorkOn()
    {
        animationInProgress = true;

    }

    public void AnimationWorkOff()
    {
        animationInProgress = false;

    }
}
