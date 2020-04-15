using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenManager : MonoBehaviour
{
    public bool doorOpen = false;
    public bool stwichOn = false;
    public bool itemInside = false;

    [SerializeField] private OvenDrop ovenDrop;

    [SerializeField] public MeshRenderer door;
    [SerializeField] public MeshRenderer plate;

    //textures
    [SerializeField] public Material doorNoActive;
    [SerializeField] public Material doorActive;
    [SerializeField] public Material doorNoActiveBlood;
    [SerializeField] public Material doorActiveBlood;

    //textures
    [SerializeField] public Material inside;
    [SerializeField] public Material insideBlood;

    private void Start()
    {
        door.material = doorNoActive;
        plate.material = inside;
    }

    public void Cook()
    {
        if (itemInside)
        {
            if (ovenDrop.collider.GetComponent<FoodInspect>())
            {
                ovenDrop.collider.GetComponent<FoodInspect>().foodCondition += 1;
            }
        }
    }
}
