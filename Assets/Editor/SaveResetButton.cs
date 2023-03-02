using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveManager))]
public class SaveResetButton : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SaveManager saveManager = (SaveManager)target;
        if (GUILayout.Button("Reset SaveFile"))
        {
            saveManager.ResetSaveFile();
        }
    }
}