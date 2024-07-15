using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralDrunkYardGenerator))]

public class ProceduralDrunkYardEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        var proceduralDrunkyard=(ProceduralDrunkYardGenerator)target;

        if(GUILayout.Button("Generate Rooms"))
        {
            proceduralDrunkyard.GenerateRoomsFromEditor();

            EditorUtility.SetDirty(proceduralDrunkyard);
        }

        

    }
}
