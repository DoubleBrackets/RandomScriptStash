using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class CustomPostProcessPass : ScriptableRenderPass
{
    [System.Serializable]
    public struct PassConfig
    {
        public RenderPassEvent renderPassEvent;
        public Material passMaterial;
    }

    private PassConfig config;

    // Used if post process volume component is required
    private CustomPostProcessComponent customComponent;
    
    private RenderTextureDescriptor colorDescriptor;
    private RTHandle cameraColorTargetHandle;
    private RTHandle cameraDepthTargetHandle;
    
    // Temp buffer for blitting
    private RTHandle destinationA;

    public CustomPostProcessPass(PassConfig config)
    {
        this.config = config;
        renderPassEvent = config.renderPassEvent;
    }

    public void Setup(in RenderingData renderingData)
    {
        colorDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        colorDescriptor.depthBufferBits = (int)DepthBits.None;
        RenderingUtils.ReAllocateIfNeeded(ref destinationA, colorDescriptor, name: "_TemporaryBufferA");
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        bool invalidConfig = config.passMaterial == null;
        
        if (invalidConfig)
        {
            return;
        }
        
        VolumeStack stack = VolumeManager.instance.stack;
        customComponent = stack.GetComponent<CustomPostProcessComponent>();

        CommandBuffer cmd = CommandBufferPool.Get();

        using (new ProfilingScope(cmd, new ProfilingSampler("DIGS Post Process Pass")))
        {
            Blit(cmd, cameraColorTargetHandle, destinationA, config.passMaterial, 0);
            Blit(cmd, destinationA, cameraColorTargetHandle);
            
            // Alternative is to blit and drawfullscreen
            /*Blitter.BlitCameraTexture(cmd, cameraColorTargetHandle, destinationA);*/
        }
        
        /*CoreUtils.SetRenderTarget(cmd, cameraColorTargetHandle);
        CoreUtils.DrawFullScreen(cmd, pixelizeMaterial);*/

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        
        CommandBufferPool.Release(cmd);
    }

    public void SetTarget(RTHandle rendererCameraColorTargetHandle, RTHandle rendererCameraDepthTargetHandle)
    {
        cameraColorTargetHandle = rendererCameraColorTargetHandle;
        cameraDepthTargetHandle = rendererCameraDepthTargetHandle;
    }

    public void Dispose()
    {
        destinationA?.Release();
    }
}
