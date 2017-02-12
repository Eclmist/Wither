using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private GameObject mainCamera;

	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown)
		{
			mainCamera.GetComponent<Animator>().enabled = true;
			mainCamera.GetComponent<TP_Camera>().enabled = true;

			Destroy(gameObject);
		}
	}
}
