using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentScript : MonoBehaviour
{
    [SerializeField] protected PickUpManager pickUpManager;

    public KeyCode interactionKey = KeyCode.F;
    public KeyCode mouseButton = KeyCode.Mouse0;

    public  virtual bool Interact()
    {
        return false;
    }

    public virtual void Interact1()
    {
        //code
    }

}
