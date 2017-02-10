using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
	private IInteractable[] childTriggers;
	private bool active = true;

	// Use this for initialization
	void Start ()
	{
		ObstacleStart();
	}
	
	// Update is called once per frame
	void Update ()
	{
		ObstacleUpdate();
	}

	protected virtual void ObstacleStart()
	{
		childTriggers = GetComponentsInChildren<IInteractable>();
	}

	protected virtual void ObstacleUpdate()
	{
		bool checkFailed = false;

		foreach (IInteractable child in childTriggers)
		{
			if (!child.GetIsInteracted())
			{
				//Some child triggers have still yet to be interacted;
				checkFailed = true;
				break;
			}
		}

		SetObstacleActive(checkFailed);
	}


	protected virtual void SetObstacleActive(bool active)
	{
		active = false;
	}

}
