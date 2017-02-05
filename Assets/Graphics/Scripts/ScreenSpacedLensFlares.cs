using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenSpacedLensFlares : MonoBehaviour
{
    public Transform t;
    [Range(0,1)] public float size;
    [Range(0, 1)] public float opacity;

    public Shader lfShader;
    private Material lfMaterial;
    private Camera cam;

	void Start ()
	{
        lfMaterial = new Material(lfShader);
	    cam = GetComponent<Camera>();
	}

    private float final;

	void OnRenderImage(RenderTexture source, RenderTexture dest) {
	    Vector3 worldPos = t.position;
        Vector3 screenPos = cam.WorldToViewportPoint(worldPos);

        //screenPos.x -= 0.5F;
        //screenPos.y -= 0.5F;
        //screenPos.z -= 0.5F;

        lfMaterial.SetVector("_sPosition", screenPos);
        lfMaterial.SetFloat("_size", size);
        lfMaterial.SetFloat("_opacity", opacity);

	    float distanceFromCamera = (transform.position - worldPos).magnitude;

	    float clipDistance = cam.farClipPlane - cam.nearClipPlane;
	    distanceFromCamera -= cam.nearClipPlane;


	    final = distanceFromCamera/clipDistance;
        lfMaterial.SetFloat("_sDepth", distanceFromCamera/clipDistance);

        Graphics.Blit(source, dest, lfMaterial);
        		
	}
}

