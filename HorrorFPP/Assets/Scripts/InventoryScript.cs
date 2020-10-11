using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    [SerializeField] private GameObject[] inventoryItems = new GameObject[100];
    [SerializeField] private Transform selectedTransformSlot;


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

    public void ShowItem(int id)
    {
        inventoryItems[id].transform.position = selectedTransformSlot.position;
        inventoryItems[id].transform.rotation = selectedTransformSlot.rotation;
        inventoryItems[id].transform.localRotation = Quaternion.Euler(0, 0, 30);
        inventoryItems[id].transform.SetParent(selectedTransformSlot.transform);
        inventoryItems[id].SetActive(true);
        StartCoroutine(RotateLoop());
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
