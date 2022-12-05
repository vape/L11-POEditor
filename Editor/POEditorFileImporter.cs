using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace L11.Sync.POEditor.Editor
{
    internal abstract class POEditorFileImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var terms = Parse(ctx.assetPath);

            var obj = ScriptableObject.CreateInstance<TermDictionaryObject>();
            obj.Terms = new TermDictionary();
            
            foreach (var kv in terms)
            {
                obj.Terms.Add(kv.Key, kv.Value);
            }

            ctx.AddObjectToAsset("main", obj);
            ctx.SetMainObject(obj);
        }

        protected abstract Dictionary<string, Term> Parse(string fileName);
    }
}
