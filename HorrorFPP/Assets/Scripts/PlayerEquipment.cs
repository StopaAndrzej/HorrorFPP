using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public List<GameObject> iteamInventory;
    public bool grabInHand = false;

    public List<GameObject> kitchenDrops;


    private void Start()
    {
        foreach(GameObject element in kitchenDrops)
        {
            element.SetActive(false);
        }
    }
}
