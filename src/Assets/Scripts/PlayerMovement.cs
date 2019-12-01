using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Import CharacterController Component from Unity
    public CharacterController2D controller;
    public Animator animator;
    public ParticleSystem Splash;

    private Rigidbody2D rb;
    float horizontalMove = 0f;

    bool isSprinting = false;
    
    private float footstepspeed = 0;
    private float footstepspeedmax = 12;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.InputEnabled())
        {
            horizontalMove = Input.GetAxisRaw("Horizontal");
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButton("Sprint") && GameManager.InputEnabled())
        {
            isSprinting = true;
            footstepspeedmax = 8;
        } 
        else
        {
            isSprinting = false;
            footstepspeedmax = 12;
        }

        if (rb.velocity.y > 0.1 && !controller.IsGrounded())
        {
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
        }
    
        // Falling Animation
        if (rb.velocity.y < -0.1 && !controller.IsGrounded())
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", true);
            // Prepare Falling Noise
            footstepspeed = -12;
        }
        else
        {
            animator.SetBool("IsFalling", false);
        }

    }
    // Grounded Check
    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    private void FixedUpdate()
    {
        // Move Our Character by calling the CharacterController2D, Time delta makes sure we move the same amount no matter how often the function is called.
        controller.Move(horizontalMove * Time.fixedDeltaTime, isSprinting);
        
        // Play Foot Step Noise In Intervals
        if ((footstepspeed >= footstepspeedmax) && controller.IsGrounded())
        {
            FindObjectOfType<AudioManager>().Play("FootStep");
            
            footstepspeed = 0;
        }
        else if ((footstepspeed <= -footstepspeedmax) && controller.IsGrounded())
        {
            FindObjectOfType<AudioManager>().Play("DroppedContact");
            CreateSplash();
            footstepspeed = 0;
        }

        if ((horizontalMove != 0) && controller.IsGrounded())
        {
            footstepspeed += 1;
        }
    }

    void CreateSplash()
    {
        Splash.Play();
    }
}