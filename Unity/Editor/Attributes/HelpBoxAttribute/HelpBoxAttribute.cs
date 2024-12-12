using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class HelpBoxAttribute : PropertyAttribute
{
    public HelpBoxAttribute(string text, int space = 1)
    {
        Text = text;
        Space = space;
    }

    public string Text { get; }
    public int Space { get; }
}