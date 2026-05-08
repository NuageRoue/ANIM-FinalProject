using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CraftingSystem))]
class InventoryTest : Editor
{
    bool wantCraftButCant = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var cs = (CraftingSystem)target;

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Create"))
        {
            if (cs.recipes.Count == 0)
            {
                wantCraftButCant = true;
            }
            else
            {
                var firstReceipe = cs.recipes.First();

                if (!cs.CanCraft(firstReceipe))
                {
                    wantCraftButCant = true;
                }
                else
                {
                    wantCraftButCant = false;
                    cs.Craft(firstReceipe);
                }
            }
        }

        if (wantCraftButCant)
        {
            if (cs.recipes.Count == 0)
            {
                EditorGUILayout.HelpBox("No receipe", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("Can't craft", MessageType.Warning);
            }
        }

        GUI.enabled = true;
    }
}
