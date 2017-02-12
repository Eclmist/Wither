using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadSceneOnDeath : MonoBehaviour
{
	private bool running;

	// Update is called once per frame
	void Update () {
		if (Player.Instance.isDead && !running)
		{
			running = true;
			FadeToBlack.Instance.QueueNextOpacity(0);
			BlurCameraOverTime.Instance.BlurScreen();
			Chronos.LateExecute(LoadScene.Instance.ReloadScene,5);
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

		GameObject[] enemiesToEnable = GameObject.FindGameObjectsWithTag("Enemy");

		foreach (GameObject enemy in enemiesToEnable)
		{
			IDamagable damagableComponent = enemy.GetComponent<IDamagable>();

			if (damagableComponent != null)
			{
				damagableComponent.TakeDamage(99999);
			}

			yield return new WaitForSeconds(0.2F);
		}
	}


}
