using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level00 : MonoBehaviour
{

	private bool playerDeathEventTriggered;
	private bool enemySpawnEventTriggered;

	[SerializeField] private GameObject[] enemiesToEnable;
 
	// Update is called once per frame
	void Update ()
	{
		if (!playerDeathEventTriggered && Player.Instance.isDead)
		{
			playerDeathEventTriggered = true;
			FadeToBlack.Instance.QueueNextOpacity(0);
			StartCoroutine(KillAllEnemies());

			Chronos.LateExecute(LoadLevel01, 5);
		}
	}

	void LoadLevel01()
	{
		LoadScene.Instance.Load("Level01");
	}

	IEnumerator KillAllEnemies()
	{
		yield return new WaitForSeconds(0.8F);

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

	IEnumerator SpawnAllEnemies()
	{

		foreach (GameObject enemy in enemiesToEnable)
		{
			enemy.SetActive(true);
			yield return new WaitForSeconds(0.2F);
		}
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			enemySpawnEventTriggered = true;
			StartCoroutine(SpawnAllEnemies());

		}
	}
}
