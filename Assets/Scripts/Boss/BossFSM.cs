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
        Spawn,
        Chase,
        Attack,
        Attack2,
        Ultimate,
        Dead,
    }

    protected FSMState currentState;


    protected override void Initialize()
    {
        currentState = FSMState.Spawn;
    }

    protected override void FSMUpdate()
    {

        HandleAnimations();

        switch (currentState)
        {
            case FSMState.Spawn:
                UpdateSpawnState();
                break;
            case FSMState.Chase:
                UpdateChaseState();
                break;
            case FSMState.Attack:
                UpdateAttackState();
                break;
            case FSMState.Attack2:
                UpdateAttack2State();
                break;
            case FSMState.Ultimate:
                UpdateUltimateState();
                break;
            case FSMState.Dead:
                UpdateDeadState();
                break;
        }

    }

    protected virtual void UpdateSpawnState() { }
    protected virtual void UpdateChaseState() { }
    protected virtual void UpdateAttackState() { }
    protected virtual void UpdateAttack2State() { }
    protected virtual void UpdateUltimateState() { }
    protected virtual void UpdateDeadState() { }
    
}
