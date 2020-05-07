using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowHorizontal : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (!isOpen)
        {
            animator.SetBool("isOpen", true);
        }
        else
        {
            animator.SetBool("isOpen", false);
        }

        isOpen = !isOpen;
    }
}
