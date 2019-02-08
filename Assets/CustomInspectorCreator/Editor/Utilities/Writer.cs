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

                File.WriteAllText(string.Format("{0}/Editor/{1}Editor.cs", path, className), content);
                MessageLogger.SuccessTextFileWrite(path, className);
            }
            else
            {
                File.WriteAllText(string.Format("{0}/{1}Editor.txt", path, className), content);
                MessageLogger.SuccessEditorFileWrite(path, className);
            }

            AssetDatabase.Refresh();
        }

        public void WriteToFile(string path, string className, string content)
        {
            File.WriteAllText(string.Format("{0}/{1}_Converted.txt", path, className), content);
            //MessageLogger.SuccessEditorFileWrite(path, className);

            AssetDatabase.Refresh();
        }

        private static void CreateEditorFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(string.Format("{0}/{1}", path, "Editor")))
            {
                AssetDatabase.CreateFolder(path, "Editor");
            }
        }
    }
}
