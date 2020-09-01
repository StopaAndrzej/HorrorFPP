using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCSeat : InteractableObjectBase
{
    private Quaternion rotOrigin;
    private Quaternion destinationRotation;
    [SerializeField] private GameObject player;

    private bool pointOnSeat = false;
    private float rotationSpeed;
    private float rotationStrength = 2f;

    private void Start()
    {
        interactText = "SIT HERE";
        rotOrigin = this.transform.rotation;
    }

    private void Update()
    {
        if(pointOnSeat)
        {
            destinationRotation = Quaternion.LookRotation(player.transform.position - this.transform.position);
            rotationSpeed = Mathf.Min(rotationStrength * Time.deltaTime, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, destinationRotation, rotationSpeed);
            transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
            
        }
        else
        {
            rotationSpeed = Mathf.Min(rotationStrength * Time.deltaTime, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotOrigin, rotationSpeed);
            transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
        }

    }

    public override void Interact()
    {
        if(!pointOnSeat)
        {
            pointOnSeat = !pointOnSeat;
        }
    }

    public override void DeInteract()
    {
        if (pointOnSeat)
        {
            pointOnSeat = !pointOnSeat;
        }
    }
}
         