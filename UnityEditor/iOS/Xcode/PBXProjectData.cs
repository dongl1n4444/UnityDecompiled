namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditor.iOS.Xcode.PBX;

    internal class PBXProjectData
    {
        [CompilerGenerated]
        private static Func<PBXGroupData, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<PBXSourcesBuildPhaseData, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<PBXFrameworksBuildPhaseData, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<PBXResourcesBuildPhaseData, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<PBXCopyFilesBuildPhaseData, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<PBXShellScriptBuildPhaseData, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<PBXNativeTargetData, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<PBXVariantGroupData, bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<XCConfigurationListData, bool> <>f__am$cache8;
        public KnownSectionBase<XCBuildConfigurationData> buildConfigs = null;
        private KnownSectionBase<PBXBuildFileData> buildFiles = null;
        public KnownSectionBase<XCConfigurationListData> configs = null;
        public KnownSectionBase<PBXContainerItemProxyData> containerItems = null;
        public KnownSectionBase<PBXCopyFilesBuildPhaseData> copyFiles = null;
        private KnownSectionBase<PBXFileReferenceData> fileRefs = null;
        public KnownSectionBase<PBXFrameworksBuildPhaseData> frameworks = null;
        private KnownSectionBase<PBXGroupData> groups = null;
        private Dictionary<string, Dictionary<string, PBXBuildFileData>> m_FileGuidToBuildFileMap = null;
        private Dictionary<string, string> m_FileRefGuidToProjectPathMap = null;
        private Dictionary<string, string> m_GroupGuidToProjectPathMap = null;
        private Dictionary<string, PBXGroupData> m_GuidToParentGroupMap = null;
        private string m_ObjectVersion = null;
        private Dictionary<string, PBXFileReferenceData> m_ProjectPathToFileRefMap = null;
        private Dictionary<string, PBXGroupData> m_ProjectPathToGroupMap = null;
        private Dictionary<PBXSourceTree, Dictionary<string, PBXFileReferenceData>> m_RealPathToFileRefMap = null;
        private PBXElementDict m_RootElements = null;
        private Dictionary<string, SectionBase> m_Section = null;
        private List<string> m_SectionOrder = null;
        private PBXElementDict m_UnknownObjects = null;
        private Dictionary<string, KnownSectionBase<PBXObjectData>> m_UnknownSections;
        public KnownSectionBase<PBXNativeTargetData> nativeTargets = null;
        public PBXProjectSection project = null;
        public KnownSectionBase<PBXReferenceProxyData> references = null;
        public KnownSectionBase<PBXResourcesBuildPhaseData> resources = null;
        public KnownSectionBase<PBXShellScriptBuildPhaseData> shellScripts = null;
        public KnownSectionBase<PBXSourcesBuildPhaseData> sources = null;
        public KnownSectionBase<PBXTargetDependencyData> targetDependencies = null;
        public KnownSectionBase<PBXVariantGroupData> variantGroups = null;

        private GUIDToCommentMap BuildCommentMap()
        {
            GUIDToCommentMap comments = new GUIDToCommentMap();
            foreach (PBXGroupData data in this.groups.GetObjects())
            {
                comments.Add(data.guid, data.name);
            }
            foreach (PBXContainerItemProxyData data2 in this.containerItems.GetObjects())
            {
                comments.Add(data2.guid, "PBXContainerItemProxy");
            }
            foreach (PBXReferenceProxyData data3 in this.references.GetObjects())
            {
                comments.Add(data3.guid, data3.path);
            }
            foreach (PBXSourcesBuildPhaseData data4 in this.sources.GetObjects())
            {
                comments.Add(data4.guid, "Sources");
                this.BuildCommentMapForBuildFiles(comments, (List<string>) data4.files, "Sources");
            }
            foreach (PBXResourcesBuildPhaseData data5 in this.resources.GetObjects())
            {
                comments.Add(data5.guid, "Resources");
                this.BuildCommentMapForBuildFiles(comments, (List<string>) data5.files, "Resources");
            }
            foreach (PBXFrameworksBuildPhaseData data6 in this.frameworks.GetObjects())
            {
                comments.Add(data6.guid, "Frameworks");
                this.BuildCommentMapForBuildFiles(comments, (List<string>) data6.files, "Frameworks");
            }
            foreach (PBXCopyFilesBuildPhaseData data7 in this.copyFiles.GetObjects())
            {
                string name = data7.name;
                if (name == null)
                {
                    name = "CopyFiles";
                }
                comments.Add(data7.guid, name);
                this.BuildCommentMapForBuildFiles(comments, (List<string>) data7.files, name);
            }
            foreach (PBXShellScriptBuildPhaseData data8 in this.shellScripts.GetObjects())
            {
                comments.Add(data8.guid, "ShellScript");
            }
            foreach (PBXTargetDependencyData data9 in this.targetDependencies.GetObjects())
            {
                comments.Add(data9.guid, "PBXTargetDependency");
            }
            foreach (PBXNativeTargetData data10 in this.nativeTargets.GetObjects())
            {
                comments.Add(data10.guid, data10.name);
                comments.Add(data10.buildConfigList, $"Build configuration list for PBXNativeTarget "{data10.name}"");
            }
            foreach (PBXVariantGroupData data11 in this.variantGroups.GetObjects())
            {
                comments.Add(data11.guid, data11.name);
            }
            foreach (XCBuildConfigurationData data12 in this.buildConfigs.GetObjects())
            {
                comments.Add(data12.guid, data12.name);
            }
            foreach (PBXProjectObjectData data13 in this.project.GetObjects())
            {
                comments.Add(data13.guid, "Project object");
                comments.Add(data13.buildConfigList, "Build configuration list for PBXProject \"Unity-iPhone\"");
            }
            foreach (PBXFileReferenceData data14 in this.fileRefs.GetObjects())
            {
                comments.Add(data14.guid, data14.name);
            }
            if (this.m_RootElements.Contains("rootObject") && (this.m_RootElements["rootObject"] is PBXElementString))
            {
                comments.Add(this.m_RootElements["rootObject"].AsString(), "Project object");
            }
            return comments;
        }

        private void BuildCommentMapForBuildFiles(GUIDToCommentMap comments, List<string> guids, string sectName)
        {
            foreach (string str in guids)
            {
                PBXBuildFileData data = this.BuildFilesGet(str);
                if (data != null)
                {
                    PBXFileReferenceData data2 = this.FileRefsGet(data.fileRef);
                    if (data2 != null)
                    {
                        comments.Add(str, $"{data2.name} in {sectName}");
                    }
                    else
                    {
                        PBXReferenceProxyData data3 = this.references[data.fileRef];
                        if (data3 != null)
                        {
                            comments.Add(str, $"{data3.path} in {sectName}");
                        }
                    }
                }
            }
        }

        public void BuildFilesAdd(string targetGuid, PBXBuildFileData buildFile)
        {
            if (!this.m_FileGuidToBuildFileMap.ContainsKey(targetGuid))
            {
                this.m_FileGuidToBuildFileMap[targetGuid] = new Dictionary<string, PBXBuildFileData>();
            }
            this.m_FileGuidToBuildFileMap[targetGuid][buildFile.fileRef] = buildFile;
            this.buildFiles.AddEntry(buildFile);
        }

        public PBXBuildFileData BuildFilesGet(string guid) => 
            this.buildFiles[guid];

        public IEnumerable<PBXBuildFileData> BuildFilesGetAll() => 
            this.buildFiles.GetObjects();

        public PBXBuildFileData BuildFilesGetForSourceFile(string targetGuid, string fileGuid)
        {
            if (!this.m_FileGuidToBuildFileMap.ContainsKey(targetGuid))
            {
                return null;
            }
            if (!this.m_FileGuidToBuildFileMap[targetGuid].ContainsKey(fileGuid))
            {
                return null;
            }
            return this.m_FileGuidToBuildFileMap[targetGuid][fileGuid];
        }

        public void BuildFilesRemove(string targetGuid, string fileGuid)
        {
            PBXBuildFileData data = this.BuildFilesGetForSourceFile(targetGuid, fileGuid);
            if (data != null)
            {
                this.m_FileGuidToBuildFileMap[targetGuid].Remove(data.fileRef);
                this.buildFiles.RemoveEntry(data.guid);
            }
        }

        public FileGUIDListBase BuildSectionAny(string sectionGuid)
        {
            if (this.frameworks.HasEntry(sectionGuid))
            {
                return this.frameworks[sectionGuid];
            }
            if (this.resources.HasEntry(sectionGuid))
            {
                return this.resources[sectionGuid];
            }
            if (this.sources.HasEntry(sectionGuid))
            {
                return this.sources[sectionGuid];
            }
            if (!this.copyFiles.HasEntry(sectionGuid))
            {
                throw new Exception($"The given GUID {sectionGuid} does not refer to a known build section");
            }
            return this.copyFiles[sectionGuid];
        }

        public FileGUIDListBase BuildSectionAny(PBXNativeTargetData target, string path, bool isFolderRef)
        {
            switch (FileTypeUtils.GetFileType(Path.GetExtension(path), isFolderRef))
            {
                case PBXFileType.Framework:
                    foreach (string str2 in (IEnumerable<string>) target.phases)
                    {
                        if (this.frameworks.HasEntry(str2))
                        {
                            return this.frameworks[str2];
                        }
                    }
                    break;

                case PBXFileType.Source:
                    foreach (string str4 in (IEnumerable<string>) target.phases)
                    {
                        if (this.sources.HasEntry(str4))
                        {
                            return this.sources[str4];
                        }
                    }
                    break;

                case PBXFileType.Resource:
                    foreach (string str3 in (IEnumerable<string>) target.phases)
                    {
                        if (this.resources.HasEntry(str3))
                        {
                            return this.resources[str3];
                        }
                    }
                    break;

                case PBXFileType.CopyFile:
                    foreach (string str5 in (IEnumerable<string>) target.phases)
                    {
                        if (this.copyFiles.HasEntry(str5))
                        {
                            return this.copyFiles[str5];
                        }
                    }
                    break;
            }
            return null;
        }

        public void Clear()
        {
            this.buildFiles = new KnownSectionBase<PBXBuildFileData>("PBXBuildFile");
            this.fileRefs = new KnownSectionBase<PBXFileReferenceData>("PBXFileReference");
            this.groups = new KnownSectionBase<PBXGroupData>("PBXGroup");
            this.containerItems = new KnownSectionBase<PBXContainerItemProxyData>("PBXContainerItemProxy");
            this.references = new KnownSectionBase<PBXReferenceProxyData>("PBXReferenceProxy");
            this.sources = new KnownSectionBase<PBXSourcesBuildPhaseData>("PBXSourcesBuildPhase");
            this.frameworks = new KnownSectionBase<PBXFrameworksBuildPhaseData>("PBXFrameworksBuildPhase");
            this.resources = new KnownSectionBase<PBXResourcesBuildPhaseData>("PBXResourcesBuildPhase");
            this.copyFiles = new KnownSectionBase<PBXCopyFilesBuildPhaseData>("PBXCopyFilesBuildPhase");
            this.shellScripts = new KnownSectionBase<PBXShellScriptBuildPhaseData>("PBXShellScriptBuildPhase");
            this.nativeTargets = new KnownSectionBase<PBXNativeTargetData>("PBXNativeTarget");
            this.targetDependencies = new KnownSectionBase<PBXTargetDependencyData>("PBXTargetDependency");
            this.variantGroups = new KnownSectionBase<PBXVariantGroupData>("PBXVariantGroup");
            this.buildConfigs = new KnownSectionBase<XCBuildConfigurationData>("XCBuildConfiguration");
            this.configs = new KnownSectionBase<XCConfigurationListData>("XCConfigurationList");
            this.project = new PBXProjectSection();
            this.m_UnknownSections = new Dictionary<string, KnownSectionBase<PBXObjectData>>();
            Dictionary<string, SectionBase> dictionary = new Dictionary<string, SectionBase> {
                { 
                    "PBXBuildFile",
                    this.buildFiles
                },
                { 
                    "PBXFileReference",
                    this.fileRefs
                },
                { 
                    "PBXGroup",
                    this.groups
                },
                { 
                    "PBXContainerItemProxy",
                    this.containerItems
                },
                { 
                    "PBXReferenceProxy",
                    this.references
                },
                { 
                    "PBXSourcesBuildPhase",
                    this.sources
                },
                { 
                    "PBXFrameworksBuildPhase",
                    this.frameworks
                },
                { 
                    "PBXResourcesBuildPhase",
                    this.resources
                },
                { 
                    "PBXCopyFilesBuildPhase",
                    this.copyFiles
                },
                { 
                    "PBXShellScriptBuildPhase",
                    this.shellScripts
                },
                { 
                    "PBXNativeTarget",
                    this.nativeTargets
                },
                { 
                    "PBXTargetDependency",
                    this.targetDependencies
                },
                { 
                    "PBXVariantGroup",
                    this.variantGroups
                },
                { 
                    "XCBuildConfiguration",
                    this.buildConfigs
                },
                { 
                    "XCConfigurationList",
                    this.configs
                },
                { 
                    "PBXProject",
                    this.project
                }
            };
            this.m_Section = dictionary;
            this.m_RootElements = new PBXElementDict();
            this.m_UnknownObjects = new PBXElementDict();
            this.m_ObjectVersion = null;
            List<string> list = new List<string> { 
                "PBXBuildFile",
                "PBXContainerItemProxy",
                "PBXCopyFilesBuildPhase",
                "PBXFileReference",
                "PBXFrameworksBuildPhase",
                "PBXGroup",
                "PBXNativeTarget",
                "PBXProject",
                "PBXReferenceProxy",
                "PBXResourcesBuildPhase",
                "PBXShellScriptBuildPhase",
                "PBXSourcesBuildPhase",
                "PBXTargetDependency",
                "PBXVariantGroup",
                "XCBuildConfiguration",
                "XCConfigurationList"
            };
            this.m_SectionOrder = list;
            this.m_FileGuidToBuildFileMap = new Dictionary<string, Dictionary<string, PBXBuildFileData>>();
            this.m_ProjectPathToFileRefMap = new Dictionary<string, PBXFileReferenceData>();
            this.m_FileRefGuidToProjectPathMap = new Dictionary<string, string>();
            this.m_RealPathToFileRefMap = new Dictionary<PBXSourceTree, Dictionary<string, PBXFileReferenceData>>();
            foreach (PBXSourceTree tree in FileTypeUtils.AllAbsoluteSourceTrees())
            {
                this.m_RealPathToFileRefMap.Add(tree, new Dictionary<string, PBXFileReferenceData>());
            }
            this.m_ProjectPathToGroupMap = new Dictionary<string, PBXGroupData>();
            this.m_GroupGuidToProjectPathMap = new Dictionary<string, string>();
            this.m_GuidToParentGroupMap = new Dictionary<string, PBXGroupData>();
        }

        public void FileRefsAdd(string realPath, string projectPath, PBXGroupData parent, PBXFileReferenceData fileRef)
        {
            this.fileRefs.AddEntry(fileRef);
            this.m_ProjectPathToFileRefMap.Add(projectPath, fileRef);
            this.m_FileRefGuidToProjectPathMap.Add(fileRef.guid, projectPath);
            this.m_RealPathToFileRefMap[fileRef.tree].Add(realPath, fileRef);
            this.m_GuidToParentGroupMap.Add(fileRef.guid, parent);
        }

        public PBXFileReferenceData FileRefsGet(string guid) => 
            this.fileRefs[guid];

        public PBXFileReferenceData FileRefsGetByProjectPath(string path)
        {
            if (this.m_ProjectPathToFileRefMap.ContainsKey(path))
            {
                return this.m_ProjectPathToFileRefMap[path];
            }
            return null;
        }

        public PBXFileReferenceData FileRefsGetByRealPath(string path, PBXSourceTree sourceTree)
        {
            if (this.m_RealPathToFileRefMap[sourceTree].ContainsKey(path))
            {
                return this.m_RealPathToFileRefMap[sourceTree][path];
            }
            return null;
        }

        public void FileRefsRemove(string guid)
        {
            PBXFileReferenceData data = this.fileRefs[guid];
            this.fileRefs.RemoveEntry(guid);
            this.m_ProjectPathToFileRefMap.Remove(this.m_FileRefGuidToProjectPathMap[guid]);
            this.m_FileRefGuidToProjectPathMap.Remove(guid);
            foreach (PBXSourceTree tree in FileTypeUtils.AllAbsoluteSourceTrees())
            {
                this.m_RealPathToFileRefMap[tree].Remove(data.path);
            }
            this.m_GuidToParentGroupMap.Remove(guid);
        }

        public void GroupsAdd(string projectPath, PBXGroupData parent, PBXGroupData gr)
        {
            this.m_ProjectPathToGroupMap.Add(projectPath, gr);
            this.m_GroupGuidToProjectPathMap.Add(gr.guid, projectPath);
            this.m_GuidToParentGroupMap.Add(gr.guid, parent);
            this.groups.AddEntry(gr);
        }

        public void GroupsAddDuplicate(PBXGroupData gr)
        {
            this.groups.AddEntry(gr);
        }

        public PBXGroupData GroupsGet(string guid) => 
            this.groups[guid];

        public PBXGroupData GroupsGetByChild(string childGuid) => 
            this.m_GuidToParentGroupMap[childGuid];

        public PBXGroupData GroupsGetByProjectPath(string sourceGroup)
        {
            if (this.m_ProjectPathToGroupMap.ContainsKey(sourceGroup))
            {
                return this.m_ProjectPathToGroupMap[sourceGroup];
            }
            return null;
        }

        public PBXGroupData GroupsGetMainGroup() => 
            this.groups[this.project.project.mainGroup];

        public void GroupsRemove(string guid)
        {
            this.m_ProjectPathToGroupMap.Remove(this.m_GroupGuidToProjectPathMap[guid]);
            this.m_GroupGuidToProjectPathMap.Remove(guid);
            this.m_GuidToParentGroupMap.Remove(guid);
            this.groups.RemoveEntry(guid);
        }

        private static PBXElementDict ParseContent(string content)
        {
            TokenList tokens = Lexer.Tokenize(content);
            Parser parser = new Parser(tokens);
            return Serializer.ParseTreeAST(parser.ParseTree(), tokens, content);
        }

        public void ReadFromStream(TextReader sr)
        {
            <ReadFromStream>c__AnonStorey0 storey = new <ReadFromStream>c__AnonStorey0();
            this.Clear();
            this.m_RootElements = ParseContent(sr.ReadToEnd());
            if (!this.m_RootElements.Contains("objects"))
            {
                throw new Exception("Invalid PBX project file: no objects element");
            }
            PBXElementDict dict = this.m_RootElements["objects"].AsDict();
            this.m_RootElements.Remove("objects");
            this.m_RootElements.SetString("objects", "OBJMARKER");
            if (this.m_RootElements.Contains("objectVersion"))
            {
                this.m_ObjectVersion = this.m_RootElements["objectVersion"].AsString();
                this.m_RootElements.Remove("objectVersion");
            }
            List<string> allGuids = new List<string>();
            storey.prevSectionName = null;
            foreach (KeyValuePair<string, PBXElement> pair in dict.values)
            {
                allGuids.Add(pair.Key);
                PBXElement element = pair.Value;
                if (!(element is PBXElementDict) || !element.AsDict().Contains("isa"))
                {
                    this.m_UnknownObjects.values.Add(pair.Key, element);
                }
                else
                {
                    PBXElementDict dict2 = element.AsDict();
                    string key = dict2["isa"].AsString();
                    if (this.m_Section.ContainsKey(key))
                    {
                        this.m_Section[key].AddObject(pair.Key, dict2);
                    }
                    else
                    {
                        KnownSectionBase<PBXObjectData> base3;
                        if (this.m_UnknownSections.ContainsKey(key))
                        {
                            base3 = this.m_UnknownSections[key];
                        }
                        else
                        {
                            base3 = new KnownSectionBase<PBXObjectData>(key);
                            this.m_UnknownSections.Add(key, base3);
                        }
                        base3.AddObject(pair.Key, dict2);
                        if (!this.m_SectionOrder.Contains(key))
                        {
                            int index = 0;
                            if (storey.prevSectionName != null)
                            {
                                index = this.m_SectionOrder.FindIndex(new Predicate<string>(storey.<>m__0)) + 1;
                            }
                            this.m_SectionOrder.Insert(index, key);
                        }
                    }
                    storey.prevSectionName = key;
                }
            }
            this.RepairStructure(allGuids);
            this.RefreshAuxMaps();
        }

        private void RefreshAuxMaps()
        {
            foreach (KeyValuePair<string, PBXNativeTargetData> pair in this.nativeTargets.GetEntries())
            {
                Dictionary<string, PBXBuildFileData> mapForTarget = new Dictionary<string, PBXBuildFileData>();
                foreach (string str in (IEnumerable<string>) pair.Value.phases)
                {
                    if (this.frameworks.HasEntry(str))
                    {
                        this.RefreshBuildFilesMapForBuildFileGuidList(mapForTarget, this.frameworks[str]);
                    }
                    if (this.resources.HasEntry(str))
                    {
                        this.RefreshBuildFilesMapForBuildFileGuidList(mapForTarget, this.resources[str]);
                    }
                    if (this.sources.HasEntry(str))
                    {
                        this.RefreshBuildFilesMapForBuildFileGuidList(mapForTarget, this.sources[str]);
                    }
                    if (this.copyFiles.HasEntry(str))
                    {
                        this.RefreshBuildFilesMapForBuildFileGuidList(mapForTarget, this.copyFiles[str]);
                    }
                }
                this.m_FileGuidToBuildFileMap[pair.Key] = mapForTarget;
            }
            this.RefreshMapsForGroupChildren("", "", PBXSourceTree.Source, this.GroupsGetMainGroup());
        }

        private void RefreshBuildFilesMapForBuildFileGuidList(Dictionary<string, PBXBuildFileData> mapForTarget, FileGUIDListBase list)
        {
            foreach (string str in (IEnumerable<string>) list.files)
            {
                PBXBuildFileData data = this.buildFiles[str];
                mapForTarget[data.fileRef] = data;
            }
        }

        private void RefreshMapsForGroupChildren(string projectPath, string realPath, PBXSourceTree realPathTree, PBXGroupData parent)
        {
            List<string> list = new List<string>(parent.children);
            foreach (string str in list)
            {
                string str2;
                string str3;
                PBXSourceTree tree;
                PBXFileReferenceData data = this.fileRefs[str];
                if (data != null)
                {
                    str2 = PBXPath.Combine(projectPath, data.name);
                    PBXPath.Combine(realPath, realPathTree, data.path, data.tree, out str3, out tree);
                    if (!this.m_ProjectPathToFileRefMap.ContainsKey(str2))
                    {
                        this.m_ProjectPathToFileRefMap.Add(str2, data);
                    }
                    if (!this.m_FileRefGuidToProjectPathMap.ContainsKey(data.guid))
                    {
                        this.m_FileRefGuidToProjectPathMap.Add(data.guid, str2);
                    }
                    if (!this.m_RealPathToFileRefMap[tree].ContainsKey(str3))
                    {
                        this.m_RealPathToFileRefMap[tree].Add(str3, data);
                    }
                    if (!this.m_GuidToParentGroupMap.ContainsKey(str))
                    {
                        this.m_GuidToParentGroupMap.Add(str, parent);
                    }
                }
                else
                {
                    PBXGroupData data2 = this.groups[str];
                    if (data2 != null)
                    {
                        str2 = PBXPath.Combine(projectPath, data2.name);
                        PBXPath.Combine(realPath, realPathTree, data2.path, data2.tree, out str3, out tree);
                        if (!this.m_ProjectPathToGroupMap.ContainsKey(str2))
                        {
                            this.m_ProjectPathToGroupMap.Add(str2, data2);
                        }
                        if (!this.m_GroupGuidToProjectPathMap.ContainsKey(data2.guid))
                        {
                            this.m_GroupGuidToProjectPathMap.Add(data2.guid, str2);
                        }
                        if (!this.m_GuidToParentGroupMap.ContainsKey(str))
                        {
                            this.m_GuidToParentGroupMap.Add(str, parent);
                        }
                        this.RefreshMapsForGroupChildren(str2, str3, tree, data2);
                    }
                }
            }
        }

        private static void RemoveMissingGuidsFromGuidList(GUIDList guidList, Dictionary<string, bool> allGuids)
        {
            List<string> list = null;
            foreach (string str in (IEnumerable<string>) guidList)
            {
                if (!allGuids.ContainsKey(str))
                {
                    if (list == null)
                    {
                        list = new List<string>();
                    }
                    list.Add(str);
                }
            }
            if (list != null)
            {
                foreach (string str2 in list)
                {
                    guidList.RemoveGUID(str2);
                }
            }
        }

        private static bool RemoveObjectsFromSection<T>(KnownSectionBase<T> section, Dictionary<string, bool> allGuids, Func<T, bool> checker) where T: PBXObjectData, new()
        {
            List<string> list = null;
            foreach (KeyValuePair<string, T> pair in section.GetEntries())
            {
                if (checker(pair.Value))
                {
                    if (list == null)
                    {
                        list = new List<string>();
                    }
                    list.Add(pair.Key);
                }
            }
            if (list != null)
            {
                foreach (string str in list)
                {
                    section.RemoveEntry(str);
                    allGuids.Remove(str);
                }
                return true;
            }
            return false;
        }

        private void RepairStructure(List<string> allGuids)
        {
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            foreach (string str in allGuids)
            {
                dictionary.Add(str, false);
            }
            while (this.RepairStructureImpl(dictionary))
            {
            }
        }

        private bool RepairStructureImpl(Dictionary<string, bool> allGuids)
        {
            <RepairStructureImpl>c__AnonStorey1 storey = new <RepairStructureImpl>c__AnonStorey1 {
                allGuids = allGuids
            };
            bool flag = false;
            flag |= RemoveObjectsFromSection<PBXBuildFileData>(this.buildFiles, storey.allGuids, new Func<PBXBuildFileData, bool>(storey.<>m__0));
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = o => o.children == null;
            }
            flag |= RemoveObjectsFromSection<PBXGroupData>(this.groups, storey.allGuids, <>f__am$cache0);
            foreach (PBXGroupData data in this.groups.GetObjects())
            {
                RemoveMissingGuidsFromGuidList(data.children, storey.allGuids);
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = o => o.files == null;
            }
            flag |= RemoveObjectsFromSection<PBXSourcesBuildPhaseData>(this.sources, storey.allGuids, <>f__am$cache1);
            foreach (PBXSourcesBuildPhaseData data2 in this.sources.GetObjects())
            {
                RemoveMissingGuidsFromGuidList(data2.files, storey.allGuids);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = o => o.files == null;
            }
            flag |= RemoveObjectsFromSection<PBXFrameworksBuildPhaseData>(this.frameworks, storey.allGuids, <>f__am$cache2);
            foreach (PBXFrameworksBuildPhaseData data3 in this.frameworks.GetObjects())
            {
                RemoveMissingGuidsFromGuidList(data3.files, storey.allGuids);
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = o => o.files == null;
            }
            flag |= RemoveObjectsFromSection<PBXResourcesBuildPhaseData>(this.resources, storey.allGuids, <>f__am$cache3);
            foreach (PBXResourcesBuildPhaseData data4 in this.resources.GetObjects())
            {
                RemoveMissingGuidsFromGuidList(data4.files, storey.allGuids);
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = o => o.files == null;
            }
            flag |= RemoveObjectsFromSection<PBXCopyFilesBuildPhaseData>(this.copyFiles, storey.allGuids, <>f__am$cache4);
            foreach (PBXCopyFilesBuildPhaseData data5 in this.copyFiles.GetObjects())
            {
                RemoveMissingGuidsFromGuidList(data5.files, storey.allGuids);
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = o => o.files == null;
            }
            flag |= RemoveObjectsFromSection<PBXShellScriptBuildPhaseData>(this.shellScripts, storey.allGuids, <>f__am$cache5);
            foreach (PBXShellScriptBuildPhaseData data6 in this.shellScripts.GetObjects())
            {
                RemoveMissingGuidsFromGuidList(data6.files, storey.allGuids);
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = o => o.phases == null;
            }
            flag |= RemoveObjectsFromSection<PBXNativeTargetData>(this.nativeTargets, storey.allGuids, <>f__am$cache6);
            foreach (PBXNativeTargetData data7 in this.nativeTargets.GetObjects())
            {
                RemoveMissingGuidsFromGuidList(data7.phases, storey.allGuids);
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = o => o.children == null;
            }
            flag |= RemoveObjectsFromSection<PBXVariantGroupData>(this.variantGroups, storey.allGuids, <>f__am$cache7);
            foreach (PBXVariantGroupData data8 in this.variantGroups.GetObjects())
            {
                RemoveMissingGuidsFromGuidList(data8.children, storey.allGuids);
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = o => o.buildConfigs == null;
            }
            flag |= RemoveObjectsFromSection<XCConfigurationListData>(this.configs, storey.allGuids, <>f__am$cache8);
            foreach (XCConfigurationListData data9 in this.configs.GetObjects())
            {
                RemoveMissingGuidsFromGuidList(data9.buildConfigs, storey.allGuids);
            }
            return flag;
        }

        public string WriteToString()
        {
            GUIDToCommentMap comments = this.BuildCommentMap();
            PropertyCommentChecker checker = new PropertyCommentChecker();
            GUIDToCommentMap map2 = new GUIDToCommentMap();
            StringBuilder sb = new StringBuilder();
            if (this.m_ObjectVersion != null)
            {
                sb.AppendFormat("objectVersion = {0};\n\t", this.m_ObjectVersion);
            }
            sb.Append("objects = {");
            foreach (string str in this.m_SectionOrder)
            {
                if (this.m_Section.ContainsKey(str))
                {
                    this.m_Section[str].WriteSection(sb, comments);
                }
                else if (this.m_UnknownSections.ContainsKey(str))
                {
                    this.m_UnknownSections[str].WriteSection(sb, comments);
                }
            }
            foreach (KeyValuePair<string, PBXElement> pair in this.m_UnknownObjects.values)
            {
                Serializer.WriteDictKeyValue(sb, pair.Key, pair.Value, 2, false, checker, map2);
            }
            sb.Append("\n\t};");
            StringBuilder builder2 = new StringBuilder();
            builder2.Append("// !$*UTF8*$!\n");
            string[] props = new string[] { "rootObject/*" };
            Serializer.WriteDict(builder2, this.m_RootElements, 0, false, new PropertyCommentChecker(props), comments);
            builder2.Append("\n");
            return builder2.ToString().Replace("objects = OBJMARKER;", sb.ToString());
        }

        [CompilerGenerated]
        private sealed class <ReadFromStream>c__AnonStorey0
        {
            internal string prevSectionName;

            internal bool <>m__0(string x) => 
                (x == this.prevSectionName);
        }

        [CompilerGenerated]
        private sealed class <RepairStructureImpl>c__AnonStorey1
        {
            internal Dictionary<string, bool> allGuids;

            internal bool <>m__0(PBXBuildFileData o) => 
                ((o.fileRef == null) || !this.allGuids.ContainsKey(o.fileRef));
        }
    }
}

