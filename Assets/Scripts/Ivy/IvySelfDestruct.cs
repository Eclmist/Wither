using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class IvySelfDestruct : MonoBehaviour
{
	public float speed;

	private Transform originalCameraTarget;
	private float originalXoffset;

	void Start ()
	{
		DialogManager.dialogManager.LoadConversationByIndex(15);
		PauseScreen.Instance.enabled = false;
		BlurCameraOverTime.Instance.BlurScreen();
		Chronos.PauseTime(0.05F);
		DialogManager.dialogManager.SetCallbackFunc(AfterDeathDialog);
		Chronos.LateExecute(DialogManager.dialogManager.ShowDialogBox, 0.6F);
	}

	void AfterDeathDialog()
	{
		PauseScreen.Instance.enabled = true;
		BlurCameraOverTime.Instance.UnblurScreen();
		Chronos.ResumeTime(0.05F);

		GetComponent<IvyController>().enabled = false;

		IvyStun stun = GetComponent<IvyStun>();

		stun.allowInput = false;
		stun.range = 25;
		stun.StartCoroutine("StunSphereAnimation");

		originalCameraTarget = TP_Camera.Instance.target;
		originalXoffset = TP_Camera.Instance.desiredXOffset;
		TP_Camera.Instance.target = transform;
		TP_Camera.Instance.desiredXOffset = 0;
		TP_Camera.Instance.useDamping = true;

		GameObject.FindObjectOfType<Morbius>().ForceKill();

		StartCoroutine(DoPulse());
		StartCoroutine(KillAllEnemies());

	}

	IEnumerator DoPulse()
	{
		float expGrowRate = 0;

		WaitForFixedUpdate wait = new WaitForFixedUpdate();

		float currentDistance = 0;

		Mask_Clear.Instance.SetPulsePosition(transform.position);
		SSDistortion.Instance.distortionMultiplier = 0;

		Vector3 pulsePosition = transform.position;

		while (currentDistance < 50)
		{
			currentDistance += expGrowRate;
			expGrowRate += speed;
			Mask_Clear.Instance.SetPulse(currentDistance, 1);
			SSDistortion.Instance.radius = currentDistance;
			SSDistortion.Instance.distortionMultiplier = Mathf.Sin(currentDistance);

			yield return wait;
		}

		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		Camera.main.GetComponent<ScreenSpacedLensFlares>().enabled = false;


		yield return new WaitForSeconds(2);


		TP_Camera.Instance.target = originalCameraTarget;
		TP_Camera.Instance.desiredXOffset = originalXoffset;
		TP_Camera.Instance.useDamping = false;

	}

	IEnumerator KillAllEnemies()
	{
		yield return new WaitForSeconds(0.1F);

		Elemental[] enemiesToEnable = FindObjectsOfType<Elemental>();

		foreach (Elemental enemy in enemiesToEnable)
		{
			IDamagable damagableComponent = enemy.GetComponent<IDamagable>();

			if (damagableComponent != null)
			{
				damagableComponent.TakeDamage(99999);
			}

			yield return new WaitForSeconds(0.1F);
		}
	}

}
