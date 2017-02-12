using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memento : MonoBehaviour, ICollectable
{
	public Texture currentTex;

	public void Collect()
	{
		BlurCameraOverTime.Instance.BlurScreen();
		Chronos.PauseTime(0.05F);
		Chronos.LateExecute(MementoManager.ShowMemento, 0.6F);
		MementoManager.IncrementPickupCount();
		MementoManager.Instance.StartCoroutine(MementoManager.Instance.TriggerMemento(true));

		foreach (ParticleToggle p in GetComponentsInChildren<ParticleToggle>())
		{
			p.ToggleParticles(false);
		}

		Destroy(gameObject, 0.1F);

	}

	public static void ShowMemento()
	{
		BlurCameraOverTime.Instance.BlurScreen();
		Chronos.PauseTime(0.05F);
		Chronos.LateExecute(MementoManager.ShowMemento, 0.6F);
		MementoManager.Instance.StartCoroutine(MementoManager.Instance.TriggerMemento(true));
	}
}
