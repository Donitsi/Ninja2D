using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Rigidbody2D playerRigidbody;
    private Animator playerAnimator;

    [SerializeField]
    private float speedMovement;

    private bool facingRight;
    private bool attack;
    private bool slide;
    private bool isGrounded;
    private bool jump;
    private bool shoot;
    private bool airShoot;

    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private bool airControl;

    [SerializeField]
    private float jumpForce;
   

	// Use this for initialization
	void Start () {

        facingRight = true;

        // reference to rigidbody
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
	}

    void Update()
    {
        HandleInput();
    }

    // Update is called once per frame
    void FixedUpdate () {

        float horizontal = Input.GetAxis("Horizontal");


        isGrounded = IsGrounded();

        HandleMovement(horizontal);

        Flip(horizontal);

        HandleAttacks();

        HandleLayers();

        ResetValues();
	}

    private void HandleMovement(float horizontal)
    {
        if (playerRigidbody.velocity.y < 0)
        {
            playerAnimator.SetBool("landing", true);
        }

        if (!playerAnimator.GetBool("slide") && !this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && (isGrounded || airControl))
        {
            playerRigidbody.velocity = new Vector2(horizontal * speedMovement, playerRigidbody.velocity.y);
        }

        // Checks if the player is on the ground or not
        if (isGrounded && jump)
        {
            isGrounded = false;
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            playerAnimator.SetTrigger("jump");
        }

        if (slide && !this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            playerAnimator.SetBool("slide", true);
        }

        else if (!this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            playerAnimator.SetBool("slide", false);
        }

        playerAnimator.SetFloat("speed", Mathf.Abs(horizontal)); // Returns horizontal to a positive value 
    }

    private void HandleAttacks()
    {
        // cannot attack until the original attack is done 
        if (attack && isGrounded && !this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            playerAnimator.SetTrigger("attack");
            // Sets velocity to 0 so that player isn't sliding anymore
            playerRigidbody.velocity = Vector2.zero;
        }

        if (shoot && isGrounded && !this.playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Shoot"))
        {
            playerAnimator.SetTrigger("shoot");
            // Sets velocity to 0 so that player isn't sliding anymore
            playerRigidbody.velocity = Vector2.zero;
        }

        if (shoot && !isGrounded && !this.playerAnimator.GetCurrentAnimatorStateInfo(1).IsTag("Shoot"))
        {
            playerAnimator.SetTrigger("shoot");
        }

        if (!shoot && !this.playerAnimator.GetCurrentAnimatorStateInfo(1).IsTag("Shoot"))
        {
            playerAnimator.ResetTrigger("shoot");
        }
    }


    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            attack = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            slide = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            shoot = true;
            airShoot = true;
        }
    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            // Make it false when it's true and vice verca 
            facingRight = !facingRight;
            
            // Reference to vector scale
            Vector3 theScale = transform.localScale;

            theScale.x *= -1;

            // adding to the player's scale
            transform.localScale = theScale;
        }
    }

    private void ResetValues()
    {
        // Resetting all values > Jumps, attacks, slide etc
        attack = false;
        slide = false;
        jump = false;
        shoot = false;
    }


    // Checks if player is on the ground or not
    private bool IsGrounded()
    {
        // if the player is falling or not moving 
        if (playerRigidbody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++)
                {
                    // if the current collider - at GroundPoint's position isn't the player
                    if (colliders[i].gameObject != gameObject)
                    {
                        playerAnimator.ResetTrigger("jump");
                        playerAnimator.SetBool("landing", false);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void HandleLayers()
    {
        if (!isGrounded)
        {
            playerAnimator.SetLayerWeight(1, 1);
        }

        else
        {
            playerAnimator.SetLayerWeight(1, 0);
        }
    }
}
