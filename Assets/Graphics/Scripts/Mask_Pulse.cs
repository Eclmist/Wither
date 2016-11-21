using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MaskGenerator))]
    [AddComponentMenu("Image Effects/Colorful/Mask_Pulse")]
    public class Mask_Pulse : PostEffectsBase
    {
        public static Mask_Pulse Instance;

        [SerializeField] private Shader pulsePass;
        private Material pulseMat;

        private MaskGenerator maskGen;
        //private RenderTexture pass;

        private Camera cam;

        void Awake()
        {
            Instance = this;
        }

        // Use this for initialization
        public override bool CheckResources()
        {
            CheckSupport(false);

            cam = GetComponent<Camera>();
            maskGen = GetComponent<MaskGenerator>();
            pulseMat = CheckShaderAndCreateMaterial(pulsePass, pulseMat);

            //pass = new RenderTexture(maskGen.GetMaskTex().width, maskGen.GetMaskTex().height, 24);
            //pass.Create();

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destionation)
        {
            RaycastCornerBlit(maskGen.GetMaskTex(1), maskGen.GetMaskTex(2), pulseMat);
            maskGen.SwapBuffer();
        }

        void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
        {
            // Compute Frustum Corners
            float camFar = cam.farClipPlane;
            float camFov = cam.fieldOfView;
            float camAspect = cam.aspect;

            float fovWHalf = camFov * 0.5f;

            Vector3 toRight = cam.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
            Vector3 toTop = cam.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

            Vector3 topLeft = (cam.transform.forward - toRight + toTop);
            float camScale = topLeft.magnitude * camFar;

            topLeft.Normalize();
            topLeft *= camScale;

            Vector3 topRight = (cam.transform.forward + toRight + toTop);
            topRight.Normalize();
            topRight *= camScale;

            Vector3 bottomRight = (cam.transform.forward + toRight - toTop);
            bottomRight.Normalize();
            bottomRight *= camScale;

            Vector3 bottomLeft = (cam.transform.forward - toRight - toTop);
            bottomLeft.Normalize();
            bottomLeft *= camScale;

            // Custom Blit, encoding Frustum Corners as additional Texture Coordinates
            RenderTexture.active = dest;

            mat.SetTexture("_MainTex", source);

            GL.PushMatrix();
            GL.LoadOrtho();

            mat.SetPass(0);

            GL.Begin(GL.QUADS);

            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.MultiTexCoord(1, bottomLeft);
            GL.Vertex3(0.0f, 0.0f, 0.0f);

            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.MultiTexCoord(1, bottomRight);
            GL.Vertex3(1.0f, 0.0f, 0.0f);

            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.MultiTexCoord(1, topRight);
            GL.Vertex3(1.0f, 1.0f, 0.0f);

            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.MultiTexCoord(1, topLeft);
            GL.Vertex3(0.0f, 1.0f, 0.0f);

            GL.End();
            GL.PopMatrix();
        }

        void FixedUpdate()
        {
        }

        public void SetPulse(float size, float width)
        {
            pulseMat.SetFloat("_PulseDistance", size);
            pulseMat.SetFloat("_PulseWidth", width);
        }

        public void SetPulsePosition(Vector3 pulsePosition)
        {
            pulseMat.SetVector("_PulsePosition", pulsePosition);
        }

        public void SetMaxDistance(float maxDist)
        {
            pulseMat.SetFloat("_MaxDistance", maxDist);
        }

    }
}