using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadSlicerBlade : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool isActive = false;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TurnOnBlade()
    {
        if (!isActive)
        {
            animator.SetBool("isActive", true);
            isActive = true;
        }
    }

    void Deactivate()
    {
        isActive = false;
        animator.SetBool("isActive", false);
    }
}
