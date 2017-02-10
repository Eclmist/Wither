using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : Obstacle
{
	private Animator animator;
	private Collider collider;
	private GameObject messageBox;
	[SerializeField] private ParticleToggle[] particles;

	protected override void ObstacleStart()
	{
		base.ObstacleStart();

		animator = GetComponent<Animator>();
		animator.speed = 1;

		collider = GetComponent<Collider>();
		collider.enabled = true;

		messageBox = GetComponentInChildren<Message>().gameObject;
		messageBox.SetActive(true);
		ToggleParticles(true);
	}

	protected override void SetObstacleActive(bool active)
	{
		base.SetObstacleActive(active);
		ToggleParticles(active);

		animator.speed = active ? 1 : 0;
		collider.enabled = active;

		messageBox.SetActive(active);
	}

	private void ToggleParticles(bool active)
	{
		foreach (ParticleToggle particle in particles)
		{
			particle.ToggleParticles(active);
		}

	}
}
