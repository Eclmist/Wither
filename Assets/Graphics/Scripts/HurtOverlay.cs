using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/HurtOverlay")]
public class HurtOverlay : MonoBehaviour
{
	public static HurtOverlay Instance;

	public Shader HurtOverlayShader;
	private Material HurtOverlayMaterial;

	public Texture overlay;
	[Range (0,1)] [SerializeField] private float opacity;

	public float fadeSpeed = 0.5f;

	private Camera cam;

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start ()
	{
		cam = GetComponent<Camera>();
		HurtOverlayMaterial = new Material(HurtOverlayShader);
	}
	
	// Update is called once per frame
	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		HurtOverlayMaterial.SetTexture("_OverlayTex", overlay);
		HurtOverlayMaterial.SetFloat("_Opacity", opacity * 10);

		Graphics.Blit(source, dest, HurtOverlayMaterial);
	}

	public void SetOpacity(float amount)
	{
		opacity = amount;
	}
}
