using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShoes : InteractableObjectBase
{
    [SerializeField] private PlayerFootprint footprintsManager;

    bool changedToHomeShoes = false;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] private GameObject homeShoes;
    [SerializeField] private GameObject outdoorShoes;

    private void Start()
    {
        homeShoes.SetActive(true);
        outdoorShoes.SetActive(false);
    }

    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if(!changedToHomeShoes)
            {
                footprintsManager.dirty = false;
                homeShoes.SetActive(false);
                outdoorShoes.SetActive(true);
            }
            else
            {
                footprintsManager.dirty = true;
                homeShoes.SetActive(true);
                outdoorShoes.SetActive(false);
            }

            changedToHomeShoes = !changedToHomeShoes;
        }
    }
    
}
