using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent (typeof (LineRenderer))]
public class IvyAttackSpellDefault : MonoBehaviour {

	[SerializeField] private bool skillActive = true;
	[SerializeField] private int arcPoints = 10;
	[SerializeField] private float curveFactor = 60;
	[SerializeField] private float noiseFactor = 0;
	[SerializeField] private float range = 26;
	[SerializeField] private float durationPerHit = 0.5f;
	[SerializeField] private int targetIndex = 0;
	[SerializeField] private int damage = 1;
	[SerializeField] private LayerMask collisionMasks;

	private LineRenderer lineRenderer;
	private Vector3[] linePoints = new Vector3[10];
	private RaycastHit hitPoint;
	private float currentCurveFactor;
	private GameObject target;
	private float timeSinceLastStart = float.MaxValue;
	private ParticleToggle enemyHitParticles;

	public bool SkillActive
	{
		get { return skillActive; }
		set { skillActive = value; }
	}

	void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	void Update()
	{

		//Check if skill enabled
		if (skillActive)
		{
			if (timeSinceLastStart > durationPerHit || !target)
			{
				// Find new Enemy
				if (enemyHitParticles)
					enemyHitParticles.ToggleParticles(false);


				enemyHitParticles = null;

				target = null;


				//Find enemy within lightning range
				Collider[] enemyWithinRange = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Enemy"));

				//Stores targeted enemy information
				float shortestDistance = float.MaxValue;

				//Finds nearest viable target
				foreach (Collider enemy in enemyWithinRange)
				{
					float distanceFromPlayer = Vector3.Distance(enemy.transform.position, transform.position);

					if (distanceFromPlayer < shortestDistance)
					{
						RaycastHit intersection;
						Physics.Linecast(transform.position, enemy.transform.position, out intersection, ~collisionMasks);
						if (intersection.collider == enemy)
						{


							IDamagable enemyDamageComponent = enemy.GetComponent<IDamagable>();

							if (enemyDamageComponent == null)
								continue;

							//Found new target
							enemyDamageComponent.TakeDamage(damage);

							enemyHitParticles = enemy.GetComponentInChildren<ParticleToggle>();

							if (enemyHitParticles)
								enemyHitParticles.ToggleParticles(true);


							target = enemy.gameObject;
							currentCurveFactor = UnityEngine.Random.Range(-curveFactor, curveFactor);
							shortestDistance = distanceFromPlayer;
							timeSinceLastStart = 0;
						}
					}
				}

				ToggleLineRenderer();
			}

			//Draws lightning arc to target
			if (target != null)
			{
				UpdateMaterial();

				Vector3 center = target.transform.position;

				if ((BoxCollider) hitPoint.collider)
				{
					center = ((BoxCollider) hitPoint.collider).center;
				}
				else if ((SphereCollider)hitPoint.collider)
				{
					center = ((SphereCollider)hitPoint.collider).center;
				}
				else if ((CapsuleCollider) hitPoint.collider)
				{
					center = ((CapsuleCollider) hitPoint.collider).center;
				}

				DrawLightningArc(center);
			}
		}

		timeSinceLastStart += Time.deltaTime;
	}

	void ToggleLineRenderer()
	{
		lineRenderer.enabled = target;
	}

	void UpdateMaterial()
	{
		lineRenderer.material.SetTextureOffset("_MainTex", 
			new Vector2((-timeSinceLastStart/durationPerHit * 1.5f)+1, 0));

		// TODO: Remove+0.5 later;
	}

	void DrawLightningArc(Vector3 target)
	{
		lineRenderer.SetVertexCount(arcPoints);

		lineRenderer.SetPosition(0, transform.position);
		lineRenderer.SetPosition(arcPoints - 1, target);

		linePoints[0] = transform.position;
		linePoints[arcPoints - 1] = target;

		float distance = Vector3.Distance(target, transform.position);
		Vector3 angle = target - transform.position;
		angle = angle.normalized;
		angle = Quaternion.AngleAxis(currentCurveFactor, Vector3.up) * angle;

		for (int i = 1; i < arcPoints / 2; i++)
		{
			Vector3 originalPos = linePoints[0];
			originalPos += angle * i * distance / 10;
			lineRenderer.SetPosition(i, originalPos);
			linePoints[i] = originalPos;
		}

		for (int i = 1; i < arcPoints / 2; i++)
		{
			Vector3 originalPos = linePoints[arcPoints / 2 - 1];
			originalPos += ((target - originalPos) / (arcPoints / 2)) * i;
			lineRenderer.SetPosition(i - 1 + (arcPoints / 2), originalPos);
			linePoints[i + (arcPoints / 2)] = originalPos;
		}
			
		//Smoothing
		
		for (int i2 = 0; i2 < 4; i2++)
		{
			for (int i = 1; i < arcPoints - 1; i++)
			{
				Vector3 newPos = linePoints[i - 1] + ((linePoints[i + 1] - linePoints[i - 1]) / 2);
				linePoints[i] = newPos;
			}
		}

		AddNoise(distance);

	}

	void AddNoise(float distance)
	{
		//Adding noise
		for (int i = 1; i < arcPoints - 1; i++)
		{
			linePoints[i].x += UnityEngine.Random.Range(-noiseFactor, noiseFactor) * (distance / 10);
			linePoints[i].z += UnityEngine.Random.Range(-noiseFactor, noiseFactor) * (distance / 10);
			lineRenderer.SetPosition(i, linePoints[i]);
		}
	}
}
