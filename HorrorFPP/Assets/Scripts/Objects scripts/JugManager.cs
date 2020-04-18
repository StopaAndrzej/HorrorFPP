using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugManager : MonoBehaviour
{
    [SerializeField] private GameObject waterInJug;
    [SerializeField] private GameObject drinkInJug;
    private float topDownPos = -0.37f;
    private float topUpPos = 0.206f;

    public bool readyToPour = false;

    // Start is called before the first frame update
    void Start()
    {
        waterInJug.SetActive(false);
        drinkInJug.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
