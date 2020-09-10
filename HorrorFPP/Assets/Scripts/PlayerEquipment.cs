using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private Canvas missionCanvas;
    public List<GameObject> iteamInventory;
    public bool grabInHand = false;

    [SerializeField] private FindInteraction findInteraction;
    [SerializeField] private KeyCode keyboardButton = KeyCode.Tab;

    private void Start()
    {
        findInteraction.showMissionsMode = false;
        missionCanvas.enabled = false;
    }

    private void Update()
    {
        if(Input.GetKey(keyboardButton))
        {
            findInteraction.showMissionsMode = true;
            missionCanvas.enabled = true;
        }
        else if(Input.GetKeyUp(keyboardButton))
        {
            findInteraction.showMissionsMode = false;
            missionCanvas.enabled = false;
        }
    }



}
