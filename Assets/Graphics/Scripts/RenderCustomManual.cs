using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/RenderCustomManual")]
public class RenderCustomManual : MonoBehaviour
{
	public CustomRenderTarget sourceTex;

    public Shader RenderCustomManualShader;
    private Material RenderCustomManualMaterial;

    private Camera cam;

	// Use this for initialization
	void Start ()
	{
		cam = GetComponent<Camera>();
	    RenderCustomManualMaterial = new Material(RenderCustomManualShader);
	}
	
	// Update is called once per frame
	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		RenderCustomManualMaterial.SetTexture("_OverlayTex", sourceTex.cachedTexture);
        Graphics.Blit(source, dest, RenderCustomManualMaterial);
	}
}
