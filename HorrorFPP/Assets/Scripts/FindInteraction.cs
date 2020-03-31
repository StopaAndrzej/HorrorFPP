using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindInteraction : MonoBehaviour
{
    private GameObject raycastedObject;

    [SerializeField] private float rayLength = 10.0f;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private KeyCode interactButton;


    [SerializeField] private Shader defaultShader;
    [SerializeField] private Shader outlineShader;

    private bool selected = false;

    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if(Physics.Raycast(transform.position, fwd, out hit, rayLength, layerMaskInteract.value))
        {
            if(hit.collider.CompareTag("Object"))
            {
                if(raycastedObject!=null && raycastedObject!= hit.collider.gameObject)
                {
                    foreach (Transform child in raycastedObject.transform)
                        child.GetComponent<MeshRenderer>().material.shader = defaultShader;
                }

                raycastedObject = hit.collider.gameObject;

                foreach (Transform child in raycastedObject.transform)
                    child.GetComponent<MeshRenderer>().material.shader = outlineShader;

                Debug.Log("Interactive object found!");

                if(Input.GetKeyDown(interactButton))
                {
                    Debug.Log("DO IT!");
                    raycastedObject.GetComponent<InteractableObjectBase>().Interact();
                }
            }

            selected = true;
        }
        else if(selected)
        {
            selected = false;
            foreach (Transform child in raycastedObject.transform)
            {
                child.GetComponent<MeshRenderer>().material.shader = defaultShader;
            }
            raycastedObject = null;


        }
    }
}
