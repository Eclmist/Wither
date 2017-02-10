using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLights : MonoBehaviour {

	public float lerpSpeed = 10;

	private float intensity;
	private float targetIntensity;

	private Light light;

	void Start()
	{
		light = GetComponent<Light>();
	}

	void Update()
	{
		intensity = light.intensity;

		float intensityDifference = targetIntensity - intensity;

		light.intensity += intensityDifference * Time.deltaTime * lerpSpeed;

		if (intensityDifference < 0.01F)
			light.intensity = targetIntensity;
	}

	public void LerpToTargetIntensity(float i)
	{
		targetIntensity = i;
	}
}
