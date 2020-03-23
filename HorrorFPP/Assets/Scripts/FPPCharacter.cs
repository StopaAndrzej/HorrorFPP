using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPPCharacter : MonoBehaviour
{
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float strafeSpeed = 4f;

    [SerializeField] private float jumpPower = 5f;

    [SerializeField] private bool walkByDefault = true;
    [SerializeField] private AdvancedSettings advanced = new AdvancedSettings();

    [System.Serializable]
    public class AdvancedSettings
    {
        public float gravityMultiplier = 1.0f;
        public PhysicMaterial zeroFrictionMaterial;
        public PhysicMaterial highFrictionMaterial;
    }

    private CapsuleCollider capsule;
    private Rigidbody rigidbody;

    private const float jumpRayLength = 0.7f;
    [SerializeField] public bool grounded { get; private set; }

    private Vector2 input;

    void Awake()
    {
        capsule = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        grounded = true;
    }

    void FixedUpdate()
    {
        bool walkOrRun = Input.GetKey(KeyCode.LeftShift);
        float speed = walkByDefault ? (walkOrRun ? runSpeed : walkSpeed) : (walkOrRun ? walkSpeed : runSpeed);

        Ray ray = new Ray(transform.position, -transform.up);

        if(grounded || rigidbody.velocity.y < 0.1f)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, capsule.height * jumpRayLength);
            float nearest = float.PositiveInfinity;
            grounded = false;

            for(int i = 0; i<hits.Length; i++)
            {
                if(!hits[i].collider.isTrigger && hits[i].distance < nearest)
                {
                    grounded = true;
                    nearest = hits[i].distance;
                }
            }
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        input = new Vector2(horizontalInput, verticalInput);

        if(input.sqrMagnitude > 1)
        {
            input.Normalize();
        }

        Vector3 desireMove = transform.forward * input.y * speed + transform.right * input.x * strafeSpeed;

        float yVelocity = rigidbody.velocity.y;
        bool didJump = Input.GetButton("Jump");

        if(grounded && didJump)
        {
            yVelocity += jumpPower;
            grounded = false;
        }

        rigidbody.velocity = desireMove + Vector3.up * yVelocity;

        if(desireMove.magnitude > 0 || !grounded)
        {
            capsule.material = advanced.zeroFrictionMaterial;
        }
        else
        {
            capsule.material = advanced.highFrictionMaterial;
        }

        rigidbody.AddForce(Physics.gravity * (advanced.gravityMultiplier - 1));
    }
}
