using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalTrigger2 : MonoBehaviour
{
    [SerializeField] AnimalCageManager managerCage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            managerCage.Warning();
        }
    }
}
