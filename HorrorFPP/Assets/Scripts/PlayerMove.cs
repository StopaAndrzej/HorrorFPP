using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;
    private float movementSpeed;

    [SerializeField] private float walkSpeed, runSpeed;
    [SerializeField] private float runBuildUpSpeed;

    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;

    private CharacterController charController;

    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private float jumpMultiplier;

    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode runKey;
    [SerializeField] private KeyCode LeftLeanKey, RightLeanKey;

    private bool isJumping = false;

    [SerializeField] private Transform camera;

    //leaning params
    [SerializeField] private float leanAngle = 35.0f;
    [SerializeField] private float leanSpeed = 5.0f;
    [SerializeField] private float leanBackSpeed = 6.0f;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        camera = GetComponent<Transform>();
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
    }

    private void SetMovementSpeed()
    {
        if(Input.GetKey(runKey))
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
        float currAngle = camera.rotation.eulerAngles.z;
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
        Quaternion rotAngle = Quaternion.Euler(camera.rotation.eulerAngles.x, camera.rotation.eulerAngles.y, angle);
        camera.rotation = rotAngle;
    }

}
