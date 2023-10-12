using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class MultiPassRenderFeature : ScriptableRendererFeature
{
    public List<string> lightModePasses;
    private MultiPassPass mainPass;
    
    public override void Create()
    {
        mainPass = new(lightModePasses);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(mainPass);
    }
}
