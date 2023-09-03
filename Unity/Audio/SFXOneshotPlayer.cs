using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXOneshotPlayer : MonoBehaviour
{
    public static SFXOneshotPlayer Instance;

    private AudioSource source;

    private void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();
    }

    public void PlaySFXOneshot(AudioClip audioClip)
    {
        source.PlayOneShot(audioClip);
    }
}