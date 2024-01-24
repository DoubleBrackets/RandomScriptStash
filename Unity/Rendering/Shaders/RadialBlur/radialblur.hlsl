#ifndef RADIALBLUR
#define RADIALBLUR

// Based on https://halisavakis.com/my-take-on-shaders-radial-blur/
void radialBlur_float(
    float iterations, 
    float strength, 
    UnityTexture2D tex, 
    SamplerState ss,
    float2 uv, 
    out float3 color) {

    float3 total;
    float2 dist = uv - float2(0.5, 0.5);
    float offset = strength / iterations;
    for(int i = 0;i < int(iterations);i++) {
        total += SAMPLE_TEXTURE2D(tex, ss, uv - offset * dist * i);
    }

    color = total / iterations;
}

#endif