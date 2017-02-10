using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CustomRenderTarget : MonoBehaviour {

	public bool isGlobal = true;

	public RenderTexture cachedTexture, cachedDepthTexture;
	public Shader depthCacheShader;
	private Material depthCacheMat;

	public void Start()
	{
		Camera cam = GetComponent<Camera>();
		cachedTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16);
		cachedDepthTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16, RenderTextureFormat.RFloat);
		depthCacheMat = new Material(depthCacheShader);
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, cachedTexture);
		Graphics.Blit(source, destination);
		Graphics.Blit(cachedTexture, cachedDepthTexture, depthCacheMat);

		if (isGlobal)
		{

			Shader.SetGlobalTexture("_PUDepthTex", cachedDepthTexture);
			Shader.SetGlobalTexture("_CachedTex", cachedTexture);
		}
	}

}
