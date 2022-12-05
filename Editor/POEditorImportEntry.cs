using L11.Sync.POEditor.Editor.Utility;
using System;

namespace L11.Sync.POEditor.Editor
{
    [Serializable]
    internal class POEditorImportEntry
    {
        public string Language;
        [PathToFile(Extension = POEditorFileJsonImporter.FileExtension)]
        public string FilePath;
    }
}
