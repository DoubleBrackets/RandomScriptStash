using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Playable assets/tracks support polymorphism, so you can create multiple types of PlayableAssets and PlayableBehaviours.
/// </summary>
[Serializable]
public class T1PlayableAsset : TemplatePlayableAsset
{
    [SerializeField]
    private string _text = "Hello World";

}
