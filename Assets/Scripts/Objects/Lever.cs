using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
	private bool isInteracted;

	public bool GetIsInteracted()
	{
		return isInteracted;
	}

	public void Interact()
	{
		if (!isInteracted)
		{
			GetComponent<Animator>().enabled = true;
			isInteracted = true;
		}
	}

	public void Pulse()
	{
		throw new NotImplementedException();
	}

	public void SetOpacity(float opacity)
	{
		throw new NotImplementedException();
	}

	public bool CanInteractWith(Interactor from)
	{
		switch (from)
		{
			case Interactor.Player:
				return true;
			case Interactor.Ivy:
				return false;
			default:
				throw new ArgumentOutOfRangeException("from", @from, null);
		}
	}

}
