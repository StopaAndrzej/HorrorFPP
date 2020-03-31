using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapSwitch : InteractableObjectBase
{
    private bool isActive = false;
    private bool fillFlag = false;

    [SerializeField] private Animator animator;

    [SerializeField] private GameObject waterFall;
    [SerializeField] private GameObject waterTap;

    [SerializeField] private float lowestWaterLevel;
    [SerializeField] private float highestWaterLevel;

    [SerializeField] private GameObject pipeValve;


    private void Start()
    {
        animator = GetComponent<Animator>();
        lowestWaterLevel = waterTap.GetComponent<Transform>().localPosition.y;
    }

    public override void Interact()
    {
        if (isActive)
        {
            interactText = "TurnOff";
            animator.SetBool("isActive", false);
            waterFall.GetComponent<TapWaterFall>().WaterFallActivate(false);
            fillFlag = false;
        }
        else
        {
            interactText = "TurnOn";
            animator.SetBool("isActive", true);
            if (pipeValve.GetComponent<Valve>().isOpen)
            {
                waterFall.GetComponent<TapWaterFall>().WaterFallActivate(true);
                fillFlag = true;
            }
        }

        isActive = !isActive;
    }

    private void Update()
    {
        //additional condition to stop water if valve is closed
        if (isActive && !pipeValve.GetComponent<Valve>().isOpen)
        {
            Interact();
        }

        if (fillFlag)
        {
            //increase level
            waterTap.GetComponent<Transform>().localPosition = new Vector3(waterTap.GetComponent<Transform>().localPosition.x, Mathf.Lerp(waterTap.GetComponent<Transform>().localPosition.y, highestWaterLevel, Time.deltaTime * 0.1f), waterTap.GetComponent<Transform>().localPosition.z);
        }
        else
        {
            //decrease level
            waterTap.GetComponent<Transform>().localPosition = new Vector3(waterTap.GetComponent<Transform>().localPosition.x, Mathf.Lerp(waterTap.GetComponent<Transform>().localPosition.y, lowestWaterLevel, Time.deltaTime * 0.1f), waterTap.GetComponent<Transform>().localPosition.z);
        }
    }

    

}