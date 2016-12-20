namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal class MainView : View, ICleanuppable
    {
        private static readonly Vector2 kMaxSize = new Vector2(10000f, 10000f);
        private static readonly Vector2 kMinSize = new Vector2(950f, 300f);
        private const float kStatusbarHeight = 20f;

        protected override void ChildrenMinMaxChanged()
        {
            if (base.children.Length == 3)
            {
                Toolbar toolbar = (Toolbar) base.children[0];
                Vector2 min = new Vector2(kMinSize.x, Mathf.Max(kMinSize.y, (toolbar.CalcHeight() + 20f) + base.children[1].minSize.y));
                base.SetMinMaxSizes(min, kMaxSize);
            }
            base.ChildrenMinMaxChanged();
        }

        public void Cleanup()
        {
            if (base.children[1].children.Length == 0)
            {
                Rect position = base.window.position;
                Toolbar toolbar = (Toolbar) base.children[0];
                position.height = toolbar.CalcHeight() + 20f;
            }
        }

        public static void MakeMain()
        {
            ContainerWindow window = ScriptableObject.CreateInstance<ContainerWindow>();
            MainView view = ScriptableObject.CreateInstance<MainView>();
            view.SetMinMaxSizes(kMinSize, kMaxSize);
            window.rootView = view;
            Resolution desktopResolution = InternalEditorUtility.GetDesktopResolution();
            int num = Mathf.Clamp((desktopResolution.width * 3) / 4, 800, 0x578);
            int num2 = Mathf.Clamp((desktopResolution.height * 3) / 4, 600, 950);
            window.position = new Rect(60f, 20f, (float) num, (float) num2);
            window.Show(ShowMode.MainWindow, true, true);
            window.DisplayAllViews();
        }

        private void OnEnable()
        {
            base.SetMinMaxSizes(kMinSize, kMaxSize);
        }

        protected override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            if (base.children.Length != 0)
            {
                Toolbar toolbar = (Toolbar) base.children[0];
                base.children[0].position = new Rect(0f, 0f, newPos.width, toolbar.CalcHeight());
                if (base.children.Length > 2)
                {
                    base.children[1].position = new Rect(0f, toolbar.CalcHeight(), newPos.width, (newPos.height - toolbar.CalcHeight()) - base.children[2].position.height);
                    base.children[2].position = new Rect(0f, newPos.height - base.children[2].position.height, newPos.width, base.children[2].position.height);
                }
            }
        }
    }
}

