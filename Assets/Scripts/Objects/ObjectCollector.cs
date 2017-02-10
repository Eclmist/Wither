using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollector : MonoBehaviour
{

	void OnTriggerEnter(Collider other)
	{
		ICollectable obj = other.GetComponent<ICollectable>();
		if (obj != null)
		{
			obj.Collect();
		}
	}
}
