using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Import CharacterController Component from Unity
    public CharacterController2D controller;

    public float runSpeed = 40f;
    public float sprintvalue = 0;

    float sprintMultiplier = 1;
    float horizontalMove = 0f;

    bool isMoving = false;
    bool isJumping = false;
    bool isShooting = false;

    // Is grounded
    public bool isGrounded;
    public LayerMask groundLayers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from Left or Right, -1 for Left, 1 for Right.
        horizontalMove = (Input.GetAxisRaw("Horizontal") * runSpeed) * sprintMultiplier;

        // Check if player is touching the ground layer
        isGrounded = Physics2D.OverlapArea(
            new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f),
            new Vector2(transform.position.x + 0.5f, transform.position.y - 0.51f), 
            groundLayers);

        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }

        // Check if Sprinting, If not Change Movement Multiplier
        if (Input.GetButton("Sprint"))
        {
            sprintMultiplier = sprintvalue;
        }
        else
        {
            sprintMultiplier = 1;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            isShooting = true;
        }

    }

    private void FixedUpdate()
    {
        // Move player by calling the CharacterController2D. Time delta makes sure we move the same amount no matter how often the function is called.
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, isJumping);
        isJumping = false;

        if (isShooting)
        {
            controller.Shoot();
            isShooting = false;
        }
        
    }
}