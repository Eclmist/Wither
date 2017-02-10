using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Every boss will inherit this FSM
/// Bosses will have 3 variations of attack, Eg. Attack, Attack2, Ultimate
/// They have a spawn state because of spawning animations.
/// </summary>
public class BossFSM : FSM
{

    protected enum FSMState
    {
        Patrol,
        Overpowered,
        Chase,
        Attack,
        Turn,
        Dead,
    }

    protected FSMState currentState;


    protected override void Initialize()
    {
        currentState = FSMState.Patrol;
    }

    protected override void FSMUpdate()
    {
    
        HandleAnimations();

        switch (currentState)
        {
            case FSMState.Patrol:
                UpdatePatrolState();
                break;
            case FSMState.Overpowered:
                UpdateOverpoweredState();
                break;
            case FSMState.Chase:
                UpdateChaseState();
                break;
            case FSMState.Attack:
                UpdateAttackState();
                break;
            case FSMState.Turn:
                UpdateTurnState();
                break;
            case FSMState.Dead:
                UpdateDeadState();
                break;
        }

    }

    protected virtual void UpdatePatrolState() { }
    protected virtual void UpdateOverpoweredState() { }
    protected virtual void UpdateChaseState() { }
    protected virtual void UpdateAttackState() { }
    protected virtual void UpdateTurnState() { }
    protected virtual void UpdateDeadState() { }
    
}
