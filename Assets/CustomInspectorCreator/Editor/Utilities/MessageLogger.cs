//#define LOG_INFO
//#define LOG_WARNING
//#define LOG_ERROR

using UnityEngine;

namespace GTS.InspectorGeneration.Utilities
{
    public class MessageLogger
    {
        public static void LogStartMessage(string className, bool isText)
        {
            if(isText)
            {
                Debug.Log(string.Format("<b><color=blue>Generating a <i>Text</i> file version of the custom Editor :</color> <i>{0}</i></b>", className));
            }
            else
            {
                Debug.Log(string.Format("<b><color=blue>Generating custom <i>inspector</i> for:</color> <i>{0}</i></b>", className));
            }
        }

        public static void LogType(string name)
        {
#if LOG_INFO
            Debug.Log("Type is: " + name);
#endif
        }

        public static void LogFieldOrPropertyInfo(string name, string type, bool isEnum, bool isSerializable)
        {
#if LOG_INFO
            Debug.Log(string.Format("Name: {0}, Type: {1}, IsEnum: {2}, IsSerializable: {3}.", name, type, isEnum, isSerializable));
#endif
        }

        public static void LogMethodName(string name)
        {
#if LOG_INFO
            Debug.Log(string.Format("Member: <color=gree><b>{0}</b></color>", name));
#endif
        }

        public static void LogAttributeType(object a)
        {
#if LOG_INFO
            Debug.Log(string.Format("Attribute: <color=green>{0}</color>", a.GetType()));
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
            Debug.Log(string.Format("<b><color=blue>Custom inspector for:</color> <i>{0}</i></b> is in: <b>{1}/{0}Editor.txt</b>", className, path));
        }

        public static void SuccessTextFileWrite(string path, string className)
        {
            Debug.Log(string.Format("<b><color=blue>Custom inspector for:</color> <i>{0}</i></b> is in: <b>{1}/Editor/{0}Editor.cs</b>", className, path));
        }

        public static void WarningNotMonoBehaviour(string className)
        {
#if LOG_WARNING
            Debug.LogError(string.Format("{0} is not derived from MonoBehaviour. An inspector cannot be created!", className));
#endif
        }

        public static void WarningNotCSScript(string fileExtension)
        {
#if LOG_WARNING
            Debug.LogError(string.Format("Asset is <b><color=red>NOT</color></b> a c# script! It has a \"<b>{0}</b>\" extension!", fileExtension));
#endif
        }

        public static void ErrorOmittedFieldOrProperty(string errorMessage)
        {
#if LOG_ERROR
            Debug.Log(string.Format("<b><color=red>{0}</color></b>", errorMessage));
#endif
        }

        public static void ErrorFailedToCompileClassAt(string path)
        {
            Debug.LogError(string.Format("Failed to compile class at {0}! See output for additional details.", path));
        }

        public static void ErrorCompileErrors(object error)
        {
            Debug.LogError(string.Format("=> {0}", error.ToString()));
        }
    }
}
