using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private MicrowaveManager manager;

    public Text timerTxt;
    public bool isPlaying= false;
    public float timer;

    private void Start()
    {
        this.GetComponent<Canvas>().enabled = false;
    }

    private void Update()
    {
        if(isPlaying)
        {
            this.GetComponent<Canvas>().enabled = true;
            if (timer <= 0)
            {
                isPlaying = false;
                this.GetComponent<Canvas>().enabled = false;
                manager.MealReady();
            }

            timer -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timer / 60F);
            int seconds = Mathf.FloorToInt(timer % 60F);

            timerTxt.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }

}
