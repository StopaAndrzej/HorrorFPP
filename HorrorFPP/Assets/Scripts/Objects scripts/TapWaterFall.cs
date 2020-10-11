using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapWaterFall : InteractableObjectBase
{
    [SerializeField] private Animator animator;
    public bool isActive = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

}
