using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindInteraction : MonoBehaviour
{
    [SerializeField] private PickUpManager pickUpManager;

    public GameObject raycastedObject;
    public GameObject savedrayCastedObject; //for peepholemode to find last used object (door) and interact with it

    [SerializeField] private float rayLength = 10.0f;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private KeyCode interactButton;
    [SerializeField] private KeyCode interactButton2;

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
    public bool peepHoleMode = false;

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
        if (!dropMode && !peepHoleMode)
        {
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            if (Physics.Raycast(transform.position, fwd, out hit, rayLength, layerMaskInteract.value))
            {
                //object to recognize items with single interaction
                //multi for objects that contains more interactable objects like doors(handle, lock...)/and objectMultiCild for one of this child object
                if (hit.collider.CompareTag("Object") || hit.collider.CompareTag("ObjectMultiInteractions") || hit.collider.CompareTag("ObjectMultiChild") || hit.collider.CompareTag("ObjectNoInteraction"))
                {

                    if (raycastedObject != null && raycastedObject != hit.collider.gameObject)
                    {
                        if ((multiInteractionSelectedChild && !hit.collider.CompareTag("ObjectMultiChild")) ||
                            (multiInteractionSelectedChild && hit.collider.CompareTag("ObjectMultiChild")) && raycastedObject.GetComponent<InteractableObjectBase>().kidID != hit.collider.gameObject.GetComponent<InteractableObjectBase>().kidID
                            || raycastedObject.CompareTag("Object") && (hit.collider.CompareTag("Object") || hit.collider.CompareTag("ObjectNoInteraction")))
                        {
                            raycastedObject.GetComponent<InteractableObjectBase>().DeInteractMulti();
                            multiInteractionSelectedChild = false;

                            if(raycastedObject.CompareTag("Object"))
                            {
                                foreach (GameObject element in raycastedObject.GetComponent<InteractableObjectBase>().outlineObjects)
                                {
                                    element.GetComponent<MeshRenderer>().enabled = false;
                                }
                            }
                        }
                    }

                    raycastedObject = hit.collider.gameObject;

                    if (hit.collider.CompareTag("ObjectMultiChild"))
                    {
                        foreach (GameObject element in raycastedObject.transform.parent.GetComponent<InteractableObjectBase>().outlineObjects)
                        {
                            element.GetComponent<MeshRenderer>().enabled = true;
                        }
                    }
                    else
                    {
                        foreach (GameObject element in raycastedObject.GetComponent<InteractableObjectBase>().outlineObjects)
                        {
                            element.GetComponent<MeshRenderer>().enabled = true;
                        }
                    }
                   
                    if (hit.collider.CompareTag("ObjectNoInteraction"))
                    {
                        actionText.GetComponent<Text>().text = raycastedObject.GetComponent<InteractableObjectBase>().interactText;
                        dot.active = false;
                        actionText.active = true;

                        multiInteractionSelected = false;
                        multiInteractionSelectedChild = false;
                        multiInteractionSelectedChild2 = false;
                    }
                    else if (hit.collider.CompareTag("Object"))
                    {

                        actionText.GetComponent<Text>().text = raycastedObject.GetComponent<InteractableObjectBase>().interactText;
                        raycastedObject.GetComponent<InteractableObjectBase>().Interact();
                        
                        //if(raycastedObject.GetComponent<InteractableObjectBase>())
                        dot.active = false;
                        actionText.active = true;

                        multiInteractionSelected = false;
                        multiInteractionSelectedChild = false;
                        multiInteractionSelectedChild2 = false;

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


                if (multiInteractionSelectedChild2)
                    raycastedObject = raycastedObject.transform.parent.gameObject;

                foreach (GameObject element in raycastedObject.GetComponent<InteractableObjectBase>().outlineObjects)
                {
                    element.GetComponent<MeshRenderer>().enabled = false;
                }
 

                if (multiInteractionSelected)
                {
                    multiInteractionSelected = false;
                }

                raycastedObject.GetComponent<InteractableObjectBase>().DeInteractMulti();
                raycastedObject = null;
            }
        }
        else if(dropMode)
        {
            //disable selection in drop mode if something was selected before
            if (raycastedObject != null)
            {
                if (multiInteractionSelectedChild2)
                    raycastedObject = raycastedObject.transform.parent.gameObject;

                foreach (GameObject element in raycastedObject.GetComponent<InteractableObjectBase>().outlineObjects)
                {
                    element.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
        else if(peepHoleMode)
        {
            if (savedrayCastedObject.GetComponent<Collider>().CompareTag("ObjectMultiInteractions"))
            {
                savedrayCastedObject.GetComponent<InteractableObjectBase>().InteractMulti();
            }
            else if (savedrayCastedObject.GetComponent<Collider>().CompareTag("ObjectMultiChild"))
            {
                savedrayCastedObject.transform.parent.GetComponent<InteractableObjectBase>().InteractMulti();
            }
        }
    }
}
