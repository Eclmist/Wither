﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elemental : EnemyFSM, IDamagable, IStunnable
{

	public LayerMask avoidanceIgnoreMask;

	[Range(10,40)]
	public float distanceToAvoidance;
	public float force = 10.0f;
	public float minimumDistToAvoid = 5.0f;
	[Range(1,100)]
	public float moveSpeed;

	[Range(0,30)]
	public float attackRange;

	public GameObject castPoint;
    public AudioClip attackSound;
	public AudioClip spawnSound;

	// Animation flags
	private bool isChasing;
	private bool isIdle;
	private bool isAttacking;
	private bool isDead;

	// Determine when to use AStar calculations
	public bool isUsingAStar;

	private GameObject ElementalProjectile;
	private GameObject ElementalDeath;

	private PathAgent pathAgent;
	private Animator animator;
	private Rigidbody rigidBody;
	private int health = 20;
	private bool stunned;
	private Renderer renderer;
	private Vector3 lastPlayerPos;

	[SerializeField]
	private AnimationCurve turnRateOverAngle;

	public void TakeDamage(int damage)
	{
		health -= damage;
	}
		
	public IEnumerator ApplyStatusEffect(float duration)
	{
		currentState = FSMState.Stun;
		stunned = true;
		animator.enabled = false;
		yield return new WaitForSeconds(duration);
		animator.enabled = true;
		stunned = false;
	}

	public IEnumerator FadeOpacity(bool show)
	{
		float currentOpacity = renderer.material.GetFloat("_Opacity");

		if (show)
		{
			while (currentOpacity <= 1)
			{
				currentOpacity += 0.05f;
				renderer.material.SetFloat("_Opacity", currentOpacity);
				yield return new WaitForSeconds(0.02F);
			}

			currentOpacity = 1;
			renderer.material.SetFloat("_Opacity", currentOpacity);
		}
		else
		{
			while (currentOpacity >= 0)
			{
				currentOpacity -= 0.1f;
				renderer.material.SetFloat("_Opacity", currentOpacity);
				yield return new WaitForSeconds(0.01F);
			}

			currentOpacity = 0;
			renderer.material.SetFloat("_Opacity", currentOpacity);
			Die();
		}

	}

	void Die()
	{

		ParticleSystem.EmissionModule em =
			GetComponentInChildren<ParticleToggle>().GetComponent<ParticleSystem>().emission;

		em.enabled = false;
		GetComponentInChildren<ParticleToggle>().gameObject.transform.parent = null;

		Destroy(gameObject);
	}

	protected override void Initialize()
	{
		base.Initialize();

		if (spawnSound)
			AudioManager.Instance.PlaySound(spawnSound, gameObject);

		ElementalProjectile = Resources.Load<GameObject>("Ice_Wave");
		ElementalDeath = Resources.Load<GameObject>("Dark_Explosion");

		pathAgent = GetComponent<PathAgent>();
		animator = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();
		player = GameObject.FindWithTag("Player");
		currentState = FSMState.Chase;

		renderer = GetComponentInChildren<Renderer>();
		StartCoroutine(FadeOpacity(true));

	}

	protected override void FSMUpdate()
	{
		base.FSMUpdate();

        if (Vector3.Distance(transform.position, player.transform.position) <= distanceToAvoidance)
            isUsingAStar = false;
        

        if (health <= 0 && !isDead)
		{
			isDead = true;

			currentState = FSMState.Dead;

			if(ElementalDeath != null)
				Instantiate(ElementalDeath,transform.position,transform.rotation);
			StartCoroutine(FadeOpacity(false));
		}

		if(Input.GetKeyDown(KeyCode.I))
		{
			health -= 50;
		}
			
		
	}

	protected override void HandleAnimations()
	{
		animator.SetBool("isAttacking",isAttacking);
        animator.SetBool("isDying",isDead);
	}

	protected override void UpdateAttackState()
	{
		isAttacking = true;
		rigidBody.velocity = Vector3.zero;
		transform.rotation = Quaternion.Slerp(transform.rotation,
			Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up),
			30 * Time.deltaTime);

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
			
			rigidBody.MovePosition(transform.position + transform.forward * moveSpeed / 100);
		}

        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            currentState = FSMState.Attack;
        }


    }

	private void LookAtPlayer()
	{

		Vector3 tempTarget = player.transform.position;

		float distance = (tempTarget - transform.position).magnitude / 60;
		float angle = Vector3.Angle(tempTarget - transform.position, transform.forward) / 180;
		float rotationSpeed = turnRateOverAngle.Evaluate(angle);


		Vector3 lookAtTarget = AvoidObstacles();
		lookAtTarget.y = 0; //Force no y change;
		transform.rotation = Quaternion.Slerp(transform.rotation,
			Quaternion.LookRotation(lookAtTarget, Vector3.up),
			rotationSpeed * Time.deltaTime);

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
			minimumDistToAvoid, ~avoidanceIgnoreMask))
		{
			// 0 if near, 1 if far
			float distanceExp = Vector3.Distance(Hit.point, transform.position) / minimumDistToAvoid;
			// 5 if near, 0 if far
			distanceExp = 5 - distanceExp * 5;
			
			return transform.forward - transform.right * force * distanceExp;
		}
		else if (Physics.Raycast(transform.position, left45, out Hit,
			minimumDistToAvoid, ~avoidanceIgnoreMask))
		{

			// 0 if near, 1 if far
			float distanceExp = Vector3.Distance(Hit.point, transform.position) / minimumDistToAvoid;
			// 5 if near, 0 if far
			distanceExp = 5 - distanceExp * 5;

			return transform.forward + transform.right * force * distanceExp;
		}

		else
		{
			//Debug.Log("Turned towards player");
			Debug.DrawLine(transform.position, player.transform.position, Color.blue);
			return player.transform.position - transform.position;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * minimumDistToAvoid/3);
		Gizmos.DrawLine(transform.position, transform.position + (transform.forward + transform.right) * minimumDistToAvoid);
		Gizmos.DrawLine(transform.position, transform.position + (transform.forward - transform.right) * minimumDistToAvoid);
	}

	//-----------------------------Animation Events-----------------------------------------//
	public void ShootProjectile()
	{
		if (ElementalDeath != null)

		{
			Quaternion rotation =
				Quaternion.LookRotation(lastPlayerPos - 
				castPoint.transform.position);

			Instantiate(ElementalProjectile, castPoint.transform.position, rotation);
		}
	}

	public void RecalculatePlayerPosForAiming()
	{
		lastPlayerPos = player.transform.position + Vector3.up;
	}

	public void Stun(float duration)
	{
		Debug.Log("Stunned");
		StopCoroutine("ApplyStatusEffect");
		StartCoroutine(ApplyStatusEffect(duration));
	}
}
