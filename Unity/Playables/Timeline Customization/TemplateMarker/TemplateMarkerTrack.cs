using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
/// <summary>
///     We need a curve on the track, or CreateTrackMixer will not be called
/// </summary>
[CustomTimelineEditor(typeof(TemplateMarkerTrack))]
public class FakeTrackEditor : TrackEditor
{
    public override void OnCreate(TrackAsset track, TrackAsset copiedFrom)
    {
        track.CreateCurves("FakeCurves");
        track.curves.SetCurve(string.Empty, typeof(GameObject), "m_FakeCurve", AnimationCurve.Linear(0, 0, 0, 0));
        base.OnCreate(track, copiedFrom);
    }
}

#endif

[DisplayName("Templates/Template Marker Track")]
public class TemplateMarkerTrack : MarkerTrack
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var notifReceiver = go.GetComponent<TemplateMarkerReceiver>();
        IEnumerable<TemplateMarker> markers = GetMarkers().OfType<TemplateMarker>();

        // Try to use notif output from the default marker track
        if (TryGetDefaultNotifOutput(graph,
                out PlayableOutput existingNotifOutput,
                out TimeNotificationBehaviour timeNotifBehaviour))
        {
            Debug.Log($"Track {name} linking markers to default marker track");

            // Hook up the notif receiver and markers to that existing behavior
            SetupNotifs(existingNotifOutput, timeNotifBehaviour, notifReceiver, markers);
        }
        else
        {
            // If that fails then we create our own playable setup
            // For instance, if the default marker track is empty or muted

            Debug.Log($"Track {name} creating new notif output");
            SetupNewNotifOutput(graph, notifReceiver, markers);
        }

        return base.CreateTrackMixer(graph, go, inputCount);
    }

    private bool TryGetDefaultNotifOutput(
        PlayableGraph graph,
        out PlayableOutput existingNotifOutput,
        out TimeNotificationBehaviour timeNotifBehaviour)
    {
        existingNotifOutput = graph.GetOutput(0);

        // Assume that the first input to the source playable is the correct time notif behavior

        try
        {
            timeNotifBehaviour =
                ((ScriptPlayable<TimeNotificationBehaviour>)existingNotifOutput.GetSourcePlayable()
                    .GetInput(0))
                .GetBehaviour();
            return true;
        }
        catch
        {
            timeNotifBehaviour = null;
            return false;
        }
    }

    /// <summary>
    ///     Create a new output for notifications, hook up a new time notif behavior to the timeline input, and hook up the
    ///     notif receiver and markers
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="receiver"></param>
    /// <param name="markers"></param>
    /// <returns></returns>
    private Playable SetupNewNotifOutput(
        PlayableGraph graph,
        INotificationReceiver receiver,
        IEnumerable<TemplateMarker> markers)
    {
        // Create a new scriptable output for notifications
        var newNotifOutput = ScriptPlayableOutput.Create(graph, "ActualNotificationOutput");

        Playable rootPlayable = graph.GetRootPlayable(0);

        // Link the output to the source playable (the root timeline playable)
        rootPlayable.SetOutputCount(rootPlayable.GetInputCount() + 1);
        newNotifOutput.SetSourcePlayable(rootPlayable, rootPlayable.GetOutputCount() - 1);
        Playable sourcePlayable = newNotifOutput.GetSourcePlayable();

        //Create a TimeNotificationBehaviour
        ScriptPlayable<TimeNotificationBehaviour> timeNotifPlayable =
            ScriptPlayable<TimeNotificationBehaviour>.Create(graph);

        timeNotifPlayable.GetBehaviour().timeSource = sourcePlayable;

        // Add a new input to the sourceplayable
        sourcePlayable.SetInputCount(sourcePlayable.GetInputCount() + 1);

        // Attach our time notif behavior playable as the last input to the source playable
        graph.Connect(
            timeNotifPlayable,
            0,
            sourcePlayable,
            sourcePlayable.GetInputCount() - 1);

        // Hook up the notif receiver and markers to the new behavior
        SetupNotifs(newNotifOutput, timeNotifPlayable.GetBehaviour(), receiver, markers);

        return timeNotifPlayable;
    }

    /// <summary>
    ///     Hook up a notif receiver and markers
    ///     We need to know the output that emits the notifications (so we can add the receiver to it)
    ///     We need to know the time notification behavior (so we can add the notifying markers to it)
    /// </summary>
    /// <param name="output"></param>
    /// <param name="timeNotifBehavior"></param>
    /// <param name="receiver"></param>
    /// <param name="markers"></param>
    private void SetupNotifs<TMarker>(
        PlayableOutput output,
        TimeNotificationBehaviour timeNotifBehavior,
        INotificationReceiver receiver,
        IEnumerable<TMarker> markers) where TMarker : IMarker, INotification, INotificationOptionProvider
    {
        output.AddNotificationReceiver(receiver);

        foreach (TMarker marker in markers)
        {
            timeNotifBehavior.AddNotification(marker.time, marker, marker.flags);
        }
    }
}