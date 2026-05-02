using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WheelUIManager))]
class WheelUITest : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var wheel = (WheelUIManager)target;

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Speen"))
        {
            Speen(wheel);
        }

        if (GUILayout.Button("Create"))
        {
            wheel.Build(new List<SegmentAttribute>());
        }

        GUI.enabled = true;
    }

    private async void Speen(WheelUIManager wheel)
    {
        Debug.Log((await wheel.Speen()).name);
    }
}
