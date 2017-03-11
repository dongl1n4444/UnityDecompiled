namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor.Connect;
    using UnityEditorInternal;
    using UnityEngine;

    internal class WindowLayout
    {
        private const string kMaximizeRestoreFile = "CurrentMaximizeLayout.dwlt";
        internal static PrefKey s_MaximizeKey = new PrefKey("Window/Maximize View", "# ");

        public static void AddSplitViewAndChildrenRecurse(View splitview, ArrayList list)
        {
            list.Add(splitview);
            DockArea area = splitview as DockArea;
            if (area != null)
            {
                list.AddRange(area.m_Panes);
            }
            HostView view = splitview as DockArea;
            if (view != null)
            {
                list.Add(area.actualView);
            }
            foreach (View view2 in splitview.children)
            {
                AddSplitViewAndChildrenRecurse(view2, list);
            }
        }

        internal static void CheckWindowConsistency()
        {
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
            foreach (EditorWindow window in objArray)
            {
                if (window.m_Parent == null)
                {
                    UnityEngine.Debug.LogError("Invalid editor window " + window.GetType());
                }
            }
        }

        public static void CloseWindows()
        {
            try
            {
                TooltipView.Close();
            }
            catch (Exception)
            {
            }
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(ContainerWindow));
            foreach (ContainerWindow window in objArray)
            {
                try
                {
                    window.Close();
                }
                catch (Exception)
                {
                }
            }
            UnityEngine.Object[] objArray3 = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
            if (objArray3.Length != 0)
            {
                string str = "";
                foreach (EditorWindow window2 in objArray3)
                {
                    str = str + "\n" + window2.GetType().Name;
                    UnityEngine.Object.DestroyImmediate(window2, true);
                }
                UnityEngine.Debug.LogError("Failed to destroy editor windows: #" + objArray3.Length + str);
            }
            UnityEngine.Object[] objArray5 = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(View));
            if (objArray5.Length != 0)
            {
                string str2 = "";
                foreach (View view in objArray5)
                {
                    str2 = str2 + "\n" + view.GetType().Name;
                    UnityEngine.Object.DestroyImmediate(view, true);
                }
                UnityEngine.Debug.LogError("Failed to destroy views: #" + objArray5.Length + str2);
            }
        }

        public static void DeleteGUI()
        {
            Rect screenPosition = FindMainView().screenPosition;
            EditorWindow.GetWindowWithRect<DeleteWindowLayout>(new Rect(screenPosition.xMax - 180f, screenPosition.y + 20f, 200f, 150f), true, "Delete Window Layout").m_Parent.window.m_DontSaveToLayout = true;
        }

        public static void EnsureMainWindowHasBeenLoaded()
        {
            if (UnityEngine.Resources.FindObjectsOfTypeAll<MainView>().Length == 0)
            {
                MainView.MakeMain();
            }
        }

        internal static EditorWindow FindEditorWindowOfType(System.Type type)
        {
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(type);
            if (objArray.Length > 0)
            {
                return (objArray[0] as EditorWindow);
            }
            return null;
        }

        [DebuggerHidden]
        private static IEnumerable<T> FindEditorWindowsOfType<T>() where T: class => 
            new <FindEditorWindowsOfType>c__Iterator0<T> { $PC = -2 };

        internal static void FindFirstGameViewAndSetToMaximizeOnPlay()
        {
            GameView view = (GameView) FindEditorWindowOfType(typeof(GameView));
            if (view != null)
            {
                view.maximizeOnPlay = true;
            }
        }

        internal static MainView FindMainView()
        {
            MainView[] viewArray = UnityEngine.Resources.FindObjectsOfTypeAll<MainView>();
            if (viewArray.Length == 0)
            {
                UnityEngine.Debug.LogError("No Main View found!");
                return null;
            }
            return viewArray[0];
        }

        internal static EditorWindow GetMaximizedWindow()
        {
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(MaximizedHostView));
            if (objArray.Length != 0)
            {
                MaximizedHostView view = objArray[0] as MaximizedHostView;
                if (view.actualView != null)
                {
                    return view.actualView;
                }
            }
            return null;
        }

        internal static bool IsMaximized(EditorWindow window) => 
            (window.m_Parent is MaximizedHostView);

        private static void LoadDefaultLayout()
        {
            InternalEditorUtility.LoadDefaultLayout();
        }

        public static bool LoadWindowLayout(string path, bool newProjectLayoutWasCreated)
        {
            Rect position = new Rect();
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(ContainerWindow));
            foreach (ContainerWindow window in objArray)
            {
                if (window.showMode == ShowMode.MainWindow)
                {
                    position = window.position;
                }
            }
            try
            {
                ContainerWindow.SetFreezeDisplay(true);
                CloseWindows();
                UnityEngine.Object[] objArray3 = InternalEditorUtility.LoadSerializedFileAndForget(path);
                List<UnityEngine.Object> list = new List<UnityEngine.Object>();
                for (int i = 0; i < objArray3.Length; i++)
                {
                    UnityEngine.Object item = objArray3[i];
                    EditorWindow window2 = item as EditorWindow;
                    if (window2 != null)
                    {
                        if (window2.m_Parent == null)
                        {
                            UnityEngine.Object.DestroyImmediate(window2, true);
                            UnityEngine.Debug.LogError(string.Concat(new object[] { "Removed unparented EditorWindow while reading window layout: window #", i, ", type=", item.GetType().ToString(), ", instanceID=", item.GetInstanceID() }));
                            continue;
                        }
                    }
                    else
                    {
                        DockArea area = item as DockArea;
                        if ((area != null) && (area.m_Panes.Count == 0))
                        {
                            area.Close(null);
                            UnityEngine.Debug.LogError(string.Concat(new object[] { "Removed empty DockArea while reading window layout: window #", i, ", instanceID=", item.GetInstanceID() }));
                            continue;
                        }
                    }
                    list.Add(item);
                }
                ContainerWindow window3 = null;
                ContainerWindow window4 = null;
                for (int j = 0; j < list.Count; j++)
                {
                    ContainerWindow window5 = list[j] as ContainerWindow;
                    if ((window5 != null) && (window5.showMode == ShowMode.MainWindow))
                    {
                        window4 = window5;
                        if (position.width != 0.0)
                        {
                            window3 = window5;
                            window3.position = position;
                        }
                    }
                }
                for (int k = 0; k < list.Count; k++)
                {
                    UnityEngine.Object obj3 = list[k];
                    if (obj3 == null)
                    {
                        UnityEngine.Debug.LogError("Error while reading window layout: window #" + k + " is null");
                    }
                    else if (obj3.GetType() == null)
                    {
                        UnityEngine.Debug.LogError(string.Concat(new object[] { "Error while reading window layout: window #", k, " type is null, instanceID=", obj3.GetInstanceID() }));
                    }
                    else if (newProjectLayoutWasCreated)
                    {
                        MethodInfo method = obj3.GetType().GetMethod("OnNewProjectLayoutWasCreated", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        if (method != null)
                        {
                            method.Invoke(obj3, null);
                        }
                    }
                }
                if (window3 != null)
                {
                    window3.position = position;
                    window3.OnResize();
                }
                if (window4 == null)
                {
                    UnityEngine.Debug.LogError("Error while reading window layout: no main window found");
                    throw new Exception();
                }
                window4.Show(window4.showMode, true, true);
                for (int m = 0; m < list.Count; m++)
                {
                    EditorWindow window6 = list[m] as EditorWindow;
                    if (window6 != null)
                    {
                        window6.minSize = window6.minSize;
                    }
                    ContainerWindow window7 = list[m] as ContainerWindow;
                    if ((window7 != null) && (window7 != window4))
                    {
                        window7.Show(window7.showMode, true, true);
                    }
                }
                GameView maximizedWindow = GetMaximizedWindow() as GameView;
                if ((maximizedWindow != null) && maximizedWindow.maximizeOnPlay)
                {
                    Unmaximize(maximizedWindow);
                }
                if (newProjectLayoutWasCreated)
                {
                    if ((UnityConnect.instance.online && UnityConnect.instance.loggedIn) && UnityConnect.instance.shouldShowServicesWindow)
                    {
                        UnityConnectServiceCollection.instance.ShowService("Hub", true);
                    }
                    else
                    {
                        UnityConnectServiceCollection.instance.CloseServices();
                    }
                }
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError("Failed to load window layout: " + exception);
                int num6 = 0;
                if (!Application.isTestRun)
                {
                    num6 = EditorUtility.DisplayDialogComplex("Failed to load window layout", "This can happen if layout contains custom windows and there are compile errors in the project.", "Load Default Layout", "Quit", "Revert Factory Settings");
                }
                if (num6 == 0)
                {
                    LoadDefaultLayout();
                }
                else if (num6 == 1)
                {
                    EditorApplication.Exit(0);
                }
                else if (num6 == 2)
                {
                    RevertFactorySettings();
                }
                return false;
            }
            finally
            {
                ContainerWindow.SetFreezeDisplay(false);
                if (Path.GetExtension(path) == ".wlt")
                {
                    Toolbar.lastLoadedLayoutName = Path.GetFileNameWithoutExtension(path);
                }
                else
                {
                    Toolbar.lastLoadedLayoutName = null;
                }
            }
            return true;
        }

        public static void Maximize(EditorWindow win)
        {
            if (MaximizePrepare(win))
            {
                MaximizePresent(win);
            }
        }

        internal static void MaximizeKeyHandler()
        {
            if ((s_MaximizeKey.activated || (Event.current.type == EditorGUIUtility.magnifyGestureEventType)) && (GUIUtility.hotControl == 0))
            {
                EventType type = Event.current.type;
                Event.current.Use();
                EditorWindow mouseOverWindow = EditorWindow.mouseOverWindow;
                if ((mouseOverWindow != null) && !(mouseOverWindow is PreviewWindow))
                {
                    if (type == EditorGUIUtility.magnifyGestureEventType)
                    {
                        if (Event.current.delta.x < -0.05)
                        {
                            if (IsMaximized(mouseOverWindow))
                            {
                                Unmaximize(mouseOverWindow);
                            }
                        }
                        else if ((Event.current.delta.x > 0.05) && !IsMaximized(mouseOverWindow))
                        {
                            Maximize(mouseOverWindow);
                        }
                    }
                    else if (IsMaximized(mouseOverWindow))
                    {
                        Unmaximize(mouseOverWindow);
                    }
                    else
                    {
                        Maximize(mouseOverWindow);
                    }
                }
            }
        }

        public static bool MaximizePrepare(EditorWindow win)
        {
            View parent = win.m_Parent.parent;
            View splitview = parent;
            while ((parent != null) && (parent is SplitView))
            {
                splitview = parent;
                parent = parent.parent;
            }
            DockArea area = win.m_Parent as DockArea;
            if (area == null)
            {
                return false;
            }
            if (parent == null)
            {
                return false;
            }
            MainView view3 = splitview.parent as MainView;
            if (view3 == null)
            {
                return false;
            }
            if (win.m_Parent.window == null)
            {
                return false;
            }
            int index = area.m_Panes.IndexOf(win);
            if (index == -1)
            {
                return false;
            }
            area.selected = index;
            SaveSplitViewAndChildren(splitview, win, Path.Combine(layoutsProjectPath, "CurrentMaximizeLayout.dwlt"));
            area.actualView = null;
            area.m_Panes[index] = null;
            MaximizedHostView child = ScriptableObject.CreateInstance<MaximizedHostView>();
            int idx = parent.IndexOfChild(splitview);
            Rect position = splitview.position;
            parent.RemoveChild(splitview);
            parent.AddChild(child, idx);
            child.actualView = win;
            child.position = position;
            UnityEngine.Object.DestroyImmediate(splitview, true);
            return true;
        }

        public static void MaximizePresent(EditorWindow win)
        {
            ContainerWindow.SetFreezeDisplay(true);
            win.Focus();
            CheckWindowConsistency();
            win.m_Parent.window.DisplayAllViews();
            win.m_Parent.MakeVistaDWMHappyDance();
            ContainerWindow.SetFreezeDisplay(false);
        }

        private static void RevertFactorySettings()
        {
            InternalEditorUtility.RevertFactoryLayoutSettings(true);
        }

        internal static void SaveCurrentFocusedWindowInSameDock(EditorWindow windowToBeFocused)
        {
            if ((windowToBeFocused.m_Parent != null) && (windowToBeFocused.m_Parent is DockArea))
            {
                DockArea parent = windowToBeFocused.m_Parent as DockArea;
                EditorWindow actualView = parent.actualView;
                if (actualView != null)
                {
                    WindowFocusState.instance.m_LastWindowTypeInSameDock = actualView.GetType().ToString();
                }
            }
        }

        public static void SaveGUI()
        {
            Rect screenPosition = FindMainView().screenPosition;
            EditorWindow.GetWindowWithRect<UnityEditor.SaveWindowLayout>(new Rect(screenPosition.xMax - 180f, screenPosition.y + 20f, 200f, 48f), true, "Save Window Layout").m_Parent.window.m_DontSaveToLayout = true;
        }

        public static void SaveSplitViewAndChildren(View splitview, EditorWindow win, string path)
        {
            ArrayList list = new ArrayList();
            AddSplitViewAndChildrenRecurse(splitview, list);
            list.Remove(splitview);
            list.Remove(win);
            list.Insert(0, splitview);
            list.Insert(1, win);
            InternalEditorUtility.SaveToSerializedFileAndForget(list.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[], path, false);
        }

        public static void SaveWindowLayout(string path)
        {
            TooltipView.Close();
            ArrayList list = new ArrayList();
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
            UnityEngine.Object[] objArray2 = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(ContainerWindow));
            UnityEngine.Object[] objArray3 = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(View));
            foreach (ContainerWindow window in objArray2)
            {
                if (!window.m_DontSaveToLayout)
                {
                    list.Add(window);
                }
            }
            foreach (View view in objArray3)
            {
                if ((view.window == null) || !view.window.m_DontSaveToLayout)
                {
                    list.Add(view);
                }
            }
            foreach (EditorWindow window2 in objArray)
            {
                if (((window2.m_Parent == null) || (window2.m_Parent.window == null)) || !window2.m_Parent.window.m_DontSaveToLayout)
                {
                    list.Add(window2);
                }
            }
            InternalEditorUtility.SaveToSerializedFileAndForget(list.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[], path, false);
        }

        internal static EditorWindow ShowAppropriateViewOnEnterExitPlaymode(bool entering)
        {
            GameView view;
            if (WindowFocusState.instance.m_CurrentlyInPlayMode == entering)
            {
                return null;
            }
            WindowFocusState.instance.m_CurrentlyInPlayMode = entering;
            EditorWindow window2 = null;
            EditorWindow maximizedWindow = GetMaximizedWindow();
            if (entering)
            {
                WindowFocusState.instance.m_WasMaximizedBeforePlay = maximizedWindow != null;
                if (maximizedWindow != null)
                {
                    return maximizedWindow;
                }
            }
            else if (WindowFocusState.instance.m_WasMaximizedBeforePlay)
            {
                return maximizedWindow;
            }
            if (maximizedWindow != null)
            {
                Unmaximize(maximizedWindow);
            }
            window2 = TryFocusAppropriateWindow(entering);
            if (window2 != null)
            {
                return window2;
            }
            if (!entering)
            {
                return window2;
            }
            EditorWindow window4 = FindEditorWindowOfType(typeof(SceneView));
            if ((window4 != null) && (window4.m_Parent is DockArea))
            {
                DockArea parent = window4.m_Parent as DockArea;
                if (parent != null)
                {
                    WindowFocusState.instance.m_LastWindowTypeInSameDock = window4.GetType().ToString();
                    view = ScriptableObject.CreateInstance<GameView>();
                    parent.AddTab(view);
                    return view;
                }
            }
            view = ScriptableObject.CreateInstance<GameView>();
            view.Show(true);
            view.Focus();
            return view;
        }

        private static void ShowWindowImmediate(EditorWindow win)
        {
            win.Show(true);
        }

        internal static EditorWindow TryFocusAppropriateWindow(bool enteringPlaymode)
        {
            if (enteringPlaymode)
            {
                GameView windowToBeFocused = (GameView) FindEditorWindowOfType(typeof(GameView));
                if (windowToBeFocused != null)
                {
                    SaveCurrentFocusedWindowInSameDock(windowToBeFocused);
                    windowToBeFocused.Focus();
                }
                return windowToBeFocused;
            }
            EditorWindow window2 = TryGetLastFocusedWindowInSameDock();
            if (window2 != null)
            {
                window2.ShowTab();
            }
            return window2;
        }

        internal static EditorWindow TryGetLastFocusedWindowInSameDock()
        {
            System.Type type = null;
            string lastWindowTypeInSameDock = WindowFocusState.instance.m_LastWindowTypeInSameDock;
            if (lastWindowTypeInSameDock != "")
            {
                type = System.Type.GetType(lastWindowTypeInSameDock);
            }
            GameView view = FindEditorWindowOfType(typeof(GameView)) as GameView;
            if (((type != null) && (view != null)) && ((view.m_Parent != null) && (view.m_Parent is DockArea)))
            {
                object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(type);
                DockArea parent = view.m_Parent as DockArea;
                for (int i = 0; i < objArray.Length; i++)
                {
                    EditorWindow window = objArray[i] as EditorWindow;
                    if ((window != null) && (window.m_Parent == parent))
                    {
                        return window;
                    }
                }
            }
            return null;
        }

        public static void Unmaximize(EditorWindow win)
        {
            HostView parent = win.m_Parent;
            if (parent == null)
            {
                UnityEngine.Debug.LogError("Host view was not found");
                RevertFactorySettings();
            }
            else
            {
                UnityEngine.Object[] objArray = InternalEditorUtility.LoadSerializedFileAndForget(Path.Combine(layoutsProjectPath, "CurrentMaximizeLayout.dwlt"));
                if (objArray.Length < 2)
                {
                    UnityEngine.Debug.Log("Maximized serialized file backup not found");
                    RevertFactorySettings();
                }
                else
                {
                    SplitView child = objArray[0] as SplitView;
                    EditorWindow item = objArray[1] as EditorWindow;
                    if (child == null)
                    {
                        UnityEngine.Debug.Log("Maximization failed because the root split view was not found");
                        RevertFactorySettings();
                    }
                    else
                    {
                        ContainerWindow window = win.m_Parent.window;
                        if (window == null)
                        {
                            UnityEngine.Debug.Log("Maximization failed because the root split view has no container window");
                            RevertFactorySettings();
                        }
                        else
                        {
                            try
                            {
                                ContainerWindow.SetFreezeDisplay(true);
                                if (parent.parent == null)
                                {
                                    throw new Exception();
                                }
                                int idx = parent.parent.IndexOfChild(parent);
                                Rect position = parent.position;
                                View view3 = parent.parent;
                                view3.RemoveChild(idx);
                                view3.AddChild(child, idx);
                                child.position = position;
                                DockArea area = item.m_Parent as DockArea;
                                int index = area.m_Panes.IndexOf(item);
                                parent.actualView = null;
                                win.m_Parent = null;
                                area.AddTab(index, win);
                                area.RemoveTab(item);
                                UnityEngine.Object.DestroyImmediate(item);
                                foreach (UnityEngine.Object obj2 in objArray)
                                {
                                    EditorWindow window3 = obj2 as EditorWindow;
                                    if (window3 != null)
                                    {
                                        window3.MakeParentsSettingsMatchMe();
                                    }
                                }
                                view3.Initialize(view3.window);
                                view3.position = view3.position;
                                child.Reflow();
                                UnityEngine.Object.DestroyImmediate(parent);
                                win.Focus();
                                window.DisplayAllViews();
                                win.m_Parent.MakeVistaDWMHappyDance();
                            }
                            catch (Exception exception)
                            {
                                UnityEngine.Debug.Log("Maximization failed: " + exception);
                                RevertFactorySettings();
                            }
                            try
                            {
                                if (((Application.platform == RuntimePlatform.OSXEditor) && SystemInfo.operatingSystem.Contains("10.7")) && SystemInfo.graphicsDeviceVendor.Contains("ATI"))
                                {
                                    foreach (GUIView view4 in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GUIView)))
                                    {
                                        view4.Repaint();
                                    }
                                }
                            }
                            finally
                            {
                                ContainerWindow.SetFreezeDisplay(false);
                            }
                        }
                    }
                }
            }
        }

        internal static string layoutsPreferencesPath =>
            (InternalEditorUtility.unityPreferencesFolder + "/Layouts");

        internal static string layoutsProjectPath =>
            (Directory.GetCurrentDirectory() + "/Library");

        [CompilerGenerated]
        private sealed class <FindEditorWindowsOfType>c__Iterator0<T> : IEnumerable, IEnumerable<T>, IEnumerator, IDisposable, IEnumerator<T> where T: class
        {
            internal T $current;
            internal bool $disposing;
            internal UnityEngine.Object[] $locvar0;
            internal int $locvar1;
            internal int $PC;
            internal UnityEngine.Object <obj>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$locvar0 = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(T));
                        this.$locvar1 = 0;
                        goto Label_00A4;

                    case 1:
                        break;

                    default:
                        goto Label_00BE;
                }
            Label_0096:
                this.$locvar1++;
            Label_00A4:
                if (this.$locvar1 < this.$locvar0.Length)
                {
                    this.<obj>__0 = this.$locvar0[this.$locvar1];
                    if (this.<obj>__0 is T)
                    {
                        this.$current = this.<obj>__0 as T;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;
                    }
                    goto Label_0096;
                }
                this.$PC = -1;
            Label_00BE:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new WindowLayout.<FindEditorWindowsOfType>c__Iterator0<T>();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();

            T IEnumerator<T>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

