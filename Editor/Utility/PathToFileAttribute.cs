using System;
using UnityEngine;

namespace L11.Sync.POEditor.Editor.Utility
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class PathToFileAttribute : PropertyAttribute
    {
        public string Extension = String.Empty;
    }
}
