using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KettleDrop : MonoBehaviour
{
    [SerializeField] private KettleManager manager;
    public Collider collider = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Object")
        {
            collider = other;
            if(collider.GetComponent<KettleInspect>())
            {
                manager.isOnStand = true;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Object")
        {
            if (collider.GetComponent<KettleInspect>())
            {
                manager.isOnStand = false;
            }
            collider = null;
        }
    }
}
