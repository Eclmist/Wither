using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CustomRenderTarget : MonoBehaviour {

	public RenderTexture cachedTexture, cachedDepthTexture;
	public Shader depthCacheShader;
	private Material depthCacheMat;

	public void Start()
	{
		Camera cam = GetComponent<Camera>();
		cachedTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16);
		cachedDepthTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16);
		depthCacheMat = new Material(depthCacheShader);
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, cachedTexture);
		Graphics.Blit(source, destination);
		Graphics.Blit(cachedTexture, cachedDepthTexture, depthCacheMat);
		Shader.SetGlobalTexture("_PUDepthTex", cachedDepthTexture);

	}

}
