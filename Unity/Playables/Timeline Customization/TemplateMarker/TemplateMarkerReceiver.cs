using UnityEngine;
using UnityEngine.Playables;

public class TemplateMarkerReceiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        var jumpMarker = notification as TemplateMarker;
        if (jumpMarker == null) return;

        Debug.Log($"Received jump marker with data: {jumpMarker.markerData}");
    }
}

