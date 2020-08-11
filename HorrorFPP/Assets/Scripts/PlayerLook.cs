using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;

    [SerializeField] private Transform playerBody;
    [SerializeField] private Vector2 rotationRange = new Vector2(-70.0f,70.0f);

    [SerializeField] private Vector2 rotationRangePeepHoleX = new Vector2(-5.0f, 5.0f);
    [SerializeField] private Vector2 rotationRangePeepHoleY = new Vector2(-5.0f, 5.0f);

    private float xAxisClamp;
    private float yAxisClamp;

    private Vector3 targetAngles;
    private Vector3 followAngles;
    private Vector3 followVelocity;

    public float dampingTime = 0.2f;
    public bool disableCamera=false;

    private void Awake()
    {
        LockCursor();
        xAxisClamp = 0.0f;
        yAxisClamp = 0.0f;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        //disable player movement if inspection mode is active
        if (!playerBody.GetComponent<PlayerMove>().inspectMode && !disableCamera)
        {
            CameraRotation();
        }    

    }

    private void CameraRotation()
    {
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        xAxisClamp += mouseY;
        yAxisClamp += mouseX;
        if(xAxisClamp > rotationRange.y)
        {
            xAxisClamp = rotationRange.y;
            mouseY = 0;
            ClampAxisRotationToValue(360.0f - rotationRange.y);
        }
        else if(xAxisClamp< rotationRange.x)
        {
            xAxisClamp = rotationRange.x;
            mouseY = 0;
            ClampAxisRotationToValue(-rotationRange.x);
        }

        targetAngles.x = mouseX;
        targetAngles.y = mouseY;
        followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followAngles, dampingTime);

        transform.Rotate(Vector3.left * (followAngles.y + playerBody.gameObject.GetComponent<PlayerMove>().xTilt * 0.25f));
        playerBody.Rotate(Vector3.up * followAngles.x);
        //transform.Rotate(Vector3.forward * playerBody.gameObject.GetComponent<PlayerMove>().zTilt);
    }

    private void ClampAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }

}
