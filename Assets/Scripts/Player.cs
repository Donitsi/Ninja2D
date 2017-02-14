using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private static Player instance;

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

    private Animator myAnimator;

    private float movementSpeed = 10f;

    private bool facingRight;


    [SerializeField]
    private Transform[] groundPoints;
    private float groundRadius = 0.2f;
    public LayerMask whatIsGround;

    [SerializeField]
    private bool airControl;

    private float jumpForce = 550f;

    [SerializeField]
    private GameObject BulletPref;


    public Rigidbody2D MyRigidbody { get; set; }

    public bool Attack { get; set; }

    public bool Slide { get; set; }

    public bool Jump { get; set; }

    public bool OnGround { get; set; }

    public bool Shoot { get; set; }



    // Use this for initialization
    void Start () {

        facingRight = true;

        // reference to rigidbody
        MyRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
    }

    // Update is called once per frame
    void FixedUpdate () {

        float horizontal = Input.GetAxis("Horizontal");

        OnGround = IsGrounded();

        HandleMovement(horizontal);

        Flip(horizontal);

        HandleLayers();

	}

    private void HandleMovement(float horizontal)
    {
        if(MyRigidbody.velocity.y < 0)
        {
            myAnimator.SetBool("land", true);
        }

        if(!Attack && !Slide && (OnGround || airControl))
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
            ShootPref(0);
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

    public void ShootPref(int value)
    {
        if (facingRight)
        {
            GameObject tmp = (GameObject)Instantiate(BulletPref, transform.position, Quaternion.Euler(new Vector3(0, 0,-90)));
            tmp.GetComponent<Bullet>().Initialize(Vector2.right);
        }
        else
        {
            GameObject tmp = (GameObject)Instantiate(BulletPref, transform.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            tmp.GetComponent<Bullet>().Initialize(Vector2.left);
        }
    }
}
