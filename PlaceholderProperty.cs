using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Modules;
using UnityEngine;

internal class PlaceholderProperty : DefaultPluginImporterExtension.Property
{
    private GUIContent[] _placeHolders;
    private GetOrignalPluginPath getOriginalPluginPath;
    private const string kNone = "(none)";
    private int selectedIndex;

    internal PlaceholderProperty(GUIContent name, string key, string buildTargetName, GetOrignalPluginPath getOriginalPluginPath) : base(name, key, "", buildTargetName)
    {
        this._placeHolders = null;
        this.selectedIndex = -1;
        this.getOriginalPluginPath = getOriginalPluginPath;
    }

    internal override void OnGUI(PluginImporterInspector inspector)
    {
        if (!((PluginImporter) inspector.target).isNativePlugin)
        {
            this.selectedIndex = EditorGUILayout.Popup(base.name, this.selectedIndex, this.placeHolders, new GUILayoutOption[0]);
            if ((this.selectedIndex >= 0) && (this.selectedIndex < this.placeHolders.Length))
            {
                string text = this.placeHolders[this.selectedIndex].text;
                if (text == "(none)")
                {
                    this.selectedIndex = -1;
                    text = "";
                }
                base.value = text;
            }
            else
            {
                base.value = "";
            }
        }
    }

    internal override void Reset(PluginImporterInspector inspector)
    {
        base.Reset(inspector);
        this._placeHolders = null;
        this.ResolveIndex();
    }

    internal void ResolveIndex()
    {
        this.selectedIndex = -1;
        int num = 0;
        foreach (GUIContent content in this.placeHolders)
        {
            if (((string) base.value) == content.text)
            {
                this.selectedIndex = num;
            }
            num++;
        }
    }

    private GUIContent[] placeHolders
    {
        get
        {
            if (this._placeHolders == null)
            {
                List<GUIContent> list = new List<GUIContent> {
                    new GUIContent("(none)")
                };
                PluginImporter[] allImporters = PluginImporter.GetAllImporters();
                string fileName = Path.GetFileName(this.getOriginalPluginPath());
                foreach (PluginImporter importer in allImporters)
                {
                    if ((this.getOriginalPluginPath() != importer.assetPath) && string.Equals(Path.GetFileNameWithoutExtension(fileName), Path.GetFileNameWithoutExtension(importer.assetPath), StringComparison.InvariantCultureIgnoreCase))
                    {
                        list.Add(new GUIContent(importer.assetPath));
                    }
                }
                this._placeHolders = list.ToArray();
            }
            return this._placeHolders;
        }
    }

    internal delegate string GetOrignalPluginPath();
}

