using UnityEditor;
using UnityEngine;

namespace GTS.Test
{
    [CustomEditor(typeof(TestClass))]
    public class TestClassEditor : Editor
    {
        private TestClass _testClass;
        SerializedProperty _aGameObject;
        SerializedProperty _aTransform;

        private void OnEnable()
        {
            _testClass = (TestClass)target;
            _aGameObject = serializedObject.FindProperty("aGameObject");
            _aTransform = serializedObject.FindProperty("aTransform");
        }

        public override void OnInspectorGUI()
        {
            // Fields
            _testClass.aBounds = EditorGUILayout.BoundsField("aBounds", _testClass.aBounds);
            _testClass.aBoundsInt = EditorGUILayout.BoundsIntField("aBoundsInt", _testClass.aBoundsInt);
            _testClass.aColor = EditorGUILayout.ColorField("aColor", _testClass.aColor);
            _testClass.aCurve = EditorGUILayout.CurveField("aCurve", _testClass.aCurve);
            _testClass.aDouble = EditorGUILayout.DoubleField("aDouble", _testClass.aDouble);
            // Could also use: DelayedDoubleField.
            _testClass.aanEnum = (AnEnum)EditorGUILayout.EnumPopup("aanEnum", _testClass.aanEnum);
            // Also could use EnumFlagsField.
            _testClass.aFloat = EditorGUILayout.FloatField("aFloat", _testClass.aFloat);
            // Could also use: DelayedFloatField.
            EditorGUILayout.PropertyField(_aGameObject, new GUIContent("A Game Object"), true);
            _testClass.aInt = EditorGUILayout.IntField("aInt", _testClass.aInt);
            // Could also use: DelayedIntField; IntPopup, IntSlider.
            _testClass.aLayer = EditorGUILayout.LayerField("aLayer", _testClass.aLayer);
            _testClass.aLong = EditorGUILayout.LongField("aLong", _testClass.aLong);
            // Omitting aSystemObject from code generation. No applicable entry for System.Object!
            _testClass.aString = EditorGUILayout.TextField("aString", _testClass.aString);
            // Could also use: DelayedTextField.
            _testClass.aBool = EditorGUILayout.Toggle("aBool", _testClass.aBool);
            // Could also use: ToggleLeft.
            EditorGUILayout.PropertyField(_aTransform, new GUIContent("A Transform"), true);
            _testClass.aVector2 = EditorGUILayout.Vector2Field("aVector2", _testClass.aVector2);
            _testClass.aVector2Int = EditorGUILayout.Vector2IntField("aVector2Int", _testClass.aVector2Int);
            _testClass.aVector3 = EditorGUILayout.Vector3Field("aVector3", _testClass.aVector3);
            _testClass.aVector3Int = EditorGUILayout.Vector3IntField("aVector3Int", _testClass.aVector3Int);
            _testClass.aVector4 = EditorGUILayout.Vector4Field("aVector4", _testClass.aVector4);
            // Omitting SomeQuaternion from code generation. No applicable entry for UnityEngine.Quaternion!

            // Properties
            _testClass.aIntProperty = EditorGUILayout.IntField("aIntProperty", _testClass.aIntProperty);
            // Could also use: DelayedIntField; IntPopup, IntSlider.
            _testClass.aStringProperty = EditorGUILayout.TextField("aStringProperty", _testClass.aStringProperty);
            // Could also use: DelayedTextField.

            // Buttons
            if (GUILayout.Button("Do A Thing)"))
            {
                _testClass.DoAThing();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}