namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Animations;
    using UnityEngine;

    /// <summary>
    /// <para>Model importer lets you modify import settings from editor scripts.</para>
    /// </summary>
    public class ModelImporter : AssetImporter
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string CalculateBestFittingPreviewGameObject();
        /// <summary>
        /// <para>Creates a mask that matches the model hierarchy, and applies it to the provided ModelImporterClipAnimation.</para>
        /// </summary>
        /// <param name="clip">Clip to which the mask will be applied.</param>
        public void CreateDefaultMaskForClip(ModelImporterClipAnimation clip)
        {
            if (this.defaultClipAnimations.Length > 0)
            {
                AvatarMask mask = new AvatarMask();
                this.defaultClipAnimations[0].ConfigureMaskFromClip(ref mask);
                clip.ConfigureClipFromMask(mask);
                Object.DestroyImmediate(mask);
            }
            else
            {
                Debug.LogError("Cannot create default mask because the current importer doesn't have any animation information");
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern AnimationClip GetPreviewAnimationClipForTake(string takeName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_humanDescription(out HumanDescription value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_humanDescription(ref HumanDescription value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void UpdateSkeletonPose(SkeletonBone[] skeletonBones, SerializedProperty serializedProperty);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void UpdateTransformMask(AvatarMask mask, SerializedProperty serializedProperty);

        /// <summary>
        /// <para>Add to imported meshes.</para>
        /// </summary>
        public bool addCollider { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Animation compression setting.</para>
        /// </summary>
        public ModelImporterAnimationCompression animationCompression { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Allowed error of animation position compression.</para>
        /// </summary>
        public float animationPositionError { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Allowed error of animation rotation compression.</para>
        /// </summary>
        public float animationRotationError { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Allowed error of animation scale compression.</para>
        /// </summary>
        public float animationScaleError { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Animator generation mode.</para>
        /// </summary>
        public ModelImporterAnimationType animationType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The default wrap mode for the generated animation clips.</para>
        /// </summary>
        public WrapMode animationWrapMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Bake Inverse Kinematics (IK) when importing.</para>
        /// </summary>
        public bool bakeIK { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Animation clips to split animation into. See Also: ModelImporterClipAnimation.</para>
        /// </summary>
        public ModelImporterClipAnimation[] clipAnimations { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Generate a list of all default animation clip based on TakeInfo.</para>
        /// </summary>
        public ModelImporterClipAnimation[] defaultClipAnimations { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Animation optimization setting.</para>
        /// </summary>
        public string[] extraExposedTransformPaths { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>File scale factor (if available) or default one. (Read-only).</para>
        /// </summary>
        public float fileScale { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Animation generation options.</para>
        /// </summary>
        public ModelImporterGenerateAnimations generateAnimations { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Material generation options.</para>
        /// </summary>
        [Obsolete("Use importMaterials, materialName and materialSearch instead")]
        public ModelImporterGenerateMaterials generateMaterials { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Generate secondary UV set for lightmapping.</para>
        /// </summary>
        public bool generateSecondaryUV { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Global scale factor for importing.</para>
        /// </summary>
        public float globalScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The human description that is used to generate an Avatar during the import process.</para>
        /// </summary>
        public HumanDescription humanDescription
        {
            get
            {
                HumanDescription description;
                this.INTERNAL_get_humanDescription(out description);
                return description;
            }
            set
            {
                this.INTERNAL_set_humanDescription(ref value);
            }
        }

        /// <summary>
        /// <para>Controls how much oversampling is used when importing humanoid animations for retargeting.</para>
        /// </summary>
        public ModelImporterHumanoidOversampling humanoidOversampling { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Import animation from file.</para>
        /// </summary>
        public bool importAnimation { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Controls import of BlendShapes.</para>
        /// </summary>
        public bool importBlendShapes { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Generates the list of all imported take.</para>
        /// </summary>
        public TakeInfo[] importedTakeInfos { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Import materials from file.</para>
        /// </summary>
        public bool importMaterials { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Vertex normal import options.</para>
        /// </summary>
        public ModelImporterNormals importNormals { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Vertex tangent import options.</para>
        /// </summary>
        public ModelImporterTangents importTangents { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal bool isAssetOlderOr42 { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is Bake Inverse Kinematics (IK) supported by this importer.</para>
        /// </summary>
        public bool isBakeIKSupported { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is FileScale was used when importing.</para>
        /// </summary>
        public bool isFileScaleUsed { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Are mesh vertices and indices accessible from script?</para>
        /// </summary>
        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Is import of tangents supported by this importer.</para>
        /// </summary>
        public bool isTangentImportSupported { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Is useFileUnits supported for this asset.</para>
        /// </summary>
        public bool isUseFileUnitsSupported { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Material naming setting.</para>
        /// </summary>
        public ModelImporterMaterialName materialName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Existing material search setting.</para>
        /// </summary>
        public ModelImporterMaterialSearch materialSearch { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Mesh compression setting.</para>
        /// </summary>
        public ModelImporterMeshCompression meshCompression { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The path of the transform used to generation the motion of the animation.</para>
        /// </summary>
        public string motionNodeName { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Normals import mode.</para>
        /// </summary>
        [Obsolete("normalImportMode is deprecated. Use importNormals instead")]
        public ModelImporterTangentSpaceMode normalImportMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Smoothing angle (in degrees) for calculating normals.</para>
        /// </summary>
        public float normalSmoothingAngle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Animation optimization setting.</para>
        /// </summary>
        public bool optimizeGameObjects { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Vertex optimization setting.</para>
        /// </summary>
        public bool optimizeMesh { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("Use animationCompression instead", true)]
        private bool reduceKeyframes
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Generates the list of all imported Animations.</para>
        /// </summary>
        public string[] referencedClips { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>If set to false, the importer will not resample curves when possible.
        /// Read more about.
        /// 
        /// Notes:
        /// 
        /// - Some unsupported FBX features (such as PreRotation or PostRotation on transforms) will override this setting. In these situations, animation curves will still be resampled even if the setting is disabled. For best results, avoid using PreRotation, PostRotation and GetRotationPivot.
        /// 
        /// - This option was introduced in Version 5.3. Prior to this version, Unity's import behaviour was as if this option was always enabled. Therefore enabling the option gives the same behaviour as pre-5.3 animation import.
        /// </para>
        /// </summary>
        public bool resampleCurves { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("use resampleCurves instead.")]
        public bool resampleRotations { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Threshold for angle distortion (in degrees) when generating secondary UV.</para>
        /// </summary>
        public float secondaryUVAngleDistortion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Threshold for area distortion when generating secondary UV.</para>
        /// </summary>
        public float secondaryUVAreaDistortion { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Hard angle (in degrees) for generating secondary UV.</para>
        /// </summary>
        public float secondaryUVHardAngle { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Margin to be left between charts when packing secondary UV.</para>
        /// </summary>
        public float secondaryUVPackMargin { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Imports the HumanDescription from the given Avatar.</para>
        /// </summary>
        public Avatar sourceAvatar
        {
            get
            {
                return this.sourceAvatarInternal;
            }
            set
            {
                Avatar avatar = value;
                if (value != null)
                {
                    ModelImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(value)) as ModelImporter;
                    if (atPath != null)
                    {
                        this.humanDescription = atPath.humanDescription;
                    }
                    else
                    {
                        Debug.LogError("Avatar must be from a ModelImporter, otherwise use ModelImporter.humanDescription");
                        avatar = null;
                    }
                }
                this.sourceAvatarInternal = avatar;
            }
        }

        internal Avatar sourceAvatarInternal { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("splitAnimations has been deprecated please use clipAnimations instead.", true)]
        public bool splitAnimations
        {
            get
            {
                return (this.clipAnimations.Length != 0);
            }
            set
            {
            }
        }

        /// <summary>
        /// <para>Should tangents be split across UV seams.</para>
        /// </summary>
        [Obsolete("Please use tangentImportMode instead")]
        public bool splitTangentsAcrossSeams
        {
            get
            {
                return (this.importTangents == ModelImporterTangents.CalculateLegacyWithSplitTangents);
            }
            set
            {
                if ((this.importTangents == ModelImporterTangents.CalculateLegacyWithSplitTangents) && !value)
                {
                    this.importTangents = ModelImporterTangents.CalculateLegacy;
                }
                else if ((this.importTangents == ModelImporterTangents.CalculateLegacy) && value)
                {
                    this.importTangents = ModelImporterTangents.CalculateLegacyWithSplitTangents;
                }
            }
        }

        /// <summary>
        /// <para>Swap primary and secondary UV channels when importing.</para>
        /// </summary>
        public bool swapUVChannels { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Tangents import mode.</para>
        /// </summary>
        [Obsolete("tangentImportMode is deprecated. Use importTangents instead")]
        public ModelImporterTangentSpaceMode tangentImportMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Generates the list of all imported Transforms.</para>
        /// </summary>
        public string[] transformPaths { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Detect file units and import as 1FileUnit=1UnityUnit, otherwise it will import as 1cm=1UnityUnit.</para>
        /// </summary>
        public bool useFileUnits { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

