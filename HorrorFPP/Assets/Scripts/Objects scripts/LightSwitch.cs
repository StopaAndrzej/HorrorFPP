using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightSwitch : InteractableObjectBase
{
    [SerializeField] private LightSwitchManager switchManager;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    private KeyCode interactionKey3 = KeyCode.Alpha3;
    private KeyCode interactionKey1 = KeyCode.Alpha1;

    [SerializeField] private List<Light> lights;
    [SerializeField] private GameObject buttonObj;
    public bool switchOn;

    [SerializeField] private Transform player;

    [SerializeField] private GameObject LeftButton;
    [SerializeField] private GameObject RightButton;

    [SerializeField] bool switchLock = false;

    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;

    [SerializeField] private List<GameObject> bulbs;

    public bool switchLeftButtonFlag = false;
    public bool switchRightButtonFlag = false;

    private void Start()
    {
        if (switchOn)
        {
            buttonObj.transform.localRotation = Quaternion.Euler(-90, 0, -180);
            interactText = "SWITCH ON";
            foreach (Light li in lights)
            {
                li.enabled = false;
            }

            foreach (GameObject el in bulbs)
            {
                el.GetComponent<MeshRenderer>().material = offMaterial;
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

            foreach (GameObject el in bulbs)
            {
                el.GetComponent<MeshRenderer>().material = onMaterial;
            }
        }

    }

    public override void Interact()
    {
        if ((Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton)))
        {
            if(switchOn)
            {
                buttonObj.transform.localRotation = Quaternion.Euler(-90, 0, -180);
                interactText = "SWITCH ON";
                foreach (Light li in lights)
                {
                    li.enabled = false;
                }

                foreach(GameObject el in bulbs)
                {
                    el.GetComponent<MeshRenderer>().material = offMaterial;
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

                foreach (GameObject el in bulbs)
                {
                    el.GetComponent<MeshRenderer>().material = onMaterial;
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

                switchRightButtonFlag = true;
                switchLeftButtonFlag = false;

                if (Input.GetKeyDown(interactionKey3) && !switchLock)
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

                    StartCoroutine(blinkButton(RightButton));
                    switchOn = !switchOn;
                }
            }
            else if (value < -60f && value > -150f)
            {
                LeftButton.SetActive(true);

                switchLeftButtonFlag = true;
                switchRightButtonFlag = false;

                if (Input.GetKeyDown(interactionKey1) && !switchLock)
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

                    StartCoroutine(blinkButton(LeftButton));
                    switchOn = !switchOn;
                }
            }
            else
            {
                switchLeftButtonFlag = false;
                switchRightButtonFlag = false;
            }
        }
        
    }


    private float CalculatePlayerPosToLightSwitch()
    {
        
        Vector3 newVector = this.transform.position - player.position;                      //vector initialPoint player and lightswitch
        Vector3 forwardPlayerVector = player.forward;
        float angleBetweenObjs = Vector3.SignedAngle(forwardPlayerVector, newVector, Vector3.up);
        return angleBetweenObjs;
    }

    private IEnumerator blinkButton(GameObject obj)
    {
        switchLock = true;
        obj.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
        yield return new WaitForSeconds(0.5f);
        obj.GetComponent<Image>().color = new Vector4(0.764f, 0.635f, 0.356f, 0.490f);
        switchLock = false;
    }
}
