using System.IO;
using UnityEditor;

namespace GTS.InspectorGeneration.Utilities
{
    public class Writer
    {
        public void WriteToFile(string path, string className, string content, bool isText)
        {
            if(isText == false)
            {
                CreateEditorFolder(path);

                File.WriteAllText($"{path}/Editor/{className}Editor.cs", content);
                MessageLogger.SuccessTextFileWrite(path, className);
            }
            else
            {
                File.WriteAllText($"{path}/{className}Editor.txt", content);
                MessageLogger.SuccessEditorFileWrite(path, className);
            }

            AssetDatabase.Refresh();
        }

        public void WriteToFile(string path, string className, string content)
        {
            File.WriteAllText($"{path}/{className}_Converted.txt", content);
            //MessageLogger.SuccessEditorFileWrite(path, className);

            AssetDatabase.Refresh();
        }

        private static void CreateEditorFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder($"{path}/Editor"))
            {
                AssetDatabase.CreateFolder($"{path}", "Editor");
            }
        }
    }
}