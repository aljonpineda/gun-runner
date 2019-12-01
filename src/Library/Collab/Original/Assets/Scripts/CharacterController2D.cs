using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_ShootForce = 700f;                         // Amount of force exerted when the player shoots.
    [SerializeField] private float m_SpeedMultiplier = 10f;                     // Amount to multiply to a computed movement value to determine horizontal speed.
    [SerializeField] private float m_RunSpeed = 40f;                            // A base movement speed.
    [SerializeField] private float m_SprintValue = 2f;                          // When sprinting, this value is multiplied to the horizontal movement value.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .1f;	// How much to smooth out the movement.
    [Range(0, 10)] [SerializeField] private float m_WallSlidingSpeed = 2f;      // The falling speed of the player when touching the wall while in mid-air.
    [SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character.
    [SerializeField] private LayerMask m_WhatIsWall;                            // A mask determining what is wall to the character.
    [SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_WallCheck;                             // A position marking where to check if the player is touching the wall.
    public ParticleSystem gunFlash;
    public ParticleSystem Dust;
    public Animator animator;
    public ParticleSystem WallDust;
    public bool TouchingWall = false;

    const float k_GroundedRadius = .1f;         // Radius of the overlap circle to determine if grounded.
    const float k_WallTouchRadius = .2f;        // Radius of the overlap circle to determine if touching wall.
	private bool m_Grounded;                    // Whether or not the player is grounded.
    private bool m_TouchingWall;                // Whether or not the player is touching the wall.

	private Rigidbody2D m_Rigidbody2D;
    private SpriteRenderer renderer;
	private bool m_FacingRight = true;          // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
    private Vector2 startPos;

    [Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

        startPos = transform.position;
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
        TouchingWall = m_TouchingWall;
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
                if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}

        m_TouchingWall = Physics2D.OverlapCircle(m_WallCheck.position, k_WallTouchRadius, m_WhatIsWall);

	}

	public void Move(float move, bool sprint)
	{
        float sprintMultiplier = sprint ? m_SprintValue : 1;
        move *= m_RunSpeed * m_SpeedMultiplier * sprintMultiplier;

        // Move the character by finding the target velocity and then smoothing it out and applying it to the character
        Vector3 targetVelocity = new Vector2(move, m_Rigidbody2D.velocity.y);

        // Set the target velocity to zero if there is no player input and the player is grounded.
        if (move == 0 && m_Grounded)
        {
            targetVelocity = new Vector2(0, m_Rigidbody2D.velocity.y);
        }

        // Don't update velocity when there is no player input while the player is mid-air.
        if (!(move == 0 && !m_Grounded))
        {
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        }

        if (m_TouchingWall && !m_Grounded && m_Rigidbody2D.velocity.y < 0)
        {
            m_Rigidbody2D.velocity = new Vector2(0, -m_WallSlidingSpeed);
            animator.SetBool("IsTouchingWall",true);
            CreateWallDust();
        }
        else
        {
            animator.SetBool("IsTouchingWall", false);
        }
        
		// If the input is moving the player in the opposite direction the player is facing...
		if (move > 0 && !m_FacingRight || move < 0 && m_FacingRight)
		{
			// ... flip the player.
			Flip();
		}
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;
        CreateDust();
        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    // Shoot adds a force to the player opposite the direction from the player to the mouse.
    public void Shoot()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D playerToMouse = new Ray2D(m_Rigidbody2D.position, mousePosition - m_Rigidbody2D.position);
        gunFlash.Emit(5);
        CreateDust();
        FindObjectOfType<AudioManager>().Play("GunShot");


        m_Rigidbody2D.velocity = Vector3.zero;
        m_Rigidbody2D.AddForce(-m_ShootForce * playerToMouse.direction);
    }

    public bool IsFacingRight()
    {
        return m_FacingRight;
    }

    public bool IsGrounded()
    {
        return m_Grounded;
    }

    void CreateDust()
    {
        Dust.Play();
    }

    void CreateWallDust()
    {
        WallDust.Play();
    }

    public IEnumerator Die()
    {
        renderer.enabled = false;
        yield return new WaitForSeconds(3);
        transform.position = startPos;
        renderer.enabled = true;
    }
}
