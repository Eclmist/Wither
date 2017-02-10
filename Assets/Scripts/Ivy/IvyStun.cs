using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvyStun : MonoBehaviour
{
	public GameObject stunSphere;
	private Renderer stunSphereRenderer;

	[Range(0,10)] public float range = 5;
	[Range(0, 2)] public float stunDuration = 1;
	[Range(0, 5)] public float cooldown = 1;

	public AnimationCurve OpacityCurve;
	public AnimationCurve DistortionCurve;
	public AnimationCurve SizeCurve;

	private float timePassedSinceLastStun;

	// Use this for initialization
	void Start ()
	{
		stunSphereRenderer = stunSphere.GetComponent<Renderer>();

		stunSphereRenderer.material.SetFloat("_Opacity", 0);
		stunSphereRenderer.material.SetFloat("_DistortionAmt", 0);
		stunSphere.transform.localScale = Vector3.zero;

	}

	// Update is called once per frame
	void Update ()
	{
		timePassedSinceLastStun += Time.deltaTime;

		if (timePassedSinceLastStun > cooldown)
		{

			if (Input.GetMouseButtonDown(1))
			{
				timePassedSinceLastStun = 0;
				Debug.Log("Coroutine Started");
				StartCoroutine(StunSphereAnimation());
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.3F, 0.2F, 0.5F, 0.2F);
		Gizmos.DrawSphere(transform.position, range);
	}

	IEnumerator StunSphereAnimation()
	{
		stunSphere.transform.position = transform.position;

		Collider[] enemies = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Enemy"));

		foreach (Collider enemy in enemies)
		{
			IStunnable stunnable = enemy.GetComponent<IStunnable>();

			if (stunnable != null)
			{
				stunnable.Stun(stunDuration);
			}
		}


		float sinIN = 0;
		while (sinIN < 1)
		{
			sinIN += 0.01F;

			float opacity = OpacityCurve.Evaluate(sinIN);
			float distortion = DistortionCurve.Evaluate(sinIN);
			float size = SizeCurve.Evaluate(sinIN) * range;

			stunSphereRenderer.material.SetFloat("_Opacity", opacity);
			stunSphereRenderer.material.SetFloat("_DistortionAmt", distortion * 5);
			stunSphere.transform.localScale = new Vector3(size, size, size);

			yield return new WaitForSeconds(0.01F);
		}

		stunSphereRenderer.material.SetFloat("_Opacity", 0);
		stunSphereRenderer.material.SetFloat("_DistortionAmt", 0);
		stunSphere.transform.localScale = Vector3.zero;
	}
}
