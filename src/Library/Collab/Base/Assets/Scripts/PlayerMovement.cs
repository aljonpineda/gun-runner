using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Import CharacterController Component from Unity
    public CharacterController2D controller;

    // Variables

    public float runSpeed = 40f;
    public float sprintvalue = 0;
    float sprintmultiplier = 1;
    float horizontalmove = 0f;
    bool isJumping = false;

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
        // Get Input from Left or Right, -1 for Left, 1 for Right.
        horizontalmove = (Input.GetAxisRaw("Horizontal") * runSpeed) * sprintmultiplier;

        // Check if player is touching the ground layer
        isGrounded = Physics2D.OverlapArea(new Vector2(transform.position.x - 0.5f, transform.position.y - 0.5f),
    new Vector2(transform.position.x + 0.5f, transform.position.y - 0.51f), groundLayers);


        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }

        // Check if Sprinting, If not Change Movement Multiplier
        if (Input.GetButton("Sprint"))
        {
            sprintmultiplier = sprintvalue;
        }
        else
        {
            sprintmultiplier = 1;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            controller.Shoot();
        }

    }

    private void FixedUpdate()
    {
        // Move Our Character by calling the CharacterController2D, Time delta makes sure we move the same amount no matter how often the function is called.
        controller.Move(horizontalmove * Time.fixedDeltaTime, false, isJumping);
        isJumping = false;
        
    }
}