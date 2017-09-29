using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class EditorAudioManager : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AudioManager am = (AudioManager)target;
        // SerializedObject serializedObject = new SerializedObject(target);
        if (GUILayout.Button("Collect audio"))
            am.CollectAudios();

        EditorUtility.SetDirty(target);

        // EditorGUILayout.LabelField(
                            // serializedObject.FindProperty("Debug").stringValue, 
                            // GUILayout.Height(serializedObject.FindProperty("strs").intValue * 15)
                        // );
    }
}
#endif
