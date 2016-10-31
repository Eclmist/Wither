using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Colorful/MaskGenerator")]
    public class MaskGenerator : PostEffectsBase
    {
        public Shader StencilCullPass;
        private Material cullMat;

        private RenderTexture tex;
        private Camera cam;

        // Use this for initialization
        public override bool CheckResources()
        {
            cullMat = CheckShaderAndCreateMaterial(StencilCullPass, cullMat);
            cam = GetComponent<Camera>();
            tex = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24);
            tex.Create();
            cam.targetTexture = tex;
            return base.CheckResources();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public RenderTexture GetMaskTex()
        {
            return tex;
        }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, cullMat);
        }

        public void OnDisable()
        {
            cam.targetTexture = null;
        }
    }
}
