namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class AnimationPropertyContextualMenu
    {
        [CompilerGenerated]
        private static Predicate<IAnimationContextualResponder> <>f__am$cache0;
        [CompilerGenerated]
        private static Predicate<IAnimationContextualResponder> <>f__am$cache1;
        private static GUIContent addAnimatedContent = EditorGUIUtility.TextContent("Key All Animated");
        private static GUIContent addCandidatesContent = EditorGUIUtility.TextContent("Key All Modified");
        private static GUIContent addKeyContent = EditorGUIUtility.TextContent("Add Key");
        private static GUIContent goToNextKeyContent = EditorGUIUtility.TextContent("Go to Next Key");
        private static GUIContent goToPreviousKeyContent = EditorGUIUtility.TextContent("Go to Previous Key");
        public static AnimationPropertyContextualMenu Instance = new AnimationPropertyContextualMenu();
        private List<IAnimationContextualResponder> m_Responders = new List<IAnimationContextualResponder>();
        private static GUIContent removeCurveContent = EditorGUIUtility.TextContent("Remove All Keys");
        private static GUIContent removeKeyContent = EditorGUIUtility.TextContent("Remove Key");
        private static GUIContent updateKeyContent = EditorGUIUtility.TextContent("Update Key");

        public AnimationPropertyContextualMenu()
        {
            EditorApplication.contextualPropertyMenu = (EditorApplication.SerializedPropertyCallbackFunction) Delegate.Combine(EditorApplication.contextualPropertyMenu, new EditorApplication.SerializedPropertyCallbackFunction(this.OnPropertyContextMenu));
        }

        public void AddResponder(IAnimationContextualResponder responder)
        {
            this.m_Responders.Add(responder);
        }

        private void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            <OnPropertyContextMenu>c__AnonStorey0 storey = new <OnPropertyContextMenu>c__AnonStorey0 {
                property = property,
                $this = this
            };
            if ((this.m_Responders.Count != 0) && this.m_Responders.Exists(new Predicate<IAnimationContextualResponder>(storey.<>m__0)))
            {
                <OnPropertyContextMenu>c__AnonStorey1 storey2 = new <OnPropertyContextMenu>c__AnonStorey1 {
                    <>f__ref$0 = storey
                };
                bool flag2 = this.m_Responders.Exists(new Predicate<IAnimationContextualResponder>(storey2.<>m__0));
                bool flag3 = flag2 && this.m_Responders.Exists(new Predicate<IAnimationContextualResponder>(storey2.<>m__1));
                bool flag4 = flag2 && this.m_Responders.Exists(new Predicate<IAnimationContextualResponder>(storey2.<>m__2));
                bool flag5 = flag2 && (flag3 || this.m_Responders.Exists(new Predicate<IAnimationContextualResponder>(storey2.<>m__3)));
                if (!flag2)
                {
                }
                bool flag6 = (<>f__am$cache0 == null) && this.m_Responders.Exists(<>f__am$cache0);
                if (!flag2)
                {
                }
                bool flag7 = (<>f__am$cache1 == null) && this.m_Responders.Exists(<>f__am$cache1);
                storey2.propertyCopy = storey.property.Copy();
                if (flag2)
                {
                    menu.AddItem((!flag3 || !flag4) ? addKeyContent : updateKeyContent, false, new GenericMenu.MenuFunction(storey2.<>m__4));
                }
                else
                {
                    menu.AddDisabledItem(addKeyContent);
                }
                if (flag3)
                {
                    menu.AddItem(removeKeyContent, false, new GenericMenu.MenuFunction(storey2.<>m__5));
                }
                else
                {
                    menu.AddDisabledItem(removeKeyContent);
                }
                if (flag5)
                {
                    menu.AddItem(removeCurveContent, false, new GenericMenu.MenuFunction(storey2.<>m__6));
                }
                else
                {
                    menu.AddDisabledItem(removeCurveContent);
                }
                menu.AddSeparator(string.Empty);
                if (flag6)
                {
                    menu.AddItem(addCandidatesContent, false, new GenericMenu.MenuFunction(storey2.<>m__7));
                }
                else
                {
                    menu.AddDisabledItem(addCandidatesContent);
                }
                if (flag7)
                {
                    menu.AddItem(addAnimatedContent, false, new GenericMenu.MenuFunction(storey2.<>m__8));
                }
                else
                {
                    menu.AddDisabledItem(addAnimatedContent);
                }
                menu.AddSeparator(string.Empty);
                if (flag5)
                {
                    menu.AddItem(goToPreviousKeyContent, false, new GenericMenu.MenuFunction(storey2.<>m__9));
                    menu.AddItem(goToNextKeyContent, false, new GenericMenu.MenuFunction(storey2.<>m__A));
                }
                else
                {
                    menu.AddDisabledItem(goToPreviousKeyContent);
                    menu.AddDisabledItem(goToNextKeyContent);
                }
            }
        }

        public void RemoveResponder(IAnimationContextualResponder responder)
        {
            this.m_Responders.Remove(responder);
        }

        [CompilerGenerated]
        private sealed class <OnPropertyContextMenu>c__AnonStorey0
        {
            internal AnimationPropertyContextualMenu $this;
            internal SerializedProperty property;

            internal bool <>m__0(IAnimationContextualResponder responder) => 
                responder.IsAnimatable(this.property);
        }

        [CompilerGenerated]
        private sealed class <OnPropertyContextMenu>c__AnonStorey1
        {
            private static Action<IAnimationContextualResponder> <>f__am$cache0;
            private static Action<IAnimationContextualResponder> <>f__am$cache1;
            internal AnimationPropertyContextualMenu.<OnPropertyContextMenu>c__AnonStorey0 <>f__ref$0;
            internal SerializedProperty propertyCopy;

            internal bool <>m__0(IAnimationContextualResponder responder) => 
                responder.IsEditable(this.<>f__ref$0.property);

            internal bool <>m__1(IAnimationContextualResponder responder) => 
                responder.KeyExists(this.<>f__ref$0.property);

            internal void <>m__10(IAnimationContextualResponder responder)
            {
                responder.GoToPreviousKeyframe(this.propertyCopy);
            }

            internal void <>m__11(IAnimationContextualResponder responder)
            {
                responder.GoToNextKeyframe(this.propertyCopy);
            }

            internal bool <>m__2(IAnimationContextualResponder responder) => 
                responder.CandidateExists(this.<>f__ref$0.property);

            internal bool <>m__3(IAnimationContextualResponder responder) => 
                responder.CurveExists(this.<>f__ref$0.property);

            internal void <>m__4()
            {
                this.<>f__ref$0.$this.m_Responders.ForEach(responder => responder.AddKey(this.propertyCopy));
            }

            internal void <>m__5()
            {
                this.<>f__ref$0.$this.m_Responders.ForEach(responder => responder.RemoveKey(this.propertyCopy));
            }

            internal void <>m__6()
            {
                this.<>f__ref$0.$this.m_Responders.ForEach(responder => responder.RemoveCurve(this.propertyCopy));
            }

            internal void <>m__7()
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = responder => responder.AddCandidateKeys();
                }
                this.<>f__ref$0.$this.m_Responders.ForEach(<>f__am$cache0);
            }

            internal void <>m__8()
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = responder => responder.AddAnimatedKeys();
                }
                this.<>f__ref$0.$this.m_Responders.ForEach(<>f__am$cache1);
            }

            internal void <>m__9()
            {
                this.<>f__ref$0.$this.m_Responders.ForEach(responder => responder.GoToPreviousKeyframe(this.propertyCopy));
            }

            internal void <>m__A()
            {
                this.<>f__ref$0.$this.m_Responders.ForEach(responder => responder.GoToNextKeyframe(this.propertyCopy));
            }

            internal void <>m__B(IAnimationContextualResponder responder)
            {
                responder.AddKey(this.propertyCopy);
            }

            internal void <>m__C(IAnimationContextualResponder responder)
            {
                responder.RemoveKey(this.propertyCopy);
            }

            internal void <>m__D(IAnimationContextualResponder responder)
            {
                responder.RemoveCurve(this.propertyCopy);
            }

            private static void <>m__E(IAnimationContextualResponder responder)
            {
                responder.AddCandidateKeys();
            }

            private static void <>m__F(IAnimationContextualResponder responder)
            {
                responder.AddAnimatedKeys();
            }
        }
    }
}

