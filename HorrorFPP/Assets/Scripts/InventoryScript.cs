using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public GameObject[] inventoryItems = new GameObject[100];
    public bool activateMenu = false;

    [SerializeField] private Transform inventoryBackSlotsParent;
    [SerializeField] private Transform selectedTransformSlot;
    [SerializeField] private Transform selectedTransformSlotRight;
    [SerializeField] private Transform selectedTransformSlotLeft;

    private GameObject selectedObj;

    [SerializeField] private Text title;

    [SerializeField] private Image AImage;
    [SerializeField] private Image DImage;
    [SerializeField] private GameObject SpaceImage;

    [SerializeField] private Text Atxt;
    [SerializeField] private Text Dtxt;
    [SerializeField] private Text SpaceTxt;
    [SerializeField] private Text SpaceTxt1;

    [SerializeField] private Text idTxt;
    [SerializeField] private Text description;

    [SerializeField] private Text infoTxt;
    [SerializeField] private Image infoBar;

    private float defaultColorValueAlpha;

    private void Start()
    {
        activateMenu = false;

        defaultColorValueAlpha = Dtxt.color.a;
        StartCoroutine(RotateLoop());
    }

    public void AddToInventory(GameObject obj)
    {
        for(int i=0; i<inventoryItems.Length;i++)
        {
            if(inventoryItems[i]==null)
            {
                inventoryItems[i] = obj;
            }
        }
    }

    public void RemoveFromInventory(int id)
    {
        inventoryItems[id] = null;

        for(int i=0; i<inventoryItems.Length; i++)
        {
            if(inventoryItems[i]==null)
            {
                if(i+1<inventoryItems.Length)
                {
                    if (inventoryItems[i + 1] != null)
                    {
                        inventoryItems[i] = inventoryItems[i + 1];
                        inventoryItems[i + 1] = null;
                    }
                    else
                    {
                        break;
                    }
                } 
            }
        }
    }

    public bool ShowItem(int id, bool progressDirection, bool alreadyLauched)
    {
        int z = 0;
        if(inventoryItems[id] != null)
        {
            for(int k=0; k<inventoryItems.Length; k++)
            {
                if(inventoryItems[k]== null)
                {
                    z = k;
                    break;
                }
            }

            idTxt.text = "<" + (id+1).ToString() + "/" + z.ToString() + ">";

            if (alreadyLauched)
            {
                if(inventoryItems[0] == null)
                {
                    infoTxt.text = "NO ITEM IN INVENTORY.";
                    infoTxt.enabled = true;
                    infoBar.enabled = true;
                    activateMenu = true;
                    title.enabled = false;
                    description.enabled = false;

                    return true;
                }

                title.enabled = true;
                description.enabled = true;
                infoTxt.enabled = false;
                infoBar.enabled = false;
                title.text = inventoryItems[id].GetComponent<ItemBase>().titleTxt;

                description.text = inventoryItems[id].GetComponent<ItemBase>().descriptionTxt.Substring(0, inventoryItems[id].GetComponent<ItemBase>().descriptionTxt.Length - 2);

                inventoryItems[id].transform.position = selectedTransformSlot.position;
                inventoryItems[id].transform.rotation = selectedTransformSlot.rotation;
                inventoryItems[id].transform.localRotation = Quaternion.Euler(0, 0, 30);
                inventoryItems[id].transform.SetParent(selectedTransformSlot.transform);

                activateMenu = true;
                StartCoroutine(blinkText());
                StartCoroutine(RotateLoop());
            }
            else if(inventoryItems[0] == null)
            {
                if(selectedTransformSlot.transform.GetChild(0))
                {
                    selectedObj = selectedTransformSlot.transform.GetChild(0).gameObject;
                    selectedObj.transform.SetParent(inventoryBackSlotsParent.transform);
                    StartCoroutine(SlideOutMove(progressDirection));
                }

                if (progressDirection)
                {
                    title.enabled = false;
                    description.enabled = false;
                    idTxt.enabled = false;
                    SpaceImage.SetActive(false);
                    SpaceTxt.enabled = false;
                    SpaceTxt1.enabled = false;

                    DImage.color = new Vector4(DImage.color.r, DImage.color.g, DImage.color.b, 1f);
                    Dtxt.color = new Vector4(0, 0, 0, 1);
                    AImage.enabled = false;
                    Atxt.enabled = false;

                    inventoryItems[id].transform.position = selectedTransformSlotRight.position;
                    inventoryItems[id].transform.rotation = selectedTransformSlotRight.rotation;
                    inventoryItems[id].transform.localRotation = Quaternion.Euler(0, 0, 30);
                    inventoryItems[id].transform.SetParent(selectedTransformSlotRight.transform);
                    inventoryItems[id].SetActive(true);
                    StartCoroutine(SlideMove(id));
                }
                else
                {
                    title.enabled = false;
                    description.enabled = false;
                    idTxt.enabled = false;
                    SpaceImage.SetActive(false);
                    SpaceTxt.enabled = false;
                    SpaceTxt1.enabled = false;

                    AImage.color = new Vector4(DImage.color.r, DImage.color.g, DImage.color.b, 1f);
                    Atxt.color = new Vector4(0, 0, 0, 1);
                    DImage.enabled = false;
                    Dtxt.enabled = false;

                    inventoryItems[id].transform.position = selectedTransformSlotLeft.position;
                    inventoryItems[id].transform.rotation = selectedTransformSlotLeft.rotation;
                    inventoryItems[id].transform.localRotation = Quaternion.Euler(0, 0, 30);
                    inventoryItems[id].transform.SetParent(selectedTransformSlotLeft.transform);
                    inventoryItems[id].SetActive(true);
                    StartCoroutine(SlideMove(id));
                }
            }

            return true;
        }

        return false;
    }

    private IEnumerator blinkText()
    {
        bool disapearing = true;

        while (activateMenu)
        {
            if (disapearing)
            {
                SpaceTxt.color = Vector4.MoveTowards(new Vector4(SpaceTxt.color.r, SpaceTxt.color.g, SpaceTxt.color.b, SpaceTxt.color.a), new Vector4(1, 1, 1, 0), Time.deltaTime);
                if (SpaceTxt.color.a == 0)
                {
                    disapearing = !disapearing;
                }
            }
            else
            {
                SpaceTxt.color = Vector4.MoveTowards(new Vector4(SpaceTxt.color.r, SpaceTxt.color.g, SpaceTxt.color.b, SpaceTxt.color.a), new Vector4(1, 1, 1, 1), Time.deltaTime);
                if (SpaceTxt.color.a == 1)
                {
                    disapearing = !disapearing;
                }
            }

            yield return null;
        }

    }

    private IEnumerator SlideMove(int id)
    {
        do
        {
            inventoryItems[id].transform.position = Vector3.MoveTowards(inventoryItems[id].transform.position, selectedTransformSlot.transform.position, Time.deltaTime *2);
            yield return null;
        } while (inventoryItems[id].transform.position != selectedTransformSlot.transform.position);

        inventoryItems[id].transform.SetParent(selectedTransformSlot.transform);
        title.text = inventoryItems[id].GetComponent<ItemBase>().titleTxt;
        description.text = inventoryItems[id].GetComponent<ItemBase>().descriptionTxt.Substring(0, inventoryItems[id].GetComponent<ItemBase>().descriptionTxt.Length - 2);
        description.enabled = true;
        title.enabled = true;
        idTxt.enabled = true;

        DImage.color = new Vector4(DImage.color.r, DImage.color.g, DImage.color.b, 0.5f);
        Dtxt.color = new Vector4(1, 1, 1, 1);
        AImage.color = new Vector4(DImage.color.r, DImage.color.g, DImage.color.b, 0.5f);
        Atxt.color = new Vector4(1, 1, 1, 1);

        AImage.enabled = true;
        Atxt.enabled = true;
        DImage.enabled = true;
        Dtxt.enabled = true;

        SpaceImage.SetActive(true);
        SpaceTxt.enabled = true;
        SpaceTxt1.enabled = true;
    }

    private IEnumerator SlideOutMove(bool direction)
    {
        Transform destinationPos;

        if(direction)
        {
            destinationPos = selectedTransformSlotLeft.transform;
        }
        else
        {
            destinationPos = selectedTransformSlotRight.transform;
        }

        do
        {
            selectedObj.transform.position = Vector3.MoveTowards(selectedObj.transform.position, destinationPos.transform.position, Time.deltaTime * 2);
            yield return null;

        } while (selectedObj.transform.position != destinationPos.transform.position);


    }

    private IEnumerator RotateLoop()
    {
        while(true)
        {
            selectedTransformSlot.Rotate(Vector3.up, 2);
            yield return null;
        }
        
    }
}
