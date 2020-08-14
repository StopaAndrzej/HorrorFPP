using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroPutPlace : MonoBehaviour
{
    [SerializeField] private MicrowaveManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Object" && other.GetComponent<FoodScript>())
        {
            manager.itemInside = true;
            manager.putObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Object" && other.GetComponent<FoodScript>())
        {
            manager.itemInside = false;
        }
    }
}
