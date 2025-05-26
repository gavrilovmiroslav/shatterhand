
using UnityEditor;

using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

[CustomEditor(typeof(Piece))]
public class PieceInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var piece = (Piece)target;
        var texture = AssetPreview.GetAssetPreview(piece.Image);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(texture);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        base.OnInspectorGUI();
    }
}