using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugDrop : MonoBehaviour
{

    public Collider collider = null;
    [SerializeField] JugManager manager;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Object")
        {
            collider = other;
            if (collider.GetComponent<KettleInspect>())
            {
                manager.readyToPour = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Object")
        {

            collider = null;
        }
    }
}
