using System.Collections;
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
    public bool disablePlayerController = false;

    //leaning params
    [SerializeField] private float leanAngle = 35.0f;
    [SerializeField] private float leanSpeed = 5.0f;
    [SerializeField] private float leanBackSpeed = 6.0f;

    //crouching params
    private float standingHight;
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private float crouchHeight = 0.0f;
    [SerializeField] private float crouchPosSpeed;
    [SerializeField] private float crouchAboveRayLength;

    //headbobing
    public Vector3 restPosition;

    [SerializeField] private float transitionSpeed = 20f; //smooths between moving to not moving
    [SerializeField] private float bobSpeed = 5.0f;
    [SerializeField] private float bobAmount = 0.05f;

    private float timer = Mathf.PI / 2;

    //headbobing v2
    [SerializeField] private float headBobFrequency = 1.5f;
    [SerializeField] private float headBobSwayAngle = 0.5f;
    [SerializeField] private float headBobSideMovement = 0.05f;
    [SerializeField] private float headBobSpeedMultiplier = 0.3f;
    [SerializeField] private float headBobHeight = 0.3f;
    [SerializeField] private float bobStrideSpeedLengthen = 0.3f;
    [SerializeField] private float jumpLandMove = 3.0f;
    [SerializeField] private float jumpLandTilt = 60.0f;

    [SerializeField] private Vector3 originalLocalPostion;

    float headBobCycle = 0.0f;
    float headBobFade = 0.0f;

    float springPosition = 0.0f;
    float springVelocity = 0.0f;
    float springElastic = 1.1f;
    float springDampen = 0.8f;
    float springVelocityThreshold = 0.05f;
    float springPositionThreshold = 0.05f;
    [SerializeField] private Vector3 previousPosition;
    Vector3 previousVelocity = Vector3.zero;
    bool prevGrounded;

    public float xTilt, zTilt;


    //headbobing diff states params
    [SerializeField] private float crouchSpeedheadBob = 2.0f;
    [SerializeField] private float runSpeedheadBob = 10.0f;
    [SerializeField] private float defaultSpeedheadBob = 5.0f;

    [SerializeField] private float crouchAmountheadBob = 0.1f;
    [SerializeField] private float runAmountheadBob = 0.1f;
    [SerializeField] private float defaultAmountheadBob = 0.2f;

    [SerializeField] private float standardCameraFieldOfView = 60.0f;
    [SerializeField] private float crouchCameraFieldOfView = 50.0f;
    [SerializeField] private float runCameraFieldOfView = 75.0f;
    [SerializeField] private float jumpCameraFieldOfView = 90.0f;

    //rotation-inspection flag. disable player movement
    public bool inspectMode = false;
    private bool lastFrameInspectMode = false; //if it was then enable dot FOR ONCE!
    public bool seatMode = false;
    public Material blurMaterial;

    //canvas dot cursor
    public GameObject dotCursor;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        standingHight = camera.localPosition.y;
        camera.gameObject.GetComponent<Camera>().fieldOfView = standardCameraFieldOfView;
    }

    private void Start()
    {
        crouchAboveRayLength = 1.0f - crouchHeight;
        restPosition = camera.transform.localPosition;

        originalLocalPostion = camera.localPosition;
        previousPosition = GetComponent<Transform>().position;

        blurMaterial.SetVector("_Color", new Vector4(1, 1, 1, 1));
        blurMaterial.SetFloat("_Size", 0.0f);

        dotCursor.SetActive(true);
    }

    private void Update()
    {
        if(!disablePlayerController)
        {
            camera.GetComponent<PlayerLook>().disableCamera = false;
            //disable player movement if inspection mode is active
            //or sit
            if (!inspectMode && !seatMode)
            {
                PlayerMovement();

                if(lastFrameInspectMode)
                    dotCursor.SetActive(true);
                //blur
                blurMaterial.SetFloat("_Size", Mathf.Lerp(blurMaterial.GetFloat("_Size"), 0.0f, Time.deltaTime * 8.0f));
                if (blurMaterial.GetFloat("_Size") < 0.1f)
                    blurMaterial.SetFloat("_Size", 0.0f);

                blurMaterial.SetVector("_Color", new Vector4(Mathf.Lerp(blurMaterial.GetVector("_Color").x, 1, Time.deltaTime * 4.0f), Mathf.Lerp(blurMaterial.GetVector("_Color").y, 1, Time.deltaTime * 4.0f), Mathf.Lerp(blurMaterial.GetVector("_Color").z, 1, Time.deltaTime * 4.0f), 1));
                if (blurMaterial.GetVector("_Color").x > 0.99f)
                    blurMaterial.SetVector("_Color", new Vector4(1, 1, 1, 1));

                lastFrameInspectMode = false;
            }
            else if (inspectMode)
            {
                dotCursor.SetActive(false);
                //blur
                blurMaterial.SetFloat("_Size", Mathf.Lerp(blurMaterial.GetFloat("_Size"), 20.0f, Time.deltaTime * 2.0f));
                if (blurMaterial.GetFloat("_Size") > 19.0f)
                    blurMaterial.SetFloat("_Size", 20.0f);

                blurMaterial.SetVector("_Color", new Vector4(Mathf.Lerp(blurMaterial.GetVector("_Color").x, 0, Time.deltaTime), Mathf.Lerp(blurMaterial.GetVector("_Color").y, 0, Time.deltaTime), Mathf.Lerp(blurMaterial.GetVector("_Color").z, 0, Time.deltaTime), 1));
                if (blurMaterial.GetVector("_Color").x < 0.01f)
                    blurMaterial.SetVector("_Color", new Vector4(0, 0, 0, 1));

                Inspect();
                lastFrameInspectMode = true;
            }
        }
        else
        {
            camera.GetComponent<PlayerLook>().disableCamera = true;
        }
        
    }

    private void Inspect()
    {
        //object rotation

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
        HeadBobing();
        HeadBobing2();
    }

    private void SetMovementSpeed()
    {
        //additional change camera field of view when jump value increase/decrease
        if(isJumping)
        {
            float jumpingVelocity = 0.0f;
            if(camera.localPosition.y>jumpingVelocity)
            {
                camera.gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(camera.gameObject.GetComponent<Camera>().fieldOfView, jumpCameraFieldOfView, Time.deltaTime * 5.0f);
            }
            else
            {
                camera.gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(camera.gameObject.GetComponent<Camera>().fieldOfView, standardCameraFieldOfView, Time.deltaTime * 10.0f);
            }

            jumpingVelocity = camera.localPosition.y;
        }


        if(isCrouching)
        {
            movementSpeed = Mathf.Lerp(movementSpeed, crouchSpeed, Time.deltaTime * crouchBuildSpeed);
            camera.gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(camera.gameObject.GetComponent<Camera>().fieldOfView, crouchCameraFieldOfView, Time.deltaTime * 1.1f);
        }
        else if(Input.GetKey(runKey))
        {
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * runBuildUpSpeed);
            camera.gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(camera.gameObject.GetComponent<Camera>().fieldOfView, runCameraFieldOfView, Time.deltaTime * 1.1f);
            bobSpeed = Mathf.Lerp(bobSpeed, runSpeedheadBob, Time.deltaTime * 10.1f);
            bobAmount = Mathf.Lerp(bobAmount, runAmountheadBob, Time.deltaTime * 10.1f);
        }
        else
        {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);
            camera.gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(camera.gameObject.GetComponent<Camera>().fieldOfView, standardCameraFieldOfView, Time.deltaTime * 1.1f);
            bobSpeed = Mathf.Lerp(bobSpeed, defaultSpeedheadBob, Time.deltaTime * 10.1f);
            bobAmount = Mathf.Lerp(bobAmount, defaultAmountheadBob, Time.deltaTime * 10.1f);
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
        if(Physics.Raycast(camera.transform.position, Vector3.up, out hit, crouchAboveRayLength))
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
                    bobSpeed = defaultSpeedheadBob;
                    bobAmount = defaultAmountheadBob;
                } 
            }
            else
            {
               isCrouching = true;
               StartCoroutine(LerpFromTo(camera.localPosition, new Vector3(camera.localPosition.x, crouchHeight, camera.localPosition.z), crouchPosSpeed));
               bobSpeed = crouchSpeedheadBob;
               bobAmount = crouchAmountheadBob;
            }
        }
    }

    private IEnumerator LerpFromTo(Vector3 pos1, Vector3 pos2, float duration)
    {
        movementSpeed = 0.0f;

        if (!isCrouching)
        {
            restPosition += Vector3.up * 0.6f;
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
            restPosition -= Vector3.up * 0.6f;


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

    private void HeadBobing()
    {

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)    //moving
        {
            timer += bobSpeed * Time.deltaTime;

            Vector3 newPosition = new Vector3(Mathf.Cos(timer) * bobAmount, restPosition.y + Mathf.Abs((Mathf.Sin(timer) * bobAmount)), restPosition.z);
            camera.transform.localPosition = newPosition;
        }
        else
        {
            timer = Mathf.PI / 2.0f;

            Vector3 newPosition = new Vector3(Mathf.Lerp(camera.transform.localPosition.x, restPosition.x, transitionSpeed * Time.deltaTime),
                Mathf.Lerp(camera.transform.localPosition.y, restPosition.y, transitionSpeed * Time.deltaTime), Mathf.Lerp(camera.transform.localPosition.z, restPosition.z, transitionSpeed * Time.deltaTime));

            camera.transform.localPosition = newPosition;
        }

        if (timer > Mathf.PI * 2)    //complete a full cycle on the unit circle. reset to 0 to aviod bloated values
        {
            timer = 0;
        }
    }

    private void HeadBobing2()
    {
        Vector3 velocity = (GetComponent<Transform>().position - previousPosition) / Time.deltaTime;
        Vector3 velocityChange = velocity - previousVelocity;
        previousPosition = GetComponent<Transform>().position;
        previousVelocity = velocity;

        springVelocity -= velocityChange.y;
        springVelocity -= springPosition * springElastic;
        springVelocity *= springDampen;

        springPosition += springVelocity * Time.deltaTime;
        springPosition = Mathf.Clamp(springPosition, -0.3f, 0.3f);

        if (Mathf.Abs(springVelocity) < springVelocityThreshold && Mathf.Abs(springPosition) < springPositionThreshold)
        {
            springVelocity = 0;
            springPosition = 0;
        }

        float flatVelocity = new Vector3(velocity.x , 0.0f, velocity.z).magnitude;

        float strideLengthen = 1 + (flatVelocity * bobStrideSpeedLengthen);

        headBobCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / headBobFrequency);

        float bobFactor = Mathf.Sin(headBobCycle * Mathf.PI * 2);
        float bobSwayFactor = Mathf.Sin(Mathf.PI * (2 * headBobCycle + 0.5f));

        bobFactor = 1 - (bobFactor * 0.5f + 1);
        bobFactor *= bobFactor;

        if (new Vector3(velocity.x, 0.0f, velocity.z).magnitude < 0.1f)
        {
            headBobFade = Mathf.Lerp(headBobFade, 0.0f, Time.deltaTime);
        }
        else
        {
            headBobFade = Mathf.Lerp(headBobFade, 1.0f, Time.deltaTime);
        }

        xTilt = -springPosition * jumpLandTilt;
        zTilt = bobSwayFactor * headBobSwayAngle * headBobFade;

        //camera.localRotation = Quaternion.Euler(xTilt, camera.localRotation.y , zTilt);
    }

}
