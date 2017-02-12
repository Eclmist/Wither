using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEvent : MonoBehaviour
{
	[SerializeField] private Morbius boss;

	public void EndCinematic()
	{
		GetComponent<Animator>().enabled = false;
		boss.enabled = true;
		FadeToBlack.Instance.opacityAddition = 0;
	}
}
