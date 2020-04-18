using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class TeaInspect : MonoBehaviour
{
    [SerializeField] private PlayerMove playerComponent;
    [SerializeField] private JugDoor jugDoor;

    [SerializeField] private Text objectName;
    [SerializeField] private Text press;
    [SerializeField] private Text describtion;
    [SerializeField] private Text inProgressBar;

    public string pressText1;
    public string pressText2;

    public string objectNameTitle;
    public string description;
    public string inProgressInfo;

    private float pressPos1, pressPos2;
    private bool changeToUpPos = false;

    private float delay;
    public float delayNormal = 0.1f;
    public float delayPause = 0.5f;
    public string fullText;
    private string currentText = "";

    [SerializeField] private KeyCode interactionKey;

    [SerializeField] private float rayLength = 10.0f;
    [SerializeField] private LayerMask layerMaskInteract;

    private PickUp pickUpScript;

    private bool inspectModeFlag = false;
    private bool descriptionShowed = false;

    public List<GameObject> boxes;

    private void Awake()
    {
        objectName.text = "TEA PACK";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
