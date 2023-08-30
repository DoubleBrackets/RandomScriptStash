using UnityEngine;
using UnityEngine.Playables;

public class DummyPlayableAsset : PlayableAsset
{
    public DummyPlayableBehaviour template;

    public float leadTime;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DummyPlayableBehaviour>.Create(graph, template);
        playable.SetLeadTime(leadTime);
        return playable;
    }
}