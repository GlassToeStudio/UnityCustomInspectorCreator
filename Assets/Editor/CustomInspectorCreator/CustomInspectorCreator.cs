#define MAKE_TEXT

using System;
using UnityEditor;
using UnityEngine;
using GTS.InspectorGeneration.Data;
using GTS.InspectorGeneration.Utilities;

namespace GTS.InspectorGeneration
{
    public class CustomInspectorCreator : Editor
    {
        #region Main
        private static void Generate(bool isText)
        {
            if (Selection.activeObject == null)
            {
                return;
            }

            FileData fileData = new FileData(Selection.activeGameObject);

            if (!IsCSharpScript(fileData.Extension))
            {
                return;
            }

            MessageLogger.LogStartMessage(fileData.ClassName, isText);

            Type type = new Compiler().GetTypeForCompiledClassAtPath(fileData.FullPath);

            if (type == null)
            {
                return;
            }

            if (IsMonoBehaviour(type, fileData.ClassName) == false)
            {
                return;
            }

            string[] generatedCode = new Generator().Generate(type, fileData.FullPath, fileData.ClassName);

            string code = new Builder().Build(generatedCode);

            new Writer().WriteToFile(fileData.FolderPath, fileData.ClassName, code, isText);
        }
        #endregion

        #region ErrorChecking
        private static bool IsCSharpScript(string fileExtension)
        {
            if (fileExtension.Equals(".cs") == false)
            {
                MessageLogger.WarningNotCSScript(fileExtension);
                return false;
            }
            return true;
        }

        private static bool IsCSharpScriptNoMessage(string fileExtension)
        {
            if (fileExtension.Equals(".cs") == false)
            {
                return false;
            }
            return true;
        }

        private static bool IsMonoBehaviour(Type type, string className)
        {
            if (type.IsSubclassOf(typeof(MonoBehaviour)) == false)
            {
                MessageLogger.WarningNotMonoBehaviour(className);
                return false;
            }

            return true;
        }

        private static bool Validate()
        {
            if (Selection.activeObject == null)
            {
                return false;
            }
            else
            {
                FileData fileData;
                try
                {
                    fileData = new FileData(Selection.activeGameObject);
                }
                catch
                {
                    return false;
                }

                return IsCSharpScriptNoMessage(fileData.Extension);
            }
        }
        #endregion

        #region Toolbar Menus
        [MenuItem("GTS/InspectorGeneration/GenerateEditor %g", priority = 1)]
        public static void GenerateInspectorScript()
        {
            Generate(false);
        }

        [MenuItem("Assets/InspectorGeneration/GenerateEditor %g")]
        public static void GenerateInspectorScriptAssetMenu()
        {
            Generate(false);
        }
        #endregion

        #region Validate
        [MenuItem("GTS/InspectorGeneration/GenerateEditor %g", priority = 1, validate = true)]
        public static bool ValidateGenerateInspectorScript()
        {
            return Validate();
        }

        [MenuItem("Assets/InspectorGeneration/GenerateEditor %g", validate = true)]
        public static bool ValidateGenerateInspectorScriptAssetMenu()
        {
            return Validate();
        }
        #endregion

#if MAKE_TEXT
        [MenuItem("GTS/InspectorGeneration/GenerateTextFile", priority = 1)]
        public static void GenerateInspectorText()
        {
            Generate(true);
        }
        [MenuItem("Assets/InspectorGeneration/GenerateTextFile")]
        public static void GenerateInspectorTextAssetMenu()
        {
            Generate(true);
        }
        [MenuItem("GTS/InspectorGeneration/GenerateTextFile", priority = 1, validate = true)]
        public static bool ValidateGenerateInspectorText()
        {
            return Validate();
        }
        [MenuItem("Assets/InspectorGeneration/GenerateTextFile", validate = true)]
        public static bool ValidateGenerateInspectorTextAssetMenu()
        {
            return Validate();
        }
#endif
    }
}