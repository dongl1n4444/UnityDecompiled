namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal static class AnimationWindowUtility
    {
        internal static string s_LastPathUsedForNewClip;

        public static bool AddClipToAnimationComponent(Animation animation, AnimationClip newClip)
        {
            SetClipAsLegacy(newClip);
            animation.AddClip(newClip, newClip.name);
            return true;
        }

        public static bool AddClipToAnimationPlayerComponent(Component animationPlayer, AnimationClip newClip)
        {
            if (animationPlayer is Animator)
            {
                return AddClipToAnimatorComponent(animationPlayer as Animator, newClip);
            }
            return ((animationPlayer is Animation) && AddClipToAnimationComponent(animationPlayer as Animation, newClip));
        }

        public static bool AddClipToAnimatorComponent(Animator animator, AnimationClip newClip)
        {
            UnityEditor.Animations.AnimatorController effectiveAnimatorController = UnityEditor.Animations.AnimatorController.GetEffectiveAnimatorController(animator);
            if (effectiveAnimatorController == null)
            {
                effectiveAnimatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerForClip(newClip, animator.gameObject);
                UnityEditor.Animations.AnimatorController.SetAnimatorController(animator, effectiveAnimatorController);
                if (effectiveAnimatorController != null)
                {
                    return true;
                }
            }
            else
            {
                ChildAnimatorState state = effectiveAnimatorController.layers[0].stateMachine.FindState(newClip.name);
                if (state.Equals(new ChildAnimatorState()))
                {
                    effectiveAnimatorController.AddMotion(newClip);
                }
                else if ((state.state != null) && (state.state.motion == null))
                {
                    state.state.motion = newClip;
                }
                else if ((state.state != null) && (state.state.motion != newClip))
                {
                    effectiveAnimatorController.AddMotion(newClip);
                }
                return true;
            }
            return false;
        }

        public static void AddKeyframes(AnimationWindowState state, AnimationWindowCurve[] curves, AnimationKeyTime time)
        {
            string undoLabel = "Add Key";
            state.SaveKeySelection(undoLabel);
            state.ClearKeySelections();
            foreach (AnimationWindowCurve curve in curves)
            {
                if (curve.animationIsEditable)
                {
                    AnimationKeyTime time2 = AnimationKeyTime.Time(time.time - curve.timeOffset, time.frameRate);
                    object currentValue = CurveBindingUtility.GetCurrentValue(state, curve);
                    AnimationWindowKeyframe keyframe = AddKeyframeToCurve(curve, currentValue, curve.valueType, time2);
                    if (keyframe != null)
                    {
                        state.SaveCurve(curve, undoLabel);
                        state.SelectKey(keyframe);
                    }
                }
            }
        }

        public static AnimationWindowKeyframe AddKeyframeToCurve(AnimationWindowCurve curve, object value, System.Type type, AnimationKeyTime time)
        {
            AnimationWindowKeyframe keyframe = curve.FindKeyAtTime(time);
            if (keyframe != null)
            {
                keyframe.value = value;
                return keyframe;
            }
            AnimationWindowKeyframe key = null;
            if (curve.isPPtrCurve)
            {
                key = new AnimationWindowKeyframe {
                    time = time.time,
                    value = value,
                    curve = curve
                };
                curve.AddKeyframe(key, time);
                return key;
            }
            if ((type == typeof(bool)) || (type == typeof(float)))
            {
                key = new AnimationWindowKeyframe();
                AnimationCurve curve2 = curve.ToAnimationCurve();
                Keyframe keyframe4 = new Keyframe(time.time, (float) value);
                if (type == typeof(bool))
                {
                    AnimationUtility.SetKeyLeftTangentMode(ref keyframe4, AnimationUtility.TangentMode.Constant);
                    AnimationUtility.SetKeyRightTangentMode(ref keyframe4, AnimationUtility.TangentMode.Constant);
                    AnimationUtility.SetKeyBroken(ref keyframe4, true);
                    key.m_TangentMode = keyframe4.tangentMode;
                    key.m_InTangent = float.PositiveInfinity;
                    key.m_OutTangent = float.PositiveInfinity;
                }
                else
                {
                    int keyIndex = curve2.AddKey(keyframe4);
                    if (keyIndex != -1)
                    {
                        CurveUtility.SetKeyModeFromContext(curve2, keyIndex);
                        Keyframe keyframe5 = curve2[keyIndex];
                        key.m_TangentMode = keyframe5.tangentMode;
                    }
                }
                key.time = time.time;
                key.value = value;
                key.curve = curve;
                curve.AddKeyframe(key, time);
            }
            return key;
        }

        public static void AddSelectedKeyframes(AnimationWindowState state, AnimationKeyTime time)
        {
            AddKeyframes(state, ((state.activeCurves.Count <= 0) ? state.allCurves : state.activeCurves).ToArray(), time);
        }

        internal static AnimationClip AllocateAndSetupClip(bool useAnimator)
        {
            AnimationClip clip = new AnimationClip();
            if (useAnimator)
            {
                AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(clip);
                animationClipSettings.loopTime = true;
                AnimationUtility.SetAnimationClipSettingsNoDirty(clip, animationClipSettings);
            }
            return clip;
        }

        public static CurveSelection AnimationWindowKeyframeToCurveSelection(AnimationWindowKeyframe keyframe, CurveEditor curveEditor)
        {
            int hashCode = keyframe.curve.GetHashCode();
            foreach (CurveWrapper wrapper in curveEditor.animationCurves)
            {
                if ((wrapper.id == hashCode) && (keyframe.GetIndex() >= 0))
                {
                    return new CurveSelection(wrapper.id, keyframe.GetIndex());
                }
            }
            return null;
        }

        public static AnimationWindowCurve BestMatchForPaste(EditorCurveBinding binding, List<AnimationWindowCurve> clipboardCurves, List<AnimationWindowCurve> targetCurves)
        {
            foreach (AnimationWindowCurve curve in targetCurves)
            {
                if (curve.binding == binding)
                {
                    return curve;
                }
            }
            using (List<AnimationWindowCurve>.Enumerator enumerator2 = targetCurves.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    <BestMatchForPaste>c__AnonStorey0 storey = new <BestMatchForPaste>c__AnonStorey0 {
                        targetCurve = enumerator2.Current
                    };
                    if ((storey.targetCurve.binding.propertyName == binding.propertyName) && !clipboardCurves.Exists(new Predicate<AnimationWindowCurve>(storey.<>m__0)))
                    {
                        return storey.targetCurve;
                    }
                }
            }
            return null;
        }

        public static bool ContainsFloatKeyframes(List<AnimationWindowKeyframe> keyframes)
        {
            if ((keyframes != null) && (keyframes.Count != 0))
            {
                foreach (AnimationWindowKeyframe keyframe in keyframes)
                {
                    if (!keyframe.isPPtrCurve)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void ControllerChanged()
        {
            foreach (AnimationWindow window in AnimationWindow.GetAllAnimationWindows())
            {
                window.OnControllerChange();
            }
        }

        public static AnimationWindowCurve CreateDefaultCurve(AnimationWindowSelectionItem selectionItem, EditorCurveBinding binding)
        {
            AnimationClip animationClip = selectionItem.animationClip;
            System.Type editorCurveValueType = selectionItem.GetEditorCurveValueType(binding);
            AnimationWindowCurve curve = new AnimationWindowCurve(animationClip, binding, editorCurveValueType);
            object currentValue = CurveBindingUtility.GetCurrentValue(selectionItem.rootGameObject, binding);
            if (animationClip.length == 0f)
            {
                AddKeyframeToCurve(curve, currentValue, editorCurveValueType, AnimationKeyTime.Time(0f, animationClip.frameRate));
                return curve;
            }
            AddKeyframeToCurve(curve, currentValue, editorCurveValueType, AnimationKeyTime.Time(0f, animationClip.frameRate));
            AddKeyframeToCurve(curve, currentValue, editorCurveValueType, AnimationKeyTime.Time(animationClip.length, animationClip.frameRate));
            return curve;
        }

        public static void CreateDefaultCurves(IAnimationRecordingState state, AnimationWindowSelectionItem selectionItem, EditorCurveBinding[] properties)
        {
            properties = RotationCurveInterpolation.ConvertRotationPropertiesToDefaultInterpolation(selectionItem.animationClip, properties);
            foreach (EditorCurveBinding binding in properties)
            {
                state.SaveCurve(CreateDefaultCurve(selectionItem, binding));
            }
        }

        internal static AnimationClip CreateNewClip(string gameObjectName)
        {
            string message = $"Create a new animation for the game object '{gameObjectName}':";
            string activeFolderPath = ProjectWindowUtil.GetActiveFolderPath();
            if (s_LastPathUsedForNewClip != null)
            {
                string directoryName = Path.GetDirectoryName(s_LastPathUsedForNewClip);
                if ((directoryName != null) && Directory.Exists(directoryName))
                {
                    activeFolderPath = directoryName;
                }
            }
            string clipPath = EditorUtility.SaveFilePanelInProject("Create New Animation", "New Animation", "anim", message, activeFolderPath);
            if (clipPath == "")
            {
                return null;
            }
            return CreateNewClipAtPath(clipPath);
        }

        internal static AnimationClip CreateNewClipAtPath(string clipPath)
        {
            s_LastPathUsedForNewClip = clipPath;
            AnimationClip clip = new AnimationClip();
            AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            animationClipSettings.loopTime = true;
            AnimationUtility.SetAnimationClipSettingsNoDirty(clip, animationClipSettings);
            AnimationClip dest = AssetDatabase.LoadMainAssetAtPath(clipPath) as AnimationClip;
            if (dest != null)
            {
                EditorUtility.CopySerialized(clip, dest);
                AssetDatabase.SaveAssets();
                UnityEngine.Object.DestroyImmediate(clip);
                return dest;
            }
            AssetDatabase.CreateAsset(clip, clipPath);
            return clip;
        }

        public static bool CurveExists(EditorCurveBinding binding, AnimationWindowCurve[] curves)
        {
            foreach (AnimationWindowCurve curve in curves)
            {
                if (((binding.propertyName == curve.binding.propertyName) && (binding.type == curve.binding.type)) && (binding.path == curve.binding.path))
                {
                    return true;
                }
            }
            return false;
        }

        public static AnimationWindowKeyframe CurveSelectionToAnimationWindowKeyframe(CurveSelection curveSelection, List<AnimationWindowCurve> allCurves)
        {
            foreach (AnimationWindowCurve curve in allCurves)
            {
                if ((curve.GetHashCode() == curveSelection.curveID) && (curve.m_Keyframes.Count > curveSelection.key))
                {
                    return curve.m_Keyframes[curveSelection.key];
                }
            }
            return null;
        }

        public static void DrawPlayHead(float positionX, float minY, float maxY, float alpha)
        {
            TimeArea.DrawVerticalLine(positionX, minY, maxY, Color.red.AlphaMultiplied(alpha));
        }

        public static void DrawRangeOfClip(Rect rect, float startOfClipPixel, float endOfClipPixel)
        {
            Color color = !EditorGUIUtility.isProSkin ? Color.gray.AlphaMultiplied(0.32f) : Color.gray.RGBMultiplied((float) 0.3f).AlphaMultiplied(0.5f);
            Color color5 = Color.white.RGBMultiplied((float) 0.4f);
            if (startOfClipPixel > rect.xMin)
            {
                Rect rect2 = new Rect(rect.xMin, rect.yMin, Mathf.Min(startOfClipPixel - rect.xMin, rect.width), rect.height);
                Vector3[] vectorArray = new Vector3[] { new Vector3(rect2.xMin, rect2.yMin), new Vector3(rect2.xMax, rect2.yMin), new Vector3(rect2.xMax, rect2.yMax), new Vector3(rect2.xMin, rect2.yMax) };
                DrawRect(vectorArray, color);
                TimeArea.DrawVerticalLine(vectorArray[1].x, vectorArray[1].y, vectorArray[2].y, color5);
                Handles.color = color5;
                Handles.DrawLine(vectorArray[1], vectorArray[2] + new Vector3(0f, -1f, 0f));
            }
            Rect rect3 = new Rect(Mathf.Max(endOfClipPixel, rect.xMin), rect.yMin, rect.width, rect.height);
            Vector3[] corners = new Vector3[] { new Vector3(rect3.xMin, rect3.yMin), new Vector3(rect3.xMax, rect3.yMin), new Vector3(rect3.xMax, rect3.yMax), new Vector3(rect3.xMin, rect3.yMax) };
            DrawRect(corners, color);
            TimeArea.DrawVerticalLine(corners[0].x, corners[0].y, corners[3].y, color5);
            Handles.color = color5;
            Handles.DrawLine(corners[0], corners[3] + new Vector3(0f, -1f, 0f));
        }

        public static void DrawRangeOfSelection(Rect rect, float startPixel, float endPixel)
        {
            Color color = !EditorGUIUtility.isProSkin ? Color.gray.AlphaMultiplied(0.25f) : Color.white.AlphaMultiplied(0.1f);
            startPixel = Mathf.Max(startPixel, rect.xMin);
            endPixel = Mathf.Max(endPixel, rect.xMin);
            Vector3[] corners = new Vector3[] { new Vector3(startPixel, rect.yMin), new Vector3(endPixel, rect.yMin), new Vector3(endPixel, rect.yMax), new Vector3(startPixel, rect.yMax) };
            DrawRect(corners, color);
        }

        private static void DrawRect(Vector3[] corners, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                HandleUtility.ApplyWireMaterial();
                GL.PushMatrix();
                GL.MultMatrix(Handles.matrix);
                GL.Begin(7);
                GL.Color(color);
                GL.Vertex(corners[0]);
                GL.Vertex(corners[1]);
                GL.Vertex(corners[2]);
                GL.Vertex(corners[3]);
                GL.End();
                GL.PopMatrix();
            }
        }

        public static Component EnsureActiveAnimationPlayer(GameObject animatedObject)
        {
            Component closestAnimationPlayerComponentInParents = GetClosestAnimationPlayerComponentInParents(animatedObject.transform);
            if (closestAnimationPlayerComponentInParents == null)
            {
                return Undo.AddComponent<Animator>(animatedObject);
            }
            return closestAnimationPlayerComponentInParents;
        }

        private static bool EnsureAnimationPlayerHasClip(Component animationPlayer)
        {
            if (animationPlayer == null)
            {
                return false;
            }
            if (AnimationUtility.GetAnimationClips(animationPlayer.gameObject).Length > 0)
            {
                return true;
            }
            AnimationClip newClip = CreateNewClip(animationPlayer.gameObject.name);
            if (newClip == null)
            {
                return false;
            }
            UnityEditor.AnimationMode.StopAnimationMode();
            return AddClipToAnimationPlayerComponent(animationPlayer, newClip);
        }

        public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, bool entireHierarchy)
        {
            List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
            if (curves != null)
            {
                foreach (AnimationWindowCurve curve in curves)
                {
                    if (curve.path.Equals(path) || (entireHierarchy && curve.path.Contains(path)))
                    {
                        list.Add(curve);
                    }
                }
            }
            return list;
        }

        public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, System.Type animatableObjectType)
        {
            List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
            if (curves != null)
            {
                foreach (AnimationWindowCurve curve in curves)
                {
                    if (curve.path.Equals(path) && (curve.type == animatableObjectType))
                    {
                        list.Add(curve);
                    }
                }
            }
            return list;
        }

        public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, System.Type animatableObjectType, string propertyName)
        {
            List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
            if (curves != null)
            {
                string propertyGroupName = GetPropertyGroupName(propertyName);
                bool flag = propertyGroupName == propertyName;
                foreach (AnimationWindowCurve curve in curves)
                {
                    bool flag2 = !flag ? curve.propertyName.Equals(propertyName) : GetPropertyGroupName(curve.propertyName).Equals(propertyGroupName);
                    if ((curve.path.Equals(path) && (curve.type == animatableObjectType)) && flag2)
                    {
                        list.Add(curve);
                    }
                }
            }
            return list;
        }

        public static bool ForceGrouping(EditorCurveBinding binding)
        {
            if (binding.type == typeof(Transform))
            {
                return true;
            }
            if (binding.type == typeof(RectTransform))
            {
                string propertyGroupName = GetPropertyGroupName(binding.propertyName);
                return (((((propertyGroupName == "m_LocalPosition") || (propertyGroupName == "m_LocalScale")) || ((propertyGroupName == "m_LocalRotation") || (propertyGroupName == "localEulerAnglesBaked"))) || (propertyGroupName == "localEulerAngles")) || (propertyGroupName == "localEulerAnglesRaw"));
            }
            return (typeof(Renderer).IsAssignableFrom(binding.type) && (GetPropertyGroupName(binding.propertyName) == "material._Color"));
        }

        internal static Rect FromToRect(Vector2 start, Vector2 end)
        {
            Rect rect = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
            if (rect.width < 0f)
            {
                rect.x += rect.width;
                rect.width = -rect.width;
            }
            if (rect.height < 0f)
            {
                rect.y += rect.height;
                rect.height = -rect.height;
            }
            return rect;
        }

        public static bool GameObjectIsAnimatable(GameObject gameObject, AnimationClip animationClip)
        {
            if (gameObject == null)
            {
                return false;
            }
            if ((gameObject.hideFlags & HideFlags.NotEditable) != HideFlags.None)
            {
                return false;
            }
            if (EditorUtility.IsPersistent(gameObject))
            {
                return false;
            }
            if ((animationClip != null) && (((animationClip.hideFlags & HideFlags.NotEditable) != HideFlags.None) || !AssetDatabase.IsOpenForEdit(animationClip, StatusQueryOptions.UseCachedIfPossible)))
            {
                return false;
            }
            return true;
        }

        public static List<EditorCurveBinding> GetAnimatableProperties(ScriptableObject scriptableObject, System.Type valueType)
        {
            EditorCurveBinding[] scriptableObjectAnimatableBindings = AnimationUtility.GetScriptableObjectAnimatableBindings(scriptableObject);
            List<EditorCurveBinding> list = new List<EditorCurveBinding>();
            foreach (EditorCurveBinding binding in scriptableObjectAnimatableBindings)
            {
                if (AnimationUtility.GetScriptableObjectEditorCurveValueType(scriptableObject, binding) == valueType)
                {
                    list.Add(binding);
                }
            }
            return list;
        }

        public static List<EditorCurveBinding> GetAnimatableProperties(GameObject gameObject, GameObject root, System.Type valueType)
        {
            EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(gameObject, root);
            List<EditorCurveBinding> list = new List<EditorCurveBinding>();
            foreach (EditorCurveBinding binding in animatableBindings)
            {
                if (AnimationUtility.GetEditorCurveValueType(root, binding) == valueType)
                {
                    list.Add(binding);
                }
            }
            return list;
        }

        public static List<EditorCurveBinding> GetAnimatableProperties(GameObject gameObject, GameObject root, System.Type objectType, System.Type valueType)
        {
            EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(gameObject, root);
            List<EditorCurveBinding> list = new List<EditorCurveBinding>();
            foreach (EditorCurveBinding binding in animatableBindings)
            {
                if ((binding.type == objectType) && (AnimationUtility.GetEditorCurveValueType(root, binding) == valueType))
                {
                    list.Add(binding);
                }
            }
            return list;
        }

        public static Animation GetClosestAnimationInParents(Transform tr)
        {
            while (true)
            {
                if (tr.GetComponent<Animation>() != null)
                {
                    return tr.GetComponent<Animation>();
                }
                if (tr == tr.root)
                {
                    break;
                }
                tr = tr.parent;
            }
            return null;
        }

        public static Component GetClosestAnimationPlayerComponentInParents(Transform tr)
        {
            Animator closestAnimatorInParents = GetClosestAnimatorInParents(tr);
            if (closestAnimatorInParents != null)
            {
                return closestAnimatorInParents;
            }
            Animation closestAnimationInParents = GetClosestAnimationInParents(tr);
            if (closestAnimationInParents != null)
            {
                return closestAnimationInParents;
            }
            return null;
        }

        public static Animator GetClosestAnimatorInParents(Transform tr)
        {
            while (true)
            {
                if (tr.GetComponent<Animator>() != null)
                {
                    return tr.GetComponent<Animator>();
                }
                if (tr == tr.root)
                {
                    break;
                }
                tr = tr.parent;
            }
            return null;
        }

        public static int GetComponentIndex(string name)
        {
            if (((name != null) && (name.Length >= 3)) && (name[name.Length - 2] == '.'))
            {
                switch (name[name.Length - 1])
                {
                    case 'w':
                        return 3;

                    case 'x':
                        return 0;

                    case 'y':
                        return 1;

                    case 'z':
                        return 2;

                    case 'a':
                        return 3;

                    case 'b':
                        return 2;

                    case 'g':
                        return 1;

                    case 'r':
                        return 0;
                }
            }
            return -1;
        }

        public static object GetCurrentValue(GameObject rootGameObject, EditorCurveBinding curveBinding)
        {
            float num;
            if (curveBinding.isPPtrCurve)
            {
                UnityEngine.Object obj2;
                AnimationUtility.GetObjectReferenceValue(rootGameObject, curveBinding, out obj2);
                return obj2;
            }
            AnimationUtility.GetFloatValue(rootGameObject, curveBinding, out num);
            return num;
        }

        public static CurveWrapper GetCurveWrapper(AnimationWindowCurve curve, AnimationClip clip)
        {
            CurveWrapper wrapper = new CurveWrapper {
                renderer = new NormalCurveRenderer(curve.ToAnimationCurve())
            };
            wrapper.renderer.SetWrap(WrapMode.Once, !clip.isLooping ? WrapMode.Once : WrapMode.Loop);
            wrapper.renderer.SetCustomRange(clip.startTime, clip.stopTime);
            wrapper.binding = curve.binding;
            wrapper.id = curve.GetHashCode();
            wrapper.color = CurveUtility.GetPropertyColor(curve.propertyName);
            wrapper.hidden = false;
            wrapper.selectionBindingInterface = curve.selectionBinding;
            return wrapper;
        }

        public static float GetNextKeyframeTime(AnimationWindowCurve[] curves, float currentTime, float frameRate)
        {
            float maxValue = float.MaxValue;
            float num2 = currentTime + (1f / frameRate);
            bool flag = false;
            foreach (AnimationWindowCurve curve in curves)
            {
                foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                {
                    float num4 = keyframe.time + curve.timeOffset;
                    if ((num4 < maxValue) && (num4 >= num2))
                    {
                        maxValue = num4;
                        flag = true;
                    }
                }
            }
            return (!flag ? currentTime : maxValue);
        }

        public static string GetNicePropertyDisplayName(System.Type animatableObjectType, string propertyName)
        {
            if (ShouldPrefixWithTypeName(animatableObjectType, propertyName))
            {
                return (ObjectNames.NicifyVariableName(animatableObjectType.Name) + "." + GetPropertyDisplayName(propertyName));
            }
            return GetPropertyDisplayName(propertyName);
        }

        public static string GetNicePropertyGroupDisplayName(System.Type animatableObjectType, string propertyGroupName)
        {
            if (ShouldPrefixWithTypeName(animatableObjectType, propertyGroupName))
            {
                return (ObjectNames.NicifyVariableName(animatableObjectType.Name) + "." + NicifyPropertyGroupName(animatableObjectType, propertyGroupName));
            }
            return NicifyPropertyGroupName(animatableObjectType, propertyGroupName);
        }

        public static float GetPreviousKeyframeTime(AnimationWindowCurve[] curves, float currentTime, float frameRate)
        {
            float minValue = float.MinValue;
            float num2 = Mathf.Max((float) 0f, (float) (currentTime - (1f / frameRate)));
            bool flag = false;
            foreach (AnimationWindowCurve curve in curves)
            {
                foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                {
                    float num4 = keyframe.time + curve.timeOffset;
                    if ((num4 > minValue) && (num4 <= num2))
                    {
                        minValue = num4;
                        flag = true;
                    }
                }
            }
            return (!flag ? currentTime : minValue);
        }

        public static string GetPropertyDisplayName(string propertyName)
        {
            propertyName = propertyName.Replace("m_LocalPosition", "Position");
            propertyName = propertyName.Replace("m_LocalScale", "Scale");
            propertyName = propertyName.Replace("m_LocalRotation", "Rotation");
            propertyName = propertyName.Replace("localEulerAnglesBaked", "Rotation");
            propertyName = propertyName.Replace("localEulerAnglesRaw", "Rotation");
            propertyName = propertyName.Replace("localEulerAngles", "Rotation");
            propertyName = propertyName.Replace("m_Materials.Array.data", "Material Reference");
            propertyName = ObjectNames.NicifyVariableName(propertyName);
            propertyName = propertyName.Replace("m_", "");
            return propertyName;
        }

        public static string GetPropertyGroupName(string propertyName)
        {
            if (GetComponentIndex(propertyName) != -1)
            {
                return propertyName.Substring(0, propertyName.Length - 2);
            }
            return propertyName;
        }

        public static int GetPropertyNodeID(int setId, string path, System.Type type, string propertyName) => 
            (setId.ToString() + path + type.Name + propertyName).GetHashCode();

        public static EditorCurveBinding GetRenamedBinding(EditorCurveBinding binding, string newPath) => 
            new EditorCurveBinding { 
                path = newPath,
                propertyName = binding.propertyName,
                type = binding.type
            };

        internal static bool HasOtherRotationCurve(AnimationClip clip, EditorCurveBinding rotationBinding)
        {
            if (rotationBinding.propertyName.StartsWith("m_LocalRotation"))
            {
                EditorCurveBinding binding = rotationBinding;
                EditorCurveBinding binding2 = rotationBinding;
                EditorCurveBinding binding3 = rotationBinding;
                binding.propertyName = "localEulerAnglesRaw.x";
                binding2.propertyName = "localEulerAnglesRaw.y";
                binding3.propertyName = "localEulerAnglesRaw.z";
                return (((AnimationUtility.GetEditorCurve(clip, binding) != null) || (AnimationUtility.GetEditorCurve(clip, binding2) != null)) || (AnimationUtility.GetEditorCurve(clip, binding3) != null));
            }
            EditorCurveBinding binding4 = rotationBinding;
            EditorCurveBinding binding5 = rotationBinding;
            EditorCurveBinding binding6 = rotationBinding;
            EditorCurveBinding binding7 = rotationBinding;
            binding4.propertyName = "m_LocalRotation.x";
            binding5.propertyName = "m_LocalRotation.y";
            binding6.propertyName = "m_LocalRotation.z";
            binding7.propertyName = "m_LocalRotation.w";
            return ((((AnimationUtility.GetEditorCurve(clip, binding4) != null) || (AnimationUtility.GetEditorCurve(clip, binding5) != null)) || (AnimationUtility.GetEditorCurve(clip, binding6) != null)) || (AnimationUtility.GetEditorCurve(clip, binding7) != null));
        }

        public static bool InitializeGameobjectForAnimation(GameObject animatedObject)
        {
            Component closestAnimationPlayerComponentInParents = GetClosestAnimationPlayerComponentInParents(animatedObject.transform);
            if (closestAnimationPlayerComponentInParents == null)
            {
                AnimationClip newClip = CreateNewClip(animatedObject.name);
                if (newClip == null)
                {
                    return false;
                }
                closestAnimationPlayerComponentInParents = EnsureActiveAnimationPlayer(animatedObject);
                bool flag2 = AddClipToAnimationPlayerComponent(closestAnimationPlayerComponentInParents, newClip);
                if (!flag2)
                {
                    UnityEngine.Object.DestroyImmediate(closestAnimationPlayerComponentInParents);
                }
                return flag2;
            }
            return EnsureAnimationPlayerHasClip(closestAnimationPlayerComponentInParents);
        }

        public static bool IsCurveCreated(AnimationClip clip, EditorCurveBinding binding)
        {
            if (binding.isPPtrCurve)
            {
                return (AnimationUtility.GetObjectReferenceCurve(clip, binding) != null);
            }
            if (IsRectTransformPosition(binding))
            {
                binding.propertyName = binding.propertyName.Replace(".x", ".z").Replace(".y", ".z");
            }
            if (IsRotationCurve(binding))
            {
                return ((AnimationUtility.GetEditorCurve(clip, binding) != null) || HasOtherRotationCurve(clip, binding));
            }
            return (AnimationUtility.GetEditorCurve(clip, binding) != null);
        }

        public static bool IsNodeAmbiguous(AnimationWindowHierarchyNode node)
        {
            if (node.binding.HasValue && (node.curves.Length > 0))
            {
                AnimationWindowSelectionItem selectionBinding = node.curves[0].selectionBinding;
                if ((selectionBinding != null) && (selectionBinding.rootGameObject != null))
                {
                    return AnimationUtility.AmbiguousBinding(node.binding.Value.path, node.binding.Value.m_ClassID, selectionBinding.rootGameObject.transform);
                }
            }
            if (node.hasChildren)
            {
                foreach (TreeViewItem item2 in node.children)
                {
                    return IsNodeAmbiguous(item2 as AnimationWindowHierarchyNode);
                }
            }
            return false;
        }

        public static bool IsNodeLeftOverCurve(AnimationWindowHierarchyNode node)
        {
            if (node.binding.HasValue && (node.curves.Length > 0))
            {
                AnimationWindowSelectionItem selectionBinding = node.curves[0].selectionBinding;
                if (selectionBinding != null)
                {
                    if ((selectionBinding.rootGameObject == null) && (selectionBinding.scriptableObject == null))
                    {
                        return false;
                    }
                    return (selectionBinding.GetEditorCurveValueType(node.binding.Value) == null);
                }
            }
            if (node.hasChildren)
            {
                foreach (TreeViewItem item2 in node.children)
                {
                    return IsNodeLeftOverCurve(item2 as AnimationWindowHierarchyNode);
                }
            }
            return false;
        }

        public static bool IsNodePhantom(AnimationWindowHierarchyNode node) => 
            (node.binding.HasValue && node.binding.Value.isPhantom);

        public static bool IsRectTransformPosition(EditorCurveBinding curveBinding) => 
            ((curveBinding.type == typeof(RectTransform)) && (GetPropertyGroupName(curveBinding.propertyName) == "m_LocalPosition"));

        internal static bool IsRotationCurve(EditorCurveBinding curveBinding)
        {
            string propertyGroupName = GetPropertyGroupName(curveBinding.propertyName);
            return ((propertyGroupName == "m_LocalRotation") || (propertyGroupName == "localEulerAnglesRaw"));
        }

        public static bool IsTransformType(System.Type type) => 
            ((type == typeof(Transform)) || (type == typeof(RectTransform)));

        public static string NicifyPropertyGroupName(System.Type animatableObjectType, string propertyGroupName)
        {
            string str = GetPropertyGroupName(GetPropertyDisplayName(propertyGroupName));
            if ((animatableObjectType == typeof(RectTransform)) & str.Equals("Position"))
            {
                str = "Position (Z)";
            }
            return str;
        }

        public static void RemoveKeyframes(AnimationWindowState state, AnimationWindowCurve[] curves, AnimationKeyTime time)
        {
            string undoLabel = "Remove Key";
            state.SaveKeySelection(undoLabel);
            foreach (AnimationWindowCurve curve in curves)
            {
                if (curve.animationIsEditable)
                {
                    AnimationKeyTime time2 = AnimationKeyTime.Time(time.time - curve.timeOffset, time.frameRate);
                    curve.RemoveKeyframe(time2);
                    state.SaveCurve(curve, undoLabel);
                }
            }
        }

        public static void RenameCurvePath(AnimationWindowCurve curve, EditorCurveBinding newBinding, AnimationClip clip)
        {
            if (curve.isPPtrCurve)
            {
                AnimationUtility.SetObjectReferenceCurve(clip, curve.binding, null);
                AnimationUtility.SetObjectReferenceCurve(clip, newBinding, curve.ToObjectCurve());
            }
            else
            {
                AnimationUtility.SetEditorCurve(clip, curve.binding, null);
                AnimationUtility.SetEditorCurve(clip, newBinding, curve.ToAnimationCurve());
            }
        }

        private static void SetClipAsLegacy(AnimationClip clip)
        {
            SerializedObject obj2 = new SerializedObject(clip);
            obj2.FindProperty("m_Legacy").boolValue = true;
            obj2.ApplyModifiedProperties();
        }

        public static bool ShouldPrefixWithTypeName(System.Type animatableObjectType, string propertyName)
        {
            if ((animatableObjectType == typeof(Transform)) || (animatableObjectType == typeof(RectTransform)))
            {
                return false;
            }
            if ((animatableObjectType == typeof(SpriteRenderer)) && (propertyName == "m_Sprite"))
            {
                return false;
            }
            return true;
        }

        public static bool ShouldShowAnimationWindowCurve(EditorCurveBinding curveBinding)
        {
            if (IsTransformType(curveBinding.type))
            {
                return !curveBinding.propertyName.EndsWith(".w");
            }
            return true;
        }

        public static void SyncTimeArea(TimeArea from, TimeArea to)
        {
            to.SetDrawRectHack(from.drawRect);
            to.m_Scale = new Vector2(from.m_Scale.x, to.m_Scale.y);
            to.m_Translation = new Vector2(from.m_Translation.x, to.m_Translation.y);
            to.EnforceScaleAndRange();
        }

        [CompilerGenerated]
        private sealed class <BestMatchForPaste>c__AnonStorey0
        {
            internal AnimationWindowCurve targetCurve;

            internal bool <>m__0(AnimationWindowCurve clipboardCurve) => 
                (clipboardCurve.binding == this.targetCurve.binding);
        }
    }
}

