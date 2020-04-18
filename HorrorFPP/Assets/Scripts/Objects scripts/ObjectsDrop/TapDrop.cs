using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapDrop : MonoBehaviour
{
    public Collider collider = null;
    [SerializeField] TapManager manager;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Object")
        {
            collider = other;
            if(collider.GetComponent<KettleInspect>())
            {
                manager.kettleInTap = true;
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
