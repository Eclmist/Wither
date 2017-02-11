using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Obstacle
{
	private Animator animator;
	private GameObject messageBox;

	protected override void ObstacleStart()
	{
		base.ObstacleStart();

		animator = GetComponent<Animator>();

		messageBox = GetComponentInChildren<Message>().gameObject;

		SetObstacleActive(true);
	}

	protected override void SetObstacleActive(bool active)
	{
		base.SetObstacleActive(active);

		animator.enabled = !active;

		messageBox.SetActive(active);
	}

}
