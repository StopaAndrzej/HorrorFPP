using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDTable : MonoBehaviour
{

    [SerializeField] private GameObject shelf1;
    [SerializeField] private GameObject shelf2;
    [SerializeField] private GameObject door;

    //shelf1
    private Transform shelf1DefaultPos;
    [SerializeField] private Transform shelf1OpenedPos;
    [SerializeField] private Shelf1CDTable shelf1Script;

    //shelf2
    private Transform shelf2DefaultPos;
    [SerializeField] private Transform shelf2OpenedPos;
    [SerializeField] private Shelf2CDTable shelf2Script;

    //door
    private Transform doorDefaultPos;
    [SerializeField] private Transform doorOpenedPos;
    [SerializeField] private DoorCDTable doorScript;

    private void Start()
    {
        //get current object transform  as default
        shelf1DefaultPos = shelf1.GetComponent<Transform>();
        shelf2DefaultPos = shelf2.GetComponent<Transform>();
        doorDefaultPos = door.GetComponent<Transform>();

        //set fields in other scripts by manager script values
        shelf1Script.closedTransform = shelf1DefaultPos;
        shelf1Script.openedTransform = shelf1OpenedPos;

        shelf2Script.closedTransform = shelf2DefaultPos;
        shelf2Script.openedTransform = shelf2OpenedPos;

        doorScript.closedTransform = doorDefaultPos;
        doorScript.openedTransform = doorOpenedPos;
    }


}
