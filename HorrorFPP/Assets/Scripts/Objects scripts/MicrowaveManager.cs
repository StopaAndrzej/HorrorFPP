using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrowaveManager : MonoBehaviour
{

    public bool doorOpen = false;
    public bool stwichOn = false;
    public bool itemInside = false;

    [SerializeField] private GameObject smokeParticle;
    [SerializeField] private  MicrowaveSwitch switchMicrowave;
    [SerializeField] private MicrowaveDoorManager doorManager;
    public GameObject putObj;

    //textures
    [SerializeField] public MeshRenderer door;
    [SerializeField] public MeshRenderer backDoor;
    [SerializeField] public MeshRenderer inside;
    [SerializeField] public MeshRenderer plate;
    [SerializeField] public MeshRenderer console;

    [SerializeField] public Material doorNoActive;
    [SerializeField] public Material backDoorNoActive;
    [SerializeField] public Material insideNoActive;
    [SerializeField] public Material plateNoActive;
    [SerializeField] public Material consoleNoActive;

    [SerializeField] public Material doorActive;
    [SerializeField] public Material backDoorActive;
    [SerializeField] public Material insideActive;
    [SerializeField] public Material plateActive;
    [SerializeField] public Material consoleActive;
                    
    [SerializeField] public Material doorNoActiveBlood;
    [SerializeField] public Material backDoorNoActiveBlood;
    [SerializeField] public Material insideNoActiveBlood;
    [SerializeField] public Material plateNoActiveBlood;
    [SerializeField] public Material consoleNoActiveBlood;
                    
    [SerializeField] public Material doorActiveBlood;
    [SerializeField] public Material backDoorActiveBlood;
    [SerializeField] public Material insideActiveBlood;
    [SerializeField] public Material plateActiveBlood;
    [SerializeField] public Material consoleActiveBlood;

    [SerializeField] private Light microLight;
    [SerializeField] private Timer clock;

    private void Start()
    {
        door.material = doorNoActive;
        backDoor.material = backDoorNoActive;
        inside.material = insideNoActive;
        plate.material = plateNoActive;
        console.material = consoleNoActive;

        microLight.enabled = false;
    }

    public void Cook()
    {
        if(itemInside)
        {
            door.material = doorActive;
            backDoor.material = backDoorActive;
            inside.material = insideActive;
            plate.material = plateActive;
            microLight.enabled = true;
            clock.isPlaying = true;
        }
    }


    public void TurnOnMicrowave()
    {
        console.material = consoleActive;
        clock.timer = 45.0f;
        switchMicrowave.TurnOn();
    }

    public void MealReady()
    {
        door.material = doorNoActive;
        backDoor.material = backDoorNoActive;
        inside.material = insideNoActive;
        plate.material = plateNoActive;
        console.material = consoleNoActive;

        microLight.enabled = false;
        doorManager.isCooking = false;

        if (putObj.GetComponent<FoodScript>())
        {
            putObj.GetComponent<FoodScript>().ChangeState();

            if(putObj.GetComponent<FoodScript>().itemMode == FoodScript.enFoodCondition.spoiled)
            {
                smokeParticle.GetComponent<ParticleSystem>().loop = true;
                smokeParticle.SetActive(true);
            }
        }
    }


}
