using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level03 : MonoBehaviour
{
	[SerializeField] private Animator cinematicCamera;
	private bool bossEventFlag;
	[SerializeField] private Morbius boss;
	private bool playerDeathEventTriggered;

	private bool playerPlannedDeathFlag;

	[SerializeField] private TreeFall tree;

	[SerializeField] private IvySelfDestruct ivySelfDestructScript;

	// Update is called once per frame
	void Update()
	{
		if (boss.currentAttackStance == Morbius.AttackStance.BROKEN)
		{
			if (!playerPlannedDeathFlag && Player.Instance.isDead)
			{
				playerPlannedDeathFlag = true;

				ivySelfDestructScript.enabled = true;
			}
		}
		else if (Player.Instance.isDead && !bossEventFlag)
		{
			playerDeathEventTriggered = true;
			FadeToBlack.Instance.QueueNextOpacity(0);
			BlurCameraOverTime.Instance.BlurScreen();
			Chronos.LateExecute(LoadScene.Instance.ReloadScene, 5);
			Chronos.LateExecute(KillAllEnemiesCoroutine, 1);
		}
	}

	void KillAllEnemiesCoroutine()
	{
		StartCoroutine(KillAllEnemies());
	}


	IEnumerator KillAllEnemies()
	{
		yield return new WaitForSeconds(0.8F);

		Elemental[] enemiesToEnable = FindObjectsOfType<Elemental>();

		foreach (Elemental enemy in enemiesToEnable)
		{
			IDamagable damagableComponent = enemy.GetComponent<IDamagable>();

			if (damagableComponent != null)
			{
				damagableComponent.TakeDamage(99999);
			}

			yield return new WaitForSeconds(0.2F);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !bossEventFlag)
		{
			tree.CollapseTree();
			bossEventFlag = true;
			cinematicCamera.enabled = true;
		}
	}
}
