using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public float speed = 1;

	[Tooltip("0 = completely follow target 1, 1 = completely follow target 2")]
	[Range(0, 1)] public float weight;

	public Transform[] target;

	private Vector3 offset;
	private Vector3 shake;

	// Use this for initialization
	void Start ()
	{
		offset = - target[0].transform.position + transform.position;
	}

	void FixedUpdate()
	{
		Vector3 targetPos = target[0].position * (1-weight);

		targetPos += target[1].position*weight;

		//transform.position = targetPos + offset ;
		transform.position = Vector3.Lerp(transform.position, targetPos + offset + shake,
			Chronos.BetaTime * speed);
	}

	public void SetShakeVector(Vector3 amount)
	{
		shake = amount;
	}
}
