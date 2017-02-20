﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{

    //protected Animator myAnimator;

    [SerializeField]
    protected Transform[] groundPoints;
    protected float groundRadius = 0.2f;
    public LayerMask whatIsGround;

    [SerializeField]
    protected Transform bulletPos;

    [SerializeField]
    protected float movementSpeed;

    protected bool facingRight;

    [SerializeField]
    protected GameObject BulletPref;

    protected Animator myAnimator;

    public bool Attack { get; set; }

    public bool Slide { get; set; }

    public bool Jump { get; set; }


    public bool Shoot { get; set; }

    protected float jumpForce = 550;





    // Use this for initialization
    public virtual void Start()
    {
        facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeDirection()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }

    public virtual void ShootPref(int value)
    {

        if (facingRight)
        {
            GameObject tmp = (GameObject)Instantiate(BulletPref, bulletPos.position, Quaternion.Euler(new Vector3(0, 0, 90)));
            tmp.GetComponent<Bullet>().Initialize(Vector2.right);
        }

        else
        {
            GameObject tmp = (GameObject)Instantiate(BulletPref, bulletPos.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            tmp.GetComponent<Bullet>().Initialize(Vector2.left);
        }
    }
}