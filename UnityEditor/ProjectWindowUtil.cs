namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor.ProjectWindowCallback;
    using UnityEditorInternal;
    using UnityEngine;

    public class ProjectWindowUtil
    {
        internal static string k_DraggingFavoriteGenericData = "DraggingFavorite";
        internal static int k_FavoritesStartInstanceID = 0x3b9aca00;
        internal static string k_IsFolderGenericData = "IsFolder";

        private static void CreateAnimatorController()
        {
            Texture2D image = EditorGUIUtility.IconContent("AnimatorController Icon").image as Texture2D;
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateAnimatorController>(), "New Animator Controller.controller", image, null);
        }

        public static void CreateAsset(UnityEngine.Object asset, string pathName)
        {
            StartNameEditingIfProjectWindowExists(asset.GetInstanceID(), ScriptableObject.CreateInstance<DoCreateNewAsset>(), pathName, AssetPreview.GetMiniThumbnail(asset), null);
        }

        private static void CreateAudioMixer()
        {
            Texture2D image = EditorGUIUtility.IconContent("AudioMixerController Icon").image as Texture2D;
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateAudioMixer>(), "NewAudioMixer.mixer", image, null);
        }

        public static void CreateFolder()
        {
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateFolder>(), "New Folder", EditorGUIUtility.IconContent(EditorResourcesUtility.emptyFolderIconName).image as Texture2D, null);
        }

        [UnityEditor.MenuItem("Assets/Create/GUI Skin", false, 0x259)]
        public static void CreateNewGUISkin()
        {
            GUISkin dest = ScriptableObject.CreateInstance<GUISkin>();
            GUISkin builtinResource = UnityEngine.Resources.GetBuiltinResource(typeof(GUISkin), "GameSkin/GameSkin.guiskin") as GUISkin;
            if (builtinResource != null)
            {
                EditorUtility.CopySerialized(builtinResource, dest);
            }
            else
            {
                Debug.LogError("Internal error: unable to load builtin GUIskin");
            }
            CreateAsset(dest, "New GUISkin.guiskin");
        }

        public static void CreatePrefab()
        {
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreatePrefab>(), "New Prefab.prefab", EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D, null);
        }

        public static void CreateScene()
        {
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateScene>(), "New Scene.unity", EditorGUIUtility.FindTexture("SceneAsset Icon"), null);
        }

        private static void CreateScriptAsset(string templatePath, string destName)
        {
            if (Path.GetFileName(templatePath).ToLower().Contains("editortest"))
            {
                string uniquePathNameAtSelectedPath = AssetDatabase.GetUniquePathNameAtSelectedPath(destName);
                if (!uniquePathNameAtSelectedPath.ToLower().Contains("/editor/"))
                {
                    uniquePathNameAtSelectedPath = uniquePathNameAtSelectedPath.Substring(0, (uniquePathNameAtSelectedPath.Length - destName.Length) - 1);
                    string path = Path.Combine(uniquePathNameAtSelectedPath, "Editor");
                    if (!Directory.Exists(path))
                    {
                        AssetDatabase.CreateFolder(uniquePathNameAtSelectedPath, "Editor");
                    }
                    uniquePathNameAtSelectedPath = Path.Combine(path, destName).Replace(@"\", "/");
                }
                destName = uniquePathNameAtSelectedPath;
            }
            Texture2D icon = null;
            switch (Path.GetExtension(destName))
            {
                case ".js":
                    icon = EditorGUIUtility.IconContent("js Script Icon").image as Texture2D;
                    break;

                case ".cs":
                    icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
                    break;

                case ".boo":
                    icon = EditorGUIUtility.IconContent("boo Script Icon").image as Texture2D;
                    break;

                case ".shader":
                    icon = EditorGUIUtility.IconContent("Shader Icon").image as Texture2D;
                    break;

                default:
                    icon = EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;
                    break;
            }
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateScriptAsset>(), destName, icon, templatePath);
        }

        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string fullPath = Path.GetFullPath(pathName);
            string contents = File.ReadAllText(resourceFile).Replace("#NOTRIM#", "");
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            contents = contents.Replace("#NAME#", fileNameWithoutExtension);
            string newValue = fileNameWithoutExtension.Replace(" ", "");
            contents = contents.Replace("#SCRIPTNAME#", newValue);
            if (char.IsUpper(newValue, 0))
            {
                newValue = char.ToLower(newValue[0]) + newValue.Substring(1);
                contents = contents.Replace("#SCRIPTNAME_LOWER#", newValue);
            }
            else
            {
                newValue = "my" + char.ToUpper(newValue[0]) + newValue.Substring(1);
                contents = contents.Replace("#SCRIPTNAME_LOWER#", newValue);
            }
            UTF8Encoding encoding = new UTF8Encoding(true);
            File.WriteAllText(fullPath, contents, encoding);
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }

        private static void CreateSpritePolygon(int sides)
        {
            string str = "";
            switch (sides)
            {
                case 0:
                    str = "Square";
                    break;

                case 3:
                    str = "Triangle";
                    break;

                case 4:
                    str = "Diamond";
                    break;

                case 6:
                    str = "Hexagon";
                    break;

                default:
                    if (sides != 0x2a)
                    {
                        if (sides == 0x80)
                        {
                            str = "Circle";
                        }
                        else
                        {
                            str = "Polygon";
                        }
                    }
                    else
                    {
                        str = "Everythingon";
                    }
                    break;
            }
            Texture2D image = EditorGUIUtility.IconContent("Sprite Icon").image as Texture2D;
            DoCreateSpritePolygon endAction = ScriptableObject.CreateInstance<DoCreateSpritePolygon>();
            endAction.sides = sides;
            StartNameEditingIfProjectWindowExists(0, endAction, str + ".png", image, null);
        }

        internal static void DuplicateSelectedAssets()
        {
            AssetDatabase.Refresh();
            UnityEngine.Object[] objects = Selection.objects;
            bool flag = true;
            foreach (UnityEngine.Object obj2 in objects)
            {
                AnimationClip clip = obj2 as AnimationClip;
                if (((clip == null) || ((clip.hideFlags & HideFlags.NotEditable) == HideFlags.None)) || !AssetDatabase.Contains(clip))
                {
                    flag = false;
                }
            }
            ArrayList list = new ArrayList();
            bool flag2 = false;
            if (flag)
            {
                foreach (UnityEngine.Object obj3 in objects)
                {
                    AnimationClip source = obj3 as AnimationClip;
                    if ((source != null) && ((source.hideFlags & HideFlags.NotEditable) != HideFlags.None))
                    {
                        string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(Path.GetDirectoryName(AssetDatabase.GetAssetPath(obj3)), source.name) + ".anim");
                        AnimationClip dest = new AnimationClip();
                        EditorUtility.CopySerialized(source, dest);
                        AssetDatabase.CreateAsset(dest, path);
                        list.Add(path);
                    }
                }
            }
            else
            {
                UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
                foreach (UnityEngine.Object obj4 in filtered)
                {
                    string assetPath = AssetDatabase.GetAssetPath(obj4);
                    string newPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                    if (newPath.Length != 0)
                    {
                        flag2 |= !AssetDatabase.CopyAsset(assetPath, newPath);
                    }
                    else
                    {
                        flag2 |= true;
                    }
                    if (!flag2)
                    {
                        list.Add(newPath);
                    }
                }
            }
            AssetDatabase.Refresh();
            UnityEngine.Object[] objArray6 = new UnityEngine.Object[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                objArray6[i] = AssetDatabase.LoadMainAssetAtPath(list[i] as string);
            }
            Selection.objects = objArray6;
        }

        internal static void EndNameEditAction(UnityEditor.ProjectWindowCallback.EndNameEditAction action, int instanceId, string pathName, string resourceFile)
        {
            pathName = AssetDatabase.GenerateUniqueAssetPath(pathName);
            if (action != null)
            {
                action.Action(instanceId, pathName, resourceFile);
                action.CleanUp();
            }
        }

        internal static void FrameObjectInProjectWindow(int instanceID)
        {
            ProjectBrowser projectBrowserIfExists = GetProjectBrowserIfExists();
            if (projectBrowserIfExists != null)
            {
                projectBrowserIfExists.FrameObject(instanceID, false);
            }
        }

        internal static string GetActiveFolderPath() => 
            GetProjectBrowserIfExists()?.GetActiveFolderPath();

        public static int[] GetAncestors(int instanceID)
        {
            int num2;
            List<int> list = new List<int>();
            int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(AssetDatabase.GetAssetPath(instanceID));
            if (mainAssetInstanceID != instanceID)
            {
                list.Add(mainAssetInstanceID);
            }
            for (string str = GetContainingFolder(AssetDatabase.GetAssetPath(mainAssetInstanceID)); !string.IsNullOrEmpty(str); str = GetContainingFolder(AssetDatabase.GetAssetPath(num2)))
            {
                num2 = AssetDatabase.GetMainAssetInstanceID(str);
                list.Add(num2);
            }
            return list.ToArray();
        }

        public static string[] GetBaseFolders(string[] folders)
        {
            if (folders.Length <= 1)
            {
                return folders;
            }
            List<string> list = new List<string>();
            List<string> list2 = new List<string>(folders);
            for (int i = 0; i < list2.Count; i++)
            {
                char[] trimChars = new char[] { '/' };
                list2[i] = list2[i].Trim(trimChars);
            }
            list2.Sort();
            for (int j = 0; j < list2.Count; j++)
            {
                if (!list2[j].EndsWith("/"))
                {
                    list2[j] = list2[j] + "/";
                }
            }
            string item = list2[0];
            list.Add(item);
            for (int k = 1; k < list2.Count; k++)
            {
                if (list2[k].IndexOf(item, StringComparison.Ordinal) != 0)
                {
                    list.Add(list2[k]);
                    item = list2[k];
                }
            }
            for (int m = 0; m < list.Count; m++)
            {
                char[] chArray2 = new char[] { '/' };
                list[m] = list[m].Trim(chArray2);
            }
            return list.ToArray();
        }

        public static string GetContainingFolder(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                char[] trimChars = new char[] { '/' };
                path = path.Trim(trimChars);
                int length = path.LastIndexOf("/", StringComparison.Ordinal);
                if (length != -1)
                {
                    return path.Substring(0, length);
                }
            }
            return null;
        }

        internal static UnityEngine.Object[] GetDragAndDropObjects(int draggedInstanceID, List<int> selectedInstanceIDs)
        {
            List<UnityEngine.Object> list = new List<UnityEngine.Object>(selectedInstanceIDs.Count);
            if (selectedInstanceIDs.Contains(draggedInstanceID))
            {
                for (int i = 0; i < selectedInstanceIDs.Count; i++)
                {
                    UnityEngine.Object objectFromInstanceID = InternalEditorUtility.GetObjectFromInstanceID(selectedInstanceIDs[i]);
                    if (objectFromInstanceID != null)
                    {
                        list.Add(objectFromInstanceID);
                    }
                }
            }
            else
            {
                UnityEngine.Object item = InternalEditorUtility.GetObjectFromInstanceID(draggedInstanceID);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        internal static string[] GetDragAndDropPaths(int draggedInstanceID, List<int> selectedInstanceIDs)
        {
            List<string> list = new List<string>();
            foreach (int num in selectedInstanceIDs)
            {
                if (AssetDatabase.IsMainAsset(num))
                {
                    string item = AssetDatabase.GetAssetPath(num);
                    list.Add(item);
                }
            }
            string assetPath = AssetDatabase.GetAssetPath(draggedInstanceID);
            if (!string.IsNullOrEmpty(assetPath))
            {
                if (list.Contains(assetPath))
                {
                    return list.ToArray();
                }
                return new string[] { assetPath };
            }
            return new string[0];
        }

        private static ProjectBrowser GetProjectBrowserIfExists() => 
            ProjectBrowser.s_LastInteractedProjectBrowser;

        internal static bool IsFavoritesItem(int instanceID) => 
            (instanceID >= k_FavoritesStartInstanceID);

        public static bool IsFolder(int instanceID) => 
            AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(instanceID));

        public static void ShowCreatedAsset(UnityEngine.Object o)
        {
            Selection.activeObject = o;
            if (o != null)
            {
                FrameObjectInProjectWindow(o.GetInstanceID());
            }
        }

        internal static void StartDrag(int draggedInstanceID, List<int> selectedInstanceIDs)
        {
            DragAndDrop.PrepareStartDrag();
            string title = "";
            if (IsFavoritesItem(draggedInstanceID))
            {
                DragAndDrop.SetGenericData(k_DraggingFavoriteGenericData, draggedInstanceID);
                DragAndDrop.objectReferences = new UnityEngine.Object[0];
            }
            else
            {
                bool flag = IsFolder(draggedInstanceID);
                DragAndDrop.objectReferences = GetDragAndDropObjects(draggedInstanceID, selectedInstanceIDs);
                DragAndDrop.SetGenericData(k_IsFolderGenericData, !flag ? "" : "isFolder");
                string[] dragAndDropPaths = GetDragAndDropPaths(draggedInstanceID, selectedInstanceIDs);
                if (dragAndDropPaths.Length > 0)
                {
                    DragAndDrop.paths = dragAndDropPaths;
                }
                if (DragAndDrop.objectReferences.Length > 1)
                {
                    title = "<Multiple>";
                }
                else
                {
                    title = ObjectNames.GetDragAndDropTitle(InternalEditorUtility.GetObjectFromInstanceID(draggedInstanceID));
                }
            }
            DragAndDrop.StartDrag(title);
        }

        public static void StartNameEditingIfProjectWindowExists(int instanceID, UnityEditor.ProjectWindowCallback.EndNameEditAction endAction, string pathName, Texture2D icon, string resourceFile)
        {
            ProjectBrowser projectBrowserIfExists = GetProjectBrowserIfExists();
            if (projectBrowserIfExists != null)
            {
                projectBrowserIfExists.Focus();
                projectBrowserIfExists.BeginPreimportedNameEditing(instanceID, endAction, pathName, icon, resourceFile);
                projectBrowserIfExists.Repaint();
            }
            else
            {
                if (!pathName.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
                {
                    pathName = "Assets/" + pathName;
                }
                EndNameEditAction(endAction, instanceID, pathName, resourceFile);
                Selection.activeObject = EditorUtility.InstanceIDToObject(instanceID);
            }
        }
    }
}

