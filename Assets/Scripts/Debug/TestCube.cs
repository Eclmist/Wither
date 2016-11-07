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

}
