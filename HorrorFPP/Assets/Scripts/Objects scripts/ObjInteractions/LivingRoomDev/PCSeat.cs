using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCSeat : InteractableObjectBase
{
    private Quaternion rotOrigin;
    private Quaternion destinationRotation;
    private Quaternion lastPlayerStandingPosRotationSeat;
    private Vector3 playerPosBeforeSit;
    [SerializeField] private GameObject player;

    private bool pointOnSeat = false;
    private bool playerOnSeat = false;
    private float rotationSpeed;
    private float rotationStrength = 2f;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    private void Start()
    {
        interactText = "SIT HERE";
        rotOrigin = this.transform.rotation;
    }

    private void Update()
    {
         if (pointOnSeat && !playerOnSeat)
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

        if ((Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton)) && !playerOnSeat)
        {
            playerPosBeforeSit = player.transform.position;
            lastPlayerStandingPosRotationSeat = destinationRotation;
            player.transform.position = this.transform.position;
            player.transform.rotation = this.transform.rotation;
            interactText = "GET UP";
            player.GetComponent<PlayerMove>().seatMode = true;
            playerOnSeat = true;
        }
        else if((Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton)) && playerOnSeat)
        {
            player.transform.position = playerPosBeforeSit;
            transform.rotation = lastPlayerStandingPosRotationSeat;
            interactText = "SIT HERE";
            player.GetComponent<PlayerMove>().seatMode = false;
            playerOnSeat = false;
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
         