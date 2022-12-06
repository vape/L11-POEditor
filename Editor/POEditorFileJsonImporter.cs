using SimpleJSON;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.AssetImporters;

namespace L11.Sync.POEditor.Editor
{
    [ScriptedImporter(0, FileExtension)]
    internal class POEditorFileJsonImporter : POEditorFileImporter
    {
        public const string FileExtension = "l11_poeditorjson";

        protected override Dictionary<string, Term> Parse(string fileName)
        {
            var json = JSON.Parse(File.ReadAllText(fileName));
            var data = new Dictionary<string, Term>();

            foreach (var node in json.AsArray.Values)
            {
                var key = node["term"].Value;
                var term = new Term()
                {
                    Context = node["context"].Value
                };

                var defNode = node["definition"];

                if (defNode.IsString)
                {
                    term.Value = defNode.Value;
                }
                else if (defNode.IsNull)
                {
                    continue;
                }
                else
                {
                    var values = new string[] {
                        defNode["zero"],
                        defNode["other"],
                        defNode["one"],
                        defNode["two"],
                        defNode["few"],
                        defNode["many"] }
                        .Where(s => !string.IsNullOrEmpty(s));

                    term.Value = $"{{0:{string.Join("|", values)}}}";
                }

                data.Add(key, term);
            }

            return data;
        }
    }
}
