using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrowaveDoor : InteractableObjectBase
{
    private bool isOpen = false;
    [SerializeField] private Animator animator;

    [SerializeField] private MicrowaveManager manager;

    //
    public Material turnOffMat;
    public Material turnOnMat;
    public Material turnOffMatBlood;
    public Material turnOnMatBlood;

    public Material turnOffMatBack;
    public Material turnOffMatBackBlood;

    public Material plate;
    public Material plateBlood;

    public Material inside;
    public Material insideBlood;

    //
    public MeshRenderer doorFrontMesh;
    public MeshRenderer doorBackMesh;
    public MeshRenderer insideMesh;
    public MeshRenderer plateMesh;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Interact()
    {
        if(!manager.stwichOn)
        {
            if (isOpen)
            {
                interactText = "Close";
                animator.SetBool("isOpen", false);
                manager.doorOpen = false;
            }
            else
            {
                interactText = "Open";
                animator.SetBool("isOpen", true);
                manager.doorOpen = true;
            }
            isOpen = !isOpen;
        }       
    }
}
