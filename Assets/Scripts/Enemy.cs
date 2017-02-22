using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    private IEnemyState currentState;

    public GameObject Target { get; set; }

    public Animator MyAnimator { get; private set; }

    public Rigidbody2D MyRigidbody { get; set; }

    public bool OnGround { get; set; }

    private Vector2 startPos;

    [SerializeField]
    private float meleeRange;

    [SerializeField]
    private float shootRange;

    [SerializeField]
    private Transform leftEdge;

    [SerializeField]
    private Transform rightEdge;

    public bool inMeleeRange
    {
        get
        {
            if(Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
            }

            return false;
        }
    }

    public bool inShootRange
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= shootRange;
            }

            return false;
        }
    }

    public override bool IsDead
    {
        get
        {
            return healthStat.CurrentVal <= 0;
        }
    }


    //private Animator enemyAnim;

    // Use this for initialization
    public override void Start () {
        base.Start();
        ChangeState(new IdleState());
        Player.Instance.Dead += new DeadEventHandler(RemoveTarget);
        MyAnimator = GetComponent<Animator>();

	}
	// Update is called once per frame
	void Update () {

        if (!IsDead)
        {
            if (!TakingDamage)
            {
                currentState.Execute();
            }

            LookAtTarget();
        }
	}

    private void LookAtTarget()
    {
        if (Target != null)
        {
            float xDir = Target.transform.position.x - transform.position.x;
            if (xDir < 0 && facingRight || xDir > 0 && !facingRight)
            {
                ChangeDirection();
            }
        }
    }

    public void RemoveTarget()
    {
        Target = null;
        ChangeState(new PatrolState());
    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);
    }

    private void HandleMovement(float horizontal)
    {
        if (MyRigidbody.velocity.y < 0)
        {
            MyAnimator.SetBool("land", true);
        }

        if (!Attack && !Slide /*&& (OnGround || airControl)*/)
        {
            MyRigidbody.velocity = new Vector2(horizontal * movementSpeed, MyRigidbody.velocity.y);

        }


        if (Jump && MyRigidbody.velocity.y == 0)
        {
            MyRigidbody.AddForce(new Vector2(0, jumpForce));
        }

        MyAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            //Destroy(gameObject);

            // Destroy bullet when hit
            Destroy(collision.gameObject);
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.tag == "Player")
        //{
        //    //MyAnimator.SetTrigger("attack");
        //    gameObject.GetComponent<Animator>().SetTrigger("attack");
        //}

        //if (collision.tag == "Edge")
        //{
        //    ChangeDirection();
        //}
        base.OnTriggerEnter2D(collision);
        currentState.OnTriggerEnter(collision);
    }


    public void Move()
    {
        if (!Attack)
        {

            if ((GetDirection().x > 0 && transform.position.x < rightEdge.position.x) || (GetDirection().x < 0 && transform.position.x > leftEdge.position.x))
            {
                //MyAnimator.SetFloat("speed", 1);
                gameObject.GetComponent<Animator>().SetFloat("speed", 1);

                transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
            }

            else if (currentState is PatrolState)
            {
                ChangeDirection();
            }
           
        }
        
    }

    public Vector2 GetDirection()
    {
        return facingRight ? Vector2.right : Vector2.left;
    }

    public override void ShootPref(int value)
    {

        if (value == 0)
        {
            base.ShootPref(value);
        }
        else if (value == 1)
        {
            base.ShootPref(value);
        }

    }

    public override IEnumerator TakeDamage()
    {
        healthStat.CurrentVal -= 10;
        if (!IsDead)
        {
            MyAnimator.SetTrigger("damage");
        }
        else
        {
            MyAnimator.SetTrigger("die");
            yield return null;
        }
    }

    public override void Death()
    {
        MyAnimator.SetTrigger("idle");
        MyAnimator.ResetTrigger("die");
        healthStat.CurrentVal = healthStat.MaxValue;
        transform.position = startPos;
    }
}
