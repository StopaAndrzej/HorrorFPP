using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;

    [SerializeField] private float runSpeed = 200.0f;
    [SerializeField] private float walkSpeed = 150.0f;
    [SerializeField] private float crouchSpeed = 100.0f;
    [SerializeField] private float actualSpeed;

    [SerializeField] private float maxLeanAngle = 20f;
    [SerializeField] private float speedLean = 100f;
    private float curAngle = 0f;

    private CharacterController characterController;
    [SerializeField] private GameObject camera;

    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private AnimationCurve crouchFallOff;
    [SerializeField] private AnimationCurve runIncreaseSpeed;
    [SerializeField] private AnimationCurve leanOffset;

    [SerializeField] private float jumpMultiplier;

    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode crouchKey;
    [SerializeField] private KeyCode runKey;
    [SerializeField] private KeyCode LeftLeanKey;
    [SerializeField] private KeyCode RightLeanKey;

    private enum PlayerMovementState { StartCrouch, CrouchStay, CrouchWalk, Walk, Run, Jump, Lean };
    [SerializeField] private PlayerMovementState movementState;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        movementState = PlayerMovementState.Walk;
        actualSpeed = walkSpeed;
    }

    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        float vertInput = Input.GetAxis(verticalInputName) * actualSpeed * Time.deltaTime;
        float horizInput = Input.GetAxis(horizontalInputName) * actualSpeed * Time.deltaTime;

        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        characterController.SimpleMove(forwardMovement + rightMovement);

        JumpInput();
        CrouchInput();
        RunInput();
        LeanInput();
    }

    private void JumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && movementState == PlayerMovementState.Walk)
        {
            movementState = PlayerMovementState.Jump;
            StartCoroutine(JumpEvent());
        }
    }

    private void CrouchInput()
    {
        if (Input.GetKeyDown(crouchKey) && (movementState == PlayerMovementState.Walk || movementState == PlayerMovementState.Run))
        {
            movementState = PlayerMovementState.StartCrouch;
            StartCoroutine(CrouchEvent());
        }
        else if (Input.GetKeyDown(crouchKey) && (movementState == PlayerMovementState.CrouchStay || movementState == PlayerMovementState.CrouchWalk))
        {
            movementState = PlayerMovementState.StartCrouch;
            StartCoroutine(StandFromCrouchEvent());
        }
    }


    private void RunInput()
    {
        if (Input.GetKey(runKey) && (movementState == PlayerMovementState.Walk || movementState == PlayerMovementState.Run))
        {
            movementState = PlayerMovementState.Run;
            StartCoroutine(RunEvent());
        }
        else if (movementState == PlayerMovementState.Run)
        {
            StartCoroutine(WalkEvent());
        }
    }

    private void LeanInput()
    {
        if (Input.GetKey(LeftLeanKey) && (movementState == PlayerMovementState.Walk || movementState == PlayerMovementState.Lean))
        {
            movementState = PlayerMovementState.Lean;
            StartCoroutine(LeanEvent(1.0f));
        }
        else if (movementState == PlayerMovementState.Lean)
        {
            StartCoroutine(LeanResetEvent(1.0f));
        }

        if (Input.GetKey(RightLeanKey) && (movementState == PlayerMovementState.Walk || movementState == PlayerMovementState.Lean))
        {
            movementState = PlayerMovementState.Lean;
            StartCoroutine(LeanEvent(-1.0f));
        }
    }

    private IEnumerator JumpEvent()
    {
        characterController.slopeLimit = 90.0f;
        float timeInAir = 0.0f;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            characterController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
        } while (!characterController.isGrounded && characterController.collisionFlags != CollisionFlags.Above);

        characterController.slopeLimit = 45.0f;
        movementState = PlayerMovementState.Walk;
    }

    private IEnumerator CrouchEvent()
    {
        float crouchTimer = 0.0f;
        actualSpeed = 0.0f;

        do
        {
            float crouchForce = crouchFallOff.Evaluate(crouchTimer);
            camera.transform.localPosition -= new Vector3(0.0f, Time.deltaTime * crouchForce, 0.0f);
            crouchTimer += Time.deltaTime;
            yield return null;
        } while (camera.transform.localPosition.y > 0.3f);

        movementState = PlayerMovementState.CrouchStay;
    }

    private IEnumerator RunEvent()
    {
        float runTimer = 0.0f;
        while (actualSpeed < runSpeed)
        {
            float runForce = runIncreaseSpeed.Evaluate(runTimer);
            actualSpeed += runForce;
            runTimer += Time.deltaTime;
            yield return null;
        }

    }

    private IEnumerator WalkEvent()
    {
        float walkTimer = 0.0f;
        do
        {
            float runForce = runIncreaseSpeed.Evaluate(walkTimer);
            actualSpeed -= runForce;
            walkTimer += Time.deltaTime;
            yield return null;
        } while (actualSpeed > walkSpeed);

        movementState = PlayerMovementState.Walk;

    }

    private IEnumerator StandFromCrouchEvent()
    {
        float crouchTimer = 0.0f;

        do
        {
            float crouchForce = crouchFallOff.Evaluate(crouchTimer);
            camera.transform.localPosition += new Vector3(0.0f, Time.deltaTime * crouchForce, 0.0f);
            crouchTimer += Time.deltaTime;
            yield return null;
        } while (camera.transform.localPosition.y < 0.9f);

        movementState = PlayerMovementState.Walk;
    }

    private IEnumerator LeanEvent(float value)
    {
        float leanTimer = 0.0f;
        do
        {
            if (curAngle != maxLeanAngle)
            {
                curAngle = Mathf.MoveTowardsAngle(curAngle, maxLeanAngle * value, speedLean * Time.deltaTime);
                camera.transform.localRotation = Quaternion.AngleAxis(curAngle, Vector3.forward);
            }

            if (camera.transform.localPosition.x > -0.5f)
            {
                float leanCurve = leanOffset.Evaluate(leanTimer);
                camera.transform.localPosition -= new Vector3(leanTimer * Time.deltaTime, 0.0f, 0.0f);
                leanTimer += Time.deltaTime;
            }


            yield return null;

        } while (curAngle != maxLeanAngle || camera.transform.localPosition.x > -0.5f);

    }

    private IEnumerator LeanResetEvent(float value)
    {
        do
        {
            curAngle = Mathf.MoveTowardsAngle(curAngle, 0.0f, speedLean * Time.deltaTime);
            camera.transform.localRotation = Quaternion.AngleAxis(curAngle, Vector3.forward);
            yield return null;
        } while (curAngle != 0);

        camera.transform.localPosition += new Vector3(0.5f * value, 0.0f, 0.0f);
    }
}
