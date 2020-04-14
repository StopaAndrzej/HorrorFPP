using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class FoodInspect : MonoBehaviour
{
    [SerializeField] private PlayerMove playerComponent;

    [SerializeField] private Text objectName;
    [SerializeField] private Text press;
    [SerializeField] private Text describtion;
    [SerializeField] private Text inProgressBar;

    private float pressPos1, pressPos2;
    private bool changeToUpPos = false;

    [SerializeField] private KeyCode interactionKey;

    [SerializeField] private float rayLength = 10.0f;
    [SerializeField] private LayerMask layerMaskInteract;

    public string pressText1;
    public string pressText2;

    public string objectNameTitle;
    public string description;
    public string inProgressInfo;

    private float delay;
    public float delayNormal = 0.1f;
    public float delayPause = 0.5f;
    public string fullText;
    private string currentText = "";

    private bool inspectModeFlag = false;
    private bool descriptionShowed = false;

    //package meshes
    [SerializeField] private GameObject[] packageMeshes;

    private PickUp pickUpScript;

    //variables use to fade out camera
    [SerializeField] private PostProcessVolume cameraPostProcVolume;
    private bool unpacked = false;
    bool backFromBlackToScreenFlag = false;

    //diff food states
    //public bool isSpoiled = false;
    public int foodCondition = 0; // 0 -raw 1 - cooked 2 - spoiled

    public Material meatRaw;
    public Material meatSpoiled;

    public Material potatoesRaw;
    public Material potatoesSpoiled;

    public Material saladRaw;
    public Material saladSpoiled;


    public MeshRenderer meatTexture;
    public MeshRenderer potatoTexture;
    public MeshRenderer saladTexture;

    private void Awake()
    {
        objectName.text = "LUNCH";

        meatTexture = transform.Find("Meat").GetComponent< MeshRenderer >();
        potatoTexture = transform.Find("Potatoes").GetComponent<MeshRenderer>();
        saladTexture = transform.Find("Salad").GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        objectName.text = objectNameTitle;
        describtion.text = description;
        inProgressBar.text = inProgressInfo;

        delay = delayNormal;
        fullText = describtion.text;

        objectName.enabled = false;
        press.enabled = false;
        objectName.enabled = false;
        inProgressBar.enabled = false;

        pickUpScript = GetComponent<PickUp>();
        pressPos1 = -182.0f;
        pressPos2 = 182.0f;

    }

    private void Update()
    {
        switch(foodCondition)
        {
            case 0:
                meatTexture.material = meatRaw;
                potatoTexture.material = potatoesRaw;
                saladTexture.material = saladRaw;
                break;
            case 1:
                break;
            case 2:
                meatTexture.material = meatSpoiled;
                potatoTexture.material = potatoesSpoiled;
                saladTexture.material = saladSpoiled;
                break;
            default:
                meatTexture.material = meatSpoiled;
                potatoTexture.material = potatoesSpoiled;
                saladTexture.material = saladSpoiled;
                break;
             
        }


        if (unpacked)
            Unpack();

        if (playerComponent.inspectMode && pickUpScript.isGrabbed)
        {
            if(!changeToUpPos)
            {
                press.GetComponent<RectTransform>().localPosition = new Vector3(press.GetComponent<RectTransform>().localPosition.x, pressPos1, press.GetComponent<RectTransform>().localPosition.z);
            }

            objectName.text = objectNameTitle;
            objectName.enabled = true;

            RaycastHit hit;
            Vector3 up = transform.up;
            Vector3 down = -transform.up;

            Debug.DrawRay(transform.position, up, Color.green);
            Debug.DrawRay(transform.position, down, Color.red);

            if(descriptionShowed)
            {
                describtion.enabled = true;
            }

            if (Physics.Raycast(transform.position, up, out hit, rayLength, layerMaskInteract.value))
            {
                if (hit.collider.CompareTag("Player") && inspectModeFlag == false)
                {
                    if(!descriptionShowed)
                    {
                        press.text = pressText1;
                        press.enabled = true;

                        if (Input.GetKeyDown(interactionKey))
                        {
                            pickUpScript.stopFlag = true;
                            press.enabled = false;
                            inspectModeFlag = true;
                            describtion.enabled = true;
                            StartCoroutine(ShowText());
                        }
                    }
                }
            }
            else if(Physics.Raycast(transform.position, down, out hit, rayLength, layerMaskInteract.value))
            {
                if (hit.collider.CompareTag("Player") && inspectModeFlag == false)
                {
                    press.text = pressText2;
                    press.enabled = true;

                    if (Input.GetKeyDown(interactionKey))
                    {
                        inProgressBar.text = inProgressInfo;
                        unpacked = true;
                    }
                }
            }
            else
            {
                press.enabled = false;
            }
        }     
        else if(pickUpScript.isGrabbed)
        {
            objectName.enabled = false;
            press.enabled = false;
            describtion.enabled = false;
            inspectModeFlag = false;
            StopCoroutine(ShowText());
        }
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);

            describtion.text = currentText;

            if (i == fullText.Length-1)
            {
                pickUpScript.stopFlag = false;
                descriptionShowed = true;
                inspectModeFlag = false;
                changeToUpPos = true;
                press.GetComponent<RectTransform>().localPosition = new Vector3(press.GetComponent<RectTransform>().localPosition.x, pressPos2, press.GetComponent<RectTransform>().localPosition.z);
            }      

            yield return new WaitForSeconds(delay);

            if (fullText[i] == '.')
                delay = delayPause;
            else
                delay = delayNormal;
        }
    }


    private void Unpack()
    {
        pickUpScript.stopFlag = true;
        cameraPostProcVolume.profile.TryGetSettings(out ColorGrading colorGradingLayer);

        if (!backFromBlackToScreenFlag)
        {
            colorGradingLayer.postExposure.value = Mathf.Lerp(colorGradingLayer.postExposure.value, -50.0f, Time.deltaTime * 0.45f);

            if (colorGradingLayer.postExposure.value < -20.0f)
            {
                inProgressBar.enabled = true;
            }
                

            if (colorGradingLayer.postExposure.value < -45.0f)
            {
                foreach (GameObject element in packageMeshes)
                {
                    element.SetActive(false);
                }
                backFromBlackToScreenFlag = true;
            }
        }
        else
        {
            colorGradingLayer.postExposure.value = Mathf.Lerp(colorGradingLayer.postExposure.value, 0.0f, Time.deltaTime*4.0f);

            if (colorGradingLayer.postExposure.value > -0.15f)
            {
                inProgressBar.enabled = false;
            }

            if (colorGradingLayer.postExposure.value > -0.05f)
            {
                colorGradingLayer.postExposure.value = 0.0f;
                unpacked = false;
                pickUpScript.stopFlag = false;
            }
        }


        
    }
}
