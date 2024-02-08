using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/ScreenGradient", typeof(UniversalRenderPipeline))]
public class ScreenGradient : VolumeComponent, IPostProcessComponent {
    // For example, an intensity parameter that goes from 0 to 1
    public ClampedFloatParameter intensity = new ClampedFloatParameter(value: 0, min: 0, max: 1, overrideState: true);

    // A color that is constant even when the weight changes
    public NoInterpColorParameter overlayColor = new NoInterpColorParameter(Color.cyan);

    // Other 'Parameter' variables you might have
    public NoInterpClampedFloatParameter start = new NoInterpClampedFloatParameter(value: 0, min: -1, max: 1);
    public NoInterpClampedFloatParameter end = new NoInterpClampedFloatParameter(value: 0, min: -1, max: 1);

    // Tells when our effect should be rendered
    public bool IsActive() => intensity.value > 0;

    // I have no idea what this does yet but I'll update the post once I find an usage
    public bool IsTileCompatible() => true;
}