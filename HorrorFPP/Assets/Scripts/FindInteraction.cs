using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindInteraction : MonoBehaviour
{
    private GameObject raycastedObject;

    [SerializeField] private float rayLength = 10.0f;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private KeyCode interactButton;


    [SerializeField] private Shader defaultShader;
    [SerializeField] private Shader outlineShader;
    [SerializeField] private Shader ignoreShader;       //that material is ignore and doesnt change

    private bool selected = false;
    private bool multiInteractionSelected = false;
    public bool dropMode = false;

    //canvas obj
    [SerializeField] private GameObject actionText;
    [SerializeField] private GameObject dot;

    private void Start()
    {
        actionText.active = false;
        dot.active = true;
    }

    private void Update()
    {
        //disable when drop mode is use
        if (!dropMode)
        {
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            if (Physics.Raycast(transform.position, fwd, out hit, rayLength, layerMaskInteract.value))
            {
                if (hit.collider.CompareTag("Object") || hit.collider.CompareTag("ObjectMultiInteractions"))
                {
                    if (raycastedObject != null && raycastedObject != hit.collider.gameObject)
                    {
                        foreach (Transform child in raycastedObject.transform)
                        {
                            if (child.GetComponent<MeshRenderer>() && child.GetComponent<MeshRenderer>().material.shader != ignoreShader)
                                child.GetComponent<MeshRenderer>().material.shader = defaultShader;
                        }
                    }

                    raycastedObject = hit.collider.gameObject;

                    foreach (Transform child in raycastedObject.transform)
                    {
                        if (child.GetComponent<MeshRenderer>() && child.GetComponent<MeshRenderer>().material.shader != ignoreShader)
                            child.GetComponent<MeshRenderer>().material.shader = outlineShader;
                    }

                    if (hit.collider.CompareTag("Object"))
                    {
                        actionText.GetComponent<Text>().text = raycastedObject.GetComponent<InteractableObjectBase>().interactText;
                        dot.active = false;
                        actionText.active = true;
                    }

                    if (Input.GetKeyDown(interactButton) && hit.collider.CompareTag("Object"))
                    {
                        raycastedObject.GetComponent<InteractableObjectBase>().Interact();
                    }

                    if ( hit.collider.CompareTag("ObjectMultiInteractions"))
                    {
                        raycastedObject.GetComponent<InteractableObjectBase>().InteractMulti();
                        multiInteractionSelected = true;
                    }
                }

                selected = true;
            }
            else if (selected)
            {
                selected = false;

                actionText.active = false;
                dot.active = true;

                foreach (Transform child in raycastedObject.transform)
                {
                    if (child.GetComponent<MeshRenderer>() && child.GetComponent<MeshRenderer>().material.shader != ignoreShader)
                        child.GetComponent<MeshRenderer>().material.shader = defaultShader;
                }
                

                if(multiInteractionSelected)
                {
                    multiInteractionSelected = false;
                    raycastedObject.GetComponent<InteractableObjectBase>().DeInteractMulti();
                }

                raycastedObject = null;
            }
        }
        else
        {
            //disable selection in drop mode if something was selected before
            if (raycastedObject != null)
            {
                foreach (Transform child in raycastedObject.transform)
                {
                    if (child.GetComponent<MeshRenderer>())
                        child.GetComponent<MeshRenderer>().material.shader = defaultShader;
                }
            }
        }
    }
}
