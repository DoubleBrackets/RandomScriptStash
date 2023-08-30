using System;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Dummy playable behaviour. Add this onto a playable track to see how overrides behave
/// </summary>
[Serializable]
public class DummyPlayableBehaviour : PlayableBehaviour
{
    // Make sure field is serializable. Custom property drawers work like normal. Can be keyframed.
    // Remember to hit F or A to reframe curves.
    [SerializeField, Range(0, 1)]
    private float value;

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        // Called every evaluation, before process frame. Trickle down
        // Timeline: Usually every time you move over while scrubbing
        // Print("Prepare Frame");
        base.PrepareFrame(playable, info);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // Called every evaluation, bubble up
        // Timeline: Usually every time you move over while scrubbing
        // Print("Process Frame");
        base.ProcessFrame(playable, info, playerData);
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        // Timeline: When the playback enters the playable (includes scrubbing) or starts playing inside of it
        Print("Behaviour Play");
        base.OnBehaviourPlay(playable, info);
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // Timeline: When the playback leaves the playable (includes scrubbing) or pauses while playback is inside.
        // Also when loading graph for the first time (might be Timeline only?)
        Print("Behaviour Pause");
        base.OnBehaviourPause(playable, info);
    }

    public override void OnPlayableCreate(Playable playable)
    {
        // Called when created
        // Timeline: basically when the graph is first created (opening timeline in editor)
        Print("Playable Create");
        base.OnPlayableCreate(playable);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        // Called when destroyed
        // Timeline: When opening the closing the relevant timeline in the timeline window (e.g switching inspect target)
        Print("Playable Destroy");
        base.OnPlayableDestroy(playable);
    }

    public override void OnGraphStart(Playable playable)
    {
        // Called when the graph starts
        // Timeline: When starting playback or loading for the first time in editor
        Print("Graph Start");
        base.OnGraphStart(playable);
    }

    public override void OnGraphStop(Playable playable)
    {
        // Called when the graph stops
        // Timeline: When pausing the playback or right before destroying the graph
        Print("Graph Stop");
        base.OnGraphStop(playable);
    }

    public override void PrepareData(Playable playable, FrameData info)
    {
        // Called when the lead time is greater than the Playable delay. Doesn't seem to do anything in timeline.
        Print("Prepare Data");
        base.PrepareData(playable, info);
    }

    private void Print(string message)
    {
        Debug.Log($"Dummy Playable: {message}");
    }
}