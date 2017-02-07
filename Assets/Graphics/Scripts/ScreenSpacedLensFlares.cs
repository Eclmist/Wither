using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenSpacedLensFlares : MonoBehaviour
{
	public Texture texture;
	public Transform t;
	[Range(0,1)] public float size;
	[Range(0, 1)] public float opacity;
	public Color c;

	public Shader lfShader;
	private Material lfMaterial;
	private Camera cam;
	private CustomRenderTarget customTex;

	void Start ()
	{
		lfMaterial = new Material(lfShader);
		cam = GetComponent<Camera>();
		customTex = GetComponentInChildren<CustomRenderTarget>();
	}

	private float final;

	void OnRenderImage(RenderTexture source, RenderTexture dest) {
		Vector3 worldPos = t.position;
		Vector3 screenPos = cam.WorldToViewportPoint(worldPos);

		float near = cam.nearClipPlane;
		float far = cam.farClipPlane;

		//screenPos.x -= 0.5F;
		//screenPos.y -= 0.5F;
		//screenPos.z -= 0.5F;
		lfMaterial.SetTexture("_LensFlare", texture);
		lfMaterial.SetVector("_sPosition", screenPos);
		lfMaterial.SetFloat("_size", size);
		lfMaterial.SetFloat("_opacity", opacity);
		lfMaterial.SetColor("_color", c);

		Vector3 nearToFarVector = transform.forward *
								   (far - near); // (green)

		Vector3 nearClipStartPos = transform.position + transform.forward*
								   near; //(Blue)


		Vector3 tToNearClipVector = t.position - nearClipStartPos;

		float depth = Vector3.Dot(tToNearClipVector, nearToFarVector) /
			nearToFarVector.magnitude;


		float linear01Depth = depth/(far - near);


		lfMaterial.SetFloat("_sDepth", depth);

		Graphics.Blit(source, dest, lfMaterial);
				
	}
}

