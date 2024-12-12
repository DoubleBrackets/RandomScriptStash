using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

[DisplayName("Templates/TemplateMarker")]
public class TemplateMarker : Marker, INotification, INotificationOptionProvider
{
    [SerializeField] public bool emitOnce;
    [SerializeField] public bool emitInEditor;

    [SerializeField]
    public string markerData;

    public PropertyName id { get; }

    NotificationFlags INotificationOptionProvider.flags =>
        (emitOnce ? NotificationFlags.TriggerOnce : default) |
        (emitInEditor ? NotificationFlags.TriggerInEditMode : default);
}