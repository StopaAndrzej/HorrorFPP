using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapManager : MonoBehaviour
{
    public List<GameObject> dishesInTapList;
    [SerializeField] private DirtyDishesTapManager dirtyDishesManager;
    [SerializeField] private PickUpManager pickUpManager;

    private static int dishesSlotsCount = 3;
    [SerializeField] Transform[] dirtyDishesSlots = new Transform[dishesSlotsCount-1];
    public int i;          //default number of dirty dishes on  tap

    private void Start()
    {
        i = 0;
        foreach (GameObject el in dishesInTapList)
        {
            if (i < dishesSlotsCount)
            {
                el.transform.position = dirtyDishesSlots[i].position;
                el.transform.rotation = dirtyDishesSlots[i].rotation;
                el.transform.parent = dirtyDishesSlots[i].transform;
                i++;
            }
        }

    }

    public void AddDishToDirtyDishesList(GameObject dish)
    {
        if(i< dishesSlotsCount)
        {
            if (dish.GetComponent<ItemManager>())
            {
                dish.GetComponent<BoxCollider>().enabled = false;
                dishesInTapList.Add(dish);
                //pickUpManager.itemMode = PickUpManager.enManagerItemMode.returnToPos;
                //playerMove.inspectMode = true;
                i++;
            }
            else
                Debug.Log("Added Obj to DirtyDishesListInTap has no ItemManager script! Cant be added!");
        }
        else
        {
            dirtyDishesManager.interactText = "FULL_TAP";
            dirtyDishesManager.resetText = true;
        }
        
    }

    public void RemoveLastDishFromDirtyDishesList(GameObject dish)
    {
        dishesInTapList.Remove(dish);

        i--;
    }


}
