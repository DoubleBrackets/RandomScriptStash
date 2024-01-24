using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class CustomPostProcessFeature :  ScriptableRendererFeature
{
    [SerializeField]
    private bool applyInEditor;
    
    [SerializeField]
    private CustomPostProcessPass.PassConfig settings;

    private CustomPostProcessPass customPass;
    
    public override void Create()
    {
        // If you want everything to be procedural, you can create a temporary material from a shader
        // pixelizeMaterial = CoreUtils.CreateEngineMaterial(pixelizeShader);
        customPass = new CustomPostProcessPass(settings);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        // Configure the custom pass texture targets
        if (customPass != null && renderingData.cameraData.cameraType == CameraType.Game)
        {
            customPass.ConfigureInput(ScriptableRenderPassInput.Depth);
            customPass.ConfigureInput(ScriptableRenderPassInput.Color);
            customPass.SetTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (applyInEditor || renderingData.cameraData.cameraType == CameraType.Game)
        {
            customPass.Setup(renderingData);
            renderer.EnqueuePass(customPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        // If you used a temporary material clean it up here
        // CoreUtils.Destroy(pixelizeMaterial);
        customPass.Dispose();
    }
}
