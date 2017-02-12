using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlLock : MonoBehaviour
{

	[SerializeField] private PlayerController player;
	[SerializeField] private TP_Camera camera;

	[SerializeField]
	private bool disableOnStart = true;

	void Start()
	{
		if (disableOnStart)
		{
			DisableControls();
		}
	}

	void DisableControls()
	{
		player.enabled = false;
		camera.enabled = false;
	}

	void EnableControls()
	{
		player.enabled = true;
		camera.enabled = true;
		camera.GetComponent<Animator>().enabled = false;
	}

}
