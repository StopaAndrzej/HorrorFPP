using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DVDManager : InteractableObjectBase
{
    [SerializeField] private TVManager tvManager;

    private Animator animator;

    //wait until animation finish to interact again
    public bool animationInProgress = false;

    public KeyCode button = KeyCode.Mouse0;

    [SerializeField] private Material offScreenMaterial;
    [SerializeField] private Material onScreenMaterial;

    [SerializeField] private MeshRenderer screen;

    public bool cdInside = false;
    private bool isOpen = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        screen.material = offScreenMaterial;
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(button) && !animationInProgress)
        {
            if (!isOpen)
            {
                TurnOnReader();
            }
            else
            {
                TurnOffReader();
            }
        }
    }

    #region ANIMATION

    private void TurnOnReader()
    {
        StartCoroutine(FlashingDVDPanel(5));
    }

    private void TurnOffReader()
    {
        animator.SetBool("isOpen", false);
    }
     
    public void AnimationWorkOn()
    {
        animationInProgress = true;

    }

    public void AnimationWorkOff()
    {
        if(isOpen)
        {
            if(!cdInside)
            {
                //turn off dvd
                StartCoroutine(FlashingDVDPanel(6));
            } 
            else
            {
                //load files from disk on TV
                tvManager.SetStandBy();
                StartCoroutine(FlashingDVDPanel(13));
            }
        }
        else
        {
            animationInProgress = false;
            isOpen = !isOpen;
        }
    }

    public IEnumerator FlashingDVDPanel(int value)
    {
        int flashingTimer = value;      //should be odd value if lcd is On before starts couroutine/vice versa

        Material fstMaterial = onScreenMaterial;
        Material scdMaterial = offScreenMaterial;

        if(value%2==0)
        {
             fstMaterial = offScreenMaterial;
             scdMaterial = onScreenMaterial;
        }

        while (true)
        {
            if(flashingTimer%2==1)
            {
                screen.material = fstMaterial;
            }
            else
            {
                screen.material = scdMaterial;
            }


            if (flashingTimer <= 1)
                break;

            flashingTimer--;
            yield return new WaitForSeconds(0.2f);
        }

        if(!isOpen)
            animator.SetBool("isOpen", true);
        else
        {
            animationInProgress = false;
            isOpen = !isOpen;
        }  
    }

    #endregion
}
