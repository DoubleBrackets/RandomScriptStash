using UnityEditor;
using UnityEngine;

public static class DrawUtilities
{
    public static void DrawBox2DGizmo(Vector2 position, Vector2 size, float zAngle, Color c)
    {
        Gizmos.color = c;

        CalculateRectanglePoints(position, size, zAngle, out var p1, out var p2, out var p3, out var p4);

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p1, p4);
    }

    public static void DrawBox2DHandle(Vector2 position, Vector2 size, float zAngle, Color c)
    {
#if UNITY_EDITOR
        Handles.color = c;
        CalculateRectanglePoints(position, size, zAngle, out var p1, out var p2, out var p3, out var p4);

        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);
        Handles.DrawLine(p3, p4);
        Handles.DrawLine(p1, p4);
#endif
    }

    private static void CalculateRectanglePoints(
        Vector2 position, Vector2 size, float zAngle,
        out Vector2 p1, out Vector2 p2, out Vector2 p3, out Vector2 p4)
    {
        var transformMatrix = Matrix4x4.TRS(
            position,
            Quaternion.Euler(0, 0, zAngle),
            size
        );

        p1 = transformMatrix.MultiplyPoint3x4(new Vector2(0.5f, 0.5f));
        p2 = transformMatrix.MultiplyPoint3x4(new Vector2(0.5f, -0.5f));
        p3 = transformMatrix.MultiplyPoint3x4(new Vector2(-0.5f, -0.5f));
        p4 = transformMatrix.MultiplyPoint3x4(new Vector2(-0.5f, 0.5f));
    }

    public static void DrawCapsule2DGizmo(Vector2 position, Vector2 size, float zAngle, Color color)
    {
        Gizmos.color = color;

        CalculateCapsulePoints(position, size, zAngle, out var p1, out var p2);

        Gizmos.DrawWireSphere(p1, size.x / 2f);
        Gizmos.DrawWireSphere(p2, size.x / 2f);

        var recSize = size;
        recSize.y = Mathf.Max(0, size.y - size.x);
        DrawBox2DGizmo(position, recSize, zAngle, color);
    }

    public static void DrawCapsule2DHandle(Vector2 position, Vector2 size, float zAngle, Color color)
    {
#if UNITY_EDITOR
        Handles.color = color;

        CalculateCapsulePoints(position, size, zAngle, out var p1, out var p2);

        Handles.DrawWireDisc(p1, Vector3.forward, size.x / 2f);
        Handles.DrawWireDisc(p2, Vector3.forward, size.x / 2f);

        var recSize = size;
        recSize.y = Mathf.Max(0, size.y - size.x);
        DrawBox2DHandle(position, recSize, zAngle, color);
#endif
    }

    public static void CalculateCapsulePoints(
        Vector2 position, Vector2 size, float zAngle,
        out Vector2 p1, out Vector2 p2)
    {
        var transformMatrix = Matrix4x4.TRS(
            position,
            Quaternion.Euler(0, 0, zAngle),
            Vector3.one
        );

        var offset = Vector3.up * Mathf.Max(0, (size.y - size.x) / 2f);

        p1 = transformMatrix.MultiplyPoint3x4(offset);
        p2 = transformMatrix.MultiplyPoint3x4(-offset);
    }

    public static void DrawCircle2DGizmo(Vector2 position, float radius, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(position, radius);
    }

    public static void DrawCircle2DHandle(Vector2 position, float radius, Color color)
    {
#if UNITY_EDITOR
        Handles.color = color;

        Handles.DrawWireDisc(position, Vector3.forward, radius);
#endif
    }
}