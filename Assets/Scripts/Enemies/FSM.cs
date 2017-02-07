using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is the base class every Ai inherits from.
/// every Ai that inherits the Finite State Machine can be(and should be) animated
/// even if the child is not an enemy.
/// </summary>
public class FSM : MonoBehaviour{

    protected GameObject player;

    // Called on Start()
    protected virtual void Initialize() { }
    // Called every frame
    protected virtual void FSMUpdate() { }
    // Handles every animation
    protected virtual void HandleAnimations() { }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        FSMUpdate();
        HandleAnimations();
    }

}
