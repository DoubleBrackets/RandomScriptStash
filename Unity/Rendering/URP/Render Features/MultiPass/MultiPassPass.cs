using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MultiPassPass : ScriptableRenderPass
{
    private List<ShaderTagId> lightModeTags = new();
    
    public MultiPassPass(List<string> lightModePasses)
    {
        foreach (var lightModePass in lightModePasses)
        {
            lightModeTags.Add(new ShaderTagId(lightModePass));
        }

        renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        FilteringSettings filteringSettings = FilteringSettings.defaultValue;

        CommandBuffer cmd = CommandBufferPool.Get();
        foreach (var id in lightModeTags)
        {
            DrawingSettings drawingSettings = CreateDrawingSettings(id, ref renderingData, SortingCriteria.CommonOpaque);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }
    }
}
