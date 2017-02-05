using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[RequireComponent (typeof(BlurOptimized))]
public class BlurCameraOverTime : MonoBehaviour {

	public static BlurCameraOverTime Instance;

	private bool blurred;

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void BlurScreen()
	{
		Instance.StartCoroutine(Blur(true));
	}

	public void UnblurScreen()
	{
		Instance.StartCoroutine(Blur(false));
	}

	IEnumerator Blur(bool enabled)
	{
		BlurOptimized blurScript = Camera.main.GetComponent<BlurOptimized>();

		float startingBlurValue = enabled ? 0 : 2;

		if (!enabled)
		{
			while (startingBlurValue >= 0)
			{
				blurScript.enabled = true;
				startingBlurValue -= 0.1F;

				if (startingBlurValue <= 0)
				{
					blurScript.enabled = false;
					startingBlurValue = 0;
				}
				else
				{
					blurScript.blurSize = startingBlurValue;
				}

				yield return new WaitForFixedUpdate();
			}
		}
		else
		{
			while (startingBlurValue <= 2)
			{

				blurScript.enabled = true;
				startingBlurValue += 0.1F;

				if (startingBlurValue >= 2)
				{
					startingBlurValue = 2;
				}

				blurScript.blurSize = startingBlurValue;

				yield return new WaitForFixedUpdate();
			}
		}
	}
}
