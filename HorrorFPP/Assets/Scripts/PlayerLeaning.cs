using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeaning : MonoBehaviour
{
    public Transform pivot;

    [SerializeField] private float speed = 100f;
    [SerializeField] private float maxAngle = 20f;

    private float curAngle = 0f;

    private void Awake()
    {
        if (pivot == null && transform.parent != null)
            pivot = transform.parent;
    }

    private void Update()
    {
        //lean left
        if (Input.GetKey(KeyCode.Q))
        {
            curAngle = Mathf.MoveTowardsAngle(curAngle, maxAngle, speed * Time.deltaTime);
            pivot.transform.localRotation = Quaternion.AngleAxis(curAngle, Vector3.forward);
        }

        //lean right
        else if (Input.GetKey(KeyCode.E))
        {
            curAngle = Mathf.MoveTowardsAngle(curAngle, -maxAngle, speed * Time.deltaTime);
            pivot.transform.localRotation = Quaternion.AngleAxis(curAngle, Vector3.forward);
        }

        ////reset
        //else
        //{
        //    curAngle = Mathf.MoveTowardsAngle(curAngle, 0.0f, speed * Time.deltaTime);
        //}

        
    }
}
