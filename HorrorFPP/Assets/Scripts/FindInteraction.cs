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

    //values to increase text aplha for multi interaction objects when selected by mouse or not
    //essential for proper work
    private bool multiInteractionSelectedChild = false;
    private bool multiInteractionSelectedChild2 = false;
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
                //object to recognize items with single interaction
                //multi for objects that contains more interactable objects like doors(handle, lock...)/and objectMultiCild for one of this child object
                if (hit.collider.CompareTag("Object") || hit.collider.CompareTag("ObjectMultiInteractions") || hit.collider.CompareTag("ObjectMultiChild"))
                {

                    if (raycastedObject != null && raycastedObject != hit.collider.gameObject && !hit.collider.CompareTag("ObjectMultiChild"))
                    {
                        foreach (Transform child in raycastedObject.transform)
                        {
                            if (child.GetComponent<MeshRenderer>() && child.GetComponent<MeshRenderer>().material.shader != ignoreShader)
                                child.GetComponent<MeshRenderer>().material.shader = defaultShader;
                        }

                        if (multiInteractionSelectedChild)
                        {
                            raycastedObject.GetComponent<InteractableObjectBase>().DeInteractMulti();
                            multiInteractionSelectedChild = false;
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
                        dot.active = true;
                        actionText.active = true;
                    }

                    if (Input.GetKeyDown(interactButton) && hit.collider.CompareTag("Object"))
                    {
                        raycastedObject.GetComponent<InteractableObjectBase>().Interact();
                    }

                    else if (hit.collider.CompareTag("ObjectMultiChild"))
                    {
                        raycastedObject.GetComponent<InteractableObjectBase>().InteractMulti();

                        //switch correct parent interaction by click on kid's interactive field
                        if (Input.GetKeyDown(interactButton))
                        {
                            int tmpValue = raycastedObject.GetComponent<InteractableObjectBase>().kidID;
                            raycastedObject.transform.parent.GetComponent<InteractableObjectBase>().kidID = tmpValue;
                        }
                        raycastedObject.transform.parent.GetComponent<InteractableObjectBase>().InteractMulti();

                        dot.active = true;
                        actionText.active = false;

                        multiInteractionSelected = true;
                        multiInteractionSelectedChild = true;
                        multiInteractionSelectedChild2 = true;
                    }

                    else if ( hit.collider.CompareTag("ObjectMultiInteractions"))
                    {
                        actionText.GetComponent<Text>().text = raycastedObject.GetComponent<InteractableObjectBase>().interactText;
                        dot.active = false;
                        actionText.active = true;

                        raycastedObject.GetComponent<InteractableObjectBase>().InteractMulti();
                        multiInteractionSelected = true;
                        multiInteractionSelectedChild2 = false;

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


                
                if (multiInteractionSelectedChild2)
                {
                    raycastedObject = raycastedObject.transform.parent.gameObject;
                }

                if (multiInteractionSelected)
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
