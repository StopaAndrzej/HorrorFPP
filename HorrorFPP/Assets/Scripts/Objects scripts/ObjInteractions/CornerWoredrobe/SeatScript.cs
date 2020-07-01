using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatScript : InteractableObjectBase
{
    private bool onSeat = false;
    [SerializeField] private PlayerMove player;

    //posses
    [SerializeField] private Transform seatPos;
    [SerializeField] private Transform standPos;

    private void Start()
    {
        interactText = "SIT HERE";
    }

    public override void Interact()
    {
        if (!onSeat)
        {
            player.GetComponent<Transform>().position = standPos.position;
            player.GetComponent<Transform>().rotation = standPos.rotation;
            interactText = "SIT HERE";
            player.seatMode = false;
        }
        else
        {
            player.GetComponent<Transform>().position = seatPos.position;
            player.GetComponent<Transform>().rotation = seatPos.rotation;
            interactText = "GET UP";
            player.seatMode = true;
        }

        onSeat = !onSeat;
    }
}
