using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : InteractableObjectBase
{
    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    private KeyCode interactionKey3 = KeyCode.Alpha3;
    private KeyCode interactionKey1 = KeyCode.Alpha1;

    [SerializeField] private List<Light> lights;
    [SerializeField] private GameObject buttonObj;
    private bool switchOn;

    [SerializeField] private Transform player;

    [SerializeField] private GameObject LeftButton;
    [SerializeField] private GameObject RightButton;

    private void Start()
    {
        switchOn = true;
        interactText = "SWITCH OFF";

        foreach (Light li in lights)
        {
            li.enabled = true;
        }
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if(switchOn)
            {
                buttonObj.transform.localRotation = Quaternion.Euler(-90, 0, -180);
                interactText = "SWITCH ON";
                foreach (Light li in lights)
                {
                    li.enabled = false;
                }
            }
            else
            {
                buttonObj.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                interactText = "SWITCH OFF";
                foreach (Light li in lights)
                {
                    li.enabled = true;
                }
            }

            switchOn = !switchOn;
        }
    }

    private void Update()
    {
        
        float distance = Vector3.Distance(this.transform.position, player.position);

        if(distance<1.2f)
        {
            float value = CalculatePlayerPosToLightSwitch();
            if (value > 60f && value < 150f)
            {
                RightButton.SetActive(true);
                LeftButton.SetActive(false);

                if (Input.GetKeyDown(interactionKey3))
                {
                    if (switchOn)
                    {
                        buttonObj.transform.localRotation = Quaternion.Euler(-90, 0, -180);
                        interactText = "SWITCH ON";
                        foreach (Light li in lights)
                        {
                            li.enabled = false;
                        }
                    }
                    else
                    {
                        buttonObj.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        interactText = "SWITCH OFF";
                        foreach (Light li in lights)
                        {
                            li.enabled = true;
                        }
                    }

                    switchOn = !switchOn;
                }
            }
            else if (value < -60f && value > -150f)
            {
                LeftButton.SetActive(true);
                RightButton.SetActive(false);

                if (Input.GetKeyDown(interactionKey1))
                {
                    if (switchOn)
                    {
                        buttonObj.transform.localRotation = Quaternion.Euler(-90, 0, -180);
                        interactText = "SWITCH ON";
                        foreach (Light li in lights)
                        {
                            li.enabled = false;
                        }
                    }
                    else
                    {
                        buttonObj.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        interactText = "SWITCH OFF";
                        foreach (Light li in lights)
                        {
                            li.enabled = true;
                        }
                    }

                    switchOn = !switchOn;
                }
            }
            else
            {
                RightButton.SetActive(false);
                LeftButton.SetActive(false);
            }
        }
        else
        {
            RightButton.SetActive(false);
            LeftButton.SetActive(false);
        }
        
    }


    private float CalculatePlayerPosToLightSwitch()
    {
        
        Vector3 newVector = this.transform.position - player.position;                      //vector initialPoint player and lightswitch
        Vector3 forwardPlayerVector = player.forward;
        float angleBetweenObjs = Vector3.SignedAngle(forwardPlayerVector, newVector, Vector3.up);
        return angleBetweenObjs;
    }
}
