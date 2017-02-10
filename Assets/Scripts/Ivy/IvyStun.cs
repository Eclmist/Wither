using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvyStun : MonoBehaviour
{

	[Range(0,10)] public float range = 5;
	[Range(0, 2)] public float stunDuration = 1;
	[Range(0, 5)] public float cooldown = 1;

	private float timePassedSinceLastStun;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		timePassedSinceLastStun += Time.deltaTime;

		if (timePassedSinceLastStun > cooldown)
		{
			timePassedSinceLastStun = 0;

			if (Input.GetKeyDown(KeyCode.E))
			{
				//StartCoroutine(StunSphereAnimation);
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.3F, 0.2F, 0.5F, 0.2F);
		Gizmos.DrawSphere(transform.position, range);
	}

//	IEnumerator StunSphereAnimation()
	//{
		//while ()
//	}
}
