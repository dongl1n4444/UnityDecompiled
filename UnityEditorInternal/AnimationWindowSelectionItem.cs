namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    internal class AnimationWindowSelectionItem : ScriptableObject, IEquatable<AnimationWindowSelectionItem>, ISelectionBinding
    {
        [SerializeField]
        private AnimationClip m_AnimationClip;
        private List<AnimationWindowCurve> m_CurvesCache = null;
        [SerializeField]
        private GameObject m_GameObject;
        [SerializeField]
        private int m_Id;
        [SerializeField]
        private ScriptableObject m_ScriptableObject;
        [SerializeField]
        private float m_TimeOffset;

        private void AssignBindingToRightSlot(EditorCurveBinding transformBinding, ref EditorCurveBinding?[] propertyGroup)
        {
            if (transformBinding.propertyName.EndsWith(".x"))
            {
                propertyGroup[0] = new EditorCurveBinding?(transformBinding);
            }
            else if (transformBinding.propertyName.EndsWith(".y"))
            {
                propertyGroup[1] = new EditorCurveBinding?(transformBinding);
            }
            else if (transformBinding.propertyName.EndsWith(".z"))
            {
                propertyGroup[2] = new EditorCurveBinding?(transformBinding);
            }
        }

        public void ClearCache()
        {
            this.m_CurvesCache = null;
        }

        public static AnimationWindowSelectionItem Create()
        {
            AnimationWindowSelectionItem item = ScriptableObject.CreateInstance(typeof(AnimationWindowSelectionItem)) as AnimationWindowSelectionItem;
            item.hideFlags = HideFlags.HideAndDontSave;
            return item;
        }

        public bool Equals(AnimationWindowSelectionItem other) => 
            ((((this.id == other.id) && (this.animationClip == other.animationClip)) && (this.gameObject == other.gameObject)) && (this.scriptableObject == other.scriptableObject));

        private void FillInMissingTransformCurves(List<AnimationWindowCurve> transformCurves, ref List<AnimationWindowCurve> curvesCache)
        {
            EditorCurveBinding lastBinding = transformCurves[0].binding;
            EditorCurveBinding?[] propertyGroup = new EditorCurveBinding?[3];
            foreach (AnimationWindowCurve curve in transformCurves)
            {
                EditorCurveBinding binding = curve.binding;
                if ((binding.path != lastBinding.path) || (AnimationWindowUtility.GetPropertyGroupName(binding.propertyName) != AnimationWindowUtility.GetPropertyGroupName(lastBinding.propertyName)))
                {
                    string propertyGroupName = AnimationWindowUtility.GetPropertyGroupName(lastBinding.propertyName);
                    this.FillPropertyGroup(ref propertyGroup, lastBinding, propertyGroupName, ref curvesCache);
                    lastBinding = binding;
                    propertyGroup = new EditorCurveBinding?[3];
                }
                this.AssignBindingToRightSlot(binding, ref propertyGroup);
            }
            this.FillPropertyGroup(ref propertyGroup, lastBinding, AnimationWindowUtility.GetPropertyGroupName(lastBinding.propertyName), ref curvesCache);
        }

        private void FillPropertyGroup(ref EditorCurveBinding?[] propertyGroup, EditorCurveBinding lastBinding, string propertyGroupName, ref List<AnimationWindowCurve> curvesCache)
        {
            EditorCurveBinding binding = lastBinding;
            binding.isPhantom = true;
            if (!propertyGroup[0].HasValue)
            {
                binding.propertyName = propertyGroupName + ".x";
                AnimationWindowCurve item = new AnimationWindowCurve(this.animationClip, binding, this.GetEditorCurveValueType(binding)) {
                    selectionBinding = this
                };
                curvesCache.Add(item);
            }
            if (!propertyGroup[1].HasValue)
            {
                binding.propertyName = propertyGroupName + ".y";
                AnimationWindowCurve curve2 = new AnimationWindowCurve(this.animationClip, binding, this.GetEditorCurveValueType(binding)) {
                    selectionBinding = this
                };
                curvesCache.Add(curve2);
            }
            if (!propertyGroup[2].HasValue)
            {
                binding.propertyName = propertyGroupName + ".z";
                AnimationWindowCurve curve3 = new AnimationWindowCurve(this.animationClip, binding, this.GetEditorCurveValueType(binding)) {
                    selectionBinding = this
                };
                curvesCache.Add(curve3);
            }
        }

        public System.Type GetEditorCurveValueType(EditorCurveBinding curveBinding)
        {
            if (this.rootGameObject != null)
            {
                return AnimationUtility.GetEditorCurveValueType(this.rootGameObject, curveBinding);
            }
            if (this.scriptableObject != null)
            {
                return AnimationUtility.GetScriptableObjectEditorCurveValueType(this.scriptableObject, curveBinding);
            }
            if (curveBinding.isPPtrCurve)
            {
                return null;
            }
            return typeof(float);
        }

        public int GetRefreshHash() => 
            ((((this.id * 0x4c93) ^ ((this.animationClip == null) ? 0 : (0x2d9 * this.animationClip.GetHashCode()))) ^ ((this.rootGameObject == null) ? 0 : (0x1b * this.rootGameObject.GetHashCode()))) ^ ((this.scriptableObject == null) ? 0 : this.scriptableObject.GetHashCode()));

        public virtual void Synchronize()
        {
        }

        public virtual AnimationClip animationClip
        {
            get => 
                this.m_AnimationClip;
            set
            {
                this.m_AnimationClip = value;
            }
        }

        public virtual bool animationIsEditable
        {
            get
            {
                if ((this.animationClip != null) && ((this.animationClip.hideFlags & HideFlags.NotEditable) != HideFlags.None))
                {
                    return false;
                }
                if (this.objectIsPrefab)
                {
                    return false;
                }
                return true;
            }
        }

        public virtual Component animationPlayer
        {
            get
            {
                if (this.gameObject != null)
                {
                    return AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(this.gameObject.transform);
                }
                return null;
            }
        }

        public virtual bool canAddCurves
        {
            get
            {
                if (this.gameObject != null)
                {
                    return (!this.objectIsPrefab && this.clipIsEditable);
                }
                return (this.scriptableObject != null);
            }
        }

        public virtual bool canChangeAnimationClip =>
            (this.rootGameObject != null);

        public virtual bool canRecord =>
            ((this.rootGameObject != null) && !this.objectIsOptimized);

        public virtual bool canSyncSceneSelection =>
            true;

        public virtual bool clipIsEditable
        {
            get
            {
                if (this.animationClip == null)
                {
                    return false;
                }
                if ((this.animationClip.hideFlags & HideFlags.NotEditable) != HideFlags.None)
                {
                    return false;
                }
                if (!AssetDatabase.IsOpenForEdit(this.animationClip, StatusQueryOptions.UseCachedIfPossible))
                {
                    return false;
                }
                return true;
            }
        }

        public List<AnimationWindowCurve> curves
        {
            get
            {
                if (this.m_CurvesCache == null)
                {
                    this.m_CurvesCache = new List<AnimationWindowCurve>();
                    if (this.animationClip != null)
                    {
                        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(this.animationClip);
                        EditorCurveBinding[] objectReferenceCurveBindings = AnimationUtility.GetObjectReferenceCurveBindings(this.animationClip);
                        List<AnimationWindowCurve> transformCurves = new List<AnimationWindowCurve>();
                        foreach (EditorCurveBinding binding in curveBindings)
                        {
                            if (AnimationWindowUtility.ShouldShowAnimationWindowCurve(binding))
                            {
                                AnimationWindowCurve item = new AnimationWindowCurve(this.animationClip, binding, this.GetEditorCurveValueType(binding)) {
                                    selectionBinding = this
                                };
                                this.m_CurvesCache.Add(item);
                                if (AnimationWindowUtility.IsTransformType(binding.type))
                                {
                                    transformCurves.Add(item);
                                }
                            }
                        }
                        foreach (EditorCurveBinding binding2 in objectReferenceCurveBindings)
                        {
                            AnimationWindowCurve curve2 = new AnimationWindowCurve(this.animationClip, binding2, this.GetEditorCurveValueType(binding2)) {
                                selectionBinding = this
                            };
                            this.m_CurvesCache.Add(curve2);
                        }
                        transformCurves.Sort();
                        if (transformCurves.Count > 0)
                        {
                            this.FillInMissingTransformCurves(transformCurves, ref this.m_CurvesCache);
                        }
                    }
                    this.m_CurvesCache.Sort();
                }
                return this.m_CurvesCache;
            }
        }

        public virtual GameObject gameObject
        {
            get => 
                this.m_GameObject;
            set
            {
                this.m_GameObject = value;
            }
        }

        public virtual int id
        {
            get => 
                this.m_Id;
            set
            {
                this.m_Id = value;
            }
        }

        public virtual bool objectIsOptimized
        {
            get
            {
                Animator animationPlayer = this.animationPlayer as Animator;
                return (animationPlayer?.isOptimizable && !animationPlayer.hasTransformHierarchy);
            }
        }

        public virtual bool objectIsPrefab
        {
            get
            {
                if (this.gameObject == null)
                {
                    return false;
                }
                return (EditorUtility.IsPersistent(this.gameObject) || ((this.gameObject.hideFlags & HideFlags.NotEditable) != HideFlags.None));
            }
        }

        public virtual GameObject rootGameObject
        {
            get
            {
                Component animationPlayer = this.animationPlayer;
                if (animationPlayer != null)
                {
                    return animationPlayer.gameObject;
                }
                return null;
            }
        }

        public virtual ScriptableObject scriptableObject
        {
            get => 
                this.m_ScriptableObject;
            set
            {
                this.m_ScriptableObject = value;
            }
        }

        public virtual UnityEngine.Object sourceObject =>
            ((this.gameObject == null) ? ((UnityEngine.Object) this.scriptableObject) : ((UnityEngine.Object) this.gameObject));

        public virtual float timeOffset
        {
            get => 
                this.m_TimeOffset;
            set
            {
                this.m_TimeOffset = value;
            }
        }
    }
}

