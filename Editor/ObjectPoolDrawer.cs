using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Atlas
{
    [CustomPropertyDrawer(typeof(ObjectPool.Pool))]
    public class ObjectPoolDrawer : PropertyDrawer
    {
        const int sizeWidth = 50;
        public override void OnGUI(Rect pos,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            SerializedProperty prefab = property.FindPropertyRelative("prefab");
            SerializedProperty size = property.FindPropertyRelative("size");
            EditorGUI.BeginProperty(pos, label, property);

            EditorGUI.PropertyField(
                new Rect(pos.x, pos.y, pos.width - sizeWidth, pos.height),
                prefab,
                GUIContent.none);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(
                new Rect(pos.width - sizeWidth, pos.y, sizeWidth, pos.height),
                size,
                GUIContent.none);
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}


