namespace UnityEditor.iOS.Xcode
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor.iOS.Xcode.PBX;

    public class PBXProject
    {
        private PBXProjectData m_Data = new PBXProjectData();

        internal string AddAppExtension(string mainTarget, string name, string infoPlistPath)
        {
            string ext = ".appex";
            PBXNativeTargetData data = this.CreateNewTarget(name, ext, "com.apple.product-type.app-extension");
            this.SetDefaultAppExtensionReleaseBuildFlags(this.buildConfigs[this.BuildConfigByName(data.guid, "Release")], infoPlistPath);
            this.SetDefaultAppExtensionDebugBuildFlags(this.buildConfigs[this.BuildConfigByName(data.guid, "Debug")], infoPlistPath);
            PBXSourcesBuildPhaseData data2 = PBXSourcesBuildPhaseData.Create();
            this.sources.AddEntry(data2);
            data.phases.AddGUID(data2.guid);
            PBXResourcesBuildPhaseData data3 = PBXResourcesBuildPhaseData.Create();
            this.resources.AddEntry(data3);
            data.phases.AddGUID(data3.guid);
            PBXFrameworksBuildPhaseData data4 = PBXFrameworksBuildPhaseData.Create();
            this.frameworks.AddEntry(data4);
            data.phases.AddGUID(data4.guid);
            PBXCopyFilesBuildPhaseData data5 = PBXCopyFilesBuildPhaseData.Create("Embed App Extensions", "13");
            this.copyFiles.AddEntry(data5);
            this.nativeTargets[mainTarget].phases.AddGUID(data5.guid);
            PBXContainerItemProxyData data6 = PBXContainerItemProxyData.Create(this.project.project.guid, "1", data.guid, name);
            this.containerItems.AddEntry(data6);
            PBXTargetDependencyData data7 = PBXTargetDependencyData.Create(data.guid, data6.guid);
            this.targetDependencies.AddEntry(data7);
            this.nativeTargets[mainTarget].dependencies.AddGUID(data7.guid);
            PBXBuildFileData buildFile = PBXBuildFileData.CreateFromFile(this.FindFileGuidByProjectPath("Products/" + name + ext), false, "");
            this.BuildFilesAdd(mainTarget, buildFile);
            data5.files.AddGUID(buildFile.guid);
            this.AddFile(infoPlistPath, name + "/Supporting Files/Info.plist", PBXSourceTree.Source);
            return data.guid;
        }

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
            foreach (string str in (IEnumerable<string>) this.configs[this.GetConfigListForTarget(targetGuid)].buildConfigs)
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

        public void AddExternalLibraryDependency(string targetGuid, string filename, string remoteFileGuid, string projectPath, string remoteInfo)
        {
            PBXNativeTargetData target = this.nativeTargets[targetGuid];
            filename = PBXPath.FixSlashes(filename);
            projectPath = PBXPath.FixSlashes(projectPath);
            string containerRef = this.FindFileGuidByRealPath(projectPath);
            if (containerRef == null)
            {
                throw new Exception("No such project");
            }
            string guid = null;
            foreach (ProjectReference reference in this.project.project.projectReferences)
            {
                if (reference.projectRef == containerRef)
                {
                    guid = reference.group;
                    break;
                }
            }
            if (guid == null)
            {
                throw new Exception("Malformed project: no project in project references");
            }
            PBXGroupData data2 = this.GroupsGet(guid);
            string extension = Path.GetExtension(filename);
            if (!FileTypeUtils.IsBuildableFile(extension))
            {
                throw new Exception("Wrong file extension");
            }
            PBXContainerItemProxyData data3 = PBXContainerItemProxyData.Create(containerRef, "2", remoteFileGuid, remoteInfo);
            this.containerItems.AddEntry(data3);
            string typeName = FileTypeUtils.GetTypeName(extension);
            PBXReferenceProxyData data4 = PBXReferenceProxyData.Create(filename, typeName, data3.guid, "BUILT_PRODUCTS_DIR");
            this.references.AddEntry(data4);
            PBXBuildFileData buildFile = PBXBuildFileData.CreateFromFile(data4.guid, false, null);
            this.BuildFilesAdd(targetGuid, buildFile);
            this.BuildSectionAny(target, extension, false).files.AddGUID(buildFile.guid);
            data2.children.AddGUID(data4.guid);
        }

        public void AddExternalProjectDependency(string path, string projectPath, PBXSourceTree sourceTree)
        {
            if (sourceTree == PBXSourceTree.Group)
            {
                throw new Exception("sourceTree must not be PBXSourceTree.Group");
            }
            path = PBXPath.FixSlashes(path);
            projectPath = PBXPath.FixSlashes(projectPath);
            PBXGroupData gr = PBXGroupData.CreateRelative("Products");
            this.GroupsAddDuplicate(gr);
            PBXFileReferenceData fileRef = PBXFileReferenceData.CreateFromFile(path, Path.GetFileName(projectPath), sourceTree);
            this.FileRefsAdd(path, projectPath, null, fileRef);
            this.CreateSourceGroup(PBXPath.GetDirectory(projectPath)).children.AddGUID(fileRef.guid);
            this.project.project.AddReference(gr.guid, fileRef.guid);
        }

        public string AddFile(string path, string projectPath) => 
            this.AddFileImpl(path, projectPath, PBXSourceTree.Source, false);

        public string AddFile(string path, string projectPath, PBXSourceTree sourceTree)
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

        public void AddFileToBuildWithFlags(string targetGuid, string fileGuid, string compileFlags)
        {
            this.AddBuildFileImpl(targetGuid, fileGuid, false, compileFlags);
        }

        public string AddFolderReference(string path, string projectPath) => 
            this.AddFileImpl(path, projectPath, PBXSourceTree.Source, true);

        public string AddFolderReference(string path, string projectPath, PBXSourceTree sourceTree)
        {
            if (sourceTree == PBXSourceTree.Group)
            {
                throw new Exception("sourceTree must not be PBXSourceTree.Group");
            }
            return this.AddFileImpl(path, projectPath, sourceTree, true);
        }

        public void AddFrameworkToProject(string targetGuid, string framework, bool weak)
        {
            string fileGuid = this.AddFile("System/Library/Frameworks/" + framework, "Frameworks/" + framework, PBXSourceTree.Sdk);
            this.AddBuildFileImpl(targetGuid, fileGuid, weak, null);
        }

        public string BuildConfigByName(string targetGuid, string name)
        {
            PBXNativeTargetData data = this.nativeTargets[targetGuid];
            foreach (string str in (IEnumerable<string>) this.configs[data.buildConfigList].buildConfigs)
            {
                XCBuildConfigurationData data2 = this.buildConfigs[str];
                if ((data2 != null) && (data2.name == name))
                {
                    return data2.guid;
                }
            }
            return null;
        }

        private void BuildFilesAdd(string targetGuid, PBXBuildFileData buildFile)
        {
            this.m_Data.BuildFilesAdd(targetGuid, buildFile);
        }

        private PBXBuildFileData BuildFilesGet(string guid) => 
            this.m_Data.BuildFilesGet(guid);

        private IEnumerable<PBXBuildFileData> BuildFilesGetAll() => 
            this.m_Data.BuildFilesGetAll();

        private PBXBuildFileData BuildFilesGetForSourceFile(string targetGuid, string fileGuid) => 
            this.m_Data.BuildFilesGetForSourceFile(targetGuid, fileGuid);

        private void BuildFilesRemove(string targetGuid, string fileGuid)
        {
            this.m_Data.BuildFilesRemove(targetGuid, fileGuid);
        }

        private FileGUIDListBase BuildSectionAny(PBXNativeTargetData target, string path, bool isFolderRef) => 
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

        internal PBXNativeTargetData CreateNewTarget(string name, string ext, string type)
        {
            XCBuildConfigurationData data = XCBuildConfigurationData.Create("Release");
            this.buildConfigs.AddEntry(data);
            XCBuildConfigurationData data2 = XCBuildConfigurationData.Create("Debug");
            this.buildConfigs.AddEntry(data2);
            XCConfigurationListData data3 = XCConfigurationListData.Create();
            this.configs.AddEntry(data3);
            data3.buildConfigs.AddGUID(data.guid);
            data3.buildConfigs.AddGUID(data2.guid);
            string path = name + ext;
            string productRef = this.AddFile(path, "Products/" + path, PBXSourceTree.Build);
            PBXNativeTargetData data4 = PBXNativeTargetData.Create(name, productRef, type, data3.guid);
            this.nativeTargets.AddEntry(data4);
            this.project.project.targets.Add(data4.guid);
            return data4;
        }

        private PBXGroupData CreateSourceGroup(string sourceGroup)
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

        private void FileRefsAdd(string realPath, string projectPath, PBXGroupData parent, PBXFileReferenceData fileRef)
        {
            this.m_Data.FileRefsAdd(realPath, projectPath, parent, fileRef);
        }

        private PBXFileReferenceData FileRefsGet(string guid) => 
            this.m_Data.FileRefsGet(guid);

        private PBXFileReferenceData FileRefsGetByProjectPath(string path) => 
            this.m_Data.FileRefsGetByProjectPath(path);

        private PBXFileReferenceData FileRefsGetByRealPath(string path, PBXSourceTree sourceTree) => 
            this.m_Data.FileRefsGetByRealPath(path, sourceTree);

        private void FileRefsRemove(string guid)
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
            return new List<string> { data.compileFlags };
        }

        private string GetConfigListForTarget(string targetGuid)
        {
            if (targetGuid == this.project.project.guid)
            {
                return this.project.project.buildConfigList;
            }
            return this.nativeTargets[targetGuid].buildConfigList;
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

        public static string GetUnityTargetName() => 
            "Unity-iPhone";

        public static string GetUnityTestTargetName() => 
            "Unity-iPhone Tests";

        private void GroupsAdd(string projectPath, PBXGroupData parent, PBXGroupData gr)
        {
            this.m_Data.GroupsAdd(projectPath, parent, gr);
        }

        private void GroupsAddDuplicate(PBXGroupData gr)
        {
            this.m_Data.GroupsAddDuplicate(gr);
        }

        private PBXGroupData GroupsGet(string guid) => 
            this.m_Data.GroupsGet(guid);

        private PBXGroupData GroupsGetByChild(string childGuid) => 
            this.m_Data.GroupsGetByChild(childGuid);

        private PBXGroupData GroupsGetByProjectPath(string sourceGroup) => 
            this.m_Data.GroupsGetByProjectPath(sourceGroup);

        private PBXGroupData GroupsGetMainGroup() => 
            this.m_Data.GroupsGetMainGroup();

        private void GroupsRemove(string guid)
        {
            this.m_Data.GroupsRemove(guid);
        }

        public bool HasFramework(string framework) => 
            this.ContainsFileByRealPath("System/Library/Frameworks/" + framework);

        public static bool IsBuildable(string ext) => 
            FileTypeUtils.IsBuildableFile(ext);

        public static bool IsKnownExtension(string ext) => 
            FileTypeUtils.IsKnownExtension(ext);

        internal string ProjectGuid() => 
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

        internal void RemoveBuildProperty(IEnumerable<string> targetGuids, string name)
        {
            foreach (string str in targetGuids)
            {
                this.RemoveBuildProperty(str, name);
            }
        }

        internal void RemoveBuildProperty(string targetGuid, string name)
        {
            foreach (string str in (IEnumerable<string>) this.configs[this.GetConfigListForTarget(targetGuid)].buildConfigs)
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
            foreach (string str in (IEnumerable<string>) this.configs[this.GetConfigListForTarget(targetGuid)].buildConfigs)
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
            string fileGuid = this.FindFileGuidByRealPath("System/Library/Frameworks/" + framework);
            if (fileGuid != null)
            {
                this.RemoveFile(fileGuid);
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

        public void SetBuildProperty(IEnumerable<string> targetGuids, string name, string value)
        {
            foreach (string str in targetGuids)
            {
                this.SetBuildProperty(str, name, value);
            }
        }

        public void SetBuildProperty(string targetGuid, string name, string value)
        {
            foreach (string str in (IEnumerable<string>) this.configs[this.GetConfigListForTarget(targetGuid)].buildConfigs)
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

        private void SetDefaultAppExtensionDebugBuildFlags(XCBuildConfigurationData config, string infoPlistPath)
        {
            config.AddProperty("ALWAYS_SEARCH_USER_PATHS", "NO");
            config.AddProperty("CLANG_CXX_LANGUAGE_STANDARD", "gnu++0x");
            config.AddProperty("CLANG_CXX_LIBRARY", "libc++");
            config.AddProperty("CLANG_ENABLE_MODULES", "YES");
            config.AddProperty("CLANG_ENABLE_OBJC_ARC", "YES");
            config.AddProperty("CLANG_WARN_BOOL_CONVERSION", "YES");
            config.AddProperty("CLANG_WARN_CONSTANT_CONVERSION", "YES");
            config.AddProperty("CLANG_WARN_DIRECT_OBJC_ISA_USAGE", "YES_ERROR");
            config.AddProperty("CLANG_WARN_EMPTY_BODY", "YES");
            config.AddProperty("CLANG_WARN_ENUM_CONVERSION", "YES");
            config.AddProperty("CLANG_WARN_INT_CONVERSION", "YES");
            config.AddProperty("CLANG_WARN_OBJC_ROOT_CLASS", "YES_ERROR");
            config.AddProperty("CLANG_WARN_UNREACHABLE_CODE", "YES");
            config.AddProperty("CLANG_WARN__DUPLICATE_METHOD_MATCH", "YES");
            config.AddProperty("COPY_PHASE_STRIP", "NO");
            config.AddProperty("ENABLE_STRICT_OBJC_MSGSEND", "YES");
            config.AddProperty("GCC_C_LANGUAGE_STANDARD", "gnu99");
            config.AddProperty("GCC_DYNAMIC_NO_PIC", "NO");
            config.AddProperty("GCC_OPTIMIZATION_LEVEL", "0");
            config.AddProperty("GCC_PREPROCESSOR_DEFINITIONS", "DEBUG=1");
            config.AddProperty("GCC_PREPROCESSOR_DEFINITIONS", "$(inherited)");
            config.AddProperty("GCC_SYMBOLS_PRIVATE_EXTERN", "NO");
            config.AddProperty("GCC_WARN_64_TO_32_BIT_CONVERSION", "YES");
            config.AddProperty("GCC_WARN_ABOUT_RETURN_TYPE", "YES_ERROR");
            config.AddProperty("GCC_WARN_UNDECLARED_SELECTOR", "YES");
            config.AddProperty("GCC_WARN_UNINITIALIZED_AUTOS", "YES_AGGRESSIVE");
            config.AddProperty("GCC_WARN_UNUSED_FUNCTION", "YES");
            config.AddProperty("INFOPLIST_FILE", infoPlistPath);
            config.AddProperty("IPHONEOS_DEPLOYMENT_TARGET", "8.0");
            config.AddProperty("LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
            config.AddProperty("LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            config.AddProperty("LD_RUNPATH_SEARCH_PATHS", "@executable_path/../../Frameworks");
            config.AddProperty("MTL_ENABLE_DEBUG_INFO", "YES");
            config.AddProperty("ONLY_ACTIVE_ARCH", "YES");
            config.AddProperty("PRODUCT_NAME", "$(TARGET_NAME)");
            config.AddProperty("SKIP_INSTALL", "YES");
        }

        private void SetDefaultAppExtensionReleaseBuildFlags(XCBuildConfigurationData config, string infoPlistPath)
        {
            config.AddProperty("ALWAYS_SEARCH_USER_PATHS", "NO");
            config.AddProperty("CLANG_CXX_LANGUAGE_STANDARD", "gnu++0x");
            config.AddProperty("CLANG_CXX_LIBRARY", "libc++");
            config.AddProperty("CLANG_ENABLE_MODULES", "YES");
            config.AddProperty("CLANG_ENABLE_OBJC_ARC", "YES");
            config.AddProperty("CLANG_WARN_BOOL_CONVERSION", "YES");
            config.AddProperty("CLANG_WARN_CONSTANT_CONVERSION", "YES");
            config.AddProperty("CLANG_WARN_DIRECT_OBJC_ISA_USAGE", "YES_ERROR");
            config.AddProperty("CLANG_WARN_EMPTY_BODY", "YES");
            config.AddProperty("CLANG_WARN_ENUM_CONVERSION", "YES");
            config.AddProperty("CLANG_WARN_INT_CONVERSION", "YES");
            config.AddProperty("CLANG_WARN_OBJC_ROOT_CLASS", "YES_ERROR");
            config.AddProperty("CLANG_WARN_UNREACHABLE_CODE", "YES");
            config.AddProperty("CLANG_WARN__DUPLICATE_METHOD_MATCH", "YES");
            config.AddProperty("COPY_PHASE_STRIP", "YES");
            config.AddProperty("ENABLE_NS_ASSERTIONS", "NO");
            config.AddProperty("ENABLE_STRICT_OBJC_MSGSEND", "YES");
            config.AddProperty("GCC_C_LANGUAGE_STANDARD", "gnu99");
            config.AddProperty("GCC_WARN_64_TO_32_BIT_CONVERSION", "YES");
            config.AddProperty("GCC_WARN_ABOUT_RETURN_TYPE", "YES_ERROR");
            config.AddProperty("GCC_WARN_UNDECLARED_SELECTOR", "YES");
            config.AddProperty("GCC_WARN_UNINITIALIZED_AUTOS", "YES_AGGRESSIVE");
            config.AddProperty("GCC_WARN_UNUSED_FUNCTION", "YES");
            config.AddProperty("INFOPLIST_FILE", infoPlistPath);
            config.AddProperty("IPHONEOS_DEPLOYMENT_TARGET", "8.0");
            config.AddProperty("LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
            config.AddProperty("LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            config.AddProperty("LD_RUNPATH_SEARCH_PATHS", "@executable_path/../../Frameworks");
            config.AddProperty("MTL_ENABLE_DEBUG_INFO", "NO");
            config.AddProperty("PRODUCT_NAME", "$(TARGET_NAME)");
            config.AddProperty("SKIP_INSTALL", "YES");
            config.AddProperty("VALIDATE_PRODUCT", "YES");
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
            foreach (string str in (IEnumerable<string>) this.configs[this.GetConfigListForTarget(targetGuid)].buildConfigs)
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

        private KnownSectionBase<XCBuildConfigurationData> buildConfigs =>
            this.m_Data.buildConfigs;

        private KnownSectionBase<XCConfigurationListData> configs =>
            this.m_Data.configs;

        private KnownSectionBase<PBXContainerItemProxyData> containerItems =>
            this.m_Data.containerItems;

        private KnownSectionBase<PBXCopyFilesBuildPhaseData> copyFiles =>
            this.m_Data.copyFiles;

        private KnownSectionBase<PBXFrameworksBuildPhaseData> frameworks =>
            this.m_Data.frameworks;

        private KnownSectionBase<PBXNativeTargetData> nativeTargets =>
            this.m_Data.nativeTargets;

        private PBXProjectSection project =>
            this.m_Data.project;

        private KnownSectionBase<PBXReferenceProxyData> references =>
            this.m_Data.references;

        private KnownSectionBase<PBXResourcesBuildPhaseData> resources =>
            this.m_Data.resources;

        private KnownSectionBase<PBXShellScriptBuildPhaseData> shellScripts =>
            this.m_Data.shellScripts;

        private KnownSectionBase<PBXSourcesBuildPhaseData> sources =>
            this.m_Data.sources;

        private KnownSectionBase<PBXTargetDependencyData> targetDependencies =>
            this.m_Data.targetDependencies;

        private KnownSectionBase<PBXVariantGroupData> variantGroups =>
            this.m_Data.variantGroups;
    }
}

