using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenDrop : MonoBehaviour
{
    [SerializeField] private OvenManager oven;
    public Collider collider = null;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Object")
        {
            oven.itemInside = true;
            collider = other;
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Object")
        {
            oven.itemInside = false;
            collider = null;
        }
    }
}
