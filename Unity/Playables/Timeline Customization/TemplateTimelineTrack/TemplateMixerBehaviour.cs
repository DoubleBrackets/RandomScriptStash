
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

// The runtime instance of a the Template. It is responsible for blending and setting the final data
// on the Light binding
public class TemplateMixerBehaviour : PlayableBehaviour
{
    Light m_TrackBinding;

    // Called every frame that the timeline is evaluated. ProcessFrame is invoked after its' inputs.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        SetDefaults(playerData as Light);
        if (m_TrackBinding == null)
            return;

        int inputCount = playable.GetInputCount();

        Color c = Color.black;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<TemplatePlayableBehaviour> inputPlayable = (ScriptPlayable<TemplatePlayableBehaviour>)playable.GetInput(i);
            TemplatePlayableBehaviour input = inputPlayable.GetBehaviour();

            c += input.color * inputWeight;
        }
        
        m_TrackBinding.color = c;
    }

    // Invoked when the playable graph is destroyed, typically when PlayableDirector.Stop is called or the timeline
    // is complete.
    public override void OnPlayableDestroy(Playable playable)
    {
        RestoreDefaults();
    }

    void SetDefaults(Light light)
    {
        if (light == m_TrackBinding)
            return;
        
        m_TrackBinding = light;
        RestoreDefaults();
    }

    void RestoreDefaults()
    {
        if (m_TrackBinding == null)
            return;
    }
}


