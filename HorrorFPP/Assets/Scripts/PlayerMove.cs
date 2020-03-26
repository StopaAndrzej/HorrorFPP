﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;
    private float movementSpeed;

    [SerializeField] private float walkSpeed, runSpeed, crouchSpeed;
    [SerializeField] private float runBuildUpSpeed, crouchBuildSpeed;

    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;

    private CharacterController charController;

    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private float jumpMultiplier;

    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode runKey;
    [SerializeField] private KeyCode LeftLeanKey, RightLeanKey;
    [SerializeField] private KeyCode crouchKey;

    private bool isJumping = false;
 
    [SerializeField] private Transform camera;

    //leaning params
    [SerializeField] private float leanAngle = 35.0f;
    [SerializeField] private float leanSpeed = 5.0f;
    [SerializeField] private float leanBackSpeed = 6.0f;

    //crouching params
    private float standingHight;
    private bool isCrouching = false;
    [SerializeField] private float crouchHeight = 0.0f;
    [SerializeField] private float crouchPosSpeed;
    [SerializeField] private float crouchAboveRayLength;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        standingHight = camera.localPosition.y;
    }

    private void Start()
    {
        //camera = GetComponent<Transform>();
        crouchAboveRayLength = 1.0f - crouchHeight;
    }

    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        float horizontalInput = Input.GetAxis(horizontalInputName) * Time.deltaTime;
        float verticalInput = Input.GetAxis(verticalInputName) * Time.deltaTime;

        Vector3 forwardMovement = transform.forward * verticalInput;
        Vector3 rightMovement = transform.right * horizontalInput;

        charController.SimpleMove(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * movementSpeed);

        if ((verticalInput != 0 || horizontalInput != 0) && OnSlope())
            charController.Move(Vector3.down * charController.height / 2 * slopeForce * Time.deltaTime);

        SetMovementSpeed();
        JumpInput();
        CheckLeaning();
        CrouchInput();
    }

    private void SetMovementSpeed()
    {
        if(isCrouching)
        {
            movementSpeed = Mathf.Lerp(movementSpeed, crouchSpeed, Time.deltaTime * crouchBuildSpeed);
        }
        else if(Input.GetKey(runKey))
        {
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * runBuildUpSpeed);
        }
        else
        {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);
        }
    }

    private bool OnSlope()
    {
        if (isJumping)
            return false;

        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit, charController.height/2 * slopeForceRayLength))
        {
            if (hit.normal != Vector3.up)
                return true;
        }

        return false;
    }

    private bool CheckIfCanStand()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.up, out hit, crouchAboveRayLength))
        {
            return false;
        }
        return true;
    }

    private void JumpInput()
    {
        if(Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }

    private IEnumerator JumpEvent()
    {
        charController.slopeLimit = 90.0f;
        float timeInAir = 0.0f;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
        } while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);
        charController.slopeLimit = 45.0f;
        isJumping = false;
    }

    private void CrouchInput()
    {
        if(Input.GetKeyDown(crouchKey))
        {
            if(isCrouching)
            {
                if (CheckIfCanStand())
                {
                    isCrouching = false;
                    StartCoroutine(LerpFromTo(camera.localPosition, new Vector3(camera.localPosition.x, standingHight, camera.localPosition.z), crouchPosSpeed));
                } 
            }
            else
            {
               isCrouching = true;
               StartCoroutine(LerpFromTo(camera.localPosition, new Vector3(camera.localPosition.x, crouchHeight, camera.localPosition.z), crouchPosSpeed));
            }
        }
    }

    private IEnumerator LerpFromTo(Vector3 pos1, Vector3 pos2, float duration)
    {
        movementSpeed = 0.0f;

        if (!isCrouching)
        {
            camera.position -= Vector3.up * 0.5f;
            charController.height = 2.0f;

        }

        duration = 1 / duration;
       for(float t =0.0f; t<duration; t+= Time.deltaTime)
        {
            camera.localPosition = Vector3.Lerp(pos1, pos2, t / duration);
            yield return null;
        }

       if(isCrouching)
        {
            charController.height = 1.0f;
        }


       
    }

    private void CheckLeaning()
    {
        if(Input.GetKey(LeftLeanKey))
        {
            Lean(-1.0f);
        }
        else if(Input.GetKey(RightLeanKey))
        {
            Lean(1.0f);
        }
        else
        {
            Lean(0.0f);
        }
    }

    private void Lean(float value)
    {
        float currAngle = transform.rotation.eulerAngles.z;
        float targetAngle;

        if (value == -1.0f)
        {
            targetAngle = leanAngle;
            if (currAngle > 180.0)
            {
                currAngle = 360.0f - currAngle;
            }
        }  

        else if (value == 1.0f)
        {
            targetAngle = leanAngle - 360.0f;
            if (currAngle > 180.0)
            {
                targetAngle = 360.0f - leanAngle;
            }
        }  

        else
        {
            targetAngle = 0.0f;
            if (currAngle > 180.0)
            {
                targetAngle = 360.0f;
            }
        }
            

       

        float angle = Mathf.Lerp(currAngle, targetAngle, leanSpeed * Time.deltaTime);
        Quaternion rotAngle = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, angle);
        transform.rotation = rotAngle;
    }

}
