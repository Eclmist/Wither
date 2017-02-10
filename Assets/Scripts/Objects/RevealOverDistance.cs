using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealOverDistance : MonoBehaviour
{
	public float distance;

	public bool autoInteract = true;
	public float autoInteractDistance;

	public LayerMask layers;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		Collider[] interractables = Physics.OverlapSphere(transform.position, distance + 1, layers); 
		// distance is offset such that objects dont get stuck at 0.01 opacity

		foreach (Collider c in interractables)
		{
			IInteractable interactable = c.GetComponent<IInteractable>();

			if (interactable != null)
			{
				float actualDistance = 
					Vector3.Distance(c.transform.position, transform.position);

				if (autoInteract && actualDistance < autoInteractDistance)
				{
					interactable.Interact();
				}

				float distanceRatio = actualDistance / distance;
				float opacity = 1 - distanceRatio;
				interactable.SetOpacity(opacity);
			}
		}
	}


	void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.3F, 0.4F, 0.6F, 0.3F);
		Gizmos.DrawSphere(transform.position, distance);
	}
}
