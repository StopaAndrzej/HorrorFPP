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
   [SerializeField] private LayerMask layerMaskIgnore;
   [SerializeField] private GameObject handHandleItemAttach;
    [SerializeField] private GameObject handHandlePivot;

    //make a distinction between interaction stages of selected obj
    public enum enManagerItemMode { clear ,isGrabbed, returnToPos,stopGrab ,inHand, inspectMode, dropped};
    public enManagerItemMode itemMode;

    [SerializeField] private KeyCode putAwayButton = KeyCode.Tab;

    [SerializeField] private KeyCode returnToHandMouseButton = KeyCode.Mouse1;

    public Transform destinationPosPick;
    public Transform destinationPosInspect;

    private Vector3 originGrabbedItemPos;
    private Quaternion originGrabbedItemRot;

    private Quaternion currentItemRot;

    private GameObject lastSelectedObj;
    private Vector3 lastObjPos;

    //values to calculate 3/4 distance between origin item pos and destination point. When the iteam crooss the distance enable player controller
    private float distance; //whole distance
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

    private void Start()
    {
        //default - no object selected
        itemMode = enManagerItemMode.clear;

        title.enabled = false;
        press.enabled = false;
        description.enabled = false;
        inProgressBar.enabled = false;
        controlInfo.enabled = false;

        for (int i = 0; i < inspectModeDirInteractionFlags.Length; i++)
            inspectModeDirInteractionFlags[i] = false;

        pressNoOffset = press.GetComponent<RectTransform>().localPosition;
    }

    //pickUp object first time so its inspect mode first
    public void PickUp(GameObject selectedObject)
    {
        if(lastSelectedObj == null && !selectedObject.GetComponent<ItemBase>().actualStateItemDescriptinShowed)
        {
            isGrabbedFirstTime = true;
        }

        if (selectedObject.GetComponent<Rigidbody>())
            selectedObject.GetComponent<Rigidbody>().useGravity = false;

        selectedObject.GetComponent<BoxCollider>().enabled = false;
        lastSelectedObj = selectedObject;

        volume.weight = 0f;
        focus.SetFocused(gameObject);

        playerMove.inspectMode = true;

        selectedObject.transform.parent = destinationPosInspect.transform;

        originGrabbedItemPos = selectedObject.GetComponent<ItemManager>().originPos;
        originGrabbedItemRot = selectedObject.GetComponent<ItemManager>().originRot;

        distance = Vector3.Distance(originGrabbedItemPos, destinationPosInspect.position);

        if (selectedObject.GetComponent<ItemBase>())
        {
            for (int i = 0; i < inspectModeDirInteractionFlags.Length; i++)
            {
                inspectModeDirInteractionFlags[i] = selectedObject.GetComponent<ItemBase>().inspectModeDirInteractionFlags[i];
            }

            title.text = selectedObject.GetComponent<ItemBase>().titleTxt;
            press.text = selectedObject.GetComponent<ItemBase>().pressTxt;
            description.text = selectedObject.GetComponent<ItemBase>().descriptionTxt;
            inProgressBar.text = selectedObject.GetComponent<ItemBase>().inProgressBarTxt;
        }

        itemMode = enManagerItemMode.isGrabbed;
    }

    private void PutAway(GameObject selectedObject)
    {
        if (selectedObject.GetComponent<Rigidbody>())
            selectedObject.GetComponent<Rigidbody>().useGravity = false;

        selectedObject.GetComponent<BoxCollider>().enabled = true;
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

    private void Update()
    {
        if(itemMode == enManagerItemMode.isGrabbed)
        {
            lastSelectedObj.transform.position = new Vector3(Mathf.Lerp(lastSelectedObj.transform.position.x, destinationPosInspect.position.x, Time.deltaTime * 5.0f), Mathf.Lerp(lastSelectedObj.transform.position.y, destinationPosInspect.position.y, Time.deltaTime * 5.0f), Mathf.Lerp(lastSelectedObj.transform.position.z, destinationPosInspect.position.z, Time.deltaTime * 5.0f));
            lastSelectedObj.transform.localEulerAngles = new Vector3(0, 0, 0);

            volume.weight = Mathf.Lerp(volume.weight, 1, Time.deltaTime * 5.0f);

            actualDistance = Vector3.Distance(lastSelectedObj.transform.position, destinationPosInspect.position);

            if(distance/6.0f >= actualDistance)
            {
                smallDistanceValueFlag = true;
                title.color = new Vector4(title.color.r, title.color.g, title.color.b, 0);
                controlInfo.color = new Vector4(controlInfo.color.r, controlInfo.color.g, controlInfo.color.b, 0);
                if(freezeDescriptionOnScreen)
                {
                    description.color = new Vector4(description.color.r, description.color.g, description.color.b, 0);
                }
                fadeInTextFlag = true;
                itemMode = enManagerItemMode.inspectMode;
            }

            lastObjPos = lastSelectedObj.transform.position;
        }
        else if(itemMode == enManagerItemMode.returnToPos)
        {
            lastSelectedObj.transform.position = new Vector3(Mathf.Lerp(lastSelectedObj.transform.position.x, originGrabbedItemPos.x, Time.deltaTime * 5.0f), Mathf.Lerp(lastSelectedObj.transform.position.y, originGrabbedItemPos.y, Time.deltaTime * 5.0f), Mathf.Lerp(lastSelectedObj.transform.position.z, originGrabbedItemPos.z, Time.deltaTime * 5.0f));
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
        else if(itemMode == enManagerItemMode.stopGrab)
        {
            lastSelectedObj.transform.position = new Vector3(Mathf.Lerp(lastSelectedObj.transform.position.x, destinationPosPick.position.x, Time.deltaTime * 5.0f), Mathf.Lerp(lastSelectedObj.transform.position.y, destinationPosPick.position.y, Time.deltaTime * 5.0f), Mathf.Lerp(lastSelectedObj.transform.position.z, destinationPosPick.position.z, Time.deltaTime * 5.0f));
            volume.weight = Mathf.Lerp(volume.weight, 0, Time.deltaTime * 5.0f);

            lastSelectedObj.transform.localEulerAngles = new Vector3(Mathf.LerpAngle(lastSelectedObj.transform.localEulerAngles.x, 0, Time.deltaTime), Mathf.Lerp(lastSelectedObj.transform.localEulerAngles.y, 0, Time.deltaTime), Mathf.Lerp(lastSelectedObj.transform.localEulerAngles.z, 0, Time.deltaTime));
            if (lastSelectedObj.transform.position == lastObjPos)
            {
                itemMode = enManagerItemMode.inHand;
                lastSelectedObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        else if(itemMode == enManagerItemMode.inspectMode)
        {
            ////////////////copy from isGrabbed mode to finish transformation//////////////////////
            
            if(smallDistanceValueFlag)
            {
                lastSelectedObj.transform.position = new Vector3(Mathf.Lerp(lastSelectedObj.transform.position.x, destinationPosInspect.position.x, Time.deltaTime * 5.0f), Mathf.Lerp(lastSelectedObj.transform.position.y, destinationPosInspect.position.y, Time.deltaTime * 5.0f), Mathf.Lerp(lastSelectedObj.transform.position.z, destinationPosInspect.position.z, Time.deltaTime * 5.0f));
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
            controlInfo.enabled = true;
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
                RotateObj(lastSelectedObj);
            }
            InspectInteractionFind(lastSelectedObj);
        }
        else if(itemMode == enManagerItemMode.clear)
        {
            title.enabled = false;
            press.enabled = false;
            description.enabled = false;
            inProgressBar.enabled = false;
        }
        else if(itemMode == enManagerItemMode.inHand)
        {
            if(!delivereToHand)
            {
                lastSelectedObj.transform.position = new Vector3(Mathf.Lerp(lastSelectedObj.transform.position.x, destinationPosPick.position.x, Time.deltaTime * 5.0f), Mathf.Lerp(lastSelectedObj.transform.position.y, destinationPosPick.position.y, Time.deltaTime * 5.0f), Mathf.Lerp(lastSelectedObj.transform.position.z, destinationPosPick.position.z, Time.deltaTime * 5.0f));
                volume.weight = Mathf.Lerp(volume.weight, 0, Time.deltaTime * 10.0f);
            }       

            if(lastSelectedObj.transform.position == destinationPosPick.position && !delivereToHand)
            {
                lastSelectedObj.transform.parent = handHandleItemAttach.transform;

                title.enabled = false;
                press.enabled = false;
                description.enabled = false;
                inProgressBar.enabled = false;

                volume.weight = 0f;
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
        if(Input.GetKeyUp(KeyCode.Mouse1) && !isGrabbedFirstTime)
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
            itemMode = enManagerItemMode.inHand;
            return;
        }

        if(Input.GetKeyDown(putAwayButton) && isGrabbedFirstTime)
        {
            stopCoroutine = true;
            StartCoroutine(FadeOutTextLoop(title));
            StartCoroutine(FadeOutTextLoop(press));
            StartCoroutine(FadeOutTextLoop(description));
            StartCoroutine(FadeOutTextLoop(controlInfo));

            itemMode = enManagerItemMode.returnToPos;
            return;
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1) && isGrabbedFirstTime)
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
                        bin.text = obj.GetComponent<ItemBase>().ShowInfoDown();
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

        //if (inspectModeDirInteractionFlags[2])
        //{
        //    if (Physics.Raycast(obj.transform.position, front, out hit, rayLength))
        //    {
        //        if (hit.collider.CompareTag("Player"))
        //        {
        //            press.enabled = true;
        //        }
        //    }
        //}

        //if (inspectModeDirInteractionFlags[3])
        //{
        //    if (Physics.Raycast(obj.transform.position, back, out hit, rayLength))
        //    {
        //        if (hit.collider.CompareTag("Player"))
        //        {
        //            press.enabled = true;
        //        }
        //    }
        //}

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

            itemMode = enManagerItemMode.isGrabbed;
            return;
        }

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

    private void RotateObj(GameObject obj)
    {
        float mouseY = Input.GetAxis("Mouse Y") * 150.0f * Time.deltaTime;

        obj.transform.Rotate(mouseY, 0, 0);
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

        if(t_x_mouse > forceValueToDropItem || t_x_mouse < -forceValueToDropItem || t_y_mouse > forceValueToDropItem || t_y_mouse < -forceValueToDropItem)
        {
            Debug.Log("ITEM DROPPED!");
            lastSelectedObj.GetComponent<ItemBase>().itemDrop = true;
            itemMode = enManagerItemMode.dropped;
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

    public  void InProgressShowTextPulseLoop()
    {
        //inProgressBar.enabled = true;
        //inProgressBar.GetComponent<Text>().color = new Vector4(inProgressBar.GetComponent<Text>().color.r, inProgressBar.GetComponent<Text>().color.g, inProgressBar.GetComponent<Text>().color.b, 0);

        //bool increaseValueDirection = true;
        //int transparentValue;

        //while (pulseTextLoopFlag)
        //{
        //    if(inProgressBar.GetComponent<Text>().color.a <= 0)
        //    {
        //        increaseValueDirection = true;
        //    }
        //    else if(inProgressBar.GetComponent<Text>().color.a >= 255)
        //    {
        //        increaseValueDirection = false;
        //    }
                
        //    if(increaseValueDirection)
        //    {
        //        transparentValue = 1;
        //    }
        //    else
        //    {
        //        transparentValue = -1;
        //    }

        //    inProgressBar.GetComponent<Text>().color = new Vector4(inProgressBar.GetComponent<Text>().color.r, inProgressBar.GetComponent<Text>().color.g, inProgressBar.GetComponent<Text>().color.b, inProgressBar.GetComponent<Text>().color.a + transparentValue);
        //}

        //inProgressBar.enabled = false;
    }


}
