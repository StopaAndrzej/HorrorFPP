using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobing : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float headBobFrequency = 1.5f;
    [SerializeField] private float headBobSwayAngle = 0.5f;
    [SerializeField] private float headBobSideMovement = 0.05f;
    [SerializeField] private float headBobSpeedMultiplier = 0.3f;
    [SerializeField] private float headBobHeight = 0.3f;
    [SerializeField] private float bobStrideSpeedLengthen = 0.3f;
    [SerializeField] private float jumpLandMove = 3.0f;
    [SerializeField] private float jumpLandTilt = 60.0f;

    [SerializeField] private Vector3 originalLocalPostion;
    float nextStepTime = 0.5f;
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

    void Start()
    {
        originalLocalPostion = head.localPosition;
        previousPosition = GetComponent<Transform>().position;
    }

    void FixedUpdate()
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

        if(Mathf.Abs(springVelocity) < springVelocityThreshold && Mathf.Abs(springPosition) < springPositionThreshold)
        {
            springVelocity = 0;
            springPosition = 0;
        }

        float flatVelocity = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;

        float strideLengthen = 1 + (flatVelocity * bobStrideSpeedLengthen);

        headBobCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / headBobFrequency);

        float bobFactor = Mathf.Sin(headBobCycle * Mathf.PI * 2);
        float bobSwayFactor = Mathf.Sin(Mathf.PI * (2 * headBobCycle + 0.5f));

        bobFactor = 1 - (bobFactor * 0.5f + 1);
        bobFactor *= bobFactor;

        if(new Vector3(velocity.x, 0.0f, velocity.z).magnitude < 0.1f)
        {
            headBobFade = Mathf.Lerp(headBobFade, 0.0f, Time.deltaTime);
        }
        else
        {
            headBobFade = Mathf.Lerp(headBobFade, 1.0f, Time.deltaTime);
        }

        float speedHeightFactor = 1 + (flatVelocity * headBobSpeedMultiplier);

        float xPos = -headBobSideMovement * bobSwayFactor;
        float yPos = springPosition * jumpLandMove + bobFactor * headBobHeight * headBobFade * speedHeightFactor;

        float xTilt = -springPosition * jumpLandTilt;
        float zTilt = bobSwayFactor * headBobSwayAngle * headBobFade;

        head.localPosition = originalLocalPostion + new Vector3(xPos, yPos, 0.0f);
        head.localRotation = Quaternion.Euler(xTilt, 0.0f, zTilt);

    }
}
