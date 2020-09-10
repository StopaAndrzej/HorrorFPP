using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardManager : InteractableObjectBase
{
    [SerializeField] private PCScreenManager screen;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    private bool pcMode;

    private void Start()
    {
        interactText = "PC MODE";
        pcMode = false;
    }

    public override void Interact()
    {
        if(Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if(!pcMode && screen.desktopDisplay)
            {
                pcMode = true;
                screen.InteractiveDesktop();
            }
        }
    }
}
