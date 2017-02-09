using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for every object/pickup that needs to be interacted with

public interface IInteractable
{

	void Interact();

	void Pulse();

	void SetOpacity(float opacity);
}
