using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public struct Mission
{
    public Text textObj;
    public string mainMissionTxt;
    public int missionOrder;                //help to sort missions tracked on screen
    public List<string> subMissions;
    public string details;

    public Mission(Text textObj1, string title, string detailTxt, int order)
    {
        textObj = textObj1;
        mainMissionTxt = title;
        missionOrder = order;
        subMissions = new List<string>();
        details = detailTxt;
    }


    public void AddSubMission(string value)
    {
        if(subMissions == null)
        {
            subMissions = new List<string>();
        }

        subMissions.Add(value);
    }

    public void ModifyTitle(string title)
    {
        mainMissionTxt = title;
    }

    public void ModifyDetail(string detailTxt)
    {
        details = detailTxt;
    }

    public void ChangeOrder(int value)
    {
        missionOrder = value;
    }
}

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private Transform TESTPARENT;

    //missions interface
    [SerializeField] private PlayerMove player;
    [SerializeField] private PlayerFootprint footsteps;
    [SerializeField] private Camera cameraPlayer;
    [SerializeField] private InventoryScript inventoryManager;

    [SerializeField] private Canvas dotCanvas;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject menuOnScreenCanvas;
    [SerializeField] private GameObject inventoryCanvas;

    [SerializeField] private Transform parentTextMissions;
    [SerializeField] private Transform parentTextSubmissions;

    [SerializeField] private Text textTemplate;
    [SerializeField] private Text missionOnScreenTemplate;
    [SerializeField] private Text missionSubOnScreenTemplate;
    [SerializeField] private Text trackTxt;
    private int trackCount;
    private int countMissions;
    private int trackCountOnScreen;
    private int subMissionsOffset;
    private int detailShownMissionId;

    [SerializeField] private Text headerMenu;
    [SerializeField] private Text spaceText;

    [SerializeField] private Mission[] missions = new Mission[100];
    [SerializeField] private Text[] missionScreenSlots = new Text[3];
    private Dictionary<string, bool> missionSelected = new Dictionary<string, bool>();

    [SerializeField] private Image selectingBar;

    private bool missionModeFlag = false;
    private bool showDetailsMode = false;
    private bool inventoryModeFlag = false;
    private int missionCursorId;
    /// 

    public List<GameObject> iteamInventory;
    public bool grabInHand = false;

    [SerializeField] private FindInteraction findInteraction;
    [SerializeField] private KeyCode keyboardButton = KeyCode.Tab;
    [SerializeField] private KeyCode keyboardButtonInventory = KeyCode.I;

    private void Start()
    {
        findInteraction.showMissionsMode = false;
        menuCanvas.SetActive(false);
        inventoryCanvas.SetActive(false);
        TESTPARENT.gameObject.SetActive(false);

        trackCount = 0;
        trackCountOnScreen = 0;
        countMissions = 0;
        subMissionsOffset = 0;
        detailShownMissionId = -1;

        trackTxt.text = "PRESS <F> TO TRACK (" + trackCount + "/3)";
        spaceText.text = "PRESS <SPACE> FOR DETAILS";

        Text text = Instantiate(textTemplate, parentTextMissions);
        Mission mission1 = new Mission(text, "CHECK MAILBOX", "TEXT TEXT TEXT TEXT TEXT TEXT TEXT", -1);
        mission1.AddSubMission("-TURN ON PC");
        mission1.AddSubMission("-OPEN MAIL BROWSER");
        mission1.AddSubMission("-SEARCH FOR NEW MESSAGES");

        missions[0] = mission1;
        missionSelected.Add(mission1.mainMissionTxt, false);

        text = Instantiate(textTemplate, parentTextMissions);
        Mission mission2 = new Mission(text, "TAKE A SHOWER", "TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT", -1);
        mission2.AddSubMission("-UNDRESS IN THE BATHROOM");
        mission2.AddSubMission("-GET IN THE SHOWER");
        mission2.AddSubMission("-FIND NEW OUTFIT");

        missions[1] = mission2;
        missionSelected.Add(mission2.mainMissionTxt, false);

        text = Instantiate(textTemplate, parentTextMissions);
        Mission mission3 = new Mission(text, "CLEAN HOME", "TEXT TEXT TEXT TEXT TEXT TEXT TEXT", -1);
        mission3.AddSubMission("-REFRESH IN KITCHEN");
        mission3.AddSubMission("-DO THE LAUNDRY");
        mission3.AddSubMission("-VENTILATE THE APARTMENT");
        mission3.AddSubMission("-FOLD CLOTHES");
        mission3.AddSubMission("V-ACUUM THE CARPET");

        missions[2] = mission3;
        missionSelected.Add(mission3.mainMissionTxt, false);


        for(int i = 0; i<missions.Length; i++)
        {
            if(missions[i].textObj == null)
            {
                break;
            }

            missions[i].textObj.enabled = true;
            missions[i].textObj.color = new Vector4(0, 0, 0, 0);
            countMissions++;
        }

        UpdateCursor();

        foreach (Transform el in menuOnScreenCanvas.transform)
        {
            el.gameObject.GetComponent<Text>().enabled = false;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(keyboardButtonInventory) && !showDetailsMode)
        {
            if(!inventoryModeFlag)
            {
                cameraPlayer.enabled = false;
                player.disablePlayerController = true;
                footsteps.stopFlag = true;

                inventoryCanvas.SetActive(true);
                inventoryManager.ShowItem(0);
            }
            else
            {
                cameraPlayer.enabled = true;
                player.disablePlayerController = false;
                footsteps.stopFlag = false;

                inventoryCanvas.SetActive(false);
            }

            inventoryModeFlag = !inventoryModeFlag;
        }
        else if (Input.GetKeyDown(keyboardButton) && !showDetailsMode)
        {
            if (!missionModeFlag)
            {
                missionModeFlag = true;

                menuCanvas.SetActive(true);
                foreach (Transform el in menuOnScreenCanvas.transform)
                {
                    el.gameObject.GetComponent<Text>().enabled = true;
                }

                TESTPARENT.gameObject.SetActive(false);
                player.disablePlayerController = true;
                footsteps.stopFlag = true;

                missionCursorId = 0;
                selectingBar.GetComponent<RectTransform>().localPosition = new Vector3(-13, 110, 0);

                int offset = 0;
                foreach (Mission el in missions)
                {
                    if(el.textObj==null)
                    {
                        break;
                    }

                    el.textObj.GetComponent<RectTransform>().localPosition = new Vector3(111, 75 - offset * 35, 0);
                    el.textObj.text = el.mainMissionTxt;
                    el.textObj.enabled = true;
                    offset++;
                }

                UpdateCursor();

            }
            else
            {
                missionModeFlag = false;

                TESTPARENT.gameObject.SetActive(true);
                menuCanvas.SetActive(false);
                foreach (Transform el in menuOnScreenCanvas.transform)
                {
                    el.gameObject.GetComponent<Text>().enabled = false;
                }

                player.disablePlayerController = false;
                footsteps.stopFlag = false;
            }
        }

        if (missionModeFlag && !showDetailsMode)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (missionCursorId < countMissions-1)
                {
                    missionCursorId++;
                }
                else
                {
                    missionCursorId = 0;
                }

                UpdateCursor();

            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (missionCursorId > 0)
                {
                    missionCursorId--;
                }
                else
                {
                    missionCursorId = countMissions - 1;
                }

                UpdateCursor();
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                SelectTracingMission1();

            }
            else if(Input.GetKeyDown(KeyCode.Space))
            {
                showDetailsMode = true;

                for(int x=0; x<missions.Length; x++)
                {
                    if(missions[x].textObj == null)
                    {
                        break;
                    }

                    if(x != missionCursorId)
                    {
                        missions[x].textObj.enabled = false;
                    }
                    else
                    {
                        detailShownMissionId = x;
                        headerMenu.text = "DETAILS:";
                        StartCoroutine(SortOnTop(missions[x]));
                        StartCoroutine(ReadMissionDescription(missions[x]));
                    }
                }
            }      
        }
        else if (Input.GetKeyDown(keyboardButton) && showDetailsMode)
        {
            headerMenu.text = "CURRENT MISSIONS:";
            showDetailsMode = false;
            trackTxt.text = "PRESS <F> TO TRACK (" + trackCount + "/3)";
            spaceText.text = "PRESS <SPACE> FOR DETAILS";
            HideSubMissions();

            missionCursorId = 0;
            selectingBar.GetComponent<RectTransform>().localPosition = new Vector3(-13, 110, 0);

            int offset = 0;
            for(int i = 0; i<missions.Length;i++)
            {

                if (missions[i].textObj == null)
                {
                    break;
                }

                missions[i].textObj.GetComponent<RectTransform>().localPosition = new Vector3(111, 75 - offset * 35, 0);
                missions[i].textObj.text = missions[i].mainMissionTxt;
                missions[i].textObj.enabled = true;
                offset++;
            }

            UpdateCursor();
            missionCursorId = 0;
        }
        else if (showDetailsMode)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                SelectTracingMission2();
            }
        }


    }

    void UpdateCursor()
    {
        selectingBar.GetComponent<RectTransform>().localPosition = new Vector3(-13, 110 - 35 * missionCursorId, 0);

        if (trackCount > 0)
            trackTxt.text = "PRESS <F> TO TRACK/UNTRACK (" + trackCount + "/3)";
        else
            trackTxt.text = "PRESS <F> TO TRACK (" + trackCount + "/3)";

        int i = 0;
        foreach (Mission el in missions)
        {
            if(el.textObj == null)
            {
                break;
            }

            if (missionCursorId == i)
            {
                el.textObj.color = new Vector4(0, 0, 0, 1);
            }
            else
            {
                el.textObj.color = new Vector4(1, 1, 1, 1);
            }

            i++;
        }
    }

    private void SelectTracingMission2()
    {
        int j = 0;

        foreach(var el in missionSelected)
        {
            if(j == detailShownMissionId)
            {
                if(el.Value == false && trackCount < 3)
                {
                    missions[detailShownMissionId].ModifyTitle(missions[detailShownMissionId].mainMissionTxt + "<");
                    missions[detailShownMissionId].textObj.text = missions[detailShownMissionId].mainMissionTxt;
                    missions[detailShownMissionId].ChangeOrder(trackCount + 1);

                    //OnScreen
                    Text text = Instantiate(missionOnScreenTemplate, TESTPARENT);
                    missionScreenSlots[trackCountOnScreen] = text;
                    text.text = "> " + missions[detailShownMissionId].mainMissionTxt.Substring(0, missions[detailShownMissionId].mainMissionTxt.Length - 1);
                    text.GetComponent<RectTransform>().position = TESTPARENT.position;
                    text.GetComponent<RectTransform>().localPosition = new Vector3(0, 380 - trackCountOnScreen * 30 - subMissionsOffset * 20, 0);
                    text.enabled = true;
                    trackCountOnScreen++;

                    int subMissionsCount = 0;
                    foreach (string sub in missions[detailShownMissionId].subMissions)
                    {
                        Text text1 = Instantiate(missionSubOnScreenTemplate, text.transform);
                        text1.text = "." + sub.Substring(1, sub.Length - 1);
                        text1.GetComponent<RectTransform>().position = text.transform.position;
                        text1.GetComponent<RectTransform>().localPosition = new Vector3(-150, -35 - 20 * subMissionsCount, 0);
                        text1.enabled = true;
                        subMissionsCount++;
                    }

                    subMissionsOffset += subMissionsCount;
                    trackCount++;
                }
                else
                {
                    missions[detailShownMissionId].ModifyTitle(missions[detailShownMissionId].mainMissionTxt.Substring(0, missions[detailShownMissionId].mainMissionTxt.Length - 1));
                    missions[detailShownMissionId].textObj.text = missions[detailShownMissionId].mainMissionTxt;

                    for (int x = 0; x < missionScreenSlots.Length; x++)
                    {
                        if (missionScreenSlots[x].text != null)
                        {
                            if (missionScreenSlots[x].text.Substring(2, missionScreenSlots[x].text.Length - 2) == missions[detailShownMissionId].textObj.text)
                            {
                                GameObject.Destroy(missionScreenSlots[x].gameObject);
                                missionScreenSlots[x] = null;
                                //reset screenSlots
                                ResetScreenSlots(x);


                                break;
                            }
                        }

                    }
                }

                missionSelected[el.Key] = !el.Value;
                break;
            }
            j++;
        }
    }

    private  void SelectTracingMission1()
    {
        for(int i=0; i<missions.Length; i++)
        {
            if(missions[i].textObj==null)
            {
                break;
            }

            if(missionCursorId == i && trackCount <=3)
            {
                int j = 0;
                foreach(var el in missionSelected)
                {
                    if (j == i)
                    {
                        if (el.Value == false && trackCount < 3)
                        {
                            missions[i].ModifyTitle(missions[i].mainMissionTxt + "<");
                            missions[i].textObj.text = missions[i].mainMissionTxt;
                            missions[i].ChangeOrder(trackCount + 1);

                            //OnScreen
                            Text text = Instantiate(missionOnScreenTemplate, TESTPARENT);
                            missionScreenSlots[trackCountOnScreen] = text;
                            text.text = "> " + missions[i].mainMissionTxt.Substring(0, missions[i].mainMissionTxt.Length - 1);
                            text.GetComponent<RectTransform>().position = TESTPARENT.position;
                            text.GetComponent<RectTransform>().localPosition = new Vector3(0,380 - trackCountOnScreen*30 -subMissionsOffset*20,0);
                            text.enabled = true;
                            trackCountOnScreen++;

                            int subMissionsCount = 0;
                            foreach (string sub in missions[i].subMissions)
                            {
                                Text text1 = Instantiate(missionSubOnScreenTemplate, text.transform);
                                text1.text = "." + sub.Substring(1, sub.Length - 1);
                                text1.GetComponent<RectTransform>().position = text.transform.position;
                                text1.GetComponent<RectTransform>().localPosition = new Vector3(-150, -35 - 20* subMissionsCount, 0);
                                text1.enabled = true;
                                subMissionsCount++;
                            }

                            subMissionsOffset += subMissionsCount;
                            trackCount++;
                        }
                        else
                        {
                            missions[i].ModifyTitle(missions[i].mainMissionTxt.Substring(0, missions[i].mainMissionTxt.Length-1));
                            missions[i].textObj.text = missions[i].mainMissionTxt;

                            for(int x=0; x< missionScreenSlots.Length; x++)
                            {
                                if (missionScreenSlots[x].text != null)
                                {
                                    if(missionScreenSlots[x].text.Substring(2, missionScreenSlots[x].text.Length - 2) == missions[i].textObj.text)
                                    {
                                        GameObject.Destroy(missionScreenSlots[x].gameObject);
                                        missionScreenSlots[x] = null;
                                        //reset screenSlots
                                        ResetScreenSlots(x);


                                        break;
                                    }
                                }

                            }

                        }

                        missionSelected[el.Key] = !el.Value;
                        break;
                    }

                    j++;
                }
            }
        }

        UpdateCursor();
    }

    private void ResetScreenSlots(int deletedSlot)
    {
        trackCountOnScreen = 0;
        subMissionsOffset = 0;
        trackCount = 0;

        for (int i = 0; i<missionScreenSlots.Length; i++)
        {

           if(missionScreenSlots[i] != null)
           {
                missionScreenSlots[i].GetComponent<RectTransform>().localPosition = new Vector3(0, 380 - trackCountOnScreen * 30 - subMissionsOffset * 20, 0);
                //count children offset
                int subMissionsCount = 0;
                foreach (Transform sub in missionScreenSlots[i].transform)
                {
                    sub.GetComponent<RectTransform>().localPosition = new Vector3(-150, -35 - 20 * subMissionsCount, 0);
                    subMissionsCount++;
                }

                subMissionsOffset += subMissionsCount;
                trackCountOnScreen++;
                trackCount++;
            }
            else
            {
                if(i+1<missionScreenSlots.Length)
                {
                    if(missionScreenSlots[i+1] != null)
                    {
                        missionScreenSlots[i] = missionScreenSlots[i + 1];
                        missionScreenSlots[i + 1] = null;

                        missionScreenSlots[i].GetComponent<RectTransform>().localPosition = new Vector3(0, 380 - trackCountOnScreen * 30 - subMissionsOffset * 20, 0);
                        //count children offset
                        int subMissionsCount = 0;
                        foreach (Transform sub in missionScreenSlots[i].transform)
                        {
                            sub.GetComponent<RectTransform>().localPosition = new Vector3(-150, -35 - 20 * subMissionsCount, 0);
                            subMissionsCount++;
                        }

                        subMissionsOffset += subMissionsCount;
                        trackCountOnScreen++;
                        trackCount++;
                    }
                }
            }
        }
    }


    IEnumerator SortOnTop(Mission mission)
    {
        float destinationPosY = mission.textObj.GetComponent<RectTransform>().localPosition.y + 35 * missionCursorId;
        float barDestinationPosY = selectingBar.GetComponent<RectTransform>().localPosition.y + 35 * missionCursorId;

        float acceptableDistance = mission.textObj.GetComponent<RectTransform>().localPosition.y - destinationPosY;

        do
        {
            mission.textObj.GetComponent<RectTransform>().localPosition = new Vector3(mission.textObj.GetComponent<RectTransform>().localPosition.x, Mathf.Lerp(mission.textObj.GetComponent<RectTransform>().localPosition.y, destinationPosY, 5.0f*Time.deltaTime), mission.textObj.GetComponent<RectTransform>().localPosition.z);
            selectingBar.GetComponent<RectTransform>().localPosition = new Vector3(-13, Mathf.Lerp(selectingBar.GetComponent<RectTransform>().localPosition.y, barDestinationPosY, 5.0f * Time.deltaTime), 0);
            yield return null;

        } while (acceptableDistance/5.0f > mission.textObj.GetComponent<RectTransform>().localPosition.y - destinationPosY);

        mission.textObj.GetComponent<RectTransform>().localPosition = new Vector3(mission.textObj.GetComponent<RectTransform>().localPosition.x, destinationPosY, mission.textObj.GetComponent<RectTransform>().localPosition.z);
        selectingBar.GetComponent<RectTransform>().localPosition = new Vector3(-13, barDestinationPosY, 0);

        ShowSubMissions(mission);
    }

    void ShowSubMissions(Mission mission)
    {
        int offset = 1;

        foreach (string sub in mission.subMissions)
        {
            Text text = Instantiate(textTemplate, parentTextSubmissions);
            text.text = sub;
            text.GetComponent<RectTransform>().localPosition = new Vector3(111, 60 - offset * 25, 0);
            text.enabled = true;
            text.fontSize = 20;
            offset++;
        }
    }

    void HideSubMissions()
    {
        foreach(Transform child in parentTextSubmissions.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    IEnumerator ReadMissionDescription(Mission mission)
    {
        for(int i=0; i<mission.details.Length; i++)
        {
            spaceText.text = mission.details.Substring(0, i);
            yield return new WaitForSeconds(0.03f);
        }
    }

}