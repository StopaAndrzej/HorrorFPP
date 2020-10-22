using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRHand : MonoBehaviour
{
    [SerializeField] private Animator animatorHand;
    [SerializeField] private PickUpManager pickUp;

    public GameObject button;
    public Text buttonTxt;

    private void Start()
    {
        animatorHand = GetComponent<Animator>();
    }

    public void HideObjAnim()
    {
        animatorHand.Play("PlayersHand");
    }

    public void HideInvAnim()
    {
        pickUp.inventory.AddToInventory(pickUp.lastSelectedObj);
        pickUp.lastSelectedObj.transform.parent = pickUp.invParent;
        pickUp.lastSelectedObj.transform.position = pickUp.invParent.position;
        pickUp.ButtonOff(button, buttonTxt);
        pickUp.itemMode = PickUpManager.enManagerItemMode.clear;
    }
}
