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

        public override bool CheckResources()
        {
            CheckSupport(false);

            saturationMat = CheckShaderAndCreateMaterial(saturationPass, saturationMat);
            maskGen = GetComponentInChildren<MaskGenerator>();

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }


        public void OnEnable()
        {
        }

        public void OnDisable()
        { }

       
        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            saturationMat.SetTexture("_MaskTex", maskGen.GetMaskTex());

            saturationMat.SetFloat("_Saturation", saturation);
            saturationMat.SetFloat("_Brightness", brightness);

            Graphics.Blit(source, destination, saturationMat);
        }
    }

}
