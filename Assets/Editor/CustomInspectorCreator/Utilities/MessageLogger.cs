#define LOG_INFO
#define LOG_WARNING
#define LOG_ERROR

using UnityEngine;

namespace GTS.InspectorGeneration.Utilities
{
    public class MessageLogger
    {
        public static void LogStartMessage(string className, bool isText)
        {
            if(isText)
            {
                Debug.Log($"<b><color=blue>Generating a <i>Text</i> file version of the custom Editor :</color> <i>{className}</i></b>");
            }
            else
            {
                Debug.Log($"<b><color=blue>Generating custom <i>inspector</i> for:</color> <i>{className}</i></b>");
            }
        }

        public static void LogFieldOrPropertyInfo(string name, string type, bool isEnum, bool isSerializable)
        {
#if LOG_INFO
            Debug.Log($"Name: {name}, Type: {type}, IsEnum: {isEnum}, IsSerializable: {isSerializable}.");
#endif
        }

        public static void LogMethodName(string name)
        {
#if LOG_INFO
            Debug.Log($"Member: <color=gree><b>{name}</b></color>");
#endif
        }

        public static void LogAttributeType(object a)
        {
#if LOG_INFO
            Debug.Log($"Attribute: <color=green>{a.GetType()}</color>");
#endif
        }

        public static void SuccessBuildSucceded()
        {
#if LOG_INFO
            Debug.Log("Build Succeeded!");
#endif
        }

        public static void SuccessEditorFileWrite(string path, string className)
        {
            Debug.Log($"<b><color=blue>Custom inspector for:</color> <i>{className}</i></b> is in: <b>{$"{path}/{className}Editor.txt"}</b>");
        }

        public static void SuccessTextFileWrite(string path, string className)
        {
            Debug.Log($"<b><color=blue>Custom inspector for:</color> <i>{className}</i></b> is in: <b>{$"{path}/Editor/{className}Editor.cs"}</b>");
        }

        public static void WarningNotMonoBehaviour(string className)
        {
#if LOG_WARNING
            Debug.LogError($"{className} is not derived from MonoBehaviour. An inspector cannot be created!");
#endif
        }

        public static void WarningNotCSScript(string fileExtension)
        {
#if LOG_WARNING
            Debug.LogError($"Asset is <b><color=red>NOT</color></b> a c# script! It has a \"<b>{fileExtension}</b>\" extension!");
#endif
        }

        public static void ErrorOmittedFieldOrProperty(string errorMessage)
        {
#if LOG_ERROR
            Debug.Log($"<b><color=red>{errorMessage}</color></b>");
#endif
        }

        public static void ErrorFailedToCompileClassAt(string path)
        {
            Debug.LogError($"Failed to compile class at {path}! See output for additional details.");
        }

        public static void ErrorCompileErrors(object error)
        {
            Debug.LogError($"=> {error.ToString()}");
        }
    }
}
