using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseRevealTrigger : MonoBehaviour, IInteractable
{

	private bool pulseStarted;

	[Header("Glow")]
	[SerializeField] private float opacity;
	[SerializeField] private float maxGlowOpacity;
	[SerializeField] [Range(0,1)] private float glowRate;

	[SerializeField] [Range(0, 3)] private float lightSourceTargetIntensity = 3;


	[SerializeField]
	private AnimationCurve opacityCurve;

	private Renderer renderer;
	private float additionalOpacity;
	private float permenentOpacity;
	private bool interacted;

	void IInteractable.Interact()
	{
		if (!interacted)
		{
			interacted = true;
			permenentOpacity = 1;
			ToggleParticles(true);
			ToggleLights(true);
		}
	}

	IEnumerator DoPulse()
	{
		pulseStarted = true;

		additionalOpacity = 0;

		WaitForFixedUpdate wait = new WaitForFixedUpdate();

		float opacityFromCurve = 0;

		while (opacityFromCurve < 1)
		{
			additionalOpacity = opacityCurve.Evaluate(opacityFromCurve);
			additionalOpacity *= maxGlowOpacity;
			opacityFromCurve += glowRate / 10;

			yield return wait;
		}


		additionalOpacity = 0;

		pulseStarted = false;

	}

	// Use this for initialization
	void Start ()
	{
		renderer = GetComponent<Renderer>();
		ToggleParticles(false);
		ToggleLights(false);
	}
	
	// Update is called once per frame
	void Update () {
		float tempOpacity = Mathf.Clamp(permenentOpacity + 
			opacity + additionalOpacity, 0, maxGlowOpacity);

		renderer.material.SetFloat("_Opacity", tempOpacity);
	}

	void ToggleParticles(bool enable)
	{
		ParticleToggle[] test = GetComponentsInChildren<ParticleToggle>();

		foreach (ParticleToggle p in GetComponentsInChildren<ParticleToggle>())
		{
			p.ToggleParticles(enable);
		}
	}

	void ToggleLights(bool enable)
	{
		foreach (ToggleLights l in GetComponentsInChildren<ToggleLights>())
		{
			l.LerpToTargetIntensity(enable ? lightSourceTargetIntensity : 0);
		}
	}


	void IInteractable.Pulse()
	{
		if (!pulseStarted)
		{
			StartCoroutine("DoPulse");
		}
	}

	void IInteractable.SetOpacity(float o)
	{
		opacity = o;
	}

	public bool GetIsInteracted()
	{
		return interacted;
	}

	public bool CanInteractWith(Interactor from)
	{
		switch (from)
		{
			case Interactor.Player:
				return false;
			case Interactor.Ivy:
				return true;
			default:
				throw new ArgumentOutOfRangeException("from", @from, null);
		}
	}
}
