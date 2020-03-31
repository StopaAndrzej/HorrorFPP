using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectBase : MonoBehaviour
{
    public string name;
    public string interactText = "Do it!";
    public virtual void Interact()
    {

    }
}
