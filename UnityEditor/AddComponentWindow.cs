namespace UnityEditor
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [InitializeOnLoad]
    internal class AddComponentWindow : EditorWindow
    {
        private const string kComponentSearch = "ComponentSearchString";
        private const Language kDefaultLanguage = Language.CSharp;
        private const int kHeaderHeight = 30;
        private const int kHelpHeight = 0;
        private const string kLanguageEditorPrefName = "NewScriptLanguage";
        private const string kSearchHeader = "Search";
        private const int kWindowHeight = 320;
        private float m_Anim = 1f;
        private int m_AnimTarget = 1;
        private string m_ClassName = "";
        private string m_DelayedSearch = null;
        private GameObject[] m_GameObjects;
        private long m_LastTime = 0L;
        private bool m_ScrollToSelected = false;
        private string m_Search = "";
        private Element[] m_SearchResultTree;
        private List<GroupElement> m_Stack = new List<GroupElement>();
        private Element[] m_Tree;
        private static AddComponentWindow s_AddComponentWindow = null;
        private static bool s_DirtyList = false;
        internal static Language s_Lang;
        private static long s_LastClosedTime;
        private static Styles s_Styles;

        static AddComponentWindow()
        {
            s_DirtyList = true;
        }

        private void CreateComponentTree()
        {
            GroupElement element2;
            string[] submenus = Unsupported.GetSubmenus("Component");
            string[] submenusCommands = Unsupported.GetSubmenusCommands("Component");
            List<string> list = new List<string>();
            List<Element> list2 = new List<Element>();
            for (int i = 0; i < submenus.Length; i++)
            {
                if (submenusCommands[i] != "ADD")
                {
                    string menuPath = submenus[i];
                    char[] separator = new char[] { '/' };
                    string[] strArray3 = menuPath.Split(separator);
                    while ((strArray3.Length - 1) < list.Count)
                    {
                        list.RemoveAt(list.Count - 1);
                    }
                    while ((list.Count > 0) && (strArray3[list.Count - 1] != list[list.Count - 1]))
                    {
                        list.RemoveAt(list.Count - 1);
                    }
                    while ((strArray3.Length - 1) > list.Count)
                    {
                        list2.Add(new GroupElement(list.Count, LocalizationDatabase.GetLocalizedString(strArray3[list.Count])));
                        list.Add(strArray3[list.Count]);
                    }
                    list2.Add(new ComponentElement(list.Count, LocalizationDatabase.GetLocalizedString(strArray3[strArray3.Length - 1]), menuPath, submenusCommands[i]));
                }
            }
            list2.Add(new NewScriptElement());
            this.m_Tree = list2.ToArray();
            if (this.m_Stack.Count == 0)
            {
                this.m_Stack.Add(this.m_Tree[0] as GroupElement);
                goto Label_02BE;
            }
            <CreateComponentTree>c__AnonStorey0 storey = new <CreateComponentTree>c__AnonStorey0 {
                $this = this
            };
            GroupElement parent = this.m_Tree[0] as GroupElement;
            storey.level = 0;
        Label_01AC:
            element2 = this.m_Stack[storey.level];
            this.m_Stack[storey.level] = parent;
            this.m_Stack[storey.level].selectedIndex = element2.selectedIndex;
            this.m_Stack[storey.level].scroll = element2.scroll;
            storey.level++;
            if (storey.level != this.m_Stack.Count)
            {
                Element element3 = Enumerable.FirstOrDefault<Element>(this.GetChildren(this.activeTree, parent), new Func<Element, bool>(storey.<>m__0));
                if ((element3 != null) && (element3 is GroupElement))
                {
                    parent = element3 as GroupElement;
                }
                else
                {
                    while (this.m_Stack.Count > storey.level)
                    {
                        this.m_Stack.RemoveAt(storey.level);
                    }
                }
                goto Label_01AC;
            }
        Label_02BE:
            s_DirtyList = false;
            this.RebuildSearch();
        }

        internal static void ExecuteAddComponentMenuItem()
        {
            InspectorWindow window = FirstInspectorWithGameObject();
            if (window != null)
            {
                window.SendEvent(EditorGUIUtility.CommandEvent("OpenAddComponentDropdown"));
            }
        }

        private static InspectorWindow FirstInspectorWithGameObject()
        {
            foreach (InspectorWindow window in InspectorWindow.GetInspectors())
            {
                if (window.GetInspectedObject() is GameObject)
                {
                    return window;
                }
            }
            return null;
        }

        private List<Element> GetChildren(Element[] tree, Element parent)
        {
            List<Element> list = new List<Element>();
            int num = -1;
            int index = 0;
            index = 0;
            while (index < tree.Length)
            {
                if (tree[index] == parent)
                {
                    num = parent.level + 1;
                    index++;
                    break;
                }
                index++;
            }
            if (num != -1)
            {
                while (index < tree.Length)
                {
                    Element item = tree[index];
                    if (item.level < num)
                    {
                        return list;
                    }
                    if ((item.level <= num) || this.hasSearch)
                    {
                        list.Add(item);
                    }
                    index++;
                }
            }
            return list;
        }

        private GroupElement GetElementRelative(int rel)
        {
            int num = (this.m_Stack.Count + rel) - 1;
            if (num < 0)
            {
                return null;
            }
            return this.m_Stack[num];
        }

        private void GoToChild(Element e, bool addIfComponent)
        {
            if ((e is NewScriptElement) && !this.hasSearch)
            {
                this.m_ClassName = AssetDatabase.GenerateUniqueAssetPath((e as NewScriptElement).TargetPath());
                this.m_ClassName = Path.GetFileNameWithoutExtension(this.m_ClassName);
            }
            if (e is ComponentElement)
            {
                if (addIfComponent)
                {
                    EditorApplication.ExecuteMenuItemOnGameObjects(((ComponentElement) e).menuPath, this.m_GameObjects);
                    base.Close();
                }
            }
            else if (!this.hasSearch || (e is NewScriptElement))
            {
                this.m_LastTime = DateTime.Now.Ticks;
                if (this.m_AnimTarget == 0)
                {
                    this.m_AnimTarget = 1;
                }
                else if (this.m_Anim == 1f)
                {
                    this.m_Anim = 0f;
                    this.m_Stack.Add(e as GroupElement);
                }
            }
        }

        private void GoToParent()
        {
            if (this.m_Stack.Count > 1)
            {
                this.m_AnimTarget = 0;
                this.m_LastTime = DateTime.Now.Ticks;
            }
        }

        private void HandleKeyboard()
        {
            Event current = Event.current;
            if (current.type == EventType.KeyDown)
            {
                if (this.activeParent is NewScriptElement)
                {
                    if ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter))
                    {
                        (this.activeParent as NewScriptElement).Create();
                        current.Use();
                        GUIUtility.ExitGUI();
                    }
                    if (current.keyCode == KeyCode.Escape)
                    {
                        this.GoToParent();
                        current.Use();
                    }
                }
                else
                {
                    if (current.keyCode == KeyCode.DownArrow)
                    {
                        GroupElement activeParent = this.activeParent;
                        activeParent.selectedIndex++;
                        this.activeParent.selectedIndex = Mathf.Min(this.activeParent.selectedIndex, this.GetChildren(this.activeTree, this.activeParent).Count - 1);
                        this.m_ScrollToSelected = true;
                        current.Use();
                    }
                    if (current.keyCode == KeyCode.UpArrow)
                    {
                        GroupElement element2 = this.activeParent;
                        element2.selectedIndex--;
                        this.activeParent.selectedIndex = Mathf.Max(this.activeParent.selectedIndex, 0);
                        this.m_ScrollToSelected = true;
                        current.Use();
                    }
                    if ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter))
                    {
                        this.GoToChild(this.activeElement, true);
                        current.Use();
                    }
                    if (!this.hasSearch)
                    {
                        if ((current.keyCode == KeyCode.LeftArrow) || (current.keyCode == KeyCode.Backspace))
                        {
                            this.GoToParent();
                            current.Use();
                        }
                        if (current.keyCode == KeyCode.RightArrow)
                        {
                            this.GoToChild(this.activeElement, false);
                            current.Use();
                        }
                        if (current.keyCode == KeyCode.Escape)
                        {
                            base.Close();
                            current.Use();
                        }
                    }
                }
            }
        }

        private void Init(Rect buttonRect)
        {
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            this.CreateComponentTree();
            base.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, 320f), null, ShowMode.PopupMenuWithKeyboardFocus);
            base.Focus();
            base.m_Parent.AddToAuxWindowList();
            base.wantsMouseMove = true;
        }

        private void ListGUI(Element[] tree, GroupElement parent)
        {
            parent.scroll = GUILayout.BeginScrollView(parent.scroll, new GUILayoutOption[0]);
            EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
            List<Element> children = this.GetChildren(tree, parent);
            Rect rect = new Rect();
            for (int i = 0; i < children.Count; i++)
            {
                Element e = children[i];
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                Rect position = GUILayoutUtility.GetRect((float) 16f, (float) 20f, options);
                if (((Event.current.type == EventType.MouseMove) || (Event.current.type == EventType.MouseDown)) && ((parent.selectedIndex != i) && position.Contains(Event.current.mousePosition)))
                {
                    parent.selectedIndex = i;
                    base.Repaint();
                }
                bool on = false;
                if (i == parent.selectedIndex)
                {
                    on = true;
                    rect = position;
                }
                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle groupButton = s_Styles.groupButton;
                    GUIContent legacyContent = e.content;
                    bool flag2 = e is ComponentElement;
                    if (flag2)
                    {
                        ComponentElement element2 = (ComponentElement) e;
                        groupButton = s_Styles.componentButton;
                        if (element2.isLegacy && this.hasSearch)
                        {
                            legacyContent = element2.legacyContent;
                        }
                    }
                    groupButton.Draw(position, legacyContent, false, false, on, on);
                    if (!flag2)
                    {
                        Rect rect3 = new Rect((position.x + position.width) - 13f, position.y + 4f, 13f, 13f);
                        s_Styles.rightArrow.Draw(rect3, false, false, false, false);
                    }
                }
                if ((Event.current.type == EventType.MouseDown) && position.Contains(Event.current.mousePosition))
                {
                    Event.current.Use();
                    parent.selectedIndex = i;
                    this.GoToChild(e, true);
                }
            }
            EditorGUIUtility.SetIconSize(Vector2.zero);
            GUILayout.EndScrollView();
            if (this.m_ScrollToSelected && (Event.current.type == EventType.Repaint))
            {
                this.m_ScrollToSelected = false;
                Rect lastRect = GUILayoutUtility.GetLastRect();
                if ((rect.yMax - lastRect.height) > parent.scroll.y)
                {
                    parent.scroll.y = rect.yMax - lastRect.height;
                    base.Repaint();
                }
                if (rect.y < parent.scroll.y)
                {
                    parent.scroll.y = rect.y;
                    base.Repaint();
                }
            }
        }

        private void ListGUI(Element[] tree, float anim, GroupElement parent, GroupElement grandParent)
        {
            anim = Mathf.Floor(anim) + Mathf.SmoothStep(0f, 1f, Mathf.Repeat(anim, 1f));
            Rect position = base.position;
            position.x = (base.position.width * (1f - anim)) + 1f;
            position.y = 30f;
            position.height -= 30f;
            position.width -= 2f;
            GUILayout.BeginArea(position);
            Rect rect = GUILayoutUtility.GetRect((float) 10f, (float) 25f);
            string name = parent.name;
            GUI.Label(rect, name, s_Styles.header);
            if (grandParent != null)
            {
                Rect rect4 = new Rect(rect.x + 4f, rect.y + 7f, 13f, 13f);
                if (Event.current.type == EventType.Repaint)
                {
                    s_Styles.leftArrow.Draw(rect4, false, false, false, false);
                }
                if ((Event.current.type == EventType.MouseDown) && rect.Contains(Event.current.mousePosition))
                {
                    this.GoToParent();
                    Event.current.Use();
                }
            }
            if (parent is NewScriptElement)
            {
                (parent as NewScriptElement).OnGUI();
            }
            else
            {
                this.ListGUI(tree, parent);
            }
            GUILayout.EndArea();
        }

        private void OnDisable()
        {
            s_LastClosedTime = DateTime.Now.Ticks / 0x2710L;
            s_AddComponentWindow = null;
        }

        private void OnEnable()
        {
            s_AddComponentWindow = this;
            s_Lang = (Language) EditorPrefs.GetInt("NewScriptLanguage", 0);
            if (!Enum.IsDefined(typeof(Language), s_Lang))
            {
                EditorPrefs.SetInt("NewScriptLanguage", 0);
                s_Lang = Language.CSharp;
            }
            this.m_Search = EditorPrefs.GetString("ComponentSearchString", "");
        }

        internal void OnGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, s_Styles.background);
            if (s_DirtyList)
            {
                this.CreateComponentTree();
            }
            this.HandleKeyboard();
            GUILayout.Space(7f);
            if (!(this.activeParent is NewScriptElement))
            {
                EditorGUI.FocusTextInControl("ComponentSearch");
            }
            Rect rect = GUILayoutUtility.GetRect((float) 10f, (float) 20f);
            rect.x += 8f;
            rect.width -= 16f;
            GUI.SetNextControlName("ComponentSearch");
            using (new EditorGUI.DisabledScope(this.activeParent is NewScriptElement))
            {
                if (this.m_DelayedSearch == null)
                {
                }
                string str = EditorGUI.SearchField(rect, this.m_Search);
                if ((str != this.m_Search) || (this.m_DelayedSearch != null))
                {
                    if (!this.isAnimating)
                    {
                        if (this.m_DelayedSearch == null)
                        {
                        }
                        this.m_Search = str;
                        EditorPrefs.SetString("ComponentSearchString", this.m_Search);
                        this.RebuildSearch();
                        this.m_DelayedSearch = null;
                    }
                    else
                    {
                        this.m_DelayedSearch = str;
                    }
                }
            }
            this.ListGUI(this.activeTree, this.m_Anim, this.GetElementRelative(0), this.GetElementRelative(-1));
            if (this.m_Anim < 1f)
            {
                this.ListGUI(this.activeTree, this.m_Anim + 1f, this.GetElementRelative(-1), this.GetElementRelative(-2));
            }
            if (this.isAnimating && (Event.current.type == EventType.Repaint))
            {
                long ticks = DateTime.Now.Ticks;
                float num2 = ((float) (ticks - this.m_LastTime)) / 1E+07f;
                this.m_LastTime = ticks;
                this.m_Anim = Mathf.MoveTowards(this.m_Anim, (float) this.m_AnimTarget, num2 * 4f);
                if ((this.m_AnimTarget == 0) && (this.m_Anim == 0f))
                {
                    this.m_Anim = 1f;
                    this.m_AnimTarget = 1;
                    this.m_Stack.RemoveAt(this.m_Stack.Count - 1);
                }
                base.Repaint();
            }
        }

        private void RebuildSearch()
        {
            if (!this.hasSearch)
            {
                this.m_SearchResultTree = null;
                if (this.m_Stack[this.m_Stack.Count - 1].name == "Search")
                {
                    this.m_Stack.Clear();
                    this.m_Stack.Add(this.m_Tree[0] as GroupElement);
                }
                this.m_AnimTarget = 1;
                this.m_LastTime = DateTime.Now.Ticks;
                this.m_ClassName = "NewBehaviourScript";
            }
            else
            {
                this.m_ClassName = this.m_Search;
                char[] separator = new char[] { ' ' };
                string[] strArray = this.m_Search.ToLower().Split(separator);
                List<Element> collection = new List<Element>();
                List<Element> list2 = new List<Element>();
                foreach (Element element in this.m_Tree)
                {
                    if (!(element is ComponentElement))
                    {
                        continue;
                    }
                    string str = element.name.ToLower().Replace(" ", "");
                    bool flag = true;
                    bool flag2 = false;
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        string str2 = strArray[i];
                        if (str.Contains(str2))
                        {
                            if ((i == 0) && str.StartsWith(str2))
                            {
                                flag2 = true;
                            }
                        }
                        else
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        if (flag2)
                        {
                            collection.Add(element);
                        }
                        else
                        {
                            list2.Add(element);
                        }
                    }
                }
                collection.Sort();
                list2.Sort();
                List<Element> list3 = new List<Element> {
                    new GroupElement(0, "Search")
                };
                list3.AddRange(collection);
                list3.AddRange(list2);
                list3.Add(this.m_Tree[this.m_Tree.Length - 1]);
                this.m_SearchResultTree = list3.ToArray();
                this.m_Stack.Clear();
                this.m_Stack.Add(this.m_SearchResultTree[0] as GroupElement);
                if (this.GetChildren(this.activeTree, this.activeParent).Count >= 1)
                {
                    this.activeParent.selectedIndex = 0;
                }
                else
                {
                    this.activeParent.selectedIndex = -1;
                }
            }
        }

        internal static bool Show(Rect rect, GameObject[] gos)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(AddComponentWindow));
            if (objArray.Length > 0)
            {
                ((EditorWindow) objArray[0]).Close();
                return false;
            }
            long num = DateTime.Now.Ticks / 0x2710L;
            if (num >= (s_LastClosedTime + 50L))
            {
                Event.current.Use();
                if (s_AddComponentWindow == null)
                {
                    s_AddComponentWindow = ScriptableObject.CreateInstance<AddComponentWindow>();
                }
                s_AddComponentWindow.Init(rect);
                s_AddComponentWindow.m_GameObjects = gos;
                return true;
            }
            return false;
        }

        internal static bool ValidateAddComponentMenuItem() => 
            (FirstInspectorWithGameObject() != null);

        private Element activeElement
        {
            get
            {
                if (this.activeTree == null)
                {
                    return null;
                }
                List<Element> children = this.GetChildren(this.activeTree, this.activeParent);
                if (children.Count == 0)
                {
                    return null;
                }
                return children[this.activeParent.selectedIndex];
            }
        }

        private GroupElement activeParent =>
            this.m_Stack[(this.m_Stack.Count - 2) + this.m_AnimTarget];

        private Element[] activeTree =>
            (!this.hasSearch ? this.m_Tree : this.m_SearchResultTree);

        internal static string className
        {
            get => 
                s_AddComponentWindow.m_ClassName;
            set
            {
                s_AddComponentWindow.m_ClassName = value;
            }
        }

        internal static GameObject[] gameObjects =>
            s_AddComponentWindow.m_GameObjects;

        private bool hasSearch =>
            !string.IsNullOrEmpty(this.m_Search);

        private bool isAnimating =>
            (this.m_Anim != this.m_AnimTarget);

        [CompilerGenerated]
        private sealed class <CreateComponentTree>c__AnonStorey0
        {
            internal AddComponentWindow $this;
            internal int level;

            internal bool <>m__0(AddComponentWindow.Element c) => 
                (c.name == this.$this.m_Stack[this.level].name);
        }

        private class ComponentElement : AddComponentWindow.Element
        {
            public bool isLegacy;
            private GUIContent m_LegacyContentCache;
            public string menuPath;

            public ComponentElement(int level, string name, string menuPath, string commandString)
            {
                base.level = level;
                this.menuPath = menuPath;
                this.isLegacy = menuPath.Contains("Legacy");
                if (commandString.StartsWith("SCRIPT"))
                {
                    Texture miniThumbnail = AssetPreview.GetMiniThumbnail(EditorUtility.InstanceIDToObject(int.Parse(commandString.Substring(6))));
                    base.content = new GUIContent(name, miniThumbnail);
                }
                else
                {
                    int classID = int.Parse(commandString);
                    base.content = new GUIContent(name, AssetPreview.GetMiniTypeThumbnailFromClassID(classID));
                }
            }

            public override int CompareTo(object o)
            {
                if (o is AddComponentWindow.ComponentElement)
                {
                    AddComponentWindow.ComponentElement element = (AddComponentWindow.ComponentElement) o;
                    if (this.isLegacy && !element.isLegacy)
                    {
                        return 1;
                    }
                    if (!this.isLegacy && element.isLegacy)
                    {
                        return -1;
                    }
                }
                return base.CompareTo(o);
            }

            public GUIContent legacyContent
            {
                get
                {
                    if (this.m_LegacyContentCache == null)
                    {
                        this.m_LegacyContentCache = new GUIContent(base.content);
                        this.m_LegacyContentCache.text = this.m_LegacyContentCache.text + " (Legacy)";
                    }
                    return this.m_LegacyContentCache;
                }
            }
        }

        private class Element : IComparable
        {
            public GUIContent content;
            public int level;

            public virtual int CompareTo(object o) => 
                this.name.CompareTo((o as AddComponentWindow.Element).name);

            public string name =>
                this.content.text;
        }

        [Serializable]
        private class GroupElement : AddComponentWindow.Element
        {
            public Vector2 scroll;
            public int selectedIndex = 0;

            public GroupElement(int level, string name)
            {
                base.level = level;
                base.content = new GUIContent(name);
            }
        }

        internal enum Language
        {
            CSharp,
            JavaScript
        }

        private class NewScriptElement : AddComponentWindow.GroupElement
        {
            private char[] kInvalidPathChars;
            private char[] kPathSepChars;
            private const string kResourcesTemplatePath = "Resources/ScriptTemplates";
            private string m_Directory;

            public NewScriptElement() : base(1, "New Script")
            {
                this.kInvalidPathChars = new char[] { '<', '>', ':', '"', '|', '?', '*', '\0' };
                this.kPathSepChars = new char[] { '/', '\\' };
                this.m_Directory = string.Empty;
            }

            public bool CanCreate() => 
                ((((AddComponentWindow.className.Length > 0) && !File.Exists(this.TargetPath())) && (!this.ClassAlreadyExists() && !this.ClassNameIsInvalid())) && !this.InvalidTargetPath());

            private bool ClassAlreadyExists()
            {
                if (AddComponentWindow.className == string.Empty)
                {
                    return false;
                }
                return this.ClassExists(AddComponentWindow.className);
            }

            private bool ClassExists(string className)
            {
                <ClassExists>c__AnonStorey0 storey = new <ClassExists>c__AnonStorey0 {
                    className = className
                };
                return Enumerable.Any<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), new Func<Assembly, bool>(storey.<>m__0));
            }

            private bool ClassNameIsInvalid() => 
                !CodeGenerator.IsValidLanguageIndependentIdentifier(AddComponentWindow.className);

            public void Create()
            {
                if (this.CanCreate())
                {
                    this.CreateScript();
                    foreach (GameObject obj2 in AddComponentWindow.gameObjects)
                    {
                        MonoScript script = AssetDatabase.LoadAssetAtPath(this.TargetPath(), typeof(MonoScript)) as MonoScript;
                        script.SetScriptTypeWasJustCreatedFromComponentMenu();
                        InternalEditorUtility.AddScriptComponentUncheckedUndoable(obj2, script);
                    }
                    AddComponentWindow.s_AddComponentWindow.Close();
                }
            }

            private void CreateScript()
            {
                ProjectWindowUtil.CreateScriptAssetFromTemplate(this.TargetPath(), this.templatePath);
                AssetDatabase.Refresh();
            }

            private string GetError()
            {
                string str = string.Empty;
                if (AddComponentWindow.className != string.Empty)
                {
                    if (File.Exists(this.TargetPath()))
                    {
                        return ("A script called \"" + AddComponentWindow.className + "\" already exists at that path.");
                    }
                    if (this.ClassAlreadyExists())
                    {
                        return ("A class called \"" + AddComponentWindow.className + "\" already exists.");
                    }
                    if (this.ClassNameIsInvalid())
                    {
                        return "The script name may only consist of a-z, A-Z, 0-9, _.";
                    }
                    if (this.InvalidTargetPath())
                    {
                        str = "The folder path contains invalid characters.";
                    }
                }
                return str;
            }

            private bool InvalidTargetPath() => 
                ((this.m_Directory.IndexOfAny(this.kInvalidPathChars) >= 0) || this.TargetDir().Split(this.kPathSepChars, StringSplitOptions.None).Contains<string>(string.Empty));

            public void OnGUI()
            {
                GUILayout.Label("Name", EditorStyles.label, new GUILayoutOption[0]);
                EditorGUI.FocusTextInControl("NewScriptName");
                GUI.SetNextControlName("NewScriptName");
                AddComponentWindow.className = EditorGUILayout.TextField(AddComponentWindow.className, new GUILayoutOption[0]);
                EditorGUILayout.Space();
                AddComponentWindow.Language language = (AddComponentWindow.Language) EditorGUILayout.EnumPopup("Language", AddComponentWindow.s_Lang, new GUILayoutOption[0]);
                if (language != AddComponentWindow.s_Lang)
                {
                    AddComponentWindow.s_Lang = language;
                    EditorPrefs.SetInt("NewScriptLanguage", (int) language);
                }
                EditorGUILayout.Space();
                bool flag = this.CanCreate();
                if (!flag && (AddComponentWindow.className != ""))
                {
                    GUILayout.Label(this.GetError(), EditorStyles.helpBox, new GUILayoutOption[0]);
                }
                GUILayout.FlexibleSpace();
                using (new EditorGUI.DisabledScope(!flag))
                {
                    if (GUILayout.Button("Create and Add", new GUILayoutOption[0]))
                    {
                        this.Create();
                    }
                }
                EditorGUILayout.Space();
            }

            private string TargetDir() => 
                Path.Combine("Assets", this.m_Directory.Trim(this.kPathSepChars));

            public string TargetPath() => 
                Path.Combine(this.TargetDir(), AddComponentWindow.className + "." + this.extension);

            private string extension
            {
                get
                {
                    AddComponentWindow.Language language = AddComponentWindow.s_Lang;
                    if (language != AddComponentWindow.Language.CSharp)
                    {
                        if (language != AddComponentWindow.Language.JavaScript)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        return "cs";
                    }
                    return "js";
                }
            }

            private string templatePath
            {
                get
                {
                    string str = Path.Combine(EditorApplication.applicationContentsPath, "Resources/ScriptTemplates");
                    AddComponentWindow.Language language = AddComponentWindow.s_Lang;
                    if (language != AddComponentWindow.Language.JavaScript)
                    {
                        if (language != AddComponentWindow.Language.CSharp)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        return Path.Combine(str, "82-Javascript-NewBehaviourScript.js.txt");
                    }
                    return Path.Combine(str, "81-C# Script-NewBehaviourScript.cs.txt");
                }
            }

            [CompilerGenerated]
            private sealed class <ClassExists>c__AnonStorey0
            {
                internal string className;

                internal bool <>m__0(Assembly a) => 
                    (a.GetType(this.className, false) != null);
            }
        }

        private class Styles
        {
            public GUIStyle background = "grey_border";
            public GUIStyle componentButton = new GUIStyle("PR Label");
            public GUIStyle groupButton;
            public GUIStyle header = new GUIStyle(EditorStyles.inspectorBig);
            public GUIStyle leftArrow = "AC LeftArrow";
            public GUIStyle previewHeader = new GUIStyle(EditorStyles.label);
            public GUIStyle previewText = new GUIStyle(EditorStyles.wordWrappedLabel);
            public GUIStyle rightArrow = "AC RightArrow";

            public Styles()
            {
                this.header.font = EditorStyles.boldLabel.font;
                this.componentButton.alignment = TextAnchor.MiddleLeft;
                RectOffset padding = this.componentButton.padding;
                padding.left -= 15;
                this.componentButton.fixedHeight = 20f;
                this.groupButton = new GUIStyle(this.componentButton);
                RectOffset offset2 = this.groupButton.padding;
                offset2.left += 0x11;
                RectOffset offset3 = this.previewText.padding;
                offset3.left += 3;
                RectOffset offset4 = this.previewText.padding;
                offset4.right += 3;
                RectOffset offset5 = this.previewHeader.padding;
                offset5.left++;
                RectOffset offset6 = this.previewHeader.padding;
                offset6.right += 3;
                RectOffset offset7 = this.previewHeader.padding;
                offset7.top += 3;
                RectOffset offset8 = this.previewHeader.padding;
                offset8.bottom += 2;
            }
        }
    }
}

