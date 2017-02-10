using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualInteractor : MonoBehaviour
{
	public Interactor interactorType;
	[Range(0,10)] public float range;

	public LayerMask objectLayers;

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
			InteractWithNearest();
	}

	void InteractWithNearest()
	{
		IInteractable nearest = GetNearestObjectWithinRange();

		if (nearest != null)
		{
			if (nearest.CanInteractWith(interactorType));
			{
				nearest.Interact();
			}
		}
	}


	IInteractable GetNearestObjectWithinRange()
	{

		IInteractable nearestInteractable = null;
		float nearestDist = float.MaxValue;

		Collider[] objects = Physics.OverlapSphere(transform.position, range, objectLayers);

		for (int i = 0; i < objects.Length; i++)
		{
			IInteractable current = objects[i].GetComponent<IInteractable>();

			if (current != null)
			{
				// Calculate current distance between object and self
				float currentDist =
					Vector3.Distance(transform.position, objects[i].transform.position);

				// Update the nearest object
				if (currentDist < nearestDist)
				{
					nearestDist = currentDist;
					nearestInteractable = current;
				}
			}

		}

		return nearestInteractable;
	}



	// For debugging purposes
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, range);
	}

}
