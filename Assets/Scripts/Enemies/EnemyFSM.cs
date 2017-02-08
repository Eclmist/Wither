using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic enemies will inherit this FSM.
/// Basic enemies only have a single variation for attacking.
/// </summary>
public class EnemyFSM : FSM {

	protected enum FSMState
	{
		Idle,
		Chase,
		Attack,
		Stun,
		Dead,
	}

	protected FSMState currentState;

	protected override void Initialize()
	{
		currentState = FSMState.Idle;
	}

	protected override void FSMUpdate()
	{
		switch (currentState)
		{
			case FSMState.Idle:
				UpdateIdleState();
				break;
			case FSMState.Chase:
				UpdateChaseState();
				break;
			case FSMState.Attack:
				UpdateAttackState();
				break;
			case FSMState.Stun:
				UpdateAttackState();
				break;
			case FSMState.Dead:
				UpdateDeadState();
				break;
		}

	}

	protected virtual void UpdateIdleState() { }
	protected virtual void UpdateChaseState() { }
	protected virtual void UpdateAttackState() { }
	protected virtual void UpdateStunState() { }
	protected virtual void UpdateDeadState() { }
 
}
