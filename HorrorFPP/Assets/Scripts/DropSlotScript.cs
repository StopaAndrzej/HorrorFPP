using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSlotScript : MonoBehaviour
{
    public GameObject arrow;
    private BoxCollider collider;
    public Transform dropPos;

    [SerializeField] private GameObject player;

    [SerializeField] private float minScale = 0.57f;
    [SerializeField] private float maxScale = 1.2f;

    private bool activate;
    public bool slotEmpty = true;
    public bool slotDenied = false;

    private void Start()
    {
        foreach(Transform el in transform)
        {
            if(el.GetComponent<BoxCollider>())
            {
                collider = el.GetComponent<BoxCollider>();
            }

            else if(el.GetComponent<Animator>())
            {
                arrow = el.gameObject;
            }

            else
            {
                dropPos = el.transform;
            }
        }

        arrow.SetActive(false);
        collider.enabled = false;
        activate = false;
    }

    public void ActivateDrop()
    {

        arrow.SetActive(true);
        collider.enabled = true;
        activate = true;
    }

    public  void DeactivateDrop()
    {
        arrow.SetActive(false);
        collider.enabled = false;
        activate = false;
    }

    public void HideArrow()
    {
        arrow.SetActive(false);
    }

    public void ShowArrow()
    {
        arrow.SetActive(true);
    }

}
