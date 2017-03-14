namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEditor.iOS.Xcode.PBX;

    public class PBXProject
    {
        private PBXProjectData m_Data = new PBXProjectData();

        public void AddAssetTagForFile(string targetGuid, string fileGuid, string tag)
        {
            PBXBuildFileData data = this.BuildFilesGetForSourceFile(targetGuid, fileGuid);
            if (data != null)
            {
                if (!data.assetTags.Contains(tag))
                {
                    data.assetTags.Add(tag);
                }
                if (!this.project.project.knownAssetTags.Contains(tag))
                {
                    this.project.project.knownAssetTags.Add(tag);
                }
            }
        }

        public void AddAssetTagToDefaultInstall(string targetGuid, string tag)
        {
            if (this.project.project.knownAssetTags.Contains(tag))
            {
                this.AddBuildProperty(targetGuid, "ON_DEMAND_RESOURCES_INITIAL_INSTALL_TAGS", tag);
            }
        }

        public void AddBuildConfig(string name)
        {
            foreach (string str in this.GetAllTargetGuids())
            {
                this.AddBuildConfigForTarget(str, name);
            }
        }

        private string AddBuildConfigForTarget(string targetGuid, string name)
        {
            if (this.BuildConfigByName(targetGuid, name) != null)
            {
                throw new Exception($"A build configuration by name {targetGuid} already exists for target {name}");
            }
            XCBuildConfigurationData data = XCBuildConfigurationData.Create(name);
            this.buildConfigs.AddEntry(data);
            this.buildConfigLists[this.GetConfigListForTarget(targetGuid)].buildConfigs.AddGUID(data.guid);
            return data.guid;
        }

        private void AddBuildFileImpl(string targetGuid, string fileGuid, bool weak, string compileFlags)
        {
            PBXNativeTargetData target = this.nativeTargets[targetGuid];
            PBXFileReferenceData data2 = this.FileRefsGet(fileGuid);
            string extension = Path.GetExtension(data2.path);
            if (FileTypeUtils.IsBuildable(extension, data2.isFolderReference) && (this.BuildFilesGetForSourceFile(targetGuid, fileGuid) == null))
            {
                PBXBuildFileData buildFile = PBXBuildFileData.CreateFromFile(fileGuid, weak, compileFlags);
                this.BuildFilesAdd(targetGuid, buildFile);
                this.BuildSectionAny(target, extension, data2.isFolderReference).files.AddGUID(buildFile.guid);
            }
        }

        public void AddBuildProperty(IEnumerable<string> targetGuids, string name, string value)
        {
            foreach (string str in targetGuids)
            {
                this.AddBuildProperty(str, name, value);
            }
        }

        public void AddBuildProperty(string targetGuid, string name, string value)
        {
            foreach (string str in (IEnumerable<string>) this.buildConfigLists[this.GetConfigListForTarget(targetGuid)].buildConfigs)
            {
                this.AddBuildPropertyForConfig(str, name, value);
            }
        }

        public void AddBuildPropertyForConfig(IEnumerable<string> configGuids, string name, string value)
        {
            foreach (string str in configGuids)
            {
                this.AddBuildPropertyForConfig(str, name, value);
            }
        }

        public void AddBuildPropertyForConfig(string configGuid, string name, string value)
        {
            this.buildConfigs[configGuid].AddProperty(name, value);
        }

        public bool AddCapability(string targetGuid, PBXCapabilityType capability, string entitlementsFilePath = null, bool addOptionalFramework = false)
        {
            if (capability.requiresEntitlements && (entitlementsFilePath == ""))
            {
                throw new Exception("Couldn't add the Xcode Capability " + capability.id + " to the PBXProject file because this capability requires an entitlement file.");
            }
            PBXProjectObjectData project = this.project.project;
            if (((project.entitlementsFile != null) && (entitlementsFilePath != null)) && (project.entitlementsFile != entitlementsFilePath))
            {
                if (project.capabilities.Count > 0)
                {
                    throw new WarningException("Attention, it seems that you have multiple entitlements file. Only one will be added the Project : " + project.entitlementsFile);
                }
                return false;
            }
            if (project.capabilities.Contains(new PBXCapabilityType.TargetCapabilityPair(targetGuid, capability)))
            {
                throw new WarningException("This capability has already been added. Method ignored");
            }
            project.capabilities.Add(new PBXCapabilityType.TargetCapabilityPair(targetGuid, capability));
            if (((capability.framework != "") && !capability.optionalFramework) || (((capability.framework != "") && capability.optionalFramework) && addOptionalFramework))
            {
                this.AddFrameworkToProject(targetGuid, capability.framework, false);
            }
            if ((entitlementsFilePath != null) && (project.entitlementsFile == null))
            {
                project.entitlementsFile = entitlementsFilePath;
                this.AddFileImpl(entitlementsFilePath, entitlementsFilePath, PBXSourceTree.Source, false);
                this.SetBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", PBXPath.FixSlashes(entitlementsFilePath));
            }
            return true;
        }

        public string AddCopyFilesBuildPhase(string targetGuid, string name, string dstPath, string subfolderSpec)
        {
            PBXCopyFilesBuildPhaseData data = PBXCopyFilesBuildPhaseData.Create(name, dstPath, subfolderSpec);
            this.copyFiles.AddEntry(data);
            this.nativeTargets[targetGuid].phases.AddGUID(data.guid);
            return data.guid;
        }

        public string AddFile(string path, string projectPath, PBXSourceTree sourceTree = 1)
        {
            if (sourceTree == PBXSourceTree.Group)
            {
                throw new Exception("sourceTree must not be PBXSourceTree.Group");
            }
            return this.AddFileImpl(path, projectPath, sourceTree, false);
        }

        private string AddFileImpl(string path, string projectPath, PBXSourceTree tree, bool isFolderReference)
        {
            PBXFileReferenceData data;
            path = PBXPath.FixSlashes(path);
            projectPath = PBXPath.FixSlashes(projectPath);
            if (!isFolderReference && (Path.GetExtension(path) != Path.GetExtension(projectPath)))
            {
                throw new Exception("Project and real path extensions do not match");
            }
            string str = this.FindFileGuidByProjectPath(projectPath);
            if (str == null)
            {
                str = this.FindFileGuidByRealPath(path);
            }
            if (str != null)
            {
                return str;
            }
            if (isFolderReference)
            {
                data = PBXFileReferenceData.CreateFromFolderReference(path, PBXPath.GetFilename(projectPath), tree);
            }
            else
            {
                data = PBXFileReferenceData.CreateFromFile(path, PBXPath.GetFilename(projectPath), tree);
            }
            PBXGroupData parent = this.CreateSourceGroup(PBXPath.GetDirectory(projectPath));
            parent.children.AddGUID(data.guid);
            this.FileRefsAdd(path, projectPath, parent, data);
            return data.guid;
        }

        public void AddFileToBuild(string targetGuid, string fileGuid)
        {
            this.AddBuildFileImpl(targetGuid, fileGuid, false, null);
        }

        public void AddFileToBuildSection(string targetGuid, string sectionGuid, string fileGuid)
        {
            PBXBuildFileData buildFile = PBXBuildFileData.CreateFromFile(fileGuid, false, null);
            this.BuildFilesAdd(targetGuid, buildFile);
            this.BuildSectionAny(sectionGuid).files.AddGUID(buildFile.guid);
        }

        public void AddFileToBuildWithFlags(string targetGuid, string fileGuid, string compileFlags)
        {
            this.AddBuildFileImpl(targetGuid, fileGuid, false, compileFlags);
        }

        public string AddFolderReference(string path, string projectPath, PBXSourceTree sourceTree = 1)
        {
            if (sourceTree == PBXSourceTree.Group)
            {
                throw new Exception("sourceTree must not be PBXSourceTree.Group");
            }
            return this.AddFileImpl(path, projectPath, sourceTree, true);
        }

        public string AddFrameworksBuildPhase(string targetGuid)
        {
            PBXFrameworksBuildPhaseData data = PBXFrameworksBuildPhaseData.Create();
            this.frameworks.AddEntry(data);
            this.nativeTargets[targetGuid].phases.AddGUID(data.guid);
            return data.guid;
        }

        public void AddFrameworkToProject(string targetGuid, string framework, bool weak)
        {
            string fileGuid = this.AddFile("System/Library/Frameworks/" + framework, "Frameworks/" + framework, PBXSourceTree.Sdk);
            this.AddBuildFileImpl(targetGuid, fileGuid, weak, null);
        }

        public string AddResourcesBuildPhase(string targetGuid)
        {
            PBXResourcesBuildPhaseData data = PBXResourcesBuildPhaseData.Create();
            this.resources.AddEntry(data);
            this.nativeTargets[targetGuid].phases.AddGUID(data.guid);
            return data.guid;
        }

        public string AddSourcesBuildPhase(string targetGuid)
        {
            PBXSourcesBuildPhaseData data = PBXSourcesBuildPhaseData.Create();
            this.sources.AddEntry(data);
            this.nativeTargets[targetGuid].phases.AddGUID(data.guid);
            return data.guid;
        }

        public string AddTarget(string name, string ext, string type)
        {
            XCConfigurationListData data = XCConfigurationListData.Create();
            this.buildConfigLists.AddEntry(data);
            string path = name + "." + FileTypeUtils.TrimExtension(ext);
            string productRef = this.AddFile(path, "Products/" + path, PBXSourceTree.Build);
            PBXNativeTargetData data2 = PBXNativeTargetData.Create(name, productRef, type, data.guid);
            this.nativeTargets.AddEntry(data2);
            this.project.project.targets.Add(data2.guid);
            foreach (string str3 in this.BuildConfigNames())
            {
                this.AddBuildConfigForTarget(data2.guid, str3);
            }
            return data2.guid;
        }

        internal void AddTargetDependency(string targetGuid, string targetDependencyGuid)
        {
            string name = this.nativeTargets[targetDependencyGuid].name;
            PBXContainerItemProxyData data = PBXContainerItemProxyData.Create(this.project.project.guid, "1", targetDependencyGuid, name);
            this.containerItems.AddEntry(data);
            PBXTargetDependencyData data2 = PBXTargetDependencyData.Create(targetDependencyGuid, data.guid);
            this.targetDependencies.AddEntry(data2);
            this.nativeTargets[targetGuid].dependencies.AddGUID(data2.guid);
        }

        internal void AppendShellScriptBuildPhase(IEnumerable<string> targetGuids, string name, string shellPath, string shellScript)
        {
            PBXShellScriptBuildPhaseData data = PBXShellScriptBuildPhaseData.Create(name, shellPath, shellScript);
            this.shellScripts.AddEntry(data);
            foreach (string str in targetGuids)
            {
                this.nativeTargets[str].phases.AddGUID(data.guid);
            }
        }

        internal void AppendShellScriptBuildPhase(string targetGuid, string name, string shellPath, string shellScript)
        {
            PBXShellScriptBuildPhaseData data = PBXShellScriptBuildPhaseData.Create(name, shellPath, shellScript);
            this.shellScripts.AddEntry(data);
            this.nativeTargets[targetGuid].phases.AddGUID(data.guid);
        }

        public string BuildConfigByName(string targetGuid, string name)
        {
            foreach (string str in (IEnumerable<string>) this.buildConfigLists[this.GetConfigListForTarget(targetGuid)].buildConfigs)
            {
                XCBuildConfigurationData data = this.buildConfigs[str];
                if ((data != null) && (data.name == name))
                {
                    return data.guid;
                }
            }
            return null;
        }

        public IEnumerable<string> BuildConfigNames()
        {
            List<string> list = new List<string>();
            foreach (string str in (IEnumerable<string>) this.buildConfigLists[this.project.project.buildConfigList].buildConfigs)
            {
                list.Add(this.buildConfigs[str].name);
            }
            return list;
        }

        internal void BuildFilesAdd(string targetGuid, PBXBuildFileData buildFile)
        {
            this.m_Data.BuildFilesAdd(targetGuid, buildFile);
        }

        internal PBXBuildFileData BuildFilesGet(string guid) => 
            this.m_Data.BuildFilesGet(guid);

        internal IEnumerable<PBXBuildFileData> BuildFilesGetAll() => 
            this.m_Data.BuildFilesGetAll();

        internal PBXBuildFileData BuildFilesGetForSourceFile(string targetGuid, string fileGuid) => 
            this.m_Data.BuildFilesGetForSourceFile(targetGuid, fileGuid);

        internal void BuildFilesRemove(string targetGuid, string fileGuid)
        {
            this.m_Data.BuildFilesRemove(targetGuid, fileGuid);
        }

        internal FileGUIDListBase BuildSectionAny(string sectionGuid) => 
            this.m_Data.BuildSectionAny(sectionGuid);

        internal FileGUIDListBase BuildSectionAny(PBXNativeTargetData target, string path, bool isFolderRef) => 
            this.m_Data.BuildSectionAny(target, path, isFolderRef);

        public bool ContainsFileByProjectPath(string path) => 
            (this.FindFileGuidByProjectPath(path) != null);

        public bool ContainsFileByRealPath(string path) => 
            (this.FindFileGuidByRealPath(path) != null);

        public bool ContainsFileByRealPath(string path, PBXSourceTree sourceTree)
        {
            if (sourceTree == PBXSourceTree.Group)
            {
                throw new Exception("sourceTree must not be PBXSourceTree.Group");
            }
            return (this.FindFileGuidByRealPath(path, sourceTree) != null);
        }

        public bool ContainsFramework(string targetGuid, string framework)
        {
            string fileGuid = this.FindFileGuidByRealPath("System/Library/Frameworks/" + framework, PBXSourceTree.Sdk);
            if (fileGuid == null)
            {
                return false;
            }
            return (this.BuildFilesGetForSourceFile(targetGuid, fileGuid) != null);
        }

        internal PBXGroupData CreateSourceGroup(string sourceGroup)
        {
            sourceGroup = PBXPath.FixSlashes(sourceGroup);
            if ((sourceGroup == null) || (sourceGroup == ""))
            {
                return this.GroupsGetMainGroup();
            }
            PBXGroupData group = this.GroupsGetByProjectPath(sourceGroup);
            if (group == null)
            {
                group = this.GroupsGetMainGroup();
                string[] strArray = PBXPath.Split(sourceGroup);
                string projectPath = null;
                foreach (string str2 in strArray)
                {
                    if (projectPath == null)
                    {
                        projectPath = str2;
                    }
                    else
                    {
                        projectPath = projectPath + "/" + str2;
                    }
                    PBXGroupData pBXGroupChildByName = this.GetPBXGroupChildByName(group, str2);
                    if (pBXGroupChildByName != null)
                    {
                        group = pBXGroupChildByName;
                    }
                    else
                    {
                        PBXGroupData gr = PBXGroupData.Create(str2, str2, PBXSourceTree.Group);
                        group.children.AddGUID(gr.guid);
                        this.GroupsAdd(projectPath, group, gr);
                        group = gr;
                    }
                }
            }
            return group;
        }

        internal void FileRefsAdd(string realPath, string projectPath, PBXGroupData parent, PBXFileReferenceData fileRef)
        {
            this.m_Data.FileRefsAdd(realPath, projectPath, parent, fileRef);
        }

        internal PBXFileReferenceData FileRefsGet(string guid) => 
            this.m_Data.FileRefsGet(guid);

        internal PBXFileReferenceData FileRefsGetByProjectPath(string path) => 
            this.m_Data.FileRefsGetByProjectPath(path);

        internal PBXFileReferenceData FileRefsGetByRealPath(string path, PBXSourceTree sourceTree) => 
            this.m_Data.FileRefsGetByRealPath(path, sourceTree);

        internal void FileRefsRemove(string guid)
        {
            this.m_Data.FileRefsRemove(guid);
        }

        public string FindFileGuidByProjectPath(string path)
        {
            path = PBXPath.FixSlashes(path);
            PBXFileReferenceData data = this.FileRefsGetByProjectPath(path);
            if (data != null)
            {
                return data.guid;
            }
            return null;
        }

        public string FindFileGuidByRealPath(string path)
        {
            path = PBXPath.FixSlashes(path);
            foreach (PBXSourceTree tree in FileTypeUtils.AllAbsoluteSourceTrees())
            {
                string str = this.FindFileGuidByRealPath(path, tree);
                if (str != null)
                {
                    return str;
                }
            }
            return null;
        }

        public string FindFileGuidByRealPath(string path, PBXSourceTree sourceTree)
        {
            if (sourceTree == PBXSourceTree.Group)
            {
                throw new Exception("sourceTree must not be PBXSourceTree.Group");
            }
            path = PBXPath.FixSlashes(path);
            PBXFileReferenceData data = this.FileRefsGetByRealPath(path, sourceTree);
            if (data != null)
            {
                return data.guid;
            }
            return null;
        }

        private IEnumerable<string> GetAllTargetGuids()
        {
            List<string> list = new List<string> {
                this.project.project.guid
            };
            list.AddRange(this.nativeTargets.GetGuids());
            return list;
        }

        public List<string> GetCompileFlagsForFile(string targetGuid, string fileGuid)
        {
            PBXBuildFileData data = this.BuildFilesGetForSourceFile(targetGuid, fileGuid);
            if (data == null)
            {
                return null;
            }
            if (data.compileFlags == null)
            {
                return new List<string>();
            }
            char[] separator = new char[] { ' ' };
            return new List<string>(data.compileFlags.Split(separator, StringSplitOptions.RemoveEmptyEntries));
        }

        internal string GetConfigListForTarget(string targetGuid)
        {
            if (targetGuid == this.project.project.guid)
            {
                return this.project.project.buildConfigList;
            }
            return this.nativeTargets[targetGuid].buildConfigList;
        }

        internal HashSet<string> GetFileRefsByProjectPaths(IEnumerable<string> paths)
        {
            HashSet<string> set = new HashSet<string>();
            foreach (string str in paths)
            {
                string path = PBXPath.FixSlashes(str);
                PBXFileReferenceData data = this.FileRefsGetByProjectPath(path);
                if (data != null)
                {
                    set.Add(data.path);
                }
            }
            return set;
        }

        internal List<string> GetGroupChildrenFiles(string projectPath)
        {
            projectPath = PBXPath.FixSlashes(projectPath);
            PBXGroupData data = this.GroupsGetByProjectPath(projectPath);
            if (data == null)
            {
                return null;
            }
            List<string> list2 = new List<string>();
            foreach (string str in (IEnumerable<string>) data.children)
            {
                PBXFileReferenceData data2 = this.FileRefsGet(str);
                if (data2 != null)
                {
                    list2.Add(data2.name);
                }
            }
            return list2;
        }

        internal HashSet<string> GetGroupChildrenFilesRefs(string projectPath)
        {
            projectPath = PBXPath.FixSlashes(projectPath);
            PBXGroupData data = this.GroupsGetByProjectPath(projectPath);
            if (data == null)
            {
                return new HashSet<string>();
            }
            HashSet<string> set2 = new HashSet<string>();
            foreach (string str in (IEnumerable<string>) data.children)
            {
                PBXFileReferenceData data2 = this.FileRefsGet(str);
                if (data2 != null)
                {
                    set2.Add(data2.path);
                }
            }
            return ((set2 != null) ? set2 : new HashSet<string>());
        }

        private PBXGroupData GetPBXGroupChildByName(PBXGroupData group, string name)
        {
            foreach (string str in (IEnumerable<string>) group.children)
            {
                PBXGroupData data = this.GroupsGet(str);
                if ((data != null) && (data.name == name))
                {
                    return data;
                }
            }
            return null;
        }

        public static string GetPBXProjectPath(string buildPath) => 
            PBXPath.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");

        internal PBXProjectObjectData GetProjectInternal() => 
            this.project.project;

        public string GetTargetProductFileRef(string targetGuid) => 
            this.nativeTargets[targetGuid].productReference;

        public static string GetUnityTargetName() => 
            "Unity-iPhone";

        public static string GetUnityTestTargetName() => 
            "Unity-iPhone Tests";

        internal void GroupsAdd(string projectPath, PBXGroupData parent, PBXGroupData gr)
        {
            this.m_Data.GroupsAdd(projectPath, parent, gr);
        }

        internal void GroupsAddDuplicate(PBXGroupData gr)
        {
            this.m_Data.GroupsAddDuplicate(gr);
        }

        internal PBXGroupData GroupsGet(string guid) => 
            this.m_Data.GroupsGet(guid);

        internal PBXGroupData GroupsGetByChild(string childGuid) => 
            this.m_Data.GroupsGetByChild(childGuid);

        internal PBXGroupData GroupsGetByProjectPath(string sourceGroup) => 
            this.m_Data.GroupsGetByProjectPath(sourceGroup);

        internal PBXGroupData GroupsGetMainGroup() => 
            this.m_Data.GroupsGetMainGroup();

        internal void GroupsRemove(string guid)
        {
            this.m_Data.GroupsRemove(guid);
        }

        public static bool IsBuildable(string ext) => 
            FileTypeUtils.IsBuildableFile(ext);

        public static bool IsKnownExtension(string ext) => 
            FileTypeUtils.IsKnownExtension(ext);

        public string ProjectGuid() => 
            this.project.project.guid;

        public void ReadFromFile(string path)
        {
            this.ReadFromString(File.ReadAllText(path));
        }

        public void ReadFromStream(TextReader sr)
        {
            this.m_Data.ReadFromStream(sr);
        }

        public void ReadFromString(string src)
        {
            TextReader sr = new StringReader(src);
            this.ReadFromStream(sr);
        }

        public void RemoveAssetTag(string tag)
        {
            foreach (PBXBuildFileData data in this.BuildFilesGetAll())
            {
                data.assetTags.Remove(tag);
            }
            foreach (string str in this.nativeTargets.GetGuids())
            {
                this.RemoveAssetTagFromDefaultInstall(str, tag);
            }
            this.project.project.knownAssetTags.Remove(tag);
        }

        public void RemoveAssetTagForFile(string targetGuid, string fileGuid, string tag)
        {
            PBXBuildFileData data = this.BuildFilesGetForSourceFile(targetGuid, fileGuid);
            if (data != null)
            {
                data.assetTags.Remove(tag);
                foreach (PBXBuildFileData data2 in this.BuildFilesGetAll())
                {
                    if (data2.assetTags.Contains(tag))
                    {
                        return;
                    }
                }
                this.project.project.knownAssetTags.Remove(tag);
            }
        }

        public void RemoveAssetTagFromDefaultInstall(string targetGuid, string tag)
        {
            string[] removeValues = new string[] { tag };
            this.UpdateBuildProperty(targetGuid, "ON_DEMAND_RESOURCES_INITIAL_INSTALL_TAGS", null, removeValues);
        }

        public void RemoveBuildConfig(string name)
        {
            foreach (string str in this.GetAllTargetGuids())
            {
                this.RemoveBuildConfigForTarget(str, name);
            }
        }

        private void RemoveBuildConfigForTarget(string targetGuid, string name)
        {
            string guid = this.BuildConfigByName(targetGuid, name);
            if (guid != null)
            {
                this.buildConfigs.RemoveEntry(guid);
                this.buildConfigLists[this.GetConfigListForTarget(targetGuid)].buildConfigs.RemoveGUID(guid);
            }
        }

        internal void RemoveBuildProperty(IEnumerable<string> targetGuids, string name)
        {
            foreach (string str in targetGuids)
            {
                this.RemoveBuildProperty(str, name);
            }
        }

        internal void RemoveBuildProperty(string targetGuid, string name)
        {
            foreach (string str in (IEnumerable<string>) this.buildConfigLists[this.GetConfigListForTarget(targetGuid)].buildConfigs)
            {
                this.RemoveBuildPropertyForConfig(str, name);
            }
        }

        internal void RemoveBuildPropertyForConfig(IEnumerable<string> configGuids, string name)
        {
            foreach (string str in configGuids)
            {
                this.RemoveBuildPropertyForConfig(str, name);
            }
        }

        internal void RemoveBuildPropertyForConfig(string configGuid, string name)
        {
            this.buildConfigs[configGuid].RemoveProperty(name);
        }

        internal void RemoveBuildPropertyValueList(IEnumerable<string> targetGuids, string name, IEnumerable<string> valueList)
        {
            foreach (string str in targetGuids)
            {
                this.RemoveBuildPropertyValueList(str, name, valueList);
            }
        }

        internal void RemoveBuildPropertyValueList(string targetGuid, string name, IEnumerable<string> valueList)
        {
            foreach (string str in (IEnumerable<string>) this.buildConfigLists[this.GetConfigListForTarget(targetGuid)].buildConfigs)
            {
                this.RemoveBuildPropertyValueListForConfig(str, name, valueList);
            }
        }

        internal void RemoveBuildPropertyValueListForConfig(IEnumerable<string> configGuids, string name, IEnumerable<string> valueList)
        {
            foreach (string str in configGuids)
            {
                this.RemoveBuildPropertyValueListForConfig(str, name, valueList);
            }
        }

        internal void RemoveBuildPropertyValueListForConfig(string configGuid, string name, IEnumerable<string> valueList)
        {
            this.buildConfigs[configGuid].RemovePropertyValueList(name, valueList);
        }

        public void RemoveFile(string fileGuid)
        {
            if (fileGuid != null)
            {
                PBXGroupData gr = this.GroupsGetByChild(fileGuid);
                if (gr != null)
                {
                    gr.children.RemoveGUID(fileGuid);
                }
                this.RemoveGroupIfEmpty(gr);
                foreach (KeyValuePair<string, PBXNativeTargetData> pair in this.nativeTargets.GetEntries())
                {
                    this.RemoveFileFromBuild(pair.Value.guid, fileGuid);
                }
                this.FileRefsRemove(fileGuid);
            }
        }

        public void RemoveFileFromBuild(string targetGuid, string fileGuid)
        {
            PBXBuildFileData data = this.BuildFilesGetForSourceFile(targetGuid, fileGuid);
            if (data != null)
            {
                this.BuildFilesRemove(targetGuid, fileGuid);
                string guid = data.guid;
                if (guid != null)
                {
                    foreach (KeyValuePair<string, PBXSourcesBuildPhaseData> pair in this.sources.GetEntries())
                    {
                        pair.Value.files.RemoveGUID(guid);
                    }
                    foreach (KeyValuePair<string, PBXResourcesBuildPhaseData> pair2 in this.resources.GetEntries())
                    {
                        pair2.Value.files.RemoveGUID(guid);
                    }
                    foreach (KeyValuePair<string, PBXCopyFilesBuildPhaseData> pair3 in this.copyFiles.GetEntries())
                    {
                        pair3.Value.files.RemoveGUID(guid);
                    }
                    foreach (KeyValuePair<string, PBXFrameworksBuildPhaseData> pair4 in this.frameworks.GetEntries())
                    {
                        pair4.Value.files.RemoveGUID(guid);
                    }
                }
            }
        }

        internal void RemoveFilesByProjectPathRecursive(string projectPath)
        {
            projectPath = PBXPath.FixSlashes(projectPath);
            PBXGroupData parent = this.GroupsGetByProjectPath(projectPath);
            if (parent != null)
            {
                this.RemoveGroupChildrenRecursive(parent);
                this.RemoveGroupIfEmpty(parent);
            }
        }

        public void RemoveFrameworkFromProject(string targetGuid, string framework)
        {
            string fileGuid = this.FindFileGuidByRealPath("System/Library/Frameworks/" + framework, PBXSourceTree.Sdk);
            if (fileGuid != null)
            {
                this.BuildFilesRemove(targetGuid, fileGuid);
            }
        }

        private void RemoveGroupChildrenRecursive(PBXGroupData parent)
        {
            List<string> list = new List<string>(parent.children);
            parent.children.Clear();
            foreach (string str in list)
            {
                if (this.FileRefsGet(str) != null)
                {
                    foreach (KeyValuePair<string, PBXNativeTargetData> pair in this.nativeTargets.GetEntries())
                    {
                        this.RemoveFileFromBuild(pair.Value.guid, str);
                    }
                    this.FileRefsRemove(str);
                }
                else
                {
                    PBXGroupData data2 = this.GroupsGet(str);
                    if (data2 != null)
                    {
                        this.RemoveGroupChildrenRecursive(data2);
                        this.GroupsRemove(data2.guid);
                    }
                }
            }
        }

        private void RemoveGroupIfEmpty(PBXGroupData gr)
        {
            if ((gr.children.Count == 0) && (gr != this.GroupsGetMainGroup()))
            {
                PBXGroupData data = this.GroupsGetByChild(gr.guid);
                data.children.RemoveGUID(gr.guid);
                this.RemoveGroupIfEmpty(data);
                this.GroupsRemove(gr.guid);
            }
        }

        internal void SetBaseReferenceForConfig(string configGuid, string baseReference)
        {
            this.buildConfigs[configGuid].baseConfigurationReference = baseReference;
        }

        public void SetBuildProperty(IEnumerable<string> targetGuids, string name, string value)
        {
            foreach (string str in targetGuids)
            {
                this.SetBuildProperty(str, name, value);
            }
        }

        public void SetBuildProperty(string targetGuid, string name, string value)
        {
            foreach (string str in (IEnumerable<string>) this.buildConfigLists[this.GetConfigListForTarget(targetGuid)].buildConfigs)
            {
                this.SetBuildPropertyForConfig(str, name, value);
            }
        }

        public void SetBuildPropertyForConfig(IEnumerable<string> configGuids, string name, string value)
        {
            foreach (string str in configGuids)
            {
                this.SetBuildPropertyForConfig(str, name, value);
            }
        }

        public void SetBuildPropertyForConfig(string configGuid, string name, string value)
        {
            this.buildConfigs[configGuid].SetProperty(name, value);
        }

        public void SetCompileFlagsForFile(string targetGuid, string fileGuid, List<string> compileFlags)
        {
            PBXBuildFileData data = this.BuildFilesGetForSourceFile(targetGuid, fileGuid);
            if (data != null)
            {
                if (compileFlags == null)
                {
                    data.compileFlags = null;
                }
                else
                {
                    data.compileFlags = string.Join(" ", compileFlags.ToArray());
                }
            }
        }

        internal void SetTargetAttributes(string key, string value)
        {
            PBXElementDict dict2;
            PBXElementDict dict3;
            PBXElementDict propertiesRaw = this.project.project.GetPropertiesRaw();
            if (propertiesRaw.Contains("attributes"))
            {
                dict2 = propertiesRaw["attributes"] as PBXElementDict;
            }
            else
            {
                dict2 = propertiesRaw.CreateDict("attributes");
            }
            if (dict2.Contains("TargetAttributes"))
            {
                dict3 = dict2["TargetAttributes"] as PBXElementDict;
            }
            else
            {
                dict3 = dict2.CreateDict("TargetAttributes");
            }
            foreach (KeyValuePair<string, PBXNativeTargetData> pair in this.nativeTargets.GetEntries())
            {
                PBXElementDict dict4;
                if (dict3.Contains(pair.Key))
                {
                    dict4 = dict3[pair.Key].AsDict();
                }
                else
                {
                    dict4 = dict3.CreateDict(pair.Key);
                }
                dict4.SetString(key, value);
            }
            this.project.project.UpdateVars();
        }

        public void SetTeamId(string targetGuid, string teamId)
        {
            this.SetBuildProperty(targetGuid, "DEVELOPMENT_TEAM", teamId);
            this.project.project.teamIDs.Add(targetGuid, teamId);
        }

        internal string ShellScriptByName(string targetGuid, string name)
        {
            foreach (string str in (IEnumerable<string>) this.nativeTargets[targetGuid].phases)
            {
                PBXShellScriptBuildPhaseData data = this.shellScripts[str];
                if ((data != null) && (data.name == name))
                {
                    return data.guid;
                }
            }
            return null;
        }

        public string TargetGuidByName(string name)
        {
            foreach (KeyValuePair<string, PBXNativeTargetData> pair in this.nativeTargets.GetEntries())
            {
                if (pair.Value.name == name)
                {
                    return pair.Key;
                }
            }
            return null;
        }

        public void UpdateBuildProperty(IEnumerable<string> targetGuids, string name, IEnumerable<string> addValues, IEnumerable<string> removeValues)
        {
            foreach (string str in targetGuids)
            {
                this.UpdateBuildProperty(str, name, addValues, removeValues);
            }
        }

        public void UpdateBuildProperty(string targetGuid, string name, IEnumerable<string> addValues, IEnumerable<string> removeValues)
        {
            foreach (string str in (IEnumerable<string>) this.buildConfigLists[this.GetConfigListForTarget(targetGuid)].buildConfigs)
            {
                this.UpdateBuildPropertyForConfig(str, name, addValues, removeValues);
            }
        }

        public void UpdateBuildPropertyForConfig(IEnumerable<string> configGuids, string name, IEnumerable<string> addValues, IEnumerable<string> removeValues)
        {
            foreach (string str in configGuids)
            {
                this.UpdateBuildProperty(str, name, addValues, removeValues);
            }
        }

        public void UpdateBuildPropertyForConfig(string configGuid, string name, IEnumerable<string> addValues, IEnumerable<string> removeValues)
        {
            XCBuildConfigurationData data = this.buildConfigs[configGuid];
            if (data != null)
            {
                if (removeValues != null)
                {
                    foreach (string str in removeValues)
                    {
                        data.RemovePropertyValue(name, str);
                    }
                }
                if (addValues != null)
                {
                    foreach (string str2 in addValues)
                    {
                        data.AddProperty(name, str2);
                    }
                }
            }
        }

        public void WriteToFile(string path)
        {
            File.WriteAllText(path, this.WriteToString());
        }

        public void WriteToStream(TextWriter sw)
        {
            sw.Write(this.WriteToString());
        }

        public string WriteToString() => 
            this.m_Data.WriteToString();

        internal KnownSectionBase<XCConfigurationListData> buildConfigLists =>
            this.m_Data.buildConfigLists;

        internal KnownSectionBase<XCBuildConfigurationData> buildConfigs =>
            this.m_Data.buildConfigs;

        internal KnownSectionBase<PBXContainerItemProxyData> containerItems =>
            this.m_Data.containerItems;

        internal KnownSectionBase<PBXCopyFilesBuildPhaseData> copyFiles =>
            this.m_Data.copyFiles;

        internal KnownSectionBase<PBXFrameworksBuildPhaseData> frameworks =>
            this.m_Data.frameworks;

        internal KnownSectionBase<PBXNativeTargetData> nativeTargets =>
            this.m_Data.nativeTargets;

        internal PBXProjectSection project =>
            this.m_Data.project;

        internal KnownSectionBase<PBXReferenceProxyData> references =>
            this.m_Data.references;

        internal KnownSectionBase<PBXResourcesBuildPhaseData> resources =>
            this.m_Data.resources;

        internal KnownSectionBase<PBXShellScriptBuildPhaseData> shellScripts =>
            this.m_Data.shellScripts;

        internal KnownSectionBase<PBXSourcesBuildPhaseData> sources =>
            this.m_Data.sources;

        internal KnownSectionBase<PBXTargetDependencyData> targetDependencies =>
            this.m_Data.targetDependencies;

        internal KnownSectionBase<PBXVariantGroupData> variantGroups =>
            this.m_Data.variantGroups;
    }
}

