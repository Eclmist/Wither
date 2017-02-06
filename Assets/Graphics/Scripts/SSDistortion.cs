using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/SSDistortion")]
public class SSDistortion : MonoBehaviour
{

	public static SSDistortion Instance;

	public Shader SSDistortionShader;
	private Material SSDistortionMaterial;

	private Camera cam;

	public float radius;
	public float radiusMultiplier = 0.01F;

	[Range(0, 1)]public float distortion;
	public float distortionMultiplier = 1;
	public Transform t;

	// Use this for initialization
	void Start ()
	{
		Instance = this;
		cam = GetComponent<Camera>();
		SSDistortionMaterial = new Material(SSDistortionShader);
	}
	
	// Update is called once per frame
	void OnRenderImage(RenderTexture source, RenderTexture dest) {

		Vector2 screenPos = cam.WorldToViewportPoint(t.position);
		SSDistortionMaterial.SetVector("_Position", screenPos);
		SSDistortionMaterial.SetFloat("_Radius", radius * radiusMultiplier);
		SSDistortionMaterial.SetFloat("_DistortionAmount", distortion * distortionMultiplier);
		SSDistortionMaterial.SetFloat("_AspectRatio", cam.aspect);

		Graphics.Blit(source, dest, SSDistortionMaterial);
	}
}
