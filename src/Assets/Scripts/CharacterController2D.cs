using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_SpeedMultiplier = 10f;                     // Amount to multiply to a computed movement value to determine horizontal speed.
    [SerializeField] private float m_RunSpeed = 40f;                            // A base movement speed.
    [SerializeField] private float m_SprintValue = 2f;                          // When sprinting, this value is multiplied to the horizontal movement value.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .1f;	// How much to smooth out the movement.
    [Range(0, 10)] [SerializeField] private float m_WallSlidingSpeed = 2f;      // The falling speed of the player when touching the wall while in mid-air.
    [SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character.
    [SerializeField] private LayerMask m_WhatIsWall;                            // A mask determining what is wall to the character.
    [SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_WallCheck;                             // A position marking where to check if the player is touching the wall.

    const float k_GroundedRadius = .1f;         // Radius of the overlap circle to determine if grounded.
    const float k_WallTouchRadius = .2f;        // Radius of the overlap circle to determine if touching wall.
	private bool m_Grounded;                    // Whether or not the player is grounded.
    private bool m_TouchingWall;                // Whether or not the player is touching the wall.

	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;          // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
    private Vector2 startPos;

    public ParticleSystem gunFlash;
    public ParticleSystem Dust;
    public ParticleSystem DeathDust;
    public TrailRenderer PlayerTrail;
    private bool EmitTrail = true;
    public Animator animator;
    public ParticleSystem WallDust;
    public bool TouchingWall = false;
    public bool Freezeplayer = false;
    public AmmoTracker ammoTracker;
    private SpriteRenderer Sprender;
    private GameObject PlayerArm;
    private bool invisible = false;
    Animator m_Animator;

    [Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

        startPos = transform.position;
        GameManager.EnableInputs();
        Freezeplayer = false;
        invisible = false;
        m_Animator = gameObject.GetComponent<Animator>();
    }

    //If Collision with Drill
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Drill")
        {
            Freezeplayer = true;
            invisible = false;

            GameManager.DisableInputs();
            m_Animator.enabled = false;
        }
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

        if (EmitTrail)
        {
            PlayerTrail.emitting = true;
        }
        else
        {
            PlayerTrail.emitting = false;
        }
    }

	public void Move(float move, bool sprint)
	{
        float sprintMultiplier = sprint ? m_SprintValue : 1;
        move *= m_RunSpeed * m_SpeedMultiplier * sprintMultiplier;

        // Move the character by finding the target velocity and then smoothing it out and applying it to the character
        Vector3 targetVelocity = new Vector2(move, m_Rigidbody2D.velocity.y);

        //Freeze Player In Place
        if (Freezeplayer)
        {
            targetVelocity = new Vector2(0, 0);
            m_Rigidbody2D.velocity = Vector3.zero;
            this.m_Rigidbody2D.gravityScale = 0.0f;

            if (invisible)
            {
                //Make All Player Objects Invisible
                Sprender = gameObject.GetComponent<SpriteRenderer>();
                Sprender.enabled = false;
                PlayerArm = GameObject.Find("GunArm1");
                PlayerArm.GetComponent<SpriteRenderer>().enabled = false;

                ammoTracker.MakeInvisible();
            }
          
        }
        else
        {
            targetVelocity = new Vector2(move, m_Rigidbody2D.velocity.y);
            //Change this value for Gravity Scale
            this.m_Rigidbody2D.gravityScale = 3.0f;
        }
        

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
    public void Shoot(float shootForce)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray2D playerToMouse = new Ray2D(m_Rigidbody2D.position, mousePosition - m_Rigidbody2D.position);
        gunFlash.Emit(5);
        CreateDust();

        if (ammoTracker.UsingPistol())
        {
            FindObjectOfType<AudioManager>().Play("GunShot");
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("Shotgun");
        }

        m_Rigidbody2D.velocity = Vector3.zero;
        m_Rigidbody2D.AddForce(-shootForce * playerToMouse.direction);
    }

    public bool IsFacingRight()
    {
        return m_FacingRight;
    }

    public bool IsGrounded()
    {
        return m_Grounded;
    }

    //Movement Dust Particle Effect
    void CreateDust()
    {
        Dust.Play();
    }
    //WallSlide Partle Effect
    void CreateWallDust()
    {
        WallDust.Play();
    }
    

    public IEnumerator Die()
    {
        FindObjectOfType<AudioManager>().Play("Death");
        CreateDeathDust();
        EmitTrail = false;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        // The time it takes to run the death animation
        float deathTime = 1.5f;
        Freezeplayer = true;
        invisible = true;
        GameManager.DisableInputs();
        yield return new WaitForSeconds(deathTime);

        Respawn();
    }

    private void Respawn()
    {
        transform.position = startPos;
        EmitTrail = true;
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        GameManager.EnableInputs();
        Freezeplayer = false;
        invisible = false;

        //Make All Player Objects visible
        Sprender = gameObject.GetComponent<SpriteRenderer>();
        Sprender.enabled = true;
        PlayerArm = GameObject.Find("GunArm1");
        PlayerArm.GetComponent<SpriteRenderer>().enabled = true;

        ammoTracker.MakeVisible();
    }

    //Death Particle Effect
    void CreateDeathDust()
    {
        DeathDust.Play();
    }


}
