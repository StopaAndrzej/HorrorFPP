using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootprint : MonoBehaviour
{
    public bool dirty = true;
    public int howManyFootprints;

    [SerializeField] private GameObject footPrint;
    [SerializeField] private Transform parent;
    private float timer;

    private bool leftFootprint = false;

    [SerializeField] private float footOffeset;
    private float opacityValue = 1;
    private float opacityStepValue;
    private int countPair;
    private int pairId ;

    void SetFootPrints(int howManyFootprints1)
    {
        howManyFootprints = howManyFootprints1;
        opacityValue = 1;
        timer = 0;
        dirty = true;
    }


    void FixedUpdate()
    {
        if(dirty)
        {
            if(opacityValue==1)
            {
                countPair = 0;
                pairId = 1;
                opacityStepValue = CalculateOpacityStepValue(howManyFootprints);
            }
            LeaveFootPrint(ref howManyFootprints);
        }
    }

    void LeaveFootPrint(ref int counter)
    {
        if(counter<=0)
        {
            dirty = false;
            return;
        }

        float verticalMove = Input.GetAxisRaw("Horizontal");
        float horizontalMove = Input.GetAxisRaw("Vertical");

        if (verticalMove != 0 || horizontalMove != 0)
        {
            if (timer > 0.1f)
            {
                timer = 0;
                countPair++;
                GameObject newFootPring = Instantiate(footPrint);

                opacityValue = 1 - opacityStepValue * pairId;
                newFootPring.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, opacityValue);

                if (leftFootprint)
                {
                    newFootPring.transform.position = new Vector3(this.transform.position.x + footOffeset, 1.42f, this.transform.position.z);
                    newFootPring.transform.Rotate(0, 0, this.transform.eulerAngles.y);
                    newFootPring.transform.localScale = new Vector3(-newFootPring.transform.localScale.x, newFootPring.transform.localScale.y, newFootPring.transform.localScale.z);             
                } 
                else
                {
                    newFootPring.transform.position = new Vector3(this.transform.position.x - footOffeset, 1.42f, this.transform.position.z);
                    newFootPring.transform.Rotate(0, 0, this.transform.eulerAngles.y);
                }

                if(countPair>=2)
                {
                    countPair = 0;
                    pairId++;
                }

                counter--;
                leftFootprint = !leftFootprint;
            }

            timer += Time.deltaTime;
        }
    }

    float CalculateOpacityStepValue(int howManyFootprints)
    {
        float value = howManyFootprints / 2;
        return 1 / value;
    }
}
