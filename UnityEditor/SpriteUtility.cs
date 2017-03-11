namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.U2D.Interface;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.U2D.Interface;

    internal static class SpriteUtility
    {
        [CompilerGenerated]
        private static Comparison<UnityEngine.Object> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<UnityEngine.Object, bool> <>f__am$cache1;
        private static DragType s_DragType;
        private static List<UnityEngine.Object> s_SceneDragObjects;

        public static void AddAnimationToGO(GameObject go, Sprite[] frames)
        {
            if ((frames != null) && (frames.Length > 0))
            {
                SpriteRenderer component = go.GetComponent<SpriteRenderer>();
                if (component == null)
                {
                    Debug.LogWarning("There should be a SpriteRenderer in dragged object");
                    component = go.AddComponent<SpriteRenderer>();
                }
                component.sprite = frames[0];
                if (frames.Length > 1)
                {
                    UsabilityAnalytics.Event("Sprite Drag and Drop", "Drop multiple sprites to scene", "null", 1);
                    if (!CreateAnimation(go, frames))
                    {
                        Debug.LogError("Failed to create animation for dragged object");
                    }
                }
                else
                {
                    UsabilityAnalytics.Event("Sprite Drag and Drop", "Drop single sprite to scene", "null", 1);
                }
            }
        }

        private static void AddSpriteAnimationToClip(AnimationClip newClip, UnityEngine.Object[] frames)
        {
            newClip.frameRate = 12f;
            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[frames.Length];
            for (int i = 0; i < keyframes.Length; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe();
                keyframes[i].value = RemapObjectToSprite(frames[i]);
                keyframes[i].time = ((float) i) / newClip.frameRate;
            }
            EditorCurveBinding binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
            AnimationUtility.SetObjectReferenceCurve(newClip, binding, keyframes);
        }

        private static void CleanUp(bool deleteTempSceneObject)
        {
            if (s_SceneDragObjects != null)
            {
                if (deleteTempSceneObject)
                {
                    foreach (GameObject obj2 in s_SceneDragObjects)
                    {
                        UnityEngine.Object.DestroyImmediate(obj2, false);
                    }
                }
                s_SceneDragObjects.Clear();
                s_SceneDragObjects = null;
            }
            HandleUtility.ignoreRaySnapObjects = null;
            s_DragType = DragType.NotInitialized;
        }

        private static bool CreateAnimation(GameObject gameObject, UnityEngine.Object[] frames)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = (a, b) => EditorUtility.NaturalCompare(a.name, b.name);
            }
            Array.Sort<UnityEngine.Object>(frames, <>f__am$cache0);
            if (AnimationWindowUtility.EnsureActiveAnimationPlayer(gameObject) == null)
            {
                return false;
            }
            Animator closestAnimatorInParents = AnimationWindowUtility.GetClosestAnimatorInParents(gameObject.transform);
            if (closestAnimatorInParents == null)
            {
                return false;
            }
            AnimationClip newClip = AnimationWindowUtility.CreateNewClip(gameObject.name);
            if (newClip == null)
            {
                return false;
            }
            AddSpriteAnimationToClip(newClip, frames);
            return AnimationWindowUtility.AddClipToAnimatorComponent(closestAnimatorInParents, newClip);
        }

        public static GameObject CreateDragGO(Sprite frame, Vector3 position)
        {
            string name = !string.IsNullOrEmpty(frame.name) ? frame.name : "Sprite";
            GameObject obj2 = new GameObject(GameObjectUtility.GetUniqueNameForSibling(null, name));
            obj2.AddComponent<SpriteRenderer>().sprite = frame;
            obj2.transform.position = position;
            obj2.hideFlags = HideFlags.HideInHierarchy;
            return obj2;
        }

        private static void CreateSceneDragObjects(List<Sprite> sprites)
        {
            if (s_SceneDragObjects == null)
            {
                s_SceneDragObjects = new List<UnityEngine.Object>();
            }
            if (s_DragType == DragType.CreateMultiple)
            {
                foreach (Sprite sprite in sprites)
                {
                    s_SceneDragObjects.Add(CreateDragGO(sprite, Vector3.zero));
                }
            }
            else
            {
                s_SceneDragObjects.Add(CreateDragGO(sprites[0], Vector3.zero));
            }
        }

        public static UnityEngine.Texture2D CreateTemporaryDuplicate(UnityEngine.Texture2D original, int width, int height)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture || (original == null))
            {
                return null;
            }
            RenderTexture active = RenderTexture.active;
            bool flag = !TextureUtil.GetLinearSampled(original);
            RenderTexture dest = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, !flag ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
            GL.sRGBWrite = flag && (QualitySettings.activeColorSpace == ColorSpace.Linear);
            Graphics.Blit(original, dest);
            GL.sRGBWrite = false;
            RenderTexture.active = dest;
            bool flag2 = (width >= SystemInfo.maxTextureSize) || (height >= SystemInfo.maxTextureSize);
            UnityEngine.Texture2D textured2 = new UnityEngine.Texture2D(width, height, TextureFormat.RGBA32, (original.mipmapCount > 1) || flag2);
            textured2.ReadPixels(new Rect(0f, 0f, (float) width, (float) height), 0, 0);
            textured2.Apply();
            RenderTexture.ReleaseTemporary(dest);
            EditorGUIUtility.SetRenderTextureNoViewport(active);
            textured2.alphaIsTransparency = original.alphaIsTransparency;
            return textured2;
        }

        public static GameObject DropSpriteToSceneToCreateGO(Sprite sprite, Vector3 position)
        {
            GameObject obj2 = new GameObject(!string.IsNullOrEmpty(sprite.name) ? sprite.name : "Sprite");
            obj2.AddComponent<SpriteRenderer>().sprite = sprite;
            obj2.transform.position = position;
            Selection.activeObject = obj2;
            return obj2;
        }

        public static bool ExistingAssets(UnityEngine.Object[] objects)
        {
            foreach (UnityEngine.Object obj2 in objects)
            {
                if (AssetDatabase.Contains(obj2))
                {
                    return true;
                }
            }
            return false;
        }

        private static void ForcedImportFor(string newPath)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                AssetDatabase.ImportAsset(newPath);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private static Sprite GenerateDefaultSprite(UnityEngine.Texture2D texture)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            UnityEditor.TextureImporter atPath = AssetImporter.GetAtPath(assetPath) as UnityEditor.TextureImporter;
            if (atPath == null)
            {
                return null;
            }
            if (atPath.textureType != TextureImporterType.Sprite)
            {
                return null;
            }
            if (atPath.spriteImportMode == SpriteImportMode.None)
            {
                atPath.spriteImportMode = SpriteImportMode.Single;
                AssetDatabase.WriteImportSettingsIfDirty(assetPath);
                ForcedImportFor(assetPath);
            }
            UnityEngine.Object obj2 = null;
            try
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = t => t is Sprite;
                }
                obj2 = Enumerable.First<UnityEngine.Object>(AssetDatabase.LoadAllAssetsAtPath(assetPath), <>f__am$cache1);
            }
            catch (Exception)
            {
                Debug.LogWarning("Texture being dragged has no Sprites.");
            }
            return (obj2 as Sprite);
        }

        public static List<Sprite> GetSpriteFromPathsOrObjects(UnityEngine.Object[] objects, string[] paths, EventType currentEventType)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (UnityEngine.Object obj2 in objects)
            {
                if (AssetDatabase.Contains(obj2))
                {
                    if (obj2 is Sprite)
                    {
                        result.Add(obj2 as Sprite);
                    }
                    else if (obj2 is UnityEngine.Texture2D)
                    {
                        result.AddRange(TextureToSprites(obj2 as UnityEngine.Texture2D));
                    }
                }
            }
            if ((!ExistingAssets(objects) && (currentEventType == EventType.DragPerform)) && (EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D))
            {
                HandleExternalDrag(paths, true, ref result);
            }
            return result;
        }

        public static SpriteImportMode GetSpriteImportMode(IAssetDatabase assetDatabase, ITexture2D texture)
        {
            string assetPath = assetDatabase.GetAssetPath((UnityEngine.Object) texture);
            if (string.IsNullOrEmpty(assetPath))
            {
                return SpriteImportMode.None;
            }
            ITextureImporter assetImporterFromPath = assetDatabase.GetAssetImporterFromPath(assetPath);
            return ((assetImporterFromPath != null) ? assetImporterFromPath.spriteImportMode : SpriteImportMode.None);
        }

        private static void HandleExternalDrag(string[] paths, bool perform, ref List<Sprite> result)
        {
            foreach (string str in paths)
            {
                if (ValidPathForTextureAsset(str))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (perform)
                    {
                        string to = AssetDatabase.GenerateUniqueAssetPath(Path.Combine("Assets", FileUtil.GetLastPathNameComponent(str)));
                        if (to.Length > 0)
                        {
                            FileUtil.CopyFileOrDirectory(str, to);
                            ForcedImportFor(to);
                            Sprite item = GenerateDefaultSprite(AssetDatabase.LoadMainAssetAtPath(to) as UnityEngine.Texture2D);
                            if (item != null)
                            {
                                result.Add(item);
                            }
                        }
                    }
                }
            }
        }

        public static void HandleSpriteSceneDrag(SceneView sceneView, IEvent evt, UnityEngine.Object[] objectReferences, string[] paths)
        {
            if (((evt.type == EventType.DragUpdated) || (evt.type == EventType.DragPerform)) || (evt.type == EventType.DragExited))
            {
                if ((objectReferences.Length == 1) && (objectReferences[0] is UnityEngine.Texture2D))
                {
                    GameObject obj2 = HandleUtility.PickGameObject(evt.mousePosition, true);
                    if ((obj2 != null) && (obj2.GetComponent<Renderer>() != null))
                    {
                        CleanUp(true);
                        return;
                    }
                }
                EventType type = evt.type;
                if (type == EventType.DragUpdated)
                {
                    DragType type2 = !evt.alt ? DragType.SpriteAnimation : DragType.CreateMultiple;
                    if ((s_DragType != type2) || (s_SceneDragObjects == null))
                    {
                        if (!ExistingAssets(objectReferences) && PathsAreValidTextures(paths))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            s_SceneDragObjects = new List<UnityEngine.Object>();
                            s_DragType = type2;
                        }
                        else
                        {
                            List<Sprite> sprites = GetSpriteFromPathsOrObjects(objectReferences, paths, evt.type);
                            if (sprites.Count == 0)
                            {
                                return;
                            }
                            if (s_DragType != DragType.NotInitialized)
                            {
                                CleanUp(true);
                            }
                            s_DragType = type2;
                            CreateSceneDragObjects(sprites);
                            IgnoreForRaycasts(s_SceneDragObjects);
                        }
                    }
                    PositionSceneDragObjects(s_SceneDragObjects, sceneView, evt.mousePosition);
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    evt.Use();
                }
                else if (type == EventType.DragPerform)
                {
                    List<Sprite> list2 = GetSpriteFromPathsOrObjects(objectReferences, paths, evt.type);
                    if ((list2.Count > 0) && (s_SceneDragObjects != null))
                    {
                        if (s_SceneDragObjects.Count == 0)
                        {
                            CreateSceneDragObjects(list2);
                            PositionSceneDragObjects(s_SceneDragObjects, sceneView, evt.mousePosition);
                        }
                        if (s_DragType == DragType.SpriteAnimation)
                        {
                            AddAnimationToGO((GameObject) s_SceneDragObjects[0], list2.ToArray());
                        }
                        foreach (GameObject obj3 in s_SceneDragObjects)
                        {
                            Undo.RegisterCreatedObjectUndo(obj3, "Create Sprite");
                            obj3.hideFlags = HideFlags.None;
                        }
                        Selection.objects = s_SceneDragObjects.ToArray();
                        CleanUp(false);
                        evt.Use();
                    }
                }
                else if ((type == EventType.DragExited) && (s_SceneDragObjects != null))
                {
                    CleanUp(true);
                    evt.Use();
                }
            }
        }

        private static void IgnoreForRaycasts(List<UnityEngine.Object> objects)
        {
            List<Transform> list = new List<Transform>();
            foreach (GameObject obj2 in objects)
            {
                list.AddRange(obj2.GetComponentsInChildren<Transform>());
            }
            HandleUtility.ignoreRaySnapObjects = list.ToArray();
        }

        public static void OnSceneDrag(SceneView sceneView)
        {
            HandleSpriteSceneDrag(sceneView, new UnityEngine.U2D.Interface.Event(), DragAndDrop.objectReferences, DragAndDrop.paths);
        }

        private static bool PathsAreValidTextures(string[] paths)
        {
            if ((paths == null) || (paths.Length == 0))
            {
                return false;
            }
            foreach (string str in paths)
            {
                if (!ValidPathForTextureAsset(str))
                {
                    return false;
                }
            }
            return true;
        }

        private static void PositionSceneDragObjects(List<UnityEngine.Object> objects, SceneView sceneView, Vector2 mousePosition)
        {
            Vector3 zero = Vector3.zero;
            zero = HandleUtility.GUIPointToWorldRay(mousePosition).GetPoint(10f);
            if (sceneView.in2DMode)
            {
                zero.z = 0f;
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                object obj2 = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(mousePosition));
                if (obj2 != null)
                {
                    RaycastHit hit = (RaycastHit) obj2;
                    zero = hit.point;
                }
            }
            foreach (GameObject obj3 in objects)
            {
                obj3.transform.position = zero;
            }
        }

        public static Sprite RemapObjectToSprite(UnityEngine.Object obj)
        {
            if (obj is Sprite)
            {
                return (Sprite) obj;
            }
            if (obj is UnityEngine.Texture2D)
            {
                UnityEngine.Object[] objArray = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj));
                for (int i = 0; i < objArray.Length; i++)
                {
                    if (objArray[i].GetType() == typeof(Sprite))
                    {
                        return (objArray[i] as Sprite);
                    }
                }
            }
            return null;
        }

        public static Sprite TextureToSprite(UnityEngine.Texture2D tex)
        {
            List<Sprite> list = TextureToSprites(tex);
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public static List<Sprite> TextureToSprites(UnityEngine.Texture2D tex)
        {
            UnityEngine.Object[] objArray = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tex));
            List<Sprite> list = new List<Sprite>();
            for (int i = 0; i < objArray.Length; i++)
            {
                if (objArray[i].GetType() == typeof(Sprite))
                {
                    list.Add(objArray[i] as Sprite);
                }
            }
            if (list.Count <= 0)
            {
                Sprite item = GenerateDefaultSprite(tex);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        private static bool ValidPathForTextureAsset(string path)
        {
            string str = FileUtil.GetPathExtension(path).ToLower();
            return ((((((str == "jpg") || (str == "jpeg")) || ((str == "tif") || (str == "tiff"))) || (((str == "tga") || (str == "gif")) || ((str == "png") || (str == "psd")))) || ((((str == "bmp") || (str == "iff")) || ((str == "pict") || (str == "pic"))) || ((str == "pct") || (str == "exr")))) || (str == "hdr"));
        }

        private enum DragType
        {
            NotInitialized,
            SpriteAnimation,
            CreateMultiple
        }
    }
}

