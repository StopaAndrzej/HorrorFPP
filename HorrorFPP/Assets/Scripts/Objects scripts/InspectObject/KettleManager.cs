using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KettleManager : InteractableObjectBase
{
    public bool isOnStand = true;

    public bool readyToFillCup = false;
    public bool readyToFillJug = false;

    public bool boiledWater = false;

    public GameObject kettleWaterLevel;

    //local values -reach levels (tested in scene editor)
    private float topDownPos = -1.71f;
    private float topUpPos = -0.32f;

    [SerializeField] private KettleDoor door;
    [SerializeField] private TapManager tapManager;

    //button
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject buttonOnStandParent;

    private void Start()
    {
        kettleWaterLevel.SetActive(false);
    }

    private void Update()
    {
        if (tapManager.kettleInTap && tapManager.switchActive && door.isOpen)
        {
            kettleWaterLevel.SetActive(true);
            kettleWaterLevel.transform.localPosition = new Vector3(kettleWaterLevel.transform.localPosition.x, Mathf.Lerp(kettleWaterLevel.transform.localPosition.y, topUpPos, Time.deltaTime*0.2f), kettleWaterLevel.transform.localPosition.z);

            //check kettle fill -volume
            if (kettleWaterLevel.transform.localPosition.y > -1.1f)
            {
                readyToFillCup = true;
                if (kettleWaterLevel.transform.localPosition.y > -0.5f)
                    readyToFillJug = true;
                else
                    readyToFillJug = false;

            }
            else
                readyToFillCup = false;
        }

        //button
        if(isOnStand)
        {
            button.GetComponent<Transform>().parent = this.transform;
        }
        else
        {
            button.GetComponent<Transform>().parent = buttonOnStandParent.transform;
        }
    }

}
