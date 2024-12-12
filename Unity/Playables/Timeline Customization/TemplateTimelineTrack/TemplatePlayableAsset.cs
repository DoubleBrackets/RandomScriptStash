
using System;
using System.ComponentModel;
using Timeline.Samples;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


// Represents the serialized data for a clip on the TemplateTrack
[Serializable]
[DisplayName("Templates/Template Clip")]
public class TemplatePlayableAsset : PlayableAsset, ITimelineClipAsset
{
    public TemplatePlayableBehaviour template = new TemplatePlayableBehaviour();

    // Implementation of ITimelineClipAsset. This specifies the capabilities of this timeline clip inside the editor.
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    // Creates the playable that represents the instance of this clip.
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // Using a template will clone the serialized values
        return ScriptPlayable<TemplatePlayableBehaviour>.Create(graph, template);
    }
}

