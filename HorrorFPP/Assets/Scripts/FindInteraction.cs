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
    [SerializeField] private LayerMask putlayerMask;
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
   // public bool dropMode = false;
    public bool peepHoleMode = false;
    public bool showMissionsMode = false;

    //canvas obj
    [SerializeField] private GameObject actionText;
    [SerializeField] private GameObject dot;

    public bool lastFrameRayHitSurface = false;

    private void Start()
    {
        actionText.active = false;
        dot.active = true;
    }

    private void FixedUpdate()
    {
        //disable when drop mode is use
        //before !dropMode &&...
        if (!peepHoleMode && !showMissionsMode && pickUpManager.itemMode != PickUpManager.enManagerItemMode.inspectMode)
        {
            RaycastHit hit;
            
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(transform.position, fwd * 10000f, Color.blue);

            if (Physics.Raycast(transform.position, fwd, out hit, rayLength, putlayerMask.value) && pickUpManager.itemMode == PickUpManager.enManagerItemMode.inHand)
            {
                if (hit.collider.CompareTag("Surface"))
                {
                    pickUpManager.visualObjectPutArea(hit.point, hit.transform.gameObject);
                    lastFrameRayHitSurface = true;
                }
                else if (hit.collider.CompareTag("SurfaceDirectly"))
                {
                    pickUpManager.visualObjectPutArea(hit.transform.gameObject);
                    lastFrameRayHitSurface = true;
                }

            }

            if (Physics.Raycast(transform.position, fwd, out hit, rayLength, layerMaskInteract.value))
            {
                //object to recognize items with single interaction
                //multi for objects that contains more interactable objects like doors(handle, lock...)/and objectMultiCild for one of this child object
                if (hit.collider.CompareTag("Object") || hit.collider.CompareTag("ObjectMultiInteractions") || hit.collider.CompareTag("ObjectMultiChild") || hit.collider.CompareTag("ObjectNoInteraction") || hit.collider.CompareTag("ObjectInspectOnly"))
                {

                    if (raycastedObject != null && raycastedObject != hit.collider.gameObject)
                    {
                        if ((multiInteractionSelectedChild && !hit.collider.CompareTag("ObjectMultiChild")) ||
                            (multiInteractionSelectedChild && hit.collider.CompareTag("ObjectMultiChild")) && raycastedObject.GetComponent<InteractableObjectBase>().kidID != hit.collider.gameObject.GetComponent<InteractableObjectBase>().kidID
                            || raycastedObject.CompareTag("Object") && (hit.collider.CompareTag("Object") || hit.collider.CompareTag("ObjectNoInteraction") || hit.collider.CompareTag("ObjectInspectOnly")))
                        {
                            raycastedObject.GetComponent<InteractableObjectBase>().DeInteractMulti();
                            raycastedObject.GetComponent<InteractableObjectBase>().DeInteract();
                           multiInteractionSelectedChild = false;

                            if (raycastedObject.CompareTag("Object") || raycastedObject.CompareTag("ObjectInspectOnly"))
                            {
                                foreach (GameObject element in raycastedObject.GetComponent<InteractableObjectBase>().outlineObjects)
                                {
                                    element.GetComponent<MeshRenderer>().enabled = false;
                                }

                            }

                            foreach (Transform child in raycastedObject.transform)
                            {
                                foreach (Transform childChild in child)
                                {
                                    if (childChild.GetComponent<MeshRenderer>())
                                    {
                                        childChild.GetComponent<MeshRenderer>().material.SetColor("_Color", new Vector4(0.5882352941176471f, 0.5882352941176471f, 0.5882352941176471f, 1));
                                        childChild.GetComponent<MeshRenderer>().material.SetColor("_SpecularColor", new Vector4(0.0f, 0.0f, 0.0f, 1));
                                    }
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
                        if(raycastedObject.GetComponent<InteractableObjectBase>().outlineObjects != null)
                        {
                            foreach (GameObject element in raycastedObject.GetComponent<InteractableObjectBase>().outlineObjects)
                            {
                                element.GetComponent<MeshRenderer>().enabled = true;
                            }
                        }
                        
                    }

                    if (hit.collider.CompareTag("ObjectNoInteraction"))
                    {
                        actionText.GetComponent<Text>().text = raycastedObject.GetComponent<InteractableObjectBase>().interactText;
                        actionText.GetComponent<RectTransform>().localPosition = new Vector3(actionText.GetComponent<RectTransform>().localPosition.x, -18f,actionText.GetComponent<RectTransform>().localPosition.z);
                        actionText.GetComponent<Text>().color = new Vector4(1, 1, 1, 0.5f);
                        dot.SetActive(true);
                        actionText.SetActive(true);

                        multiInteractionSelected = false;
                        multiInteractionSelectedChild = false;
                        multiInteractionSelectedChild2 = false;
                    }
                    else if (hit.collider.CompareTag("Object"))         //attachments
                    {
                        if(raycastedObject.GetComponent<AttachmentScript>() !=null)
                        {
                            if(raycastedObject.GetComponent<AttachmentScript>().Interact())
                            {
                                actionText.GetComponent<Text>().text = raycastedObject.GetComponent<InteractableObjectBase>().interactText1;
                                actionText.GetComponent<RectTransform>().localPosition = new Vector3(actionText.GetComponent<RectTransform>().localPosition.x, 0, actionText.GetComponent<RectTransform>().localPosition.z);
                                actionText.GetComponent<Text>().color = new Vector4(1, 1, 1, 1);
                            }
                            else
                            {

                            }
                        }

                        actionText.GetComponent<Text>().text = raycastedObject.GetComponent<InteractableObjectBase>().interactText;
                        actionText.GetComponent<RectTransform>().localPosition = new Vector3(actionText.GetComponent<RectTransform>().localPosition.x, 0, actionText.GetComponent<RectTransform>().localPosition.z);
                        actionText.GetComponent<Text>().color = new Vector4(1, 1, 1, 1);

                        raycastedObject.GetComponent<InteractableObjectBase>().Interact();

                        dot.SetActive(false);
                        actionText.SetActive(true);

                        multiInteractionSelected = false;
                        multiInteractionSelectedChild = false;
                        multiInteractionSelectedChild2 = false;

                    }
                    else if (hit.collider.CompareTag("ObjectMultiChild"))
                    {
                        raycastedObject.GetComponent<InteractableObjectBase>().InteractMulti();

                        //switch correct parent interaction by click on kid's interactive field
                        if (Input.GetKeyDown(interactButton) || Input.GetKeyDown(interactButton2))
                        {
                            int tmpValue = raycastedObject.GetComponent<InteractableObjectBase>().kidID;
                            raycastedObject.transform.parent.GetComponent<InteractableObjectBase>().kidID = tmpValue;
                        }

                        if (raycastedObject.transform.parent.tag == "Object")
                        {
                            raycastedObject.transform.parent.GetComponent<InteractableObjectBase>().Interact();
                        }
                        else
                        {
                            raycastedObject.transform.parent.GetComponent<InteractableObjectBase>().InteractMulti();
                        }

                        dot.SetActive(true);
                        actionText.SetActive(false);

                        multiInteractionSelected = true;
                        multiInteractionSelectedChild = true;
                        multiInteractionSelectedChild2 = true;
                    }

                    else if (hit.collider.CompareTag("ObjectMultiInteractions"))
                    {
                        actionText.GetComponent<Text>().text = raycastedObject.GetComponent<InteractableObjectBase>().interactText;
                        actionText.GetComponent<RectTransform>().localPosition = new Vector3(actionText.GetComponent<RectTransform>().localPosition.x, 0, actionText.GetComponent<RectTransform>().localPosition.z);
                        actionText.GetComponent<Text>().color = new Vector4(1, 1, 1, 1);
                        dot.SetActive(false);
                        actionText.SetActive(true);

                        raycastedObject.GetComponent<InteractableObjectBase>().InteractMulti();
                        multiInteractionSelected = true;
                        multiInteractionSelectedChild2 = false;

                    }

                    else if (hit.collider.CompareTag("ObjectInspectOnly"))
                    {
                        actionText.GetComponent<Text>().text = raycastedObject.GetComponent<InteractableObjectBase>().interactText;
                        actionText.GetComponent<RectTransform>().localPosition = new Vector3(actionText.GetComponent<RectTransform>().localPosition.x, 0, actionText.GetComponent<RectTransform>().localPosition.z);
                        actionText.GetComponent<Text>().color = new Vector4(1, 0, 0, 1);

                        raycastedObject.GetComponent<InteractableObjectBase>().Interact();

                        dot.SetActive(false);
                        actionText.SetActive(true);

                        multiInteractionSelected = false;
                        multiInteractionSelectedChild = false;
                        multiInteractionSelectedChild2 = false;
                    }
                }

                if (lastFrameRayHitSurface)
                {
                    lastFrameRayHitSurface = false;
                    pickUpManager.disableVisualPutAre();
                }

                selected = true;
            }
            //else if(Physics.Raycast(transform.position, fwd, out hit, rayLength, layerMaskInteract.value))
            else if (selected)
            {
                selected = false;

                actionText.SetActive(false);
                dot.SetActive(true);


                if (multiInteractionSelectedChild2)
                    raycastedObject = raycastedObject.transform.parent.gameObject;

                if (raycastedObject.GetComponent<InteractableObjectBase>().outlineObjects != null)
                {
                    foreach (GameObject element in raycastedObject.GetComponent<InteractableObjectBase>().outlineObjects)
                    {
                        element.GetComponent<MeshRenderer>().enabled = false;
                    }
                }


                if (multiInteractionSelected)
                {
                    multiInteractionSelected = false;
                }

                raycastedObject.GetComponent<InteractableObjectBase>().DeInteractMulti();
                raycastedObject.GetComponent<InteractableObjectBase>().DeInteract();

                /////////////highlight object OFF//////////////////////
                foreach (Transform child in raycastedObject.transform)
                {
                    foreach (Transform childChild in child)
                    {
                        if (childChild.GetComponent<MeshRenderer>())
                        {
                            childChild.GetComponent<MeshRenderer>().material.SetColor("_Color", new Vector4(0.5882352941176471f, 0.5882352941176471f, 0.5882352941176471f, 1));
                            childChild.GetComponent<MeshRenderer>().material.SetColor("_SpecularColor", new Vector4(0.0f, 0.0f, 0.0f, 1));
                        }
                    }

                }

                raycastedObject = null;
            }
        }
        //else if (dropMode)
        //{
        //    //disable selection in drop mode if something was selected before
        //    if (raycastedObject != null)
        //    {
        //        if (multiInteractionSelectedChild2)
        //            raycastedObject = raycastedObject.transform.parent.gameObject;

        //        foreach (GameObject element in raycastedObject.GetComponent<InteractableObjectBase>().outlineObjects)
        //        {
        //            element.GetComponent<MeshRenderer>().enabled = false;
        //        }
        //    }
        //}
        else if (peepHoleMode)
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
        else if(pickUpManager.itemMode == PickUpManager.enManagerItemMode.inspectMode)
        {
            actionText.active = false;
            dot.active = false;
        }
    }
}