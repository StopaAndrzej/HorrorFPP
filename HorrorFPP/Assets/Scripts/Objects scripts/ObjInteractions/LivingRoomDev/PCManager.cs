using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCManager : InteractableObjectBase
{
    [SerializeField] private PCScreenManager screenManager;

    private KeyCode interactionKey = KeyCode.F;
    private KeyCode mouseButton = KeyCode.Mouse0;

    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;

    [SerializeField] private MeshRenderer pcBox;
    private bool isOn = false;

    public bool animationInProgress = false;

    // Start is called before the first frame update
    void Start()
    {
        pcBox.material = offMaterial;
        interactText = "TURN ON";
    }

    public override void Interact()
    {
        if((Input.GetKeyDown(interactionKey) || Input.GetKeyDown(mouseButton)) && !animationInProgress)
        {
            if(isOn)
            {
                pcBox.material = offMaterial;
                interactText = "TURN ON";
                
            }
            else
            {
                StartCoroutine(FlashingDVDPanel(5));
                pcBox.material = onMaterial;
                interactText = "TURN OFF";
            }

            isOn = !isOn;
        }
    }

    public IEnumerator FlashingDVDPanel(int value)
    {
        animationInProgress = true;

        int flashingTimer = value;      //should be odd value if lcd is On before starts couroutine/vice versa

        Material fstMaterial = onMaterial;
        Material scdMaterial = offMaterial;

        if (value % 2 == 0)
        {
            fstMaterial = offMaterial;
            scdMaterial = onMaterial;
        }

        while (true)
        {
            if (flashingTimer % 2 == 1)
            {
                pcBox.material = fstMaterial;
            }
            else
            {
                pcBox.material = scdMaterial;
            }


            if (flashingTimer <= 1)
                break;

            flashingTimer--;
            yield return new WaitForSeconds(0.2f);
        }

        screenManager.SetScreenBoot();
        animationInProgress = false;
    }
}
