namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

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
            return obj2;
        }

        public static GameObject DropSpriteToSceneToCreateGO(Sprite sprite, Vector3 position)
        {
            GameObject obj2 = new GameObject(!string.IsNullOrEmpty(sprite.name) ? sprite.name : "Sprite");
            obj2.AddComponent<SpriteRenderer>().sprite = sprite;
            obj2.transform.position = position;
            Selection.activeObject = obj2;
            return obj2;
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

        private static Sprite GenerateDefaultSprite(Texture2D texture)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            TextureImporter atPath = AssetImporter.GetAtPath(assetPath) as TextureImporter;
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

        public static Sprite[] GetSpriteFromPathsOrObjects(UnityEngine.Object[] objects, string[] paths, EventType currentEventType)
        {
            List<Sprite> list = new List<Sprite>();
            bool flag = false;
            foreach (UnityEngine.Object obj2 in objects)
            {
                if (AssetDatabase.Contains(obj2))
                {
                    if (obj2 is Sprite)
                    {
                        list.Add(obj2 as Sprite);
                    }
                    else if (obj2 is Texture2D)
                    {
                        list.AddRange(TextureToSprites(obj2 as Texture2D));
                    }
                    flag = true;
                }
            }
            if (list.Count > 0)
            {
                return list.ToArray();
            }
            if (!flag)
            {
                Sprite sprite = HandleExternalDrag(paths, currentEventType == EventType.DragPerform);
                if (sprite != null)
                {
                    return new Sprite[] { sprite };
                }
            }
            return null;
        }

        public static Sprite[] GetSpritesFromDraggedObjects()
        {
            List<Sprite> list = new List<Sprite>();
            foreach (UnityEngine.Object obj2 in DragAndDrop.objectReferences)
            {
                if (obj2.GetType() == typeof(Sprite))
                {
                    list.Add(obj2 as Sprite);
                }
                else if (obj2.GetType() == typeof(Texture2D))
                {
                    list.AddRange(TextureToSprites(obj2 as Texture2D));
                }
            }
            return list.ToArray();
        }

        private static Sprite HandleExternalDrag(string[] paths, bool perform)
        {
            if (paths.Length == 0)
            {
                return null;
            }
            string path = paths[0];
            if (!ValidPathForTextureAsset(path))
            {
                return null;
            }
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (!perform)
            {
                return null;
            }
            string to = AssetDatabase.GenerateUniqueAssetPath(Path.Combine("Assets", FileUtil.GetLastPathNameComponent(path)));
            if (to.Length <= 0)
            {
                return null;
            }
            FileUtil.CopyFileOrDirectory(path, to);
            ForcedImportFor(to);
            return GenerateDefaultSprite(AssetDatabase.LoadMainAssetAtPath(to) as Texture2D);
        }

        public static void OnSceneDrag(SceneView sceneView)
        {
            Event current = Event.current;
            if (((current.type == EventType.DragUpdated) || (current.type == EventType.DragPerform)) || (current.type == EventType.DragExited))
            {
                if (!sceneView.in2DMode)
                {
                    GameObject obj2 = HandleUtility.PickGameObject(Event.current.mousePosition, true);
                    if (((obj2 != null) && (DragAndDrop.objectReferences.Length == 1)) && ((DragAndDrop.objectReferences[0] is Texture) && (obj2.GetComponent<Renderer>() != null)))
                    {
                        CleanUp(true);
                        return;
                    }
                }
                EventType type = current.type;
                if (type == EventType.DragUpdated)
                {
                    DragType type2 = !current.alt ? DragType.SpriteAnimation : DragType.CreateMultiple;
                    if ((s_DragType != type2) || (s_SceneDragObjects == null))
                    {
                        Sprite[] spriteArray = GetSpriteFromPathsOrObjects(DragAndDrop.objectReferences, DragAndDrop.paths, Event.current.type);
                        if ((spriteArray == null) || (spriteArray.Length == 0))
                        {
                            return;
                        }
                        Sprite sprite = spriteArray[0];
                        if (sprite == null)
                        {
                            return;
                        }
                        if (s_DragType != DragType.NotInitialized)
                        {
                            CleanUp(true);
                        }
                        s_DragType = type2;
                        s_SceneDragObjects = new List<UnityEngine.Object>();
                        if (s_DragType == DragType.CreateMultiple)
                        {
                            foreach (Sprite sprite2 in spriteArray)
                            {
                                s_SceneDragObjects.Add(CreateDragGO(sprite2, Vector3.zero));
                            }
                        }
                        else
                        {
                            s_SceneDragObjects.Add(CreateDragGO(spriteArray[0], Vector3.zero));
                        }
                        List<Transform> list = new List<Transform>();
                        foreach (GameObject obj3 in s_SceneDragObjects)
                        {
                            list.AddRange(obj3.GetComponentsInChildren<Transform>());
                            obj3.hideFlags = HideFlags.HideInHierarchy;
                        }
                        HandleUtility.ignoreRaySnapObjects = list.ToArray();
                    }
                    Vector3 zero = Vector3.zero;
                    zero = HandleUtility.GUIPointToWorldRay(current.mousePosition).GetPoint(10f);
                    if (sceneView.in2DMode)
                    {
                        zero.z = 0f;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        object obj4 = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(current.mousePosition));
                        if (obj4 != null)
                        {
                            RaycastHit hit = (RaycastHit) obj4;
                            zero = hit.point;
                        }
                    }
                    foreach (GameObject obj5 in s_SceneDragObjects)
                    {
                        obj5.transform.position = zero;
                    }
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    current.Use();
                }
                else if (type == EventType.DragPerform)
                {
                    Sprite[] frames = GetSpriteFromPathsOrObjects(DragAndDrop.objectReferences, DragAndDrop.paths, Event.current.type);
                    if ((frames != null) && (s_SceneDragObjects != null))
                    {
                        if (s_DragType == DragType.SpriteAnimation)
                        {
                            AddAnimationToGO((GameObject) s_SceneDragObjects[0], frames);
                        }
                        foreach (GameObject obj6 in s_SceneDragObjects)
                        {
                            Undo.RegisterCreatedObjectUndo(obj6, "Create Sprite");
                            obj6.hideFlags = HideFlags.None;
                        }
                        Selection.objects = s_SceneDragObjects.ToArray();
                        CleanUp(false);
                        current.Use();
                    }
                }
                else if ((type == EventType.DragExited) && ((s_SceneDragObjects != null) && (s_SceneDragObjects != null)))
                {
                    CleanUp(true);
                    current.Use();
                }
            }
        }

        public static Sprite RemapObjectToSprite(UnityEngine.Object obj)
        {
            if (obj is Sprite)
            {
                return (Sprite) obj;
            }
            if (obj is Texture2D)
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

        public static Sprite TextureToSprite(Texture2D tex)
        {
            Sprite[] spriteArray = TextureToSprites(tex);
            if (spriteArray.Length > 0)
            {
                return spriteArray[0];
            }
            return null;
        }

        public static Sprite[] TextureToSprites(Texture2D tex)
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
            if (list.Count > 0)
            {
                return list.ToArray();
            }
            return new Sprite[] { GenerateDefaultSprite(tex) };
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

