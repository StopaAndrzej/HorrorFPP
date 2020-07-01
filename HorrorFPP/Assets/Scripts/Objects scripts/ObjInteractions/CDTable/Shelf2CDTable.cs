using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf2CDTable : InteractableObjectBase
{
    public Transform closedTransform;
    public Transform openedTransform;

    private Transform attachedToScriptObjectTransform;
    private bool isOpen = false;

    private void Start()
    {
        attachedToScriptObjectTransform = this.GetComponent<Transform>();
    }

    public override void Interact()
    {
        if (!isOpen)
        {
            attachedToScriptObjectTransform.position = openedTransform.position;
            attachedToScriptObjectTransform.rotation = openedTransform.rotation;
        }
        else
        {
            attachedToScriptObjectTransform.position = closedTransform.position;
            attachedToScriptObjectTransform.rotation = closedTransform.rotation;
        }

        isOpen = !isOpen;
    }
}
