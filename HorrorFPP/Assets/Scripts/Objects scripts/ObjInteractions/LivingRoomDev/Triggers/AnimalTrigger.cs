using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalTrigger : MonoBehaviour
{
    [SerializeField] AnimalCageManager managerCage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            managerCage.CageBite();
        }
    }
}
