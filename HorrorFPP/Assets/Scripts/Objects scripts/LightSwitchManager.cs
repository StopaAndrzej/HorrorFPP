using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitchManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> switches;
    [SerializeField] private GameObject LeftButton;
    [SerializeField] private GameObject RightButton;

    private void Start()
    {
        RightButton.SetActive(false);
        LeftButton.SetActive(false);
    }

    public void CheckButtons()
    {
        foreach(GameObject el in switches)
        {
            if(!el.GetComponent<LightSwitch>().switchRightButtonFlag)
            {
                RightButton.SetActive(false);
                continue;
            }

            RightButton.SetActive(true);
            break;
        }

        foreach (GameObject el in switches)
        {
            if (!el.GetComponent<LightSwitch>().switchLeftButtonFlag)
            {
                LeftButton.SetActive(false);
                continue;
            }

            LeftButton.SetActive(true);
            break;
        }
    }

}
