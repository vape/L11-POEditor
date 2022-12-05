using System.IO;
using UnityEditor;
using UnityEngine;

namespace L11.Sync.POEditor.Editor
{
    internal class POEditorSyncSettings : ScriptableObject
    {
        private static POEditorSyncSettings instance;
        public static POEditorSyncSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LoadOrCreate();
                }

                return instance;
            }
        }

        public string POEditorApiKey;
        public string POEditorProjectId;
        public POEditorImportEntry[] Entries;

        private static string GetFilePath()
        {
            return Path.Combine("ProjectSettings", "L11POEditorSettings.json");
        }

        private static POEditorSyncSettings LoadOrCreate()
        {
            var filePath = GetFilePath();
            var instance = ScriptableObject.CreateInstance<POEditorSyncSettings>();
            instance.hideFlags = HideFlags.DontSave;

            if (File.Exists(filePath))
            {
                EditorJsonUtility.FromJsonOverwrite(File.ReadAllText(filePath), instance);
            }
            else
            {
                File.WriteAllText(filePath, EditorJsonUtility.ToJson(instance));
            }

            return instance;
        }

        public void Save()
        {
            File.WriteAllText(GetFilePath(), EditorJsonUtility.ToJson(instance));
        }
    }
}