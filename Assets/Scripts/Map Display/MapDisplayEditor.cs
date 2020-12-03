using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// - Goatbandit

[CustomEditor (typeof (MapDisplay))]
public class MapDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapDisplay mapDisplay = (MapDisplay) target;

        if (DrawDefaultInspector())
        {
            if (mapDisplay.autoUpdate)
            {
                mapDisplay.Generate();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapDisplay.Generate();
        }
    }
}

// - Goatbandit
