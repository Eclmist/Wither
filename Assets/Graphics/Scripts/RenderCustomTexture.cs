using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RenderCustomTexture : MonoBehaviour {

    CustomRenderTarget customTex;

	void Start ()
    {
        customTex = GetComponentInChildren<CustomRenderTarget>();
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture tex = RenderTexture.GetTemporary(source.width, source.height, 16);
        Graphics.Blit(customTex.cachedTexture, tex);
		Graphics.Blit(tex, destination);
        RenderTexture.ReleaseTemporary(tex);
    }

}
