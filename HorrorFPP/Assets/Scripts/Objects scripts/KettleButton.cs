using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KettleButton : InteractableObjectBase
{
    public bool isActive = false;
    [SerializeField] private Animator animator;
    [SerializeField] private float boilingTime;
    private float timer;

    [SerializeField] private KettleDoor door;
    [SerializeField] private KettleManager manager;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Interact()
    {
        if(!door.isOpen && manager.readyToFillCup)
        {
            if (isActive)
            {
                interactText = "isActive";
                animator.SetBool("isActive", false);
            }
            else
            {
                interactText = "isActive";
                animator.SetBool("isActive", true);
                timer = boilingTime;
            }
            isActive = !isActive;
        }
    }

    private void Update()
    {
        if (isActive)
            BoildWaterTimer();
    }
    void BoildWaterTimer()
    {
        timer -= Time.deltaTime;
        if(timer<=0)
        {
            manager.boiledWater = true;
            Interact();
        }
    }
}
