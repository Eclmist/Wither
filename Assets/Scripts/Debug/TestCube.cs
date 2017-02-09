using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube :MonoBehaviour, IInteractable
{

	public void Interact()
	{
		Debug.Log("Interact() called from IInteractable");
	}

	public void Pulse()
	{
		throw new NotImplementedException();
	}
	public void SetOpacity(float t)
	{
		throw new NotImplementedException();
	}

}
