using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RenderCustomTexture : MonoBehaviour {

    CustomRenderTarget customTex;

	public Shader globalTexPass;
	private Material globalTexMat;

	void Start ()
	{
		globalTexMat = new Material(globalTexPass);
		customTex = GetComponentInChildren<CustomRenderTarget>();
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
		Graphics.Blit(source, destination, globalTexMat);
    }

}
