using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState
{
    private Enemy enemy;
    private float patrolTimer;
    private float patrolDuration;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
        patrolDuration = UnityEngine.Random.Range(1, 10);
    }

    public void Execute()
    {
        Patrol();
        Debug.Log("Patroling");
        enemy.Move();

        if (enemy.Target != null && enemy.inShootRange)
        {
            enemy.ChangeState(new RangedState());
        }
    }
    public void Exit()
    {

    }

    public void OnTriggerEnter(Collider2D other)
    {
        //if(other.tag == "Edge")
        //{
        //    enemy.ChangeDirection();
        //}

        if (other.gameObject.tag == "Player")
        {
            //MyAnimator.SetTrigger("attack");
            enemy.MyAnimator.SetTrigger("attack");
            Debug.Log("Attacking");
        }

        if (other.tag == "Bullet" || other.tag =="Sword")
        {
            enemy.Target = Player.Instance.gameObject;
        }
    }

    private void Patrol()
    {

        patrolTimer += Time.deltaTime;

        if (patrolTimer >= patrolDuration)
        {
            enemy.ChangeState(new IdleState());
        }
    }
}
