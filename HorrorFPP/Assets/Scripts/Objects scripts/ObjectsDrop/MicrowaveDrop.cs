using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrowaveDrop : MonoBehaviour
{
    [SerializeField] private MicrowaveManager microwave;
    public Collider collider = null;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Object" )
        {
            microwave.itemInside = true;
            collider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Object")
        {
            microwave.itemInside = false;
        }
    }
}
