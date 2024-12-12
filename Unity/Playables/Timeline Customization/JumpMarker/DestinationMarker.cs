using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[DisplayName("Jump/DestinationMarker"), CustomStyle("DestinationMarker")]
public class DestinationMarker : Marker
{
    [SerializeField] public bool active;

    private void Reset()
    {
        active = true;
    }
}