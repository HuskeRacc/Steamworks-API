using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
    [Header("Character Movement")]
    public CharacterController controller;
    public float moveSpeed = 12f;
    public float jumpForce = 3f;

    [Header("Velocity and Gravity")]
    public float gravity = -9.81f;
    Vector3 velocity;

    [Header("Ground Checking")]
    public Transform groundCheck;
    public float checkRadius = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;


    private void Update()
    {
        //Creates a sphere around a groundcheck gameobject placed at the players feet to check if it collides with the ground layer mask. Returns true or false.
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundMask);

        //Resets velocity
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // These Axis' are between -1 and 1.
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        /*
         * Declare Direction. 
         * x = -1 - 1. 
         * z = -1 - 1.
         */
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * moveSpeed * Time.deltaTime);
        // or controller.Move(transform.right * x + transform.forward * z);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        //Gravity increases player velocity.
        velocity.y += gravity * Time.deltaTime;

        //apply gravity
        controller.Move(velocity * Time.deltaTime);
    }

}
