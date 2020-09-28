using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PVPSetupWindow : EditorWindow
{
    // Add menu named "My Window" to the Window menu
    [MenuItem("Avatar PVP/Setup Layers")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PVPSetupWindow window = EditorWindow.GetWindow<PVPSetupWindow>();
        window.Show();
    }

    void OnGUI()
    {
        if(GUILayout.Button("Setup layers for PVP (Will override layers 31, 30, 29)"))
        {
            //Based on https://forum.unity.com/threads/create-tags-and-layers-in-the-editor-using-script-both-edit-and-runtime-modes.732119/
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            SerializedProperty HOTLayer = layersProp.GetArrayElementAtIndex(29);
            SerializedProperty HealLayer = layersProp.GetArrayElementAtIndex(30);
            SerializedProperty DOTLayer = layersProp.GetArrayElementAtIndex(31);
            HOTLayer.stringValue = "HOT";
            HealLayer.stringValue = "Heal";
            DOTLayer.stringValue = "DOT";
            tagManager.ApplyModifiedProperties();
        }

        EditorGUILayout.Space();
        if (!Physics.GetIgnoreLayerCollision(10, 29))
        {
            EditorGUILayout.LabelField("Warning! Layer Collision Matrix not setup for HOT layer!");
        }

        EditorGUILayout.Space();
        if (!Physics.GetIgnoreLayerCollision(10, 30))
        {
            EditorGUILayout.LabelField("Warning! Layer Collision Matrix not setup for Heal layer!");

        }
        EditorGUILayout.Space();
        if (!Physics.GetIgnoreLayerCollision(10, 31))
        {
            EditorGUILayout.LabelField("Warning! Layer Collision Matrix not setup for DOT layer!");

        }

        if (GUILayout.Button("Setup Layer Collision matrix"))
        {
            Physics.IgnoreLayerCollision(10, 29, true);
            Physics.IgnoreLayerCollision(10, 30, true);
            Physics.IgnoreLayerCollision(10, 31, true);
        }
    }
}
