using UnityEditor;
using UnityEditor.Timeline;

public static class TimelineEditorUtilities
{
    [MenuItem("Tools/Refresh Timeline Editor Window")]
    public static void RefreshTimelineEditor()
    {
        TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
    }
}