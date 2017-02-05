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

        /* Temp */
        //public Shader pulsePass;
        //private Material pulseMat;


        private RenderTexture pass1, pass2;
        private Camera cam;

        // Use this for initialization
        public override bool CheckResources()
        {
            CheckSupport(false);

            cullMat = CheckShaderAndCreateMaterial(StencilCullPass, cullMat);
            cam = GetComponent<Camera>();
            pass1 = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24);

            if (QualitySettings.antiAliasing != 0)
                pass1.antiAliasing = QualitySettings.antiAliasing;

            pass1.Create();

            cam.targetTexture = pass1;                  //Commented out for testing of overlaying masks 

            pass2 = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24);

            if (QualitySettings.antiAliasing != 0)
                pass2.antiAliasing = QualitySettings.antiAliasing;

            pass2.Create();
            //pulseMat = CheckShaderAndCreateMaterial(pulsePass, pulseMat);


            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public RenderTexture GetMaskTex(int index)
        {
            switch (index)
            {
                case 1:
                    return pass1;
                case 2:
                    return pass2;
                default:
                    return pass1;
            }
        }

        public RenderTexture GetMaskTex()
        {
            return pass1;
        }


        public void SwapBuffer()
        {
            Graphics.Blit(pass2, pass1);
        }

        public void SetBackBuffer(RenderTexture tex)
        {
            pass2 = tex;
        }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, pass1, cullMat);
            //Graphics.Blit(pass1, pass2, pulseMat);
        }

        public void OnPostRender()
        {
            RenderTexture.active = pass1;
            GL.Clear(true, true, Color.black);

            RenderTexture.active = pass2;
            GL.Clear(true, true, Color.black);
        }

        public void OnDisable()
        {
            cam.targetTexture = null;
        }

        public void OnDestroy()
        {
            cam.targetTexture = null;
        }
    }
}
