using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class SpriteFixerEditorWindow : EditorWindow
{
    private bool castShadows;
    private bool receiveShadows;
    private bool checkChildren;

    private Material spriteMaterial;
    
    // Pref names
    private const string castShadowsPref = "SpriteFixerEditorWindow.castShadows";
    private const string receiveShadowsPref = "SpriteFixerEditorWindow.receiveShadows";
    private const string spriteMaterialPref = "SpriteFixerEditorWindow.spriteMaterial";
    private const string checkChildrenPref = "SpriteFixerEditorWindow.checkChildren";

    private SpriteRenderer[] selectedSpriteRenderers;

    // simple unity editor window
    [MenuItem("Tools/Sprite Fixer")]
    public static void ShowWindow()
    {
        GetWindow<SpriteFixerEditorWindow>("Sprite Fixer");
    }

    private void OnBecameVisible()
    {
        // Load from editor prefs
        castShadows = EditorPrefs.GetBool(castShadowsPref, false);
        receiveShadows = EditorPrefs.GetBool(receiveShadowsPref, false);
        checkChildren = EditorPrefs.GetBool(checkChildrenPref, false);

        // load material by path
        spriteMaterial = AssetDatabase.LoadAssetAtPath<Material>(
            EditorPrefs.GetString(spriteMaterialPref, String.Empty));
        
        OnSelectionChange();    

        Selection.selectionChanged += OnSelectionChange;
    }
    
    private void OnBecameInvisible()
    {
        Selection.selectionChanged -= OnSelectionChange;
    }

    private void OnGUI()
    {
        GUILayout.Label("Settings to Apply");

        DrawFields();
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        GUILayout.Label("Apply to Selection");

        DrawApplyToSelectionButton();
        
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        GUILayout.Label("Apply to All in Scene");

        DrawApplyToAllButton();
    }

    private void DrawFields()
    {
        // toggle for shadows
        castShadows = EditorGUILayout.Toggle("Cast Shadows", castShadows);
        receiveShadows = EditorGUILayout.Toggle("Receive Shadows", receiveShadows);

        // Material field
        spriteMaterial = (Material)EditorGUILayout.ObjectField("Sprite Material", spriteMaterial, typeof(Material), false);

        // save to editor prefs
        EditorPrefs.SetBool(castShadowsPref, castShadows);
        EditorPrefs.SetBool(receiveShadowsPref, receiveShadows);
        EditorPrefs.SetString(spriteMaterialPref, 
            spriteMaterial != null ? AssetDatabase.GetAssetPath(spriteMaterial) : String.Empty);
    }

    private void DrawApplyToSelectionButton()
    {

        EditorGUILayout.LabelField("Selected Sprite Renderers: " + selectedSpriteRenderers.Length);
        
        bool prevCheckChildren = checkChildren;
        checkChildren = EditorGUILayout.Toggle("Check Children", checkChildren);
        EditorPrefs.SetBool(checkChildrenPref, checkChildren);
        
        if (prevCheckChildren != checkChildren)
        {
            OnSelectionChange();
        }
        
        // disable button if no sprite renderers are selected
        EditorGUI.BeginDisabledGroup(selectedSpriteRenderers.Length == 0);
        
        if (GUILayout.Button("Apply Settings to Sprite Renderer"))
        {
            foreach (var ren in selectedSpriteRenderers)
            {
                ren.receiveShadows = receiveShadows;
                ren.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
                if (spriteMaterial != null)
                {
                    ren.material = spriteMaterial;
                }
            }
        }
        
        EditorGUI.EndDisabledGroup();
    }

    private void DrawApplyToAllButton()
    {
        // Button for applying settings to all sprite renderers
        if (GUILayout.Button("Apply Settings to All Sprite Renderers"))
        {
            var allSpriteRenderers = FindObjectsOfType<SpriteRenderer>();
            foreach (var ren in allSpriteRenderers)
            {
                ren.receiveShadows = receiveShadows;
                ren.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
                if (spriteMaterial != null)
                {
                    ren.material = spriteMaterial;
                }
            }
        }
    }
    
    private void OnSelectionChange()
    {
        var selectedGameObjects = Selection.gameObjects;
        IEnumerable<SpriteRenderer> foundSpriteRenderers;

        if (checkChildren)
        {
            foundSpriteRenderers = selectedGameObjects.SelectMany(
                go => go.GetComponentsInChildren<SpriteRenderer>());
        }
        else
        {
            foundSpriteRenderers = selectedGameObjects.Select(
                go => go.GetComponent<SpriteRenderer>());
        }
        
        selectedSpriteRenderers = 
            foundSpriteRenderers.Distinct().Where(ren => ren != null).ToArray();
        Repaint();
    }
}
