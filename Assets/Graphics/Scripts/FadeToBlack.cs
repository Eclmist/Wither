using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/FadeToBlack")]
public class FadeToBlack : MonoBehaviour
{
	public static FadeToBlack Instance;

	[Range(0,1)]public float opacityAddition = 0;

	private float opacity;
	[Range(0, 10)] public float speed;

	public Shader FadeToBlackShader;
	private Material FadeToBlackMaterial;

	private Camera cam;

	private Queue<float> targetOpacities = new Queue<float>();
	private bool coroutineActive;

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start ()
	{
		cam = GetComponent<Camera>();
		FadeToBlackMaterial = new Material(FadeToBlackShader);
	}
	
	// Update is called once per frame
	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		FadeToBlackMaterial.SetFloat("_Opacity", opacity + opacityAddition);
		Graphics.Blit(source, dest, FadeToBlackMaterial);
	}

	void Update()
	{
		if (!coroutineActive && targetOpacities.Count > 0)
		{
			StartCoroutine(LerpOpacity(targetOpacities.Dequeue()));
		}

	}

	public void QueueNextOpacity(float target)
	{
		targetOpacities.Enqueue(1 - target);
	}

	IEnumerator LerpOpacity(float targetOpacity)
	{
		coroutineActive = true;
		float diff = targetOpacity - opacity;

		while (Mathf.Abs(diff) > 0.01F)
		{
			opacity += diff * Time.deltaTime * speed;
			yield return new WaitForEndOfFrame();
		}

		coroutineActive = false;
	}

	public float GetOpacity()
	{
		return 1 - opacity;
	}
}
