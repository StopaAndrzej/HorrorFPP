using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapDrop : MonoBehaviour
{
    public Collider collider = null;
    [SerializeField] TapWaterFall tapWaterFall;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Object")
        {
            collider = other;
            if(collider.GetComponent<KettleInspect>())
            {

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
