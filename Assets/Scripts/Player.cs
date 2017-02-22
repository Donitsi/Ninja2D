using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DeadEventHandler();

public class Player : Character {

    private static Player instance;

    public event DeadEventHandler Dead; 

    public static Player Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<Player>();
            }
            return instance;
        }

        set
        {
            instance = value;
        }
    }


    //private bool facingRight;

    public bool OnGround { get; set; }

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private bool airControl;
    private bool immortal = false;

    [SerializeField]
    private float immortalTime;

    private Vector2 startPos;

    public Rigidbody2D MyRigidbody { get; set; }


    

    public override bool IsDead
    {
        get
        {
            if (healthStat.CurrentVal <= 0)
            {
                OnDead();
            }
            
            return healthStat.CurrentVal <= 0;
        }
    }



    // Use this for initialization
    public override void Start () {

        //facingRight = true;
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        // reference to rigidbody
        MyRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        

        if (!TakingDamage && !IsDead)
        {

            if (transform.position.y <= -10f)
            {
                Death();
            }

            HandleInput();
        }
    }

    // Update is called once per frame
    void FixedUpdate () {

        if (!TakingDamage && !IsDead)
        {
            float horizontal = Input.GetAxis("Horizontal");

            OnGround = IsGrounded();

            HandleMovement(horizontal);

            Flip(horizontal);

            HandleLayers();
        }

	}

    public void OnDead()
    {
        if(Dead != null)
        {
            Dead();
        }
    }

    private void HandleMovement(float horizontal)
    {
        if(MyRigidbody.velocity.y < 0)
        {
            myAnimator.SetBool("land", true);
        }

        if(!Attack && !Slide /*&& (OnGround || airControl)*/)
        {
            MyRigidbody.velocity = new Vector2(horizontal * movementSpeed, MyRigidbody.velocity.y);
            
        }

        if(Jump && MyRigidbody.velocity.y == 0)
        {
            MyRigidbody.AddForce(new Vector2(0, jumpForce));
        }

        myAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }




    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            myAnimator.SetTrigger("attack");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            myAnimator.SetTrigger("slide");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            myAnimator.SetTrigger("jump");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            myAnimator.SetTrigger("shoot");
            myAnimator.SetTrigger("shootAir");
        }
    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            //// Make it false when it's true and vice verca 
            //facingRight = !facingRight;

            //// Reference to vector scale
            //Vector3 theScale = transform.localScale;

            //theScale.x *= -1;

            //// adding to the player's scale
            //transform.localScale = theScale;

            ChangeDirection();
        }
    }



    // Checks if player is on the ground or not
    private bool IsGrounded()
    {
        // if the player is falling or not moving 
        if (MyRigidbody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++)
                {
                    // if the current collider - at GroundPoint's position isn't the player
                    if (colliders[i].gameObject != gameObject)
                    {
                        
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void HandleLayers()
    {
        if (!OnGround)
        {
            myAnimator.SetLayerWeight(1, 1);
        }

        else
        {
            myAnimator.SetLayerWeight(1, 0);
        }
    }

    public override void ShootPref(int value)
    {

        if ( OnGround && value == 0)
        {
            base.ShootPref(value);
        }
        else if(!OnGround && value == 1)
        {
            base.ShootPref(value);
        }
        
    }

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Bullet")
    //    {
    //        //Destroy(gameObject);

    //        // Destroy bullet when hit
    //        Destroy(collision.gameObject);
    //    }
    //}

    private IEnumerator IndicateImmortal()
    {
        while (immortal)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
    }

    public override IEnumerator TakeDamage()
    {
        healthStat.CurrentVal -= 10;

        if (!immortal)
        {
            if (!IsDead)
            {
                myAnimator.SetTrigger("damage");
                immortal = true;
                StartCoroutine(IndicateImmortal());

                yield return new WaitForSeconds(immortalTime);

                immortal = false;
            }

            else
            {
                myAnimator.SetLayerWeight(1, 0);
                myAnimator.SetTrigger("die");
            }

        }
    }

    public override void Death()
    {
        MyRigidbody.velocity = Vector2.zero;
        myAnimator.SetTrigger("idle");
        healthStat.CurrentVal = healthStat.MaxValue;
        transform.position = startPos;
    }
}
