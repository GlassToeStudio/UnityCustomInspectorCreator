using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace GTS.InspectorGeneration.Utilities
{
    public class Generator
    {
        private string tabLevel;
        private bool didHaveProps;
        private string classTypeField;
        private string onEnableBody;

        public string[] Generate(Type type, string fullPath, string className)
        {     
            string varName = GetVarName(className);

            string usings = GenerateUsings(fullPath);
            string namespaceBegin = GenerateNamespaceBegin(type);
            string inspectorAttribute = GenerateInspectorAttribute(className);
            string classDeclarationBegin = GenerateClassDeclarationBegin(className);
            classTypeField = GenerateClassTypeMember(className, varName);   // Might be modified later.
            string onEnableBegin = GenerateOnEnableBegin();
            onEnableBody = GenerateOnEnableBody(varName, className);        // Might be modified later.
            string onEnableEnd = GenerateOnEnableEnd();
            string onInspectorGUIBegin = GenerateOnInspectorGUIBegin();
            string onInspectorBodyFields = GenerateOnInspectorGUIBodyFields(type, varName);
            string onInspectorBodyProperties = GenerateOnInspectorGUIBodyProperties(type, varName);
            string onInspectorConetxtMenuButtons = GenerateOnInspectorGUIBodyMethodButtons(type, varName);
            string onInspectorGUIEnd = GenerateOnInspectorGUIEnd();
            string classDeclarationEnd = GenerateClassDeclarationEnd();
            string nameSpaceEnd = GenerateNamespaceEnd(type);

            string[] results = {
                usings,
                namespaceBegin,
                inspectorAttribute,
                classDeclarationBegin,
                classTypeField,
                onEnableBegin,
                onEnableBody,
                onEnableEnd,
                onInspectorGUIBegin,
                onInspectorBodyFields,
                onInspectorBodyProperties,
                onInspectorConetxtMenuButtons,
                onInspectorGUIEnd,
                classDeclarationEnd,
                nameSpaceEnd
            };

            return results;
        }

        #region Generate
        // Header
        private string GenerateUsings(string path)
        {
            StringBuilder sb = new StringBuilder("using UnityEditor;\n");
            using (var reader = new StreamReader(path))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    // Could add unintended using statements, example here in the StreamReader.
                    if (line.Contains("using") && line.Contains(";")) //Test a 'using' fix.
                    {
                        sb.Append($"{line}\n");
                    }
                }
            }
            return sb.ToString();
        }

        private string GenerateNamespaceBegin(Type type)
        {
            string myNamespace = type.Namespace;
            if (string.IsNullOrEmpty(myNamespace))
            {
                return myNamespace;
            }
            else
            {
                Indent();
                return $"\nnamespace {myNamespace}\n{{";
            }
        }

        private string GenerateInspectorAttribute(string className)
        {
            return $"\n{tabLevel}[CustomEditor(typeof({className}))]\n";
        }

        private string GenerateClassDeclarationBegin(string className)
        {
            return $"{tabLevel}public class {className}Editor : Editor\n{tabLevel}{{\n";
        }

        // Members
        private string GenerateClassTypeMember(string className, string varName)
        {
            Indent();
            return $"{tabLevel}private {className} {varName};\n";
        }

        // OnEnable
        private string GenerateOnEnableBegin()
        {
            string onEnableStart = $"\n{tabLevel}private void OnEnable()\n";
            string openBrace = $"{tabLevel}{{\n";

            return $"{onEnableStart}{openBrace}";
        }

        private string GenerateOnEnableBody(string varName, string className)
        {
            Indent();
            string body = $"{tabLevel}{varName} = ({className})target;\n";
            Dedent();
            return $"{body}";
        }

        private string GenerateOnEnableEnd()
        {
            string closeBrace = $"{tabLevel}}}\n\n";
            return $"{closeBrace}";
        }

        //OnInspectorGUI
        private string GenerateOnInspectorGUIBegin()
        {
            return $"{tabLevel}public override void OnInspectorGUI()\n{tabLevel}{{\n";
        }

        private string GenerateOnInspectorGUIBodyFields(Type type, string varName)
        {
            Indent();

            StringBuilder sb = new StringBuilder($"{tabLevel}// Fields\n");

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            var fields = type.GetFields(bindingFlags);
            foreach (var f in fields)
            {
                sb.Append(ParseFieldsAndProperties(varName, f.Name, f.FieldType.ToString(), f.FieldType.IsEnum, f.FieldType.IsSerializable, f.FieldType));
            }

            return sb.ToString();
        }

        private string GenerateOnInspectorGUIBodyProperties(Type type, string varName)
        {
            StringBuilder sb = new StringBuilder($"\n{tabLevel}// Properties\n");

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            var properties = type.GetProperties(bindingFlags);
            foreach (var p in properties)
            {
                sb.Append(ParseFieldsAndProperties(varName, p.Name, p.PropertyType.ToString(), p.PropertyType.IsEnum, p.PropertyType.IsSerializable, p.PropertyType));
            }

            return sb.ToString();
        }

        private string GenerateOnInspectorGUIBodyMethodButtons(Type type, string varName)
        {
            StringBuilder sb = new StringBuilder($"\n{tabLevel}// Buttons");

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic;

            var members = type.GetMethods(bindingFlags);
            foreach (var m in members)
            {
                MessageLogger.LogMethodName(m.ToString());

                var attributes = m.GetCustomAttributes(typeof(ContextMenu), false);
                foreach (var a in attributes)
                {
                    MessageLogger.LogAttributeType(a);

                    string buttonStart = $"\n{tabLevel}if(GUILayout.Button(\"{GetLabelName(m.Name)}\"))\n{tabLevel}{{\n";
                    Indent();
                    string buttonBody = $"{tabLevel}{varName}.{m.Name}();\n";
                    Dedent();
                    string buttonEnd = $"{tabLevel}}}\n";

                    sb.Append($"{buttonStart}{buttonBody}{buttonEnd}");

                }
            }

            return sb.ToString();
        }

        private string GenerateOnInspectorGUIEnd()
        {
            string result = string.Empty;
            if (didHaveProps)
            {
                result = $"\n{tabLevel}serializedObject.ApplyModifiedProperties();\n";
            }

            Dedent();

            result = $"{result}{tabLevel}}}\n";
            return result;
        }
        
        // Finialize
        private string GenerateClassDeclarationEnd()
        {
            Dedent();
            return $"{tabLevel}}}\n";
        }

        private string GenerateNamespaceEnd(Type type)
        {
            Dedent();
            string myNamespace = type.Namespace;
            if (string.IsNullOrEmpty(myNamespace))
            {
                return string.Empty;
            }
            else
            {
                return "}";
            }
        }
        #endregion

        // This bad boy needs work
        private string ParseFieldsAndProperties(string varName, string name, string type, bool isEnum, bool isSerializable, Type t)
        {
            MessageLogger.LogFieldOrPropertyInfo(name, type, isEnum, isSerializable);

            StringBuilder layoutGenerator = new StringBuilder();

            // Special cases:
            if (isEnum)
            {
                layoutGenerator.Append(GenerateEditorGUILayoutEnumPopup(varName, name, type));
                return layoutGenerator.ToString();
            }

            string result = LookUpFieldType(type);

            if (string.IsNullOrEmpty(result))
            {
                // These will not be written for the custom Inspector!
                if (type.Equals("System.Object") || type.Contains("Dict") || type.Contains("Quaternion"))
                {
                    string errorMessage = GenerateErrorMessage(name, type);

                    layoutGenerator.Append(errorMessage);

                    MessageLogger.ErrorOmittedFieldOrProperty(errorMessage.Trim());
                }
                else if (!isSerializable)
                {
                    if (IsMonoBehaviour(t))
                    {
                        layoutGenerator.Append(GoBackAndAddSerializedProperty(name));
                    }
                    else if (IsScriptableObject(t))
                    {
                        layoutGenerator.Append(GoBackAndAddSerializedProperty(name));
                    }
                }
                else
                {
                    layoutGenerator.Append(GoBackAndAddSerializedProperty(name));
                }
            }
            else if (result.Equals("_USEPROP_"))
            {
                layoutGenerator.Append(GoBackAndAddSerializedProperty(name));
            }
            // Default case
            else
            {
                layoutGenerator.Append(GenerateEditorGUILayoutForGivenType(varName, name, result, type));
            }

            return layoutGenerator.ToString();
        }

        /// <summary>
        /// This method will edit the two strings for ClassFieldType and OnEnableBody as well as add PropertyField to OnInspectorGUI.
        /// </summary>
        private string GoBackAndAddSerializedProperty(string name)
        {
            didHaveProps = true;

            string ammendMemberDeclarations = AmmendMemberDeclarations(name);
            classTypeField = $"{classTypeField}{ammendMemberDeclarations}";

            string ammendOnEnable = AmmendOnEnable(name);
            onEnableBody = $"{onEnableBody}{ammendOnEnable}";

            return GenerateEditorGUILayoutForSerializedProperty(name);
        }

        #region OnInspectorGUI method body
        // A special case
        private string GenerateEditorGUILayoutEnumPopup(string varName, string name, string type)
        {
            string result = $"{tabLevel}{varName}.{name} = ({type.Split('.').Last()})EditorGUILayout.EnumPopup(\"{name}\", {varName}.{name});\n{tabLevel}// Also could use EnumFlagsField.\n";
            return result;
        }
        // Default
        private string GenerateEditorGUILayoutForGivenType(string varName, string name, string result, string type)
        {
            string toAdd = string.Empty;
            string comment = LookUpComments(type);
            if (!string.IsNullOrEmpty(comment))
            {
                toAdd = $"{tabLevel}{comment}";
            }
            string r = $"{tabLevel}{varName}.{name} = EditorGUILayout.{result}(\"{name}\", {varName}.{name});\n{toAdd}";
            return r;
        }
        // A special case
        private string GenerateEditorGUILayoutForSerializedProperty(string name)
        {
            return $"{tabLevel}EditorGUILayout.PropertyField({GetVarName(name)}, new GUIContent(\"{GetLabelName(name)}\"), true);\n";
        }

        private string GenerateErrorMessage(string name, string type)
        {
            return $"{tabLevel}// Omitting {name} from code generation. No applicable entry for {type}!\n";
        }
        #endregion

        #region Ammendments
        private string AmmendOnEnable(string name)
        {
            // m_IntProp = serializedObject.FindProperty("m_MyInt")
            return $"{tabLevel}{GetVarName(name)} = serializedObject.FindProperty(\"{name}\");\n";
        }

        private string AmmendMemberDeclarations(string name)
        {
            Dedent();
            string result = $"{tabLevel}SerializedProperty {GetVarName(name)};\n";
            Indent();

            return result;
        }
        #endregion

        #region Helpers
        private void Indent()
        {
            tabLevel = $"{tabLevel }\t";
        }

        private void Dedent()
        {
            if(tabLevel.Length > 0)
            {
                tabLevel = tabLevel.Remove(tabLevel.Length - 1);
            }
        }

        private string GetVarName(string className)
        {
            return $"_{Char.ToLowerInvariant(className[0])}{className.Substring(1)}";
        }

        private string GetLabelName(string name)
        {
            string result = string.Empty;
            string[] split = System.Text.RegularExpressions.Regex.Split(name, @"(?<!^)(?=[A-Z])");
            if (split.Length != 0)
            {
                foreach (var s in split)
                {
                    result = $"{result} {s}";
                }

                result = result.Trim();
                result = $"{Char.ToUpperInvariant(result[0])}{result.Substring(1)}";
                return result;
            }

            return name;
        }

        private string LookUpFieldType(string type)
        {
            string value = String.Empty;
            FieldDrawers.TryGetValue(type, out value);
            return value;
        }

        private string LookUpComments(string type)
        {
            string value = String.Empty;
            Comments.TryGetValue(type, out value);
            return value;
        }
        #endregion

        #region Error Checking
        private bool IsMonoBehaviour(Type t)
        {
            return t.IsSubclassOf(typeof(MonoBehaviour));
        }

        private bool IsScriptableObject(Type t)
        {
            return t.IsSubclassOf(typeof(ScriptableObject));
        }
        #endregion

        #region Dictionaries
        Dictionary<string, string> FieldDrawers = new Dictionary<string, string>(){
            { "UnityEngine.Bounds", "BoundsField"},
            { "UnityEngine.BoundsInt", "BoundsIntField"},
            { "UnityEngine.Color", "ColorField"},
            { "UnityEngine.AnimationCurve", "CurveField"},
            { "System.Double", "DoubleField"},
            { "System.Single", "FloatField"},
            { "System.Int32", "IntField"}, // IntPopup, IntSlider
            { "UnityEngine.LayerMask", "LayerField"},
            { "System.Int64", "LongField"},
            { "System.String","TextField"},
            { "System.Boolean", "Toggle"},
            { "UnityEngine.Rect", "RectField"},
            { "UnityEngine.RectInt", "RectIntField"},
            { "UnityEngine.Vector2", "Vector2Field"},
            { "UnityEngine.Vector2Int", "Vector2IntField"},
            { "UnityEngine.Vector3", "Vector3Field"},
            { "UnityEngine.Vector3Int", "Vector3IntField"},
            { "UnityEngine.Vector4", "Vector4Field"},
            { "UnityEngine.GameObject", "_USEPROP_"},
            { "UnityEngine.Transform", "_USEPROP_"},
            // Add GradientField
        };
        Dictionary<string, string> Comments = new Dictionary<string, string>(){
            { "System.Double", "// Could also use: DelayedDoubleField.\n"},
            { "System.Single", "// Could also use: DelayedFloatField.\n"},
            { "System.Int32", "// Could also use: DelayedIntField; IntPopup, IntSlider.\n"},
            { "System.String", "// Could also use: DelayedTextField.\n"},
            { "System.Boolean", "// Could also use: ToggleLeft.\n"},
        };
        #endregion
    }
}