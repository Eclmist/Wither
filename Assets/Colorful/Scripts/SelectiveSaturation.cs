using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Colorful/Selective Saturation")]

    public class SelectiveSaturation : PostEffectsBase
    {

        public Shader saturationPass;
        private Material saturationMat;

        [Range(0, 1)] public float saturation;
        [Range(0, 6)] public float brightness;

        private MaskGenerator maskGen;

        private ManualBlur manualBlur;

        public override bool CheckResources()
        {
            CheckSupport(false);

            saturationMat = CheckShaderAndCreateMaterial(saturationPass, saturationMat);
            maskGen = GetComponentInChildren<MaskGenerator>();
            manualBlur = GetComponent<ManualBlur>();

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }


        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            RenderTexture blurredMaskTex = RenderTexture.GetTemporary(maskGen.GetMaskTex().width, maskGen.GetMaskTex().height);

            manualBlur.Blur(maskGen.GetMaskTex(), blurredMaskTex);

            saturationMat.SetTexture("_MaskTex", blurredMaskTex);

            saturationMat.SetFloat("_Saturation", saturation);
            saturationMat.SetFloat("_Brightness", brightness);

            Graphics.Blit(source, destination, saturationMat);

            RenderTexture.ReleaseTemporary(blurredMaskTex);
        }
    }

}
