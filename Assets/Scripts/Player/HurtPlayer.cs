using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayer : MonoBehaviour
{

	public int damage;
	public bool destroySelf;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Player>())
		{
			Player.Instance.TakeDamage(damage);

			if (destroySelf)
				Destroy(gameObject);
		}
	}
}
