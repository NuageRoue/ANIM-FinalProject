using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventDemo))]
class EventsDemoTest : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var ed = (EventDemo)target;

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Next sceen"))
        {
            ed.NextScene();
        }

        GUI.enabled = true;
    }
}
