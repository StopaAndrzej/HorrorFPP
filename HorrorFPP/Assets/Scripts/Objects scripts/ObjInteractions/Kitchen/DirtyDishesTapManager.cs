using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtyDishesTapManager : InteractableObjectBase
{
    [SerializeField] private TapManager tapManager;
    [SerializeField] private PickUpManager pickUpManager;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    public bool resetText = false;
    private int x = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (tapManager != null)
        {
            if(tapManager.dishesInTapList.Count == 0)
            {
                interactText = "TAP";
            }
            else
            {
                interactText = "DIRTY_DISHES";
            }
        }
    }


    public override void Interact()
    {
        if (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton))
        {
            if(tapManager.dishesInTapList.Count != 0 && pickUpManager.itemMode == PickUpManager.enManagerItemMode.clear)
            {
                //pick up dirty dish
                x = 0;
                foreach(GameObject el in tapManager.dishesInTapList)
                {
                    if(x==tapManager.dishesInTapList.Count-1)
                    {
                        tapManager.RemoveLastDishFromDirtyDishesList(el);
                        pickUpManager.PickUp(el, 1.5f, 0, true);

                        if(x==0)
                        {
                            interactText = "TAP";
                        }

                        break;
                    }
                    x++;
                }

            }
            else if(pickUpManager.itemMode == PickUpManager.enManagerItemMode.inHand && pickUpManager.lastSelectedObj.GetComponent<DishManager>())
            {
                //add obj to dirty dishes list
                tapManager.AddDishToDirtyDishesList(pickUpManager.lastSelectedObj);
            }
            
        }
    }

    public override void DeInteract()
    {
        if (resetText)
        {
            if (tapManager.dishesInTapList.Count == 0)
            {
                interactText = "TAP";
            }
            else
            {
                interactText = "DIRTY_DISHES";
            }

            resetText = false;
        }
    }

}
