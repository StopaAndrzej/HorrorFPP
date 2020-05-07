using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowHandle : InteractableObjectBase
{
    public bool isOpen11 = false;
    public bool isOpen22 = false;
    [SerializeField] private Animator animator;
    [SerializeField] private WindowFrame frame;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public override void Interact()
    {
        if(!frame.isOpen)
        {
            if (!isOpen11 && !isOpen22)
            {
                interactText = "Open1";
                animator.SetBool("isOpen", true);
                isOpen11 = true;
            }
            else if (isOpen11 && !isOpen22)
            {
                interactText = "Open2";
                animator.SetBool("isOpen2", true);
                isOpen22 = true;
            }
            else if (isOpen11 && isOpen22)
            {
                interactText = "Close";
                animator.SetBool("isOpen", false);
                animator.SetBool("isOpen2", false);
                isOpen11 = false;
                isOpen22 = false;
            }
        }
    }
}
