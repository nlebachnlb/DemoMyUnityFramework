using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PuzzleGenerator))]
public class PuzzleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PuzzleGenerator puzzle = target as PuzzleGenerator;

        if (DrawDefaultInspector())
        {
            //map.GenerateMap();
        }

        if (GUILayout.Button("Generate Map"))
        {
            puzzle.GenerateMap();
        }
    }
}