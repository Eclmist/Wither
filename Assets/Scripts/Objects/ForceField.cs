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

		collider = GetComponent<Collider>();

		messageBox = GetComponentInChildren<Message>().gameObject;

		SetObstacleActive(true);
	}

	protected override void SetObstacleActive(bool active)
	{
		base.SetObstacleActive(active);
		ToggleParticles(active);

		animator.enabled = !active;
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
