using System;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

public static class TimelineUtilities
{
    [MenuItem("Tools/Refresh Timeline Editor Window")]
    public static void RefreshTimelineEditor()
    {
        TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
    }

    /// <summary>
    /// Create empty curves for a clip with provided default values.
    /// Use in conjunction with PlayableTrack.
    /// Intended for quick-starting animating properties
    /// </summary>
    /// <param name="clip">Target timeline clip</param>
    /// <param name="propertyDefaults">
    /// Array of tuples, first value is the property path (e.g: "propertyName.nestedPropertyName"), second is
    /// the default value
    /// </param>
    public static void CreateEmptyCurvesOnTemplate(TimelineClip clip, Type playableAssetType, params (string, float)[] propertyDefaults)
    {
        if (!clip.hasCurves)
        {
            clip.CreateCurves($"{playableAssetType.Name}Curves");
        }

        foreach (var propertyPath in propertyDefaults)
        {
            var curve = new AnimationCurve(new Keyframe(0, propertyPath.Item2));
            // Curves can only be bound to UnityObjects, and timeline adds to curve to the asset, not the behavior
            clip.curves.SetCurve("", playableAssetType, propertyPath.Item1, curve);
        }
    }
}