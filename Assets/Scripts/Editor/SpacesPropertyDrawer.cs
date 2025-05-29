using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Spaces))]
public class SpacesPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect a_Rect, SerializedProperty a_Property, GUIContent a_Label)
    {
        float lh = EditorGUIUtility.singleLineHeight;

        Rect labelRect = new(a_Rect.x, a_Rect.y, a_Rect.width - lh * 7f, a_Rect.height);
        EditorGUI.LabelField(labelRect, a_Label);

        float boolStartX = a_Rect.x + labelRect.width;
        Rect boolRect = new(boolStartX, a_Rect.y, lh, lh);
        for (int y = 0; y < 7; y++)
        {
            boolRect.x = boolStartX;
            for (int x = 0; x < 7; x++)
            {
                int currentBool = x + y * 7 + 1;
                if (x == 3 && y == 3)
                {
                    EditorGUI.BeginDisabledGroup(true);
                }
                EditorGUI.PropertyField(boolRect, a_Property.FindPropertyRelative("space" + currentBool), GUIContent.none);
                if (x == 3 && y == 3)
                {
                    EditorGUI.EndDisabledGroup();
                }
                boolRect.x += boolRect.height;
            }

            boolRect.y += boolRect.width;
        }
    }

    public override float GetPropertyHeight(SerializedProperty a_Property, GUIContent a_Label)
    {
        return EditorGUIUtility.singleLineHeight * 7f;
    }
}