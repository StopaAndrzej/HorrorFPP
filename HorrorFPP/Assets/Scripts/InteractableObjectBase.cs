using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectBase : MonoBehaviour
{
    public string name;
    public string interactText = "Do it!";
    //0 if its not one of another interactive object interaction
    public int kidID = 0;

    public List<GameObject> outlineObjects;


    public virtual void Interact()
    {

    }

    public virtual void DeInteract()
    {

    }

    //for multiObject tag
    public virtual void InteractMulti()
    {

    }

    public virtual void DeInteractMulti()
    {

    }
}
