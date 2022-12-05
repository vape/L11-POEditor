using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace L11.Sync.POEditor.Editor
{
    internal class POEditorSyncSettingsProvider : SettingsProvider
    {
        private static class Styles
        {
            public static readonly GUIStyle VerticalStyle;

            static Styles()
            {
                VerticalStyle = new GUIStyle(EditorStyles.inspectorFullWidthMargins);
                VerticalStyle.margin = new RectOffset(8, 4, 10, 10);
            }
        }

        [SettingsProvider]
        public static SettingsProvider RegisterSettingsProvider()
        {
            return new POEditorSyncSettingsProvider("L11/POEditor Sync");
        }

        private bool importing;
        private POEditorSyncSettings target;
        private SerializedObject serializedObject;

        public POEditorSyncSettingsProvider(string path)
            : base(path, SettingsScope.Project)
        { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            target = POEditorSyncSettings.Instance;
            serializedObject = new SerializedObject(target);
        }

        public override void OnGUI(string searchContext)
        {
            using (new EditorGUILayout.VerticalScope(Styles.VerticalStyle))
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(target.POEditorApiKey)), new GUIContent("API Key"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(target.POEditorProjectId)), new GUIContent("Project ID"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(target.Entries)));

                if (changeScope.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                    target.Save();
                }

                using (new EditorGUI.DisabledGroupScope(importing))
                {
                    if (GUILayout.Button("Import"))
                    {
                        ImportAll();
                    }
                }
            }
        }

        private async void ImportAll()
        {
            importing = true;

            var importer = new POEditorImporter();

            foreach (var entry in target.Entries)
            {
                if (string.IsNullOrEmpty(entry.FilePath))
                {
                    continue;
                }

                var result = await importer.ImportAsync(target, entry);
                if (result.Success)
                {
                    var path = Path.ChangeExtension(entry.FilePath, POEditorFileJsonImporter.FileExtension);
                    File.WriteAllText(path, result.Json);
                    AssetDatabase.ImportAsset(path);
                }
            }

            importing = false;
        }
    }
}
