using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerDetect : MonoBehaviour
{
    [SerializeField] private PickUp pickUpScript;
    [SerializeField] public Text text;



    private void Awake()
    {
        pickUpScript = transform.parent.GetComponent<PickUp>();
        text.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && pickUpScript.playerController.inspectMode)
        {
            text.gameObject.SetActive(true);
        }
        else
        {
            text.gameObject.SetActive(false);
        }
    }

 
}
