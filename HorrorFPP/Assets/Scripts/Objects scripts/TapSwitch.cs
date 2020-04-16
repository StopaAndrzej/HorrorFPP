using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapSwitch : InteractableObjectBase
{
    private bool isActive = false;

    [SerializeField] private Animator animator;

    [SerializeField] private GameObject waterFall;
    [SerializeField] private GameObject waterTap;

    [SerializeField] private float lowestWaterLevel;
    [SerializeField] private float highestWaterLevel = 1.707f;

    [SerializeField] private GameObject pipeValve;
    [SerializeField] private TapManager tapManager;


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
            tapManager.switchActive = false;

        }
        else
        {
            interactText = "TurnOn";
            animator.SetBool("isActive", true);
            if (pipeValve.GetComponent<Valve>().isOpen)
            {
                waterFall.GetComponent<TapWaterFall>().WaterFallActivate(true);
                tapManager.switchActive = true;
            }
        }

        isActive = !isActive;
    }

    private void Update()
    {
        if(tapManager.switchActive)
        {
            waterTap.GetComponent<Transform>().localPosition = new Vector3(waterTap.GetComponent<Transform>().localPosition.x, Mathf.Lerp(waterTap.GetComponent<Transform>().localPosition.y, highestWaterLevel, Time.deltaTime*0.1f), waterTap.GetComponent<Transform>().localPosition.z);
        }
        else
        {
            waterTap.GetComponent<Transform>().localPosition = new Vector3(waterTap.GetComponent<Transform>().localPosition.x, Mathf.Lerp(waterTap.GetComponent<Transform>().localPosition.y, lowestWaterLevel, Time.deltaTime * 0.1f), waterTap.GetComponent<Transform>().localPosition.z);
        }
    }

}