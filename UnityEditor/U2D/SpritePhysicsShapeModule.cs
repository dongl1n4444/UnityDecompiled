namespace UnityEditor.U2D
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.U2D.Interface;
    using UnityEngine;
    using UnityEngine.U2D.Interface;

    internal class SpritePhysicsShapeModule : SpriteOutlineModule
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ISpriteEditor <spriteEditorWindow>k__BackingField;
        private readonly byte kDefaultPhysicsAlphaTolerance;
        private readonly float kDefaultPhysicsTessellationDetail;

        public SpritePhysicsShapeModule(ISpriteEditor sem, IEventSystem ege, IUndoSystem us, IAssetDatabase ad, IGUIUtility gu, IShapeEditorFactory sef, ITexture2D outlineTexture) : base(sem, ege, us, ad, gu, sef, outlineTexture)
        {
            this.kDefaultPhysicsTessellationDetail = 0.25f;
            this.kDefaultPhysicsAlphaTolerance = 200;
            this.spriteEditorWindow = sem;
        }

        protected override bool HasShapeOutline(SpriteRect spriteRect) => 
            ((spriteRect.physicsShape != null) && (spriteRect.physicsShape.Count > 0));

        protected override void SetupShapeEditorOutline(SpriteRect spriteRect)
        {
            if ((spriteRect.physicsShape == null) || (spriteRect.physicsShape.Count == 0))
            {
                spriteRect.physicsShape = SpriteOutlineModule.GenerateSpriteRectOutline(spriteRect.rect, this.spriteEditorWindow.selectedTexture, (Math.Abs((float) (spriteRect.tessellationDetail - -1f)) >= Mathf.Epsilon) ? spriteRect.tessellationDetail : this.kDefaultPhysicsTessellationDetail, this.kDefaultPhysicsAlphaTolerance);
                this.spriteEditorWindow.SetDataModified();
            }
        }

        public override string moduleName =>
            "Edit Physics Shape";

        protected override List<SpriteOutline> selectedShapeOutline
        {
            get => 
                base.m_Selected.physicsShape;
            set
            {
                base.m_Selected.physicsShape = value;
            }
        }

        private ISpriteEditor spriteEditorWindow { get; set; }
    }
}

