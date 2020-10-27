using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PickUpManager : MonoBehaviour
{
    public Text title;
    public Text press;
    public Text description;
    public Text inProgressBar;
    public Text controlInfo;
    public Text bin;                        //additional Text to collect non readable data

    [SerializeField] private GameObject invButton;
    [SerializeField] private Text invButtonText;

    public bool freezeInspectRotationFlag = false;
    public bool freezeDescriptionOnScreen = false;
    public bool pressOffsetFlag = false;
    public bool pulseTextLoopFlag = false;
    private bool fadeInTextFlag = false;
    private bool stopCoroutine = false;                     //if fade in coroutine is playing during fade out coroutine disable fade in
    private bool delivereToHand = false;

    private Vector3 pressOffset = new Vector3(0, 0, 0);
    private Vector3 pressNoOffset;
           
    private bool isGrabbedFirstTime = false;

   [SerializeField] private FocusSwitcher focus;
   [SerializeField] private PlayerMove playerMove;
   [SerializeField] private PlayerLook playerLook;
   [SerializeField] private FindInteraction interaction;
   [SerializeField] private LayerMask layerMaskIgnore;
   [SerializeField] private GameObject handHandleItemAttach;
   [SerializeField] private GameObject handHandlePivot;

    //shaders
    [SerializeField] private Shader originalShader;
    [SerializeField] private Shader transparecyShader;

    private Color dropOriginalPosColor = new Color(0.333f, 0.384f, 0.576f, 0.6f);
    private Color dropNormalColor = new Color(0.176f, 0.176f, 0.176f, 0.6f);
    private Color dropActionColor = new Color(0.878f, 0.650f, 0.384f, 0.6f);

    //make a distinction between interaction stages of selected obj
    public enum enManagerItemMode { clear ,isGrabbed, returnToPos, getToPos ,inHand, inspectMode, dropped, moveToHand, wait};
    public enManagerItemMode itemMode;

    [SerializeField] private KeyCode putAwayButton = KeyCode.Y;

    [SerializeField] private KeyCode returnToHandMouseButton = KeyCode.Mouse1;

    public Transform destinationPosPick;
    public Transform destinationPosInspect;

    public Vector3 originGrabbedItemPos;
    public Quaternion originGrabbedItemRot;

    private Quaternion currentItemRot;

    public GameObject lastSelectedObj;
    private Vector3 lastObjPos;

    private Vector3 objectInspectScale;
    private Vector3 objectDefaultScale;

    //values to calculate 3/4 distance between origin item pos and destination point. When the iteam crooss the distance enable player controller
    public float distance; //whole distance
    private float actualDistance; // <= distance
    private bool smallDistanceValueFlag = false;        //set true to allow lerp function finished and allow next item mode function set active

    public KeyCode inspectKey = KeyCode.Mouse1;
    public KeyCode putDownKey = KeyCode.Tab;

    public PostProcessVolume volume;

    [SerializeField] private float rayLength = 10.0f;
    private bool anyHitRayInteractionPress = false;                  //disable text when any interactive direction of item is not used during frame lifetime 
    private bool anyHitRayInteractionDescriptions = false;

    public bool[] inspectModeDirInteractionFlags = new bool[4];

    //item movement in hand
    [SerializeField] private float  intensitySway = 1;
    [SerializeField] private float smoothSway = 10;
    private Vector3 targetItemBobPos;
    private float idleCounter = 0;
    private float moveCounter = 0;
    public float forceValueToDropItem = 3;

    //copy  of selected obj -  for placing item system
    public GameObject copiedObj;
    private bool objDestructible;
    private bool stopBlinkText = false;
    private bool inspectObjOnly = false;

    [SerializeField] private GameObject binFolderParent;
    [SerializeField] private PlayerRHand playerRHand;

    public InventoryScript inventory;
    public Transform invParent;


    private void Start()
    {

        //default - no object selected
        itemMode = enManagerItemMode.clear;

        title.enabled = false;
        press.enabled = false;
        description.enabled = false;
        inProgressBar.enabled = false;
        controlInfo.enabled = false;

        invButton.SetActive(false);
        invButtonText.enabled = false;

        for (int i = 0; i < inspectModeDirInteractionFlags.Length; i++)
            inspectModeDirInteractionFlags[i] = false;

        pressNoOffset = press.GetComponent<RectTransform>().localPosition;

        
    }

    public void PickUpFromInventory(GameObject selectedObject)
    {
        lastSelectedObj = selectedObject;
        selectedObject.transform.parent = destinationPosPick;
        selectedObject.transform.position = destinationPosPick.position;
        selectedObject.transform.rotation = destinationPosPick.rotation;
        focus.OnDisable();
        playerMove.inspectMode = false;
        delivereToHand = true;
        itemMode = enManagerItemMode.inHand;
    }

    public void PickUp(GameObject selectedObject, float objInspectModeOffsetScale = 10, float titleOffset = 0, bool destructible = false)
    {
        if (lastSelectedObj == null && !selectedObject.GetComponent<ItemBase>().actualStateItemDescriptinShowed)
        {
            isGrabbedFirstTime = true;
            title.text = selectedObject.GetComponent<ItemBase>().titleTxt;
            description.text = selectedObject.GetComponent<ItemBase>().descriptionTxt;
            press.text = selectedObject.GetComponent<ItemBase>().pressTxt;
            controlInfo.text = selectedObject.GetComponent<ItemBase>().controlText;
            inProgressBar.text = selectedObject.GetComponent<ItemBase>().inProgressBarTxt;

        }

        inspectObjOnly = false;

        if (selectedObject.GetComponent<Rigidbody>())
            selectedObject.GetComponent<Rigidbody>().useGravity = false;

        if (selectedObject.GetComponent<BoxCollider>())
            selectedObject.GetComponent<BoxCollider>().enabled = false;

        if (selectedObject.GetComponent<ItemBase>())
            selectedObject.GetComponent<ItemBase>().SpecialActionAfterGrab();

        lastSelectedObj = selectedObject;
        objDestructible = destructible;

        selectedObject.transform.parent = destinationPosPick.transform;
        volume.weight = 0;

        objInspectModeOffsetScale = selectedObject.GetComponent<ItemManager>().objInspectModeOffset;
        objectInspectScale = new Vector3(selectedObject.transform.localScale.x * objInspectModeOffsetScale, selectedObject.transform.localScale.y * objInspectModeOffsetScale, selectedObject.transform.localScale.z * objInspectModeOffsetScale);
        objectDefaultScale = new Vector3(selectedObject.transform.localScale.x, selectedObject.transform.localScale.y, selectedObject.transform.localScale.z);

        //clone with no components
        copiedObj = Instantiate(selectedObject, this.transform);

        foreach (var comp in copiedObj.GetComponents<Component>())
        {
            if (!(comp is Transform))
            {
                Destroy(comp);
            }
        }

        foreach(Transform el in copiedObj.transform)
        {
            if(el.gameObject.transform.childCount==0)
            {
                Destroy(el.gameObject);
            }
        }

        copiedObj.SetActive(false);
        copiedObj.transform.parent = binFolderParent.transform;

        originGrabbedItemPos = selectedObject.GetComponent<ItemManager>().originPos;
        originGrabbedItemRot = selectedObject.GetComponent<ItemManager>().originRot;
        distance = Vector3.Distance(originGrabbedItemPos, destinationPosPick.position);
        delivereToHand = false;

        if (selectedObject.GetComponent<ItemBase>())
        {
            for (int i = 0; i < inspectModeDirInteractionFlags.Length; i++)
            {
                inspectModeDirInteractionFlags[i] = selectedObject.GetComponent<ItemBase>().inspectModeDirInteractionFlags[i];
            }
        }

        itemMode = enManagerItemMode.inHand;
    }

    ////pickUp object first time so its inspect mode first
    public void PickUpOnlyInspect(GameObject selectedObject, float objInspectModeOffsetScale = 1, float titleOffset = 0)
    {
        if (lastSelectedObj == null && !selectedObject.GetComponent<ItemBase>().actualStateItemDescriptinShowed)
        {
            isGrabbedFirstTime = true;
            title.text = selectedObject.GetComponent<ItemBase>().titleTxt;
            description.text = selectedObject.GetComponent<ItemBase>().descriptionTxt;
            press.text = selectedObject.GetComponent<ItemBase>().pressTxt;
            controlInfo.text = selectedObject.GetComponent<ItemBase>().controlText;
            controlInfo.text = "\n...PRESS <ESC> TO IT BACK...";

        }

        inspectObjOnly = true;

        if (selectedObject.GetComponent<Rigidbody>())
            selectedObject.GetComponent<Rigidbody>().useGravity = false;

        selectedObject.GetComponent<BoxCollider>().enabled = false;
        lastSelectedObj = selectedObject;

        volume.weight = 0f;
        focus.SetFocused(gameObject);

        playerMove.inspectMode = true;

        selectedObject.transform.parent = destinationPosInspect.transform;

        objInspectModeOffsetScale = selectedObject.GetComponent<ItemManager>().objInspectModeOffset;
        objectInspectScale = new Vector3(selectedObject.transform.localScale.x * objInspectModeOffsetScale, selectedObject.transform.localScale.y * objInspectModeOffsetScale, selectedObject.transform.localScale.z * objInspectModeOffsetScale);
        objectDefaultScale = new Vector3(selectedObject.transform.localScale.x, selectedObject.transform.localScale.y, selectedObject.transform.localScale.z);


        originGrabbedItemPos = selectedObject.GetComponent<ItemManager>().originPos;
        originGrabbedItemRot = selectedObject.GetComponent<ItemManager>().originRot;

        destinationPosInspect.position = new Vector3(destinationPosInspect.position.x, destinationPosInspect.position.y, destinationPosInspect.position.z);
        distance = Vector3.Distance(originGrabbedItemPos, destinationPosInspect.position);

        if (selectedObject.GetComponent<ItemBase>())
        {
            for (int i = 0; i < inspectModeDirInteractionFlags.Length; i++)
            {
                inspectModeDirInteractionFlags[i] = selectedObject.GetComponent<ItemBase>().inspectModeDirInteractionFlags[i];
            }
        }

        itemMode = enManagerItemMode.isGrabbed;
    }

    public void ActivateSpecialAction(GameObject selectedObject)
    {
        //selectedObject.GetComponent<ItemBase>().ActivateSpecialAction();
    }

    private void PutAway(GameObject selectedObject)
    {
        if (selectedObject.GetComponent<Rigidbody>())
        {
           selectedObject.GetComponent<Rigidbody>().useGravity = true;
           selectedObject.GetComponent<Rigidbody>().isKinematic = false;
        }
            

        selectedObject.GetComponent<BoxCollider>().enabled = true;

        if (selectedObject.GetComponent<ItemBase>())
            selectedObject.GetComponent<ItemBase>().SpecialActionAfterDrop();

        lastSelectedObj = null;

        title.enabled = false;
        press.enabled = false;
        description.enabled = false;
        inProgressBar.enabled = false;
        controlInfo.enabled = false;

        stopCoroutine = false;
        itemMode = enManagerItemMode.clear;
    }

    private void Drop(GameObject selectedObject)
    {
        selectedObject.GetComponent<BoxCollider>().enabled = true;
        selectedObject.GetComponent<BoxCollider>().isTrigger = false;

        lastSelectedObj.transform.parent = lastSelectedObj.GetComponent<ItemManager>().parent;

        if (selectedObject.GetComponent<Rigidbody>())
        {
            selectedObject.GetComponent<Rigidbody>().useGravity = true;
            selectedObject.GetComponent<Rigidbody>().isKinematic = false;
        }
            

        focus.OnDisable();

        lastSelectedObj = null;


        itemMode = enManagerItemMode.clear;
    }

    //if u put away item but not use dedicated functions to do it ex.plate to dryer(dryerManager)
    public void ForceToClearHandNoGravity(GameObject selectedObject)
    {
        selectedObject.GetComponent<BoxCollider>().enabled = true;
        selectedObject.GetComponent<Rigidbody>().useGravity = false;
        selectedObject.GetComponent<Rigidbody>().isKinematic = true;
        focus.OnDisable();
        lastSelectedObj = null;
        itemMode = enManagerItemMode.clear;
    }

    private void Update()
    {
        if(itemMode == enManagerItemMode.isGrabbed)
        {
            lastSelectedObj.transform.position = new Vector3(Mathf.Lerp(lastSelectedObj.transform.position.x, destinationPosInspect.position.x, Time.deltaTime *10), Mathf.Lerp(lastSelectedObj.transform.position.y, destinationPosInspect.position.y, Time.deltaTime * 10), Mathf.Lerp(lastSelectedObj.transform.position.z, destinationPosInspect.position.z, Time.deltaTime * 10));
            lastSelectedObj.transform.localScale = objectInspectScale;
            lastSelectedObj.transform.localEulerAngles = new Vector3(0, 0, 0);

            volume.weight = Mathf.Lerp(volume.weight, 1, Time.deltaTime * 5.0f);

            actualDistance = Vector3.Distance(lastSelectedObj.transform.position, destinationPosInspect.position);

            if(distance/6.0f >= actualDistance)
            {
                smallDistanceValueFlag = true;
                title.color = new Vector4(title.color.r, title.color.g, title.color.b, 0);
                controlInfo.color = new Vector4(controlInfo.color.r, controlInfo.color.g, controlInfo.color.b, 1);
                if(freezeDescriptionOnScreen)
                {
                    description.color = new Vector4(description.color.r, description.color.g, description.color.b, 1);
                }
                fadeInTextFlag = true;
                itemMode = enManagerItemMode.inspectMode;
            }

            lastObjPos = lastSelectedObj.transform.position;
        }
        else if(itemMode == enManagerItemMode.returnToPos)
        {
            if(copiedObj!=null)
            {
                originGrabbedItemPos = copiedObj.transform.position;
                Destroy(copiedObj);
                copiedObj = null;
            }

            lastSelectedObj.transform.localScale = objectDefaultScale;
            lastSelectedObj.transform.position = new Vector3(Mathf.Lerp(lastSelectedObj.transform.position.x, originGrabbedItemPos.x, Time.deltaTime * 10), Mathf.Lerp(lastSelectedObj.transform.position.y, originGrabbedItemPos.y, Time.deltaTime * 10), Mathf.Lerp(lastSelectedObj.transform.position.z, originGrabbedItemPos.z, Time.deltaTime * 10));
            lastSelectedObj.transform.rotation = originGrabbedItemRot;

            volume.weight = Mathf.Lerp(volume.weight, 0, Time.deltaTime * 10.0f);
            actualDistance = Vector3.Distance(lastSelectedObj.transform.position, originGrabbedItemPos);

            if (distance / 6.0f >= actualDistance)
            {
                playerMove.inspectMode = false;
                lastSelectedObj.transform.parent = lastSelectedObj.GetComponent<ItemManager>().parent;
                volume.weight = 0f;
                focus.OnDisable();
            }

            if (lastSelectedObj.transform.position == lastObjPos)
            {
                PutAway(lastSelectedObj);
                return;
            }

            lastObjPos = lastSelectedObj.transform.position;
        }
        else if(itemMode == enManagerItemMode.getToPos)
        {
            //playerLook.disableCamera = true;
            //playerMove.disablePlayerController = true;

            lastSelectedObj.transform.localPosition = new Vector3(Mathf.Lerp(lastSelectedObj.transform.localPosition.x, originGrabbedItemPos.x, Time.deltaTime *10), Mathf.Lerp(lastSelectedObj.transform.localPosition.y, originGrabbedItemPos.y, Time.deltaTime * 10), Mathf.Lerp(lastSelectedObj.transform.localPosition.z, originGrabbedItemPos.z, Time.deltaTime * 10));
            //lastSelectedObj.transform.localScale = objectDefaultScale;
            lastSelectedObj.transform.rotation = originGrabbedItemRot;

            volume.weight = Mathf.Lerp(volume.weight, 0, Time.deltaTime * 10.0f);
            actualDistance = Vector3.Distance(lastSelectedObj.transform.localPosition, originGrabbedItemPos);

            if (distance / 6.0f >= actualDistance)
            {
                volume.weight = 0f;
                focus.OnDisable();
            }

        }   
        else if(itemMode == enManagerItemMode.inspectMode)
        {
            ////////////////copy from isGrabbed mode to finish transformation//////////////////////
            
            if(smallDistanceValueFlag)
            {
                lastSelectedObj.transform.position = new Vector3(Mathf.Lerp(lastSelectedObj.transform.position.x, destinationPosInspect.position.x, Time.deltaTime * 10), Mathf.Lerp(lastSelectedObj.transform.position.y, destinationPosInspect.position.y, Time.deltaTime * 10), Mathf.Lerp(lastSelectedObj.transform.position.z, destinationPosInspect.position.z, Time.deltaTime * 10));
                lastSelectedObj.transform.localScale = objectInspectScale;
                volume.weight = Mathf.Lerp(volume.weight, 1, Time.deltaTime * 5.0f);
                actualDistance = Vector3.Distance(lastSelectedObj.transform.position, destinationPosInspect.position);

                if (lastSelectedObj.transform.position == lastObjPos)
                {
                    smallDistanceValueFlag = false;
                }

                lastObjPos = lastSelectedObj.transform.position;
            }

            //////////////////////////////////////////////////////////////////////////////////////


            title.enabled = true;
            if(!inspectObjOnly)
            {
                controlInfo.enabled = false;
            }
            

            if (freezeDescriptionOnScreen)
                description.enabled = true;

            if (fadeInTextFlag && isGrabbedFirstTime)
            {
                StartCoroutine(FadeInTextLoop(title));
                StartCoroutine(FadeInTextLoop(controlInfo));
                fadeInTextFlag = !fadeInTextFlag;
            }
            else if(fadeInTextFlag && !isGrabbedFirstTime)
            {
                StartCoroutine(FadeInTextLoop(title));
                StartCoroutine(FadeInTextLoop(description));
                fadeInTextFlag = !fadeInTextFlag;
            }

            if(!freezeInspectRotationFlag)
            {
                RotateObj(lastSelectedObj, lastSelectedObj.GetComponent<ItemBase>().isRotationVertical);
            }

            
            if(!stopBlinkText)
            {
                stopBlinkText = true;
                StartCoroutine(blinkText());
            }
           

            InspectInteractionFind(lastSelectedObj);
        }
        else if(itemMode == enManagerItemMode.moveToHand)
        {
            lastSelectedObj.transform.position = new Vector3(Mathf.MoveTowards(lastSelectedObj.transform.position.x, destinationPosPick.position.x, Time.deltaTime * 10), Mathf.Lerp(lastSelectedObj.transform.position.y, destinationPosPick.position.y, Time.deltaTime * 10), Mathf.Lerp(lastSelectedObj.transform.position.z, destinationPosPick.position.z, Time.deltaTime * 10));
            lastSelectedObj.transform.localScale = objectDefaultScale;
            lastSelectedObj.transform.localEulerAngles = new Vector3(0, 0, 0);

            volume.weight = Mathf.Lerp(volume.weight, 0, Time.deltaTime * 5.0f);

            actualDistance = Vector3.Distance(lastSelectedObj.transform.position, destinationPosPick.position);

            if(distance / 3.0f >= actualDistance)
            {
                if(volume.weight <= 0.05f)
                {
                    delivereToHand = false;
                    itemMode = enManagerItemMode.inHand;
                }
            }

            lastObjPos = lastSelectedObj.transform.position;

        }
        else if(itemMode == enManagerItemMode.clear)
        {
            title.enabled = false;
            press.enabled = false;
            description.enabled = false;
            inProgressBar.enabled = false;

            if (copiedObj != null)
                Destroy(copiedObj);
        }
        else if(itemMode == enManagerItemMode.inHand)
        {
            if (!delivereToHand)
            {
                lastSelectedObj.transform.position = destinationPosPick.position;
                lastSelectedObj.transform.rotation = destinationPosPick.rotation;
                lastSelectedObj.transform.parent = handHandleItemAttach.transform;
                currentItemRot = handHandlePivot.transform.localRotation;
                delivereToHand = true;
                idleCounter = 0;
                moveCounter = 0;
            }

            if(delivereToHand)
            {
                HoldInHand(lastSelectedObj);
            }
            

        }
        else if(itemMode == enManagerItemMode.dropped)
        {
            Drop(lastSelectedObj);
        }

    }

    public void PutAwayObject()
    {

    }

    public void InspectInteractionFind(GameObject obj)
    {
        if(Input.GetKeyUp(KeyCode.Mouse1) && !isGrabbedFirstTime && !inspectObjOnly)
        {
            stopCoroutine = true;
            StartCoroutine(FadeOutTextLoop(title));
            StartCoroutine(FadeOutTextLoop(press));
            StartCoroutine(FadeOutTextLoop(description));
            StartCoroutine(FadeOutTextLoop(controlInfo));

            lastSelectedObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            currentItemRot = handHandlePivot.transform.localRotation;

            focus.OnDisable();
            playerMove.inspectMode = false;
            delivereToHand = false;
            stopBlinkText = false;

            distance = UpdateDistance(destinationPosPick.position);

            itemMode = enManagerItemMode.moveToHand;
            return;
        }

        if(Input.GetKeyDown(putAwayButton) && isGrabbedFirstTime && inspectObjOnly)
        {
            stopCoroutine = true;
            StartCoroutine(FadeOutTextLoop(title));
            StartCoroutine(FadeOutTextLoop(press));
            StartCoroutine(FadeOutTextLoop(description));
            StartCoroutine(FadeOutTextLoop(controlInfo));

            itemMode = enManagerItemMode.returnToPos;
            return;
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1) && isGrabbedFirstTime && !inspectObjOnly)
        {
            stopCoroutine = true;
            StartCoroutine(FadeOutTextLoop(title));
            StartCoroutine(FadeOutTextLoop(description));
            StartCoroutine(FadeOutTextLoop(controlInfo));

            lastSelectedObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            currentItemRot = handHandlePivot.transform.localRotation;

            focus.OnDisable();
            playerMove.inspectMode = false;
            delivereToHand = false;
            itemMode = enManagerItemMode.inHand;
            return;
        }

        anyHitRayInteractionPress = false;
        anyHitRayInteractionDescriptions = false;

        RaycastHit hit;
        Vector3 up = obj.transform.up;
        Vector3 down = -obj.transform.up;
        Vector3 front = obj.transform.forward;
        Vector3 back = -obj.transform.forward;

        Debug.DrawRay(obj.transform.position, up, Color.green);
        Debug.DrawRay(obj.transform.position, down, Color.red);

        if(inspectModeDirInteractionFlags[0])
        {
            if (Physics.Raycast(obj.transform.position, up, out hit, rayLength))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    press.text = obj.GetComponent<ItemBase>().InteractionUp();
                    ///////////////when botton description is written///////////////////
                    if(!freezeDescriptionOnScreen)
                        description.text = obj.GetComponent<ItemBase>().ShowInfoUp();
                    else
                        bin.text = obj.GetComponent<ItemBase>().ShowInfoUp();
                    ///////////////////////////////////////////////////////////////////
                    if (press.text != "")
                    {
                        if (!pressOffsetFlag)
                        {
                            press.GetComponent<RectTransform>().localPosition = pressNoOffset;
                        }
                        else
                        {
                            press.GetComponent<RectTransform>().localPosition = pressOffset;
                        }

                        anyHitRayInteractionPress = true;
                        press.enabled = true;
                    }
                    else
                    {
                        anyHitRayInteractionDescriptions = true;
                        description.enabled = true;
                    }
                }

            }
        }

        if (inspectModeDirInteractionFlags[1])
        {
            if (Physics.Raycast(obj.transform.position, down, out hit, rayLength))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    press.text = obj.GetComponent<ItemBase>().InteractionDown();
                    ///////////////when botton description is written///////////////////
                    if (!freezeDescriptionOnScreen)
                        description.text = obj.GetComponent<ItemBase>().ShowInfoDown();
                    else
                        bin.text = obj.GetComponent<ItemBase>().ShowInfoDown();
                    ///////////////////////////////////////////////////////////////////
                    if (press.text != "")
                    {
                        if(!pressOffsetFlag)
                        {
                            press.GetComponent<RectTransform>().localPosition = pressNoOffset;
                        }
                        else
                        {
                            press.GetComponent<RectTransform>().localPosition = pressOffset;
                        }

                        anyHitRayInteractionPress = true;
                        press.enabled = true;

                    }
                    else
                    {
                        anyHitRayInteractionDescriptions = true;
                        description.enabled = true;
                    }
                }

            }
        }

        if (inspectModeDirInteractionFlags[2])
        {
            if (Physics.Raycast(obj.transform.position, front, out hit, rayLength))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    press.text = obj.GetComponent<ItemBase>().InteractionFront();
                    ///////////////when botton description is written///////////////////
                    if (!freezeDescriptionOnScreen)
                        description.text = obj.GetComponent<ItemBase>().ShowInfoFront();
                    else
                        bin.text = obj.GetComponent<ItemBase>().ShowInfoFront();
                    ///////////////////////////////////////////////////////////////////
                    if (press.text != "")
                    {
                        if (!pressOffsetFlag)
                        {
                            press.GetComponent<RectTransform>().localPosition = pressNoOffset;
                        }
                        else
                        {
                            press.GetComponent<RectTransform>().localPosition = pressOffset;
                        }

                        anyHitRayInteractionPress = true;
                        press.enabled = true;

                    }
                    else
                    {
                        anyHitRayInteractionDescriptions = true;
                        description.enabled = true;
                    }
                }
            }
        }

        if (inspectModeDirInteractionFlags[3])
        {
            if (Physics.Raycast(obj.transform.position, back, out hit, rayLength))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    press.text = obj.GetComponent<ItemBase>().InteractionBack();
                    ///////////////when botton description is written///////////////////
                    if (!freezeDescriptionOnScreen)
                        description.text = obj.GetComponent<ItemBase>().ShowInfoBack();
                    else
                        bin.text = obj.GetComponent<ItemBase>().ShowInfoBack();
                    ///////////////////////////////////////////////////////////////////
                    if (press.text != "")
                    {
                        if (!pressOffsetFlag)
                        {
                            press.GetComponent<RectTransform>().localPosition = pressNoOffset;
                        }
                        else
                        {
                            press.GetComponent<RectTransform>().localPosition = pressOffset;
                        }

                        anyHitRayInteractionPress = true;
                        press.enabled = true;

                    }
                    else
                    {
                        anyHitRayInteractionDescriptions = true;
                        description.enabled = true;
                    }
                }
            }
        }

        if (!anyHitRayInteractionPress)
            press.enabled = false;


        if (!anyHitRayInteractionDescriptions)
        {
            if(!freezeDescriptionOnScreen)
                description.enabled = false;
        }
           

    }

    public void HoldInHand(GameObject obj)
    {
        if(Input.GetKey(KeyCode.Mouse1))
        {
            stopCoroutine = false;
            isGrabbedFirstTime = false;

            lastSelectedObj.transform.parent = destinationPosInspect.transform;

            StartCoroutine(FadeOutTextLoop(title));
            StartCoroutine(FadeOutTextLoop(description));

            focus.SetFocused(lastSelectedObj);
            playerMove.inspectMode = true;

            originGrabbedItemPos = obj.transform.position;
            distance = UpdateDistance(destinationPosInspect.position);

            itemMode = enManagerItemMode.isGrabbed;
            return;
        }
        else if(Input.GetKeyDown(KeyCode.G))
        {
            playerRHand.button = invButton;
            playerRHand.buttonTxt = invButtonText;
            ButtonOn(invButton, invButtonText);
            playerRHand.HideObjAnim();
            return;
        }

        invButton.SetActive(true);
        invButtonText.enabled = true;

        float t_hmove = Input.GetAxisRaw("Horizontal");
        float t_vmove = Input.GetAxisRaw("Vertical");

        if(t_hmove==0 && t_vmove==0)
        {
            ItemBobing(idleCounter, 0.005f, 0.005f);
            idleCounter += Time.deltaTime;
            handHandlePivot.transform.localPosition = Vector3.Lerp(handHandlePivot.transform.localPosition, targetItemBobPos, Time.deltaTime * 2f);
        }
        else
        {
            ItemBobing(moveCounter, 0.02f, 0.01f);
            moveCounter += Time.deltaTime *7;
            handHandlePivot.transform.localPosition = Vector3.Lerp(handHandlePivot.transform.localPosition, targetItemBobPos, Time.deltaTime * 6f);
        }

        ItemSway(obj);
    }

    private void RotateObj(GameObject obj, bool isRotationVertical)
    {
        float mouseY = Input.GetAxis("Mouse Y") * 150.0f * Time.deltaTime;

        if(isRotationVertical)
            obj.transform.Rotate(mouseY, 0, 0);
        else
            obj.transform.Rotate(0, mouseY, 0);
    }

    private void ItemBobing(float p_z, float p_x_intensity, float p_y_intensity)
    {
        targetItemBobPos = handHandlePivot.transform.localPosition + new Vector3(Mathf.Cos(p_z) * p_x_intensity, Mathf.Sin(p_z * 2) * p_y_intensity, 0);
    }

    private void ItemSway(GameObject obj)
    {
        //controls
        float t_x_mouse = Input.GetAxis("Mouse X");
        float t_y_mouse = Input.GetAxis("Mouse Y");

        ////calculate target rotation
        Quaternion t_x_adj = Quaternion.AngleAxis(-intensitySway * t_x_mouse, Vector3.up);
        Quaternion t_y_adj = Quaternion.AngleAxis(intensitySway * t_y_mouse, Vector3.right);
        Quaternion target_rotation = currentItemRot * t_x_adj * t_y_adj;

        //rotate towards target rotation
        handHandlePivot.transform.localRotation = Quaternion.Lerp(handHandlePivot.transform.localRotation, target_rotation, Time.deltaTime * smoothSway);


        if (objDestructible)
        {
            if (t_x_mouse > forceValueToDropItem || t_x_mouse < -forceValueToDropItem || t_y_mouse > forceValueToDropItem || t_y_mouse < -forceValueToDropItem)
            {
                lastSelectedObj.GetComponent<ItemBase>().itemDrop = true;
                itemMode = enManagerItemMode.dropped;
            }
        }
        
    }

    public IEnumerator FadeInTextLoop(Text text)
    {
        float value = 0.05f;
        while(text.color.a < 1 && !stopCoroutine)
        {
            text.color = new Vector4(text.color.r, text.color.g, text.color.b, text.color.a + value);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator FadeOutTextLoop(Text text)
    {
        float value = 0.2f;
        while (text.color.a > 0)
        {
            text.color = new Vector4(text.color.r, text.color.g, text.color.b, text.color.a - value);
            yield return new WaitForSeconds(0.01f);
        }

        text.enabled = false;
    }

   //public void visualObjectPutArea(Vector3 hitPos, GameObject hitObj)
   // {
   //     copiedObj.SetActive(true);



   //     copiedObj.transform.position = new Vector3(hitPos.x, hitObj.transform.position.y, hitPos.z);
   //     copiedObj.transform.rotation = originGrabbedItemRot;

   //     int i = 0;
   //     foreach (Transform child in copiedObj.transform)
   //     {
   //         if(child.GetComponent<MeshRenderer>())
   //         {
   //             child.GetComponent<MeshRenderer>().material.shader = transparecyShader;
   //             child.GetComponent<MeshRenderer>().material.color = new Color(child.GetComponent<MeshRenderer>().material.color.r, child.GetComponent<MeshRenderer>().material.color.g, child.GetComponent<MeshRenderer>().material.color.b, 0.5f);
   //             i++;
   //         }
   //     }

   //     copiedObj.transform.rotation = new Quaternion(0, 0, 0, 0);

   //     if(Input.GetKeyDown(KeyCode.Mouse0))
   //     {
   //         itemMode = enManagerItemMode.returnToPos;
   //         playerMove.inspectMode = true;
   //     }
   // }

    public void disableVisualPutAre()
    {
        if(copiedObj != null)
            copiedObj.SetActive(false);
    }

    public void visualObjectPutArea(GameObject hitObj, int colorId)
    {
        copiedObj.SetActive(true);
        Color color1;
        switch(colorId)
        {
            case 0:
                color1 = dropNormalColor;
                break;
            case 1:
                color1 = dropOriginalPosColor;
                break;
            case 2:
                color1 = dropActionColor;
                break;
            default:
                color1 = dropNormalColor;
                break;
        }

        if (hitObj.transform.parent.GetComponent<DropSlotScript>())
        {
            copiedObj.transform.position = hitObj.transform.parent.GetComponent<DropSlotScript>().dropPos.position;
            copiedObj.transform.localRotation = hitObj.transform.parent.GetComponent<DropSlotScript>().dropPos.localRotation;

            foreach (Transform child in copiedObj.transform)
            {
                if (child.GetComponent<MeshRenderer>())
                {
                    child.GetComponent<MeshRenderer>().material.shader = transparecyShader;
                    child.GetComponent<MeshRenderer>().material.color = color1;
                }

                foreach(Transform child1 in child)
                {
                    if (child1.GetComponent<MeshRenderer>())
                    {
                        child1.GetComponent<MeshRenderer>().material.shader = transparecyShader;
                        child1.GetComponent<MeshRenderer>().material.color = color1;
                    }  
                }
            }

        }
        else
        {
            copiedObj.transform.position = hitObj.transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            itemMode = enManagerItemMode.returnToPos;
            playerMove.inspectMode = true;
        }
    }

    public void UpdateDropObjClone()
    {
        if (copiedObj != null)
            Destroy(copiedObj);

        copiedObj = Instantiate(lastSelectedObj, this.transform);

        foreach (var comp in copiedObj.GetComponents<Component>())
        {
            if (!(comp is Transform))
            {
                Destroy(comp);
            }
        }

    }

    public float UpdateDistance(Vector3 destination)
    {
        float tmp;
        tmp = Vector3.Distance(originGrabbedItemPos, destination);
        return tmp;
    }

    public void HideInvAnim()
    {
        inventory.AddToInventory(lastSelectedObj);
        lastSelectedObj.transform.parent = invParent;
        lastSelectedObj.transform.position = invParent.position;
        itemMode = enManagerItemMode.clear;
    }

    public void ButtonOn(GameObject button, Text buttonTxt)
    {
        button.GetComponent<RectTransform>().localScale = new Vector3(0.45f, 0.35f, 0.45f);
        if(button.transform.GetChild(0).GetComponent<Image>())
        {
            button.transform.GetChild(0).GetComponent<Image>().color = new Vector4(button.transform.GetChild(0).GetComponent<Image>().color.r, button.transform.GetChild(0).GetComponent<Image>().color.g, button.transform.GetChild(0).GetComponent<Image>().color.b, 1f);
        }

        buttonTxt.color = new Vector4(1, 1, 1, 1);
        buttonTxt.GetComponent<RectTransform>().localPosition = new Vector3(-242, -364,0);
       
    }

    public void ButtonOff(GameObject button, Text buttonTxt)
    {
        button.GetComponent<RectTransform>().localScale = new Vector3(0.45f, 0.45f, 0.45f);
        if (button.transform.GetChild(0).GetComponent<Image>())
        {
            button.transform.GetChild(0).GetComponent<Image>().color = new Vector4(button.transform.GetChild(0).GetComponent<Image>().color.r, button.transform.GetChild(0).GetComponent<Image>().color.g, button.transform.GetChild(0).GetComponent<Image>().color.b, 0.49f);
        }

        buttonTxt.color = new Vector4(1, 1, 1, 0.784f);
        buttonTxt.GetComponent<RectTransform>().localPosition = new Vector3(-242, -359, 0);

        invButton.SetActive(false);
        invButtonText.enabled = false;
    }

    private IEnumerator blinkText()
    {
        bool disapearing = true;
        press.color = new Vector4(1, 1, 1, 1);

        while (stopBlinkText)
        {
            if (disapearing)
            {
                press.color = Vector4.MoveTowards(new Vector4(press.color.r, press.color.g, press.color.b, press.color.a), new Vector4(1, 1, 1, 0), Time.deltaTime*2);
                if (press.color.a == 0)
                {
                    disapearing = !disapearing;
                }
            }
            else
            {
                press.color = Vector4.MoveTowards(new Vector4(press.color.r, press.color.g, press.color.b, press.color.a), new Vector4(1, 1, 1, 1), Time.deltaTime*2);
                if (press.color.a == 1)
                {
                    disapearing = !disapearing;
                }
            }

            yield return null;
        }

    }
}
