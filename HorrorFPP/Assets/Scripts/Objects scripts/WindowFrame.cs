using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowFrame : InteractableObjectBase
{
    [SerializeField] private WindowHandle handle;
    [SerializeField] private WindowVertical vertical;
    [SerializeField] private WindowHorizontal horizontal;

    public bool isOpen = false;

    public override void Interact()
    {
        if(handle.isOpen22)
        {
            vertical.Interact();
            handle.GetComponent<Transform>().SetParent(vertical.GetComponent<Transform>());
            this.GetComponent<Transform>().SetParent(vertical.GetComponent<Transform>());
            isOpen = !isOpen;
        }
        else if (handle.isOpen11)
        {
            horizontal.Interact();
            handle.GetComponent<Transform>().SetParent(horizontal.GetComponent<Transform>());
            this.GetComponent<Transform>().SetParent(horizontal.GetComponent<Transform>());
            isOpen = !isOpen;
        }
    }
}
