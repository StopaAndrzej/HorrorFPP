using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrowaveSwitch : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private MicrowaveManager manager;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TurnOn()
    {
        animator.SetBool("Start", true);
    }

    public void TurnOff()
    {
        animator.SetBool("Start", false);
    }

    public void AllowToCook()
    {
        manager.Cook();
    }


}

