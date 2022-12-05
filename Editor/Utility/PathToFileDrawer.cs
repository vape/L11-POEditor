using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace L11.Sync.POEditor.Editor.Utility
{
    [CustomPropertyDrawer(typeof(PathToFileAttribute))]
    internal class PathToFileDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var currentPath = property.stringValue;
            var buttonRect = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));
            var buttonContent = new GUIContent(currentPath ?? "", currentPath);

            if (GUI.Button(buttonRect, buttonContent, GUI.skin.textField))
            {
                string defaultDirectory = String.Empty;
                string defaultName = String.Empty;
                string defaultExtension = (attribute as PathToFileAttribute).Extension;

                if (!string.IsNullOrWhiteSpace(currentPath))
                {
                    var directory = Path.GetDirectoryName(currentPath);
                    if (Directory.Exists(directory))
                    {
                        defaultDirectory = directory;
                    }

                    defaultName = Path.GetFileNameWithoutExtension(currentPath);
                }

                var selectedFile = EditorUtility.SaveFilePanel("Select file", defaultDirectory, defaultName, defaultExtension);
                if (!string.IsNullOrEmpty(selectedFile))
                {
                    property.stringValue = TryConvertToRelative(selectedFile, Application.dataPath);
                }
            }
        }

        private static string TryConvertToRelative(string filePath, string referencePath)
        {
            var fileUri = new Uri(filePath);
            var referenceUri = new Uri(referencePath);

            return Uri.UnescapeDataString(referenceUri.MakeRelativeUri(fileUri).ToString()).Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
