﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elemental : EnemyFSM, IDamagable {

	[Range(10,40)]
	public float distanceToAvoidance;
	public float force = 10.0f;
	public float minimumDistToAvoid = 5.0f;
	[Range(1,100)]
	public float moveSpeed;

	[Range(10,30)]
	public float attackRange;

	public GameObject castPoint;


	// Animation flags
	private bool isChasing;
	private bool isIdle;
	private bool isAttacking;
	private bool isDead;

	// Determine when to use AStar calculations
	private bool isUsingAStar;

    private GameObject ElementalProjectile;
    private GameObject ElementalDeath;

    private PathAgent pathAgent;
	private Animator animator;
	private Rigidbody rigidBody;
	private float health = 70;
	private bool stunned;

	[SerializeField]
	private AnimationCurve turnRateOverAngle;

	public void TakeDamage(float damage)
	{
		health -= damage;
	}

	public void ApplyStun(float duration)
	{
		StopAllCoroutines();
		StartCoroutine(ApplyStatusEffect(duration));
	}
		
	public IEnumerator ApplyStatusEffect(float duration)
	{
		currentState = FSMState.Stun;
		stunned = true;
		yield return new WaitForSeconds(duration);
		animator.enabled = true;
		stunned = true;
	}

	protected override void Initialize()
	{
		base.Initialize();

        ElementalProjectile = Resources.Load("ElementalProjectile") as GameObject;
        ElementalDeath = Resources.Load("ElementalDeath") as GameObject;

        pathAgent = GetComponent<PathAgent>();
		animator = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();
		player = GameObject.FindWithTag("Player");
		isUsingAStar = true;
		currentState = FSMState.Chase;

	}

	protected override void FSMUpdate()
	{
		base.FSMUpdate();
		if (Vector3.Distance(transform.position, player.transform.position) <= distanceToAvoidance)
			isUsingAStar = false;

		if (health <= 0)
		{
            if(ElementalDeath != null)
			Instantiate(ElementalDeath,transform.position,transform.rotation);
			Destroy(gameObject);
		}

		if(Input.GetKeyDown(KeyCode.I))
		{
			health -= 50;
		}
			
		
	}

	protected override void HandleAnimations()
	{
		animator.SetBool("isAttacking",isAttacking);
	}

	protected override void UpdateAttackState()
	{
		isAttacking = true;
		rigidBody.velocity = Vector3.zero;
		LookAtPlayer();

		if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
		{
			currentState = FSMState.Chase;
			isAttacking = false;
		}


	}

	protected override void UpdateStunState()
	{
		if (!stunned)
			currentState = FSMState.Chase;
	}

	protected override void UpdateChaseState()
	{

		if (isUsingAStar)
		{
			if (pathAgent != null)
				pathAgent.enabled = true;
			else
				Debug.Log("Elemental does not have a PathAgent");
		}
		else
		{
			if(pathAgent != null)
			pathAgent.enabled = false;

			// use simple chasing with avoidance
			LookAtPlayer();
			
			rigidBody.velocity = transform.forward * moveSpeed;

			if(Vector3.Distance(transform.position, player.transform.position) <= attackRange)
			{
				currentState = FSMState.Attack;
			}
			
			
		}


	}

	private void LookAtPlayer()
	{

			//Quaternion rot = Quaternion.LookRotation(AvoidObstacles());
			//transform.rotation = Quaternion.Slerp(transform.rotation, rot, 2.0f * Time.deltaTime);

		Vector3 tempTarget = player.transform.position;

		float distance = (tempTarget - transform.position).magnitude / 60;
		float angle = Vector3.Angle(tempTarget - transform.position, transform.forward) / 180;
		float rotationSpeed = turnRateOverAngle.Evaluate(angle) / 10;
		//rotationSpeed += turnRateOverDistance.Evaluate((hit.point - transform.position).magnitude / 60);

		Vector3 lookAtTarget = AvoidObstacles();

		transform.rotation = Quaternion.Slerp(transform.rotation,
			Quaternion.LookRotation(lookAtTarget, Vector3.up),
			rotationSpeed);

		//Vector3 targetPosCurrFrame = transform.position + transform.forward;
		//transform.position = Vector3.Lerp(transform.position, targetPosCurrFrame, moveSpeed);

	}


	protected override void UpdateDeadState()
	{
		
	}
	protected override void UpdateIdleState()
	{
		pathAgent.enabled = false;
	}


	public Vector3 AvoidObstacles()
	{
		RaycastHit Hit;
		//Check that the vehicle hit with the obstacles within it's minimum distance to avoid
		Vector3 right45 = (transform.forward + transform.right).normalized;
		Vector3 left45 = (transform.forward - transform.right).normalized;

		if (Physics.Raycast(transform.position, right45, out Hit,
			minimumDistToAvoid))
		{

			//Get the new directional vector by adding force to vehicle's current forward vector
			return transform.forward - transform.right * force;
		}
		else if (Physics.Raycast(transform.position, left45, out Hit,
			minimumDistToAvoid))
		{

			//Get the new directional vector by adding force to vehicle's current forward vector
			return transform.forward + transform.right * force;
		}
		else if (Physics.Raycast(transform.position, transform.forward, out Hit,
			minimumDistToAvoid))
		{

			//Get the normal of the hit point to calculate the new direction
			Vector3 hitNormal = Hit.normal;
			hitNormal.y = 0.0f; //Don't want to move in Y-Space
			return transform.forward + hitNormal * force;
		}
		else
			return player.transform.position - transform.position;
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * minimumDistToAvoid/2);
		Gizmos.DrawLine(transform.position, transform.position + (transform.forward + transform.right) * minimumDistToAvoid);
		Gizmos.DrawLine(transform.position, transform.position + (transform.forward - transform.right) * minimumDistToAvoid);
	}

	//-----------------------------Animation Events-----------------------------------------//
	public void ShootProjectile()
	{
        if (ElementalDeath != null)
            Instantiate(ElementalProjectile, transform.position, transform.rotation);
       
	}
}
