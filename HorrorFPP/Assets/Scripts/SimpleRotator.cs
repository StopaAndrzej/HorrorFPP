using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    public Vector2 rotationRange = new Vector2(70.0f, 70.0f);
    public float rotationSpeed = 10f;
    public float dampingTime = 0.2f;
    public bool relative = true;

    private Vector3 targetAngles;
    private Vector3 followAngles;
    private Vector3 followVelocity;
    private Quaternion orginalRotation;

    void Start()
    {
        orginalRotation = transform.localRotation;
    }

    void Update()
    {
        float horizontalInput;
        float verticalInput;

        if(relative)
        {
            horizontalInput = Input.GetAxis("Mouse X");
            verticalInput = Input.GetAxis("Mouse Y");


            if(targetAngles.y > 180.0f)
            {
                targetAngles.y -= 360.0f;
                followAngles.y -= 360.0f;
            }
            else if(targetAngles.y < -180.0f)
            {
                targetAngles.y += 360.0f;
                followAngles.y += 360.0f;
            }

            if (targetAngles.x > 180.0f)
            {
                targetAngles.x -= 360.0f;
                followAngles.x -= 360.0f;
            }
            else if (targetAngles.x < -180.0f)
            {
                targetAngles.x += 360.0f;
                followAngles.x += 360.0f;
            }

            targetAngles.y += horizontalInput * rotationSpeed;
            targetAngles.x += verticalInput * rotationSpeed;
            targetAngles.y = Mathf.Clamp(targetAngles.y, -0.5f * rotationRange.y, 0.5f * rotationRange.y);
            targetAngles.x = Mathf.Clamp(targetAngles.x, -0.5f * rotationRange.x, 0.5f * rotationRange.x);

        }
        else
        {
            horizontalInput = Input.mousePosition.x;
            verticalInput = Input.mousePosition.y;

            targetAngles.y = Mathf.Lerp(rotationRange.y * -0.5f, rotationRange.y * 0.5f, horizontalInput / Screen.width);
            targetAngles.x = Mathf.Lerp(rotationRange.x * -0.5f, rotationRange.x * 0.5f, verticalInput / Screen.height);
        }

        followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, dampingTime);

        transform.localRotation = orginalRotation * Quaternion.Euler(-followAngles.x, followAngles.y, 0);

    }
}
