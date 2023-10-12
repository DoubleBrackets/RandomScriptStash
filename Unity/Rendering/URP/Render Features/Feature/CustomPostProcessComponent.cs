using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("DIGSPostProcess", typeof(UniversalRenderPipeline))]
public class CustomPostProcessComponent : VolumeComponent, IPostProcessComponent
{
    public Vector2Parameter pixelizeResolution = new (new Vector2(480,270), true);

    public bool IsActive() => active;

    public bool IsTileCompatible() => false;
}
