using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp_Button : InteractableObjectBase
{
    private Animator animator;
    private Material defaultMaterial;
    [SerializeField] private Material activeMaterial;

    [SerializeField] private Material shadeMaterial;
    private Vector4 shadeMaterialParam;

    private Vector4 lampShadeDefaultParam;


    private bool isActive = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        defaultMaterial = this.GetComponent<MeshRenderer>().material;
        shadeMaterialParam = shadeMaterial.GetVector("_EmissionColor");
        shadeMaterial.SetVector("_EmissionColor", new Vector4(shadeMaterialParam.x, shadeMaterialParam.y, shadeMaterialParam.z, 0));
    }

    public override void Interact()
    {
        if (!isActive)
        {
            animator.SetBool("isActive", true);
        }
        else
        {
            animator.SetBool("isActive", false);
        }

        isActive = !isActive;
    }

    public void ChangeMaterialToActive()
    {
        this.GetComponent<MeshRenderer>().material = activeMaterial;
        shadeMaterial.SetVector("_EmissionColor", new Vector4(shadeMaterialParam.x, shadeMaterialParam.y, shadeMaterialParam.z, 1.0f));
    }
    
    public void ChangeMaterialToDefault()
    {
        this.GetComponent<MeshRenderer>().material = defaultMaterial;
        shadeMaterial.SetVector("_EmissionColor", new Vector4(shadeMaterialParam.x, shadeMaterialParam.y, shadeMaterialParam.z, 0));
    }
}
