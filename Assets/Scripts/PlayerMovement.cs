using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{

    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("Assignables")]
    public Transform playerCam;
    public Transform orientation;
    public GameObject playerGFX;
    public Animator animator;
    public CapsuleCollider capsuleColl;

    [Header("Other")]
    private Rigidbody rb;

    [Header("Rot and Look")]
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;
    private bool paused = false;

    [Header("Movement")]
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;
    public float maxSlopeAngle = 35f;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSpeedSaved;
    public float crouchSpeed;

    [Header("Crouch & Slide")]
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;
    private float crouchScale = 1f;
    private float playerScale;

    [Header("Jumping")]
    public float jumpForce = 550f;
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;


    [Header("Sprinting")]
    public float sprintSpeed;
    public bool stamina;

    [Header("Input")]
    public float x, y;
    bool jumping, sprinting, crouching;

    [Header("Sliding")]
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        crouchSpeed = maxSpeed / 2;
        maxSpeedSaved = maxSpeed;
        sprintSpeed = maxSpeed * 2;
        playerScale = 2f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        if(!isOwned) { return; }
        Movement();
    }

    private void Update()
    {
        if(!isOwned) { return; }
        MyInput();
        Look();
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    /// 
    private void MyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(paused == false)
            {
                Debug.Log("Paused");
                paused = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Debug.Log("Unpaused");
                paused = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }


        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
        sprinting = Input.GetKey(KeyCode.LeftShift);

        /*
        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
        */

        if (Input.GetKeyDown(KeyCode.LeftShift))
            StartSprint();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            StopSprint();
    }

    void StartSprint()
    {
        animator.SetBool("Running", true);
        maxSpeed = sprintSpeed;
    }

    void StopSprint()
    {
        animator.SetBool("Running", false);
        maxSpeed = maxSpeedSaved;
    }

    private void StartCrouch()
    {
        maxSpeed = crouchSpeed;
        animator.SetBool("Crouching", true);
        capsuleColl.height = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f)
        {
            if (grounded)
            {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    private void StopCrouch()
    {
        animator.SetBool("Crouching", false);

        maxSpeed = maxSpeedSaved;

        capsuleColl.height = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }


    private void Movement()
    {
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;

        /*
        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }
        */

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = 1f;
            multiplierV = 1f;
        }

        // Movement while sliding
        //if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);

        animator.SetFloat("Forward", yMag);
        animator.SetFloat("Sideways", xMag);

    }

    private void Jump()
    {
        if (grounded && readyToJump && !crouching)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);


            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private float desiredX;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        if(paused) { return; }
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
        playerGFX.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }
}