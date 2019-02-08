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
        private string tabLevel = "";
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
                        sb.Append(string.Format("{0}\n", line));
                    }
                }
            }
            return sb.ToString();
        }

        public string GetNameSpace(string path)
        {
            //Debug.Log("generating usings");

            string nameSpace = String.Empty;
            using (var reader = new StreamReader(path))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    // Could add unintended using statements, example here in the StreamReader.
                    if (line.Contains("namespace")) //Test a 'using' fix.
                    {
                        nameSpace = line.Replace("namespace", "").Replace("{", "").Trim();
                    }
                }
            }
            return nameSpace;
        }

        private string GenerateNamespaceBegin(Type type)
        {
            //Debug.Log("generating NameSpace");
            string myNamespace = type.Namespace;
            if (string.IsNullOrEmpty(myNamespace))
            {
                return myNamespace;
            }
            else
            {
                Indent();
                return ("\nnamespace " + myNamespace + "\n{");
            }
        }

        private string GenerateInspectorAttribute(string className)
        {
            //Debug.Log("generating Inspector Attribute");

            return string.Format("\n{0}[CustomEditor(typeof({1}))]\n", tabLevel, className);
        }

        private string GenerateClassDeclarationBegin(string className)
        {
            //Debug.Log("generating Class Declaration Begin");

            return (tabLevel + "public class " + className + "Editor : Editor\n" + tabLevel + "{\n");
        }

        // Members
        private string GenerateClassTypeMember(string className, string varName)
        {
            //Debug.Log("generating Class Type Member");
            return (string.Format("{0}private {1} {2}; \n", tabLevel, className, varName));
        }

        // OnEnable
        private string GenerateOnEnableBegin()
        {
            //Debug.Log("generating OnEnable Begin");
            string onEnableStart = (string.Format("\n{0}private void OnEnable()\n", tabLevel));
            string openBrace = (tabLevel + "{\n");

            return (string.Format("{0}{1}", onEnableStart, openBrace));
        }

        private string GenerateOnEnableBody(string varName, string className)
        {
            //Debug.Log("generating OnEnable Body");
            Indent();
            string body = string.Format("{0}{1} = ({2})target; \n", tabLevel, varName, className);
            Dedent();
            return string.Format("{0}", body);
        }

        private string GenerateOnEnableEnd()
        {
            //Debug.Log("generating OnEnable End");
            string closeBrace = tabLevel + "}\n\n";
            return string.Format("{0}", closeBrace);
        }

        //OnInspectorGUI
        private string GenerateOnInspectorGUIBegin()
        {
            //Debug.Log("generating OnInspectorGUI Begin");
            return (tabLevel + "public override void OnInspectorGUI()\n" + tabLevel + "{\n");
        }

        private string GenerateOnInspectorGUIBodyFields(Type type, string varName)
        {
            //Debug.Log("generating OnInspectorGUI Body Fields");
            Indent();

            StringBuilder sb = new StringBuilder(string.Format("{0}// Fields\n", tabLevel));

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
            //Debug.Log("generating OnInspectorGUI Body Properties");

            StringBuilder sb = new StringBuilder(string.Format("\n{0}// Properties\n", tabLevel));

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
            //Debug.Log("generating OnInspectorGUI Body Buttons");

            StringBuilder sb = new StringBuilder(string.Format("\n{0}// Buttons", tabLevel));

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic;

            var members = type.GetMethods(bindingFlags);
            foreach (var m in members)
            {
                MessageLogger.LogMethodName(m.ToString());

                var attributes = m.GetCustomAttributes(typeof(ContextMenu), false);
                foreach (var a in attributes)
                {
                    MessageLogger.LogAttributeType(a);

                    string buttonStart = ("\n" + tabLevel + "if(GUILayout.Button(\"" + GetLabelName(m.Name) + ")\"))\n" + tabLevel + "{\n");
                    Indent();
                    string buttonBody = string.Format("{0}{1}.{2}(); \n", tabLevel, varName, m.Name);
                    Dedent();
                    string buttonEnd = tabLevel + "}\n";

                    sb.Append(string.Format("{0}{1}{2}", buttonStart, buttonBody, buttonEnd));

                }
            }

            return sb.ToString();
        }

        private string GenerateOnInspectorGUIEnd()
        {
            string result = string.Empty;
            if (didHaveProps)
            {
                result = string.Format("\n{0}serializedObject.ApplyModifiedProperties(); \n", tabLevel);
            }

            Dedent();

            result = result + tabLevel + "}\n";
            return result;
        }

        // Finialize
        private string GenerateClassDeclarationEnd()
        {
            Dedent();
            return tabLevel + "}\n";
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
            classTypeField = string.Format("{0}{1}", classTypeField, ammendMemberDeclarations);

            string ammendOnEnable = AmmendOnEnable(name);
            onEnableBody = string.Format("{0}{1}", onEnableBody, ammendOnEnable);

            return GenerateEditorGUILayoutForSerializedProperty(name);
        }

        #region OnInspectorGUI method body
        // A special case
        private string GenerateEditorGUILayoutEnumPopup(string varName, string name, string type)
        {
            string result = string.Format("{0}{1}.{2} = ({3})EditorGUILayout.EnumPopup(\"{2}\", {1}.{2}); \n{0}// Also could use EnumFlagsField.\n", tabLevel, varName, name, type.Split('.').Last());
            return result;
        }
        // Default
        private string GenerateEditorGUILayoutForGivenType(string varName, string name, string result, string type)
        {
            string toAdd = string.Empty;
            string comment = LookUpComments(type);
            if (!string.IsNullOrEmpty(comment))
            {
                toAdd = string.Format("{0}{1}", tabLevel, comment);
            }
            string r = string.Format("{0}{1}.{2} = EditorGUILayout.{3}(\"{2}\", {1}.{2}); \n{4}", tabLevel, varName, name, result, toAdd);
            return r;
        }
        // A special case
        private string GenerateEditorGUILayoutForSerializedProperty(string name)
        {
            return string.Format("{0}EditorGUILayout.PropertyField({1}, new GUIContent(\"{2}\"), true); \n", tabLevel, GetVarName(name), GetLabelName(name));
        }

        private string GenerateErrorMessage(string name, string type)
        {
            return string.Format("{0}// Omitting {1} from code generation. No applicable entry for {2}!\n", tabLevel, name, type);
        }
        #endregion

        #region Ammendments
        private string AmmendOnEnable(string name)
        {
            // m_IntProp = serializedObject.FindProperty("m_MyInt")
            return string.Format("{0}{1} = serializedObject.FindProperty(\"{2}\"); \n", tabLevel, GetVarName(name), name);
        }

        private string AmmendMemberDeclarations(string name)
        {
            Dedent();
            string result = string.Format("{0}SerializedProperty {1}; \n", tabLevel, GetVarName(name));
            Indent();

            return result;
        }
        #endregion

        #region Helpers
        private void Indent()
        {
            tabLevel += "    ";
        }

        private void Dedent()
        {
            if (tabLevel.Length > 0)
            {
                tabLevel = tabLevel.Remove(tabLevel.Length - 1);
            }
        }

        private string GetVarName(string className)
        {
            return string.Format("_{0}{1}", Char.ToLowerInvariant(className[0]), className.Substring(1));
        }

        private string GetLabelName(string name)
        {
            string result = string.Empty;
            string[] split = System.Text.RegularExpressions.Regex.Split(name, @"(?<!^)(?=[A-Z])");
            if (split.Length != 0)
            {
                foreach (var s in split)
                {
                    result = string.Format("{0} {1}", result, s);
                }

                result = result.Trim();
                result = string.Format("{0}{1}", Char.ToUpperInvariant(result[0]), result.Substring(1));
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