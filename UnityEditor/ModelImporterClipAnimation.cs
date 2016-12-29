namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor.Animations;
    using UnityEngine;

    /// <summary>
    /// <para>Animation clips to split animation into.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public sealed class ModelImporterClipAnimation
    {
        private string m_TakeName;
        private string m_Name;
        private float m_FirstFrame;
        private float m_LastFrame;
        private int m_WrapMode;
        private int m_Loop;
        private float m_OrientationOffsetY;
        private float m_Level;
        private float m_CycleOffset;
        private float m_AdditiveReferencePoseFrame;
        private int m_HasAdditiveReferencePose;
        private int m_LoopTime;
        private int m_LoopBlend;
        private int m_LoopBlendOrientation;
        private int m_LoopBlendPositionY;
        private int m_LoopBlendPositionXZ;
        private int m_KeepOriginalOrientation;
        private int m_KeepOriginalPositionY;
        private int m_KeepOriginalPositionXZ;
        private int m_HeightFromFeet;
        private int m_Mirror;
        private int m_MaskType = 3;
        private AvatarMask m_MaskSource;
        private int[] m_BodyMask;
        private AnimationEvent[] m_AnimationEvents;
        private ClipAnimationInfoCurve[] m_AdditionnalCurves;
        private TransformMaskElement[] m_TransformMask;
        private bool m_MaskNeedsUpdating;
        /// <summary>
        /// <para>Take name.</para>
        /// </summary>
        public string takeName
        {
            get => 
                this.m_TakeName;
            set
            {
                this.m_TakeName = value;
            }
        }
        /// <summary>
        /// <para>Clip name.</para>
        /// </summary>
        public string name
        {
            get => 
                this.m_Name;
            set
            {
                this.m_Name = value;
            }
        }
        /// <summary>
        /// <para>First frame of the clip.</para>
        /// </summary>
        public float firstFrame
        {
            get => 
                this.m_FirstFrame;
            set
            {
                this.m_FirstFrame = value;
            }
        }
        /// <summary>
        /// <para>Last frame of the clip.</para>
        /// </summary>
        public float lastFrame
        {
            get => 
                this.m_LastFrame;
            set
            {
                this.m_LastFrame = value;
            }
        }
        /// <summary>
        /// <para>The wrap mode of the animation.</para>
        /// </summary>
        public WrapMode wrapMode
        {
            get => 
                ((WrapMode) this.m_WrapMode);
            set
            {
                this.m_WrapMode = (int) value;
            }
        }
        /// <summary>
        /// <para>Is the clip a looping animation?</para>
        /// </summary>
        public bool loop
        {
            get => 
                (this.m_Loop != 0);
            set
            {
                this.m_Loop = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Offset in degrees to the root rotation.</para>
        /// </summary>
        public float rotationOffset
        {
            get => 
                this.m_OrientationOffsetY;
            set
            {
                this.m_OrientationOffsetY = value;
            }
        }
        /// <summary>
        /// <para>Offset to the vertical root position.</para>
        /// </summary>
        public float heightOffset
        {
            get => 
                this.m_Level;
            set
            {
                this.m_Level = value;
            }
        }
        /// <summary>
        /// <para>Offset to the cycle of a looping animation, if a different time in it is desired to be the start.</para>
        /// </summary>
        public float cycleOffset
        {
            get => 
                this.m_CycleOffset;
            set
            {
                this.m_CycleOffset = value;
            }
        }
        /// <summary>
        /// <para>Enable to make the clip loop.</para>
        /// </summary>
        public bool loopTime
        {
            get => 
                (this.m_LoopTime != 0);
            set
            {
                this.m_LoopTime = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Enable to make the motion loop seamlessly.</para>
        /// </summary>
        public bool loopPose
        {
            get => 
                (this.m_LoopBlend != 0);
            set
            {
                this.m_LoopBlend = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Enable to make root rotation be baked into the movement of the bones. Disable to make root rotation be stored as root motion.</para>
        /// </summary>
        public bool lockRootRotation
        {
            get => 
                (this.m_LoopBlendOrientation != 0);
            set
            {
                this.m_LoopBlendOrientation = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Enable to make vertical root motion be baked into the movement of the bones. Disable to make vertical root motion be stored as root motion.</para>
        /// </summary>
        public bool lockRootHeightY
        {
            get => 
                (this.m_LoopBlendPositionY != 0);
            set
            {
                this.m_LoopBlendPositionY = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Enable to make horizontal root motion be baked into the movement of the bones. Disable to make horizontal root motion be stored as root motion.</para>
        /// </summary>
        public bool lockRootPositionXZ
        {
            get => 
                (this.m_LoopBlendPositionXZ != 0);
            set
            {
                this.m_LoopBlendPositionXZ = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Keeps the vertical position as it is authored in the source file.</para>
        /// </summary>
        public bool keepOriginalOrientation
        {
            get => 
                (this.m_KeepOriginalOrientation != 0);
            set
            {
                this.m_KeepOriginalOrientation = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Keeps the vertical position as it is authored in the source file.</para>
        /// </summary>
        public bool keepOriginalPositionY
        {
            get => 
                (this.m_KeepOriginalPositionY != 0);
            set
            {
                this.m_KeepOriginalPositionY = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Keeps the vertical position as it is authored in the source file.</para>
        /// </summary>
        public bool keepOriginalPositionXZ
        {
            get => 
                (this.m_KeepOriginalPositionXZ != 0);
            set
            {
                this.m_KeepOriginalPositionXZ = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Keeps the feet aligned with the root transform position.</para>
        /// </summary>
        public bool heightFromFeet
        {
            get => 
                (this.m_HeightFromFeet != 0);
            set
            {
                this.m_HeightFromFeet = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Mirror left and right in this clip.</para>
        /// </summary>
        public bool mirror
        {
            get => 
                (this.m_Mirror != 0);
            set
            {
                this.m_Mirror = !value ? 0 : 1;
            }
        }
        /// <summary>
        /// <para>Define mask type.</para>
        /// </summary>
        public ClipAnimationMaskType maskType
        {
            get => 
                ((ClipAnimationMaskType) this.m_MaskType);
            set
            {
                this.m_MaskType = (int) value;
            }
        }
        /// <summary>
        /// <para>The AvatarMask used to mask transforms during the import process.</para>
        /// </summary>
        public AvatarMask maskSource
        {
            get => 
                this.m_MaskSource;
            set
            {
                this.m_MaskSource = value;
            }
        }
        /// <summary>
        /// <para>AnimationEvents that will be added during the import process.</para>
        /// </summary>
        public AnimationEvent[] events
        {
            get => 
                this.m_AnimationEvents;
            set
            {
                this.m_AnimationEvents = value;
            }
        }
        /// <summary>
        /// <para>Additionnal curves that will be that will be added during the import process.</para>
        /// </summary>
        public ClipAnimationInfoCurve[] curves
        {
            get => 
                this.m_AdditionnalCurves;
            set
            {
                this.m_AdditionnalCurves = value;
            }
        }
        /// <summary>
        /// <para>Returns true when the source AvatarMask has changed. This only happens when  ModelImporterClipAnimation.maskType is set to ClipAnimationMaskType.CopyFromOther
        /// To force a reload of the mask, simply set  ModelImporterClipAnimation.maskSource to the desired AvatarMask.</para>
        /// </summary>
        public bool maskNeedsUpdating =>
            this.m_MaskNeedsUpdating;
        /// <summary>
        /// <para>The additive reference pose frame.</para>
        /// </summary>
        public float additiveReferencePoseFrame
        {
            get => 
                this.m_AdditiveReferencePoseFrame;
            set
            {
                this.m_AdditiveReferencePoseFrame = value;
            }
        }
        /// <summary>
        /// <para>Enable to defines an additive reference pose.</para>
        /// </summary>
        public bool hasAdditiveReferencePose
        {
            get => 
                (this.m_HasAdditiveReferencePose != 0);
            set
            {
                this.m_HasAdditiveReferencePose = !value ? 0 : 1;
            }
        }
        public void ConfigureMaskFromClip(ref AvatarMask mask)
        {
            mask.transformCount = this.m_TransformMask.Length;
            for (int i = 0; i < mask.transformCount; i++)
            {
                mask.SetTransformPath(i, this.m_TransformMask[i].path);
                mask.SetTransformActive(i, this.m_TransformMask[i].weight > 0f);
            }
            for (int j = 0; j < this.m_BodyMask.Length; j++)
            {
                mask.SetHumanoidBodyPartActive((AvatarMaskBodyPart) j, this.m_BodyMask[j] != 0);
            }
        }

        /// <summary>
        /// <para>Copy the mask settings from an AvatarMask to the clip configuration.</para>
        /// </summary>
        /// <param name="mask">AvatarMask from which the mask settings will be imported.</param>
        public void ConfigureClipFromMask(AvatarMask mask)
        {
            this.m_TransformMask = new TransformMaskElement[mask.transformCount];
            for (int i = 0; i < mask.transformCount; i++)
            {
                this.m_TransformMask[i].path = mask.GetTransformPath(i);
                this.m_TransformMask[i].weight = !mask.GetTransformActive(i) ? 0f : 1f;
            }
            this.m_BodyMask = new int[13];
            for (int j = 0; j < 13; j++)
            {
                this.m_BodyMask[j] = !mask.GetHumanoidBodyPartActive((AvatarMaskBodyPart) j) ? 0 : 1;
            }
        }

        public override bool Equals(object o)
        {
            ModelImporterClipAnimation animation = o as ModelImporterClipAnimation;
            return ((((((animation != null) && (this.takeName == animation.takeName)) && ((this.name == animation.name) && (this.firstFrame == animation.firstFrame))) && (((this.lastFrame == animation.lastFrame) && (this.m_WrapMode == animation.m_WrapMode)) && ((this.m_Loop == animation.m_Loop) && (this.loopPose == animation.loopPose)))) && ((((this.lockRootRotation == animation.lockRootRotation) && (this.lockRootHeightY == animation.lockRootHeightY)) && ((this.lockRootPositionXZ == animation.lockRootPositionXZ) && (this.mirror == animation.mirror))) && (((this.maskType == animation.maskType) && (this.maskSource == animation.maskSource)) && (this.additiveReferencePoseFrame == animation.additiveReferencePoseFrame)))) && (this.hasAdditiveReferencePose == animation.hasAdditiveReferencePose));
        }

        public override int GetHashCode() => 
            this.name.GetHashCode();
    }
}

