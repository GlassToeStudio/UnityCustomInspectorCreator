using System.IO;
using UnityEditor;

namespace GTS.InspectorGeneration.Data
{
    public class FileData
    {
        public string FullPath;
        public string FolderPath;
        public string ClassName;
        public string Extension;

        public FileData(UnityEngine.Object activeObject)
        {
            this.FullPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            this.FolderPath = Path.GetDirectoryName(FullPath);
            this.Extension = Path.GetExtension(FullPath);
            this.ClassName = Path.GetFileNameWithoutExtension(FullPath);
        }
    }
}