using UnityEngine;
using UnityEngine.Playables;

public class DummyPlayableGraph : MonoBehaviour
{
    [SerializeField, Range(-1, 11)]
    private float mixerTime;

    [SerializeField]
    private bool propogateTime;

    private PlayableGraph graph;
    private Playable mixerPlayable;

    private void Awake()
    {
        // Create the playable graph
        graph = PlayableGraph.Create("DummyPlayableGraph");

        // Create script output
        var scriptOutput = ScriptPlayableOutput.Create(graph, "DummyBehaviourScriptOutput");

        // Create the mixer playable
        mixerPlayable = Playable.Create(graph);
        scriptOutput.SetSourcePlayable(mixerPlayable);

        // When time is updated by the graph, the playable time doesn't go over the duration
        var dummyPlayable = ScriptPlayable<DummyPlayableBehaviour>.Create(graph);
        dummyPlayable.SetDuration(10f);

        var dummyPlayable2 = ScriptPlayable<DummyPlayableBehaviour>.Create(graph);
        dummyPlayable.SetDuration(5f);

        mixerPlayable.SetInputCount(2);
        mixerPlayable.ConnectInput(0, dummyPlayable, 0);
        mixerPlayable.ConnectInput(1, dummyPlayable2, 0);

        graph.Play();
    }

    private void Update()
    {
        // This makes it so that the dummy playables have their times sync'd
        // Without it the slider would only affect mixerPlayable's time, not its inputs
        mixerPlayable.SetPropagateSetTime(propogateTime);

        mixerPlayable.SetTime(mixerTime);
    }

    private void OnDestroy()
    {
        graph.Destroy();
    }

    [ContextMenu("Play Graph")]
    private void Play()
    {
        // This will call behavior play
        graph.Play();
    }

    [ContextMenu("Stop Graph")]
    private void Stop()
    {
        // This will call behavior pause 
        graph.Stop();
    }
}