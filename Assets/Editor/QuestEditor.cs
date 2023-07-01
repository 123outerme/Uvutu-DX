/*
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestEditor : EditorWindow
{
    [MenuItem("Tools/Quest Editor")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<QuestEditor>();
        wnd.titleContent = new GUIContent("Uvutu DX Quest Editor");
    }
}
//*/

/*
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor 
{
    SerializedProperty quest;
    
    void OnEnable()
    {
        quest = serializedObject.FindProperty("Quest");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(quest);
        serializedObject.ApplyModifiedProperties();
    }
}
//*/