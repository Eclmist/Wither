using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleSeek : MonoBehaviour
{
	public Transform target;
	public float force = 10.0f;

	private ParticleSystem ps;

	void Start ()
	{
		ps = GetComponent<ParticleSystem>();
	}
	
	void LateUpdate ()
	{
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
		ps.GetParticles(particles);


		for (int i = 0; i < particles.Length; i++)
		{
			ParticleSystem.Particle p = particles[i];


			Vector3 particleWorldPosition;

			if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
			{
				particleWorldPosition = transform.TransformPoint(p.position);
			}
			else if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
			{
				particleWorldPosition = ps.main.customSimulationSpace.TransformPoint(p.position);
			}
			else
			{
				particleWorldPosition = p.position;
			}

			Vector3 directionToTarget = (target.position - particleWorldPosition).normalized;

			Vector3 seekForce = (directionToTarget*force)*Time.deltaTime;

			p.velocity += seekForce;

			if (Vector3.Distance(p.position, target.position) < 0.05f)
			{
				p.velocity = Vector3.zero;
				p.position = target.position;
			}
			particles[i] = p;
		}

		ps.SetParticles(particles, particles.Length);
	}
}
