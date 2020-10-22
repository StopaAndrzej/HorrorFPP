using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sofa : InteractableObjectBase
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerStandPos;
    [SerializeField] private Transform playerSitPos;

    private bool playerOnSeat = false;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    private void Start()
    {
        interactText = "SIT HERE";
    }

    public override void Interact()
    {
        if ((Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton)) && !playerOnSeat)
        {
            interactText = "GET UP";
            playerOnSeat = true;
            player.GetComponent<PlayerMove>().seatMode = true;
            player.transform.position = playerSitPos.position;
            player.transform.rotation = playerSitPos.rotation;
        }
        else if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            interactText = "SIT HERE";
            playerOnSeat = false;
            player.GetComponent<PlayerMove>().seatMode = false;
            player.transform.position = playerStandPos.position;
            player.transform.rotation = playerStandPos.rotation;
        }

        foreach (Transform child in transform)
        {
            foreach (Transform childChild in child)
            {
                if (childChild.GetComponent<MeshRenderer>())
                {
                    childChild.GetComponent<MeshRenderer>().material.SetColor("_Color", new Vector4(1, 1, 1, 1));
                    childChild.GetComponent<MeshRenderer>().material.SetColor("_SpecularColor", new Vector4(0.2f, 0.2f, 0.2f, 1));
                }
            }

        }
    }

    public override void DeInteract()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform childChild in child)
            {
                if (childChild.GetComponent<MeshRenderer>())
                {
                    childChild.GetComponent<MeshRenderer>().material.SetColor("_Color", new Vector4(0.5882352941176471f, 0.5882352941176471f, 0.5882352941176471f, 1));
                    childChild.GetComponent<MeshRenderer>().material.SetColor("_SpecularColor", new Vector4(0.0f, 0.0f, 0.0f, 1));
                }
            }

        }
    }
}
