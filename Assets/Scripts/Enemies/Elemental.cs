using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elemental : EnemyFSM, IDamagable {

    [Range(1,100)]
    public float distanceToChaseMode;
    public float force = 10.0f;
    public float minimumDistToAvoid = 5.0f;

    [Range(1,10)]
    public float attackRange;


    // Animation flags
    private bool isChasing;
    private bool isIdle;
    private bool isAttacking;
    private bool isDead;

    // Determine when to use AStar calculations
    private bool isUsingAStar;

    private PathAgent pathAgent;
    private Animator animator;
    private float health;


    public void TakeDamage(float damage)
    {
       
    }

    protected override void Initialize()
    {
        base.Initialize();
        pathAgent = GetComponent<PathAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        isUsingAStar = false;
        currentState = FSMState.Chase;

    }

    protected override void FSMUpdate()
    {
        base.FSMUpdate();
        if (Vector3.Distance(transform.position, player.transform.position) <= distanceToChaseMode)
            isUsingAStar = false;
        
    }

    protected override void HandleAnimations()
    {
        
    }

    protected override void UpdateAttackState()
    {

        if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
        {
            currentState = FSMState.Chase;
        }


    }

    protected override void UpdateChaseState()
    {

        if (isUsingAStar)
        {
            if (pathAgent != null)
            {
                pathAgent.enabled = true;
            }
        }
        else
        {
            pathAgent.StopAllInstances();
            pathAgent.enabled = false;
            //transform.LookAt(player.transform.position);
            // use simple chasing with avoidance
            Quaternion rot = Quaternion.LookRotation(AvoidObstacles());
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 2.0f * Time.deltaTime);
            transform.position += transform.forward * 0.07f;

            if(Vector3.Distance(transform.position, player.transform.position) <= attackRange)
            {
                currentState = FSMState.Attack;
            }
            
            
        }


    }
    protected override void UpdateDeadState()
    {
        
    }
    protected override void UpdateIdleState()
    {
        pathAgent.enabled = false;
    }

    private Vector3 AvoidObstacles()
    {
        RaycastHit Hit;
        
        Vector3 right45 = (transform.forward + transform.right).normalized;
        Vector3 left45 = (transform.forward - transform.right).normalized;
        Vector3 startPos = transform.position + transform.up * 1;
        if (Physics.Raycast(startPos, right45, out Hit, minimumDistToAvoid))
        {
            
            return transform.forward - transform.right * force;
        }
        else if (Physics.Raycast(startPos, left45, out Hit, minimumDistToAvoid))
        {
           
            return transform.forward + transform.right * force;
        }
        else if (Physics.Raycast(startPos, transform.forward, out Hit, minimumDistToAvoid))
        {
            
            Vector3 hitNormal = Hit.normal;
            hitNormal.y = 0.0f; 
            return transform.forward + hitNormal * force;
        }
        else
            return player.transform.position - transform.position;

    }



}
