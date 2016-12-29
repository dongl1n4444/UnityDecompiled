namespace UnityEditor.Graphs.AnimationStateMachine
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    internal class AddStateMachineBehaviourComponentWindow : EditorWindow
    {
        private const int kHeaderHeight = 30;
        private const int kHelpHeight = 0;
        private const string kStateMachineBehaviourSearch = "StateMachineBehaviourSearchString";
        private const int kWindowHeight = 320;
        private string m_ClassName = "";
        private AnimatorController m_Controller;
        private int m_LayerIndex;
        private Vector2 m_ScrollPosition;
        private string m_Search;
        private Element[] m_SearchTree;
        private int m_SelectedIndex;
        private UnityEngine.Object[] m_Targets;
        private Element[] m_Tree;
        private static AddStateMachineBehaviourComponentWindow s_AddComponentWindow = null;
        private static long s_LastClosedTime;
        private static Styles s_Styles;

        internal void AddBehaviour(MonoScript stateMachineBehaviour)
        {
            foreach (UnityEngine.Object obj2 in targets)
            {
                AnimatorState objectToUndo = obj2 as AnimatorState;
                AnimatorStateMachine machine = obj2 as AnimatorStateMachine;
                if ((objectToUndo != null) || (machine != null))
                {
                    int instanceID = AnimatorController.CreateStateMachineBehaviour(stateMachineBehaviour);
                    if (instanceID == 0)
                    {
                        Debug.LogError("Could not create state machine behaviour " + stateMachineBehaviour.name);
                        break;
                    }
                    string format = "Add Behaviour '{0}' to state '{1}'";
                    if (objectToUndo != null)
                    {
                        if (controller != null)
                        {
                            Undo.RegisterCompleteObjectUndo(controller, string.Format(format, stateMachineBehaviour.name, objectToUndo.name));
                            Undo.RegisterCompleteObjectUndo(objectToUndo, string.Format(format, stateMachineBehaviour.name, objectToUndo.name));
                            controller.AddStateEffectiveBehaviour(objectToUndo, layerIndex, instanceID);
                        }
                        else
                        {
                            Undo.RegisterCompleteObjectUndo(objectToUndo, string.Format(format, stateMachineBehaviour.name, objectToUndo.name));
                            objectToUndo.AddBehaviour(instanceID);
                        }
                        AssetDatabase.AddInstanceIDToAssetWithRandomFileId(instanceID, objectToUndo, true);
                    }
                    else if (machine != null)
                    {
                        Undo.RegisterCompleteObjectUndo(machine, string.Format(format, stateMachineBehaviour.name, machine.name));
                        machine.AddBehaviour(instanceID);
                        AssetDatabase.AddInstanceIDToAssetWithRandomFileId(instanceID, machine, true);
                    }
                }
            }
        }

        private void CreateBehaviourTree()
        {
            List<Element> list = new List<Element>();
            MonoScript[] scriptArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(MonoScript)) as MonoScript[];
            for (int i = 0; i < scriptArray.Length; i++)
            {
                System.Type type = scriptArray[i].GetClass();
                if (((type != null) && !type.IsAbstract) && type.IsSubclassOf(typeof(StateMachineBehaviour)))
                {
                    list.Add(new ScriptElement(scriptArray[i].name, scriptArray[i]));
                }
            }
            list.Add(new NewScriptElement(targets[0] is AnimatorStateMachine));
            this.m_Tree = list.ToArray();
            this.CreateSearchTree(this.m_Search);
        }

        private void CreateSearchTree(string newSearch)
        {
            char[] separator = new char[] { ' ' };
            string[] strArray = newSearch.ToLower().Split(separator);
            List<Element> collection = new List<Element>();
            List<Element> list2 = new List<Element>();
            foreach (Element element in this.m_Tree)
            {
                if (element is NewScriptElement)
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
            List<Element> list3 = new List<Element>();
            list3.AddRange(collection);
            list3.AddRange(list2);
            list3.Add(this.m_Tree[this.m_Tree.Length - 1]);
            this.m_SearchTree = list3.ToArray();
            this.selectedIndex = 0;
        }

        private void HandleKeyboard()
        {
            Event current = Event.current;
            if (current.type == EventType.KeyDown)
            {
                if (this.activeElement.IsShow())
                {
                    if ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter))
                    {
                        this.activeElement.Create();
                        current.Use();
                        GUIUtility.ExitGUI();
                    }
                    if (current.keyCode == KeyCode.Escape)
                    {
                        this.activeElement.Hide();
                        current.Use();
                    }
                }
                else
                {
                    if (current.keyCode == KeyCode.DownArrow)
                    {
                        this.selectedIndex++;
                        current.Use();
                    }
                    if (current.keyCode == KeyCode.UpArrow)
                    {
                        this.selectedIndex--;
                        current.Use();
                    }
                    if ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter))
                    {
                        this.activeElement.Create();
                        current.Use();
                    }
                    if ((this.m_Search == "") && (current.keyCode == KeyCode.Escape))
                    {
                        base.Close();
                        current.Use();
                    }
                    if ((this.m_Search == "") && (current.keyCode == KeyCode.RightArrow))
                    {
                        this.activeElement.Create();
                        current.Use();
                    }
                }
            }
        }

        private void Init(Rect rect)
        {
            rect = GUIUtility.GUIToScreenRect(rect);
            this.CreateBehaviourTree();
            base.ShowAsDropDown(rect, new Vector2(rect.width, 320f));
            base.Focus();
            base.m_Parent.AddToAuxWindowList();
            base.wantsMouseMove = true;
        }

        private void OnDisable()
        {
            s_LastClosedTime = DateTime.Now.Ticks / 0x2710L;
            s_AddComponentWindow = null;
        }

        private void OnEnable()
        {
            s_AddComponentWindow = this;
            this.m_SelectedIndex = 0;
            this.m_Search = EditorPrefs.GetString("StateMachineBehaviourSearchString", "");
            this.m_Tree = null;
            this.m_SearchTree = null;
        }

        public void OnGUI()
        {
            string str;
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, s_Styles.background);
            this.HandleKeyboard();
            GUILayout.Space(7f);
            Element activeElement = this.activeElement;
            if (!(activeElement is NewScriptElement) || !activeElement.IsShow())
            {
                EditorGUI.FocusTextInControl("StateMachineBehaviourSearch");
            }
            Rect rect = GUILayoutUtility.GetRect((float) 10f, (float) 20f);
            rect.x += 8f;
            rect.width -= 16f;
            GUI.SetNextControlName("StateMachineBehaviourSearch");
            EditorGUI.BeginChangeCheck();
            bool disabled = (activeElement is NewScriptElement) && activeElement.IsShow();
            using (new EditorGUI.DisabledScope(disabled))
            {
                str = EditorGUI.SearchField(rect, this.m_Search);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Search = str;
                EditorPrefs.SetString("StateMachineBehaviourSearchString", this.m_Search);
                this.CreateSearchTree(this.m_Search);
            }
            Rect position = base.position;
            position.x = 0f;
            position.y = 30f;
            position.height -= 30f;
            position.width -= 2f;
            GUILayout.BeginArea(position);
            GUI.Label(GUILayoutUtility.GetRect((float) 10f, (float) 25f), this.activeHeader, s_Styles.header);
            if ((this.activeElement is NewScriptElement) && this.activeElement.IsShow())
            {
                this.activeElement.OnGUI(false);
            }
            else
            {
                this.m_ScrollPosition = GUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
                for (int i = 0; i < this.activeTree.Length; i++)
                {
                    if (this.activeTree[i].OnGUI(i == this.selectedIndex))
                    {
                        this.selectedIndex = i;
                        base.Repaint();
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }

        internal static bool Show(Rect rect, AnimatorController controller, int layerIndex, UnityEngine.Object[] targets)
        {
            UnityEngine.Object[] objArray = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(AddStateMachineBehaviourComponentWindow));
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
                    s_AddComponentWindow = ScriptableObject.CreateInstance<AddStateMachineBehaviourComponentWindow>();
                }
                s_AddComponentWindow.m_Controller = controller;
                s_AddComponentWindow.m_LayerIndex = layerIndex;
                s_AddComponentWindow.m_Targets = targets;
                s_AddComponentWindow.Init(rect);
                return true;
            }
            return false;
        }

        private Element activeElement
        {
            get
            {
                Element[] activeTree = this.activeTree;
                if ((this.selectedIndex < 0) || (this.selectedIndex >= activeTree.Length))
                {
                    return null;
                }
                return activeTree[this.selectedIndex];
            }
        }

        private GUIContent activeHeader
        {
            get
            {
                if ((this.activeElement != null) && this.activeElement.IsShow())
                {
                    return this.activeElement.content;
                }
                if (this.activeTree == this.m_SearchTree)
                {
                    return s_Styles.searchContent;
                }
                return s_Styles.behaviourContent;
            }
        }

        private Element[] activeTree =>
            ((this.m_Search != "") ? this.m_SearchTree : this.m_Tree);

        internal static string className
        {
            get => 
                s_AddComponentWindow.m_ClassName;
            set
            {
                s_AddComponentWindow.m_ClassName = value;
            }
        }

        internal static AnimatorController controller =>
            s_AddComponentWindow.m_Controller;

        internal static int layerIndex =>
            s_AddComponentWindow.m_LayerIndex;

        internal static string searchName
        {
            get => 
                s_AddComponentWindow.m_Search;
            set
            {
                s_AddComponentWindow.m_Search = value;
            }
        }

        private int selectedIndex
        {
            get => 
                this.m_SelectedIndex;
            set
            {
                Element[] activeTree = this.activeTree;
                this.m_SelectedIndex = Math.Min(Math.Max(value, 0), activeTree.Length - 1);
            }
        }

        internal static UnityEngine.Object[] targets =>
            s_AddComponentWindow.m_Targets;

        private class Element : IComparable
        {
            public GUIContent content;

            public Element(string name)
            {
                this.content = new GUIContent(name);
            }

            public virtual bool CanShow() => 
                false;

            public int CompareTo(object o) => 
                this.name.CompareTo((o as AddStateMachineBehaviourComponentWindow.Element).name);

            public virtual void Create()
            {
            }

            public virtual void Hide()
            {
            }

            public virtual bool IsShow() => 
                false;

            public virtual bool OnGUI(bool selected)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                Rect position = GUILayoutUtility.GetRect(this.content, AddStateMachineBehaviourComponentWindow.s_Styles.componentButton, options);
                if (Event.current.type == EventType.Repaint)
                {
                    AddStateMachineBehaviourComponentWindow.s_Styles.componentButton.Draw(position, this.content, false, false, selected, selected);
                }
                if (Event.current.type == EventType.MouseMove)
                {
                    return position.Contains(Event.current.mousePosition);
                }
                if ((Event.current.type == EventType.MouseDown) && position.Contains(Event.current.mousePosition))
                {
                    Event.current.Use();
                    this.Create();
                    return true;
                }
                return false;
            }

            public virtual void Show()
            {
            }

            public string name =>
                this.content.text;
        }

        internal enum Language
        {
            JavaScript,
            CSharp,
            Boo
        }

        private class NewScriptElement : AddStateMachineBehaviourComponentWindow.Element
        {
            public bool isShow;
            public bool isStateMachine;
            private char[] kInvalidPathChars;
            private char[] kPathSepChars;
            private const string kResourcesTemplatePath = "Resources/ScriptTemplates";
            private string m_Directory;

            public NewScriptElement(bool stateMachine) : base("New Script")
            {
                this.kInvalidPathChars = new char[] { '<', '>', ':', '"', '|', '?', '*', '\0' };
                this.kPathSepChars = new char[] { '/', '\\' };
                this.m_Directory = string.Empty;
                this.isShow = false;
                this.isStateMachine = false;
                this.isStateMachine = stateMachine;
            }

            public bool CanCreate() => 
                ((((AddStateMachineBehaviourComponentWindow.className.Length > 0) && !File.Exists(this.TargetPath())) && (!this.ClassAlreadyExists() && !this.ClassNameIsInvalid())) && !this.InvalidTargetPath());

            public override bool CanShow() => 
                true;

            private bool ClassAlreadyExists()
            {
                if (AddStateMachineBehaviourComponentWindow.className == string.Empty)
                {
                    return false;
                }
                return this.ClassExists(AddStateMachineBehaviourComponentWindow.className);
            }

            private bool ClassExists(string className)
            {
                <ClassExists>c__AnonStorey0 storey = new <ClassExists>c__AnonStorey0 {
                    className = className
                };
                return Enumerable.Any<Assembly>(AppDomain.CurrentDomain.GetAssemblies(), new Func<Assembly, bool>(storey, (IntPtr) this.<>m__0));
            }

            private bool ClassNameIsInvalid() => 
                !CodeGenerator.IsValidLanguageIndependentIdentifier(AddStateMachineBehaviourComponentWindow.className);

            public override void Create()
            {
                if (!this.IsShow())
                {
                    this.Show();
                    AddStateMachineBehaviourComponentWindow.className = AddStateMachineBehaviourComponentWindow.searchName;
                }
                else if (this.CanCreate())
                {
                    this.CreateScript();
                    MonoScript stateMachineBehaviour = AssetDatabase.LoadAssetAtPath(this.TargetPath(), typeof(MonoScript)) as MonoScript;
                    stateMachineBehaviour.SetScriptTypeWasJustCreatedFromComponentMenu();
                    AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.AddBehaviour(stateMachineBehaviour);
                    AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.Close();
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
                if (AddStateMachineBehaviourComponentWindow.className != string.Empty)
                {
                    if (File.Exists(this.TargetPath()))
                    {
                        return ("A script called \"" + AddStateMachineBehaviourComponentWindow.className + "\" already exists at that path.");
                    }
                    if (this.ClassAlreadyExists())
                    {
                        return ("A class called \"" + AddStateMachineBehaviourComponentWindow.className + "\" already exists.");
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

            public override void Hide()
            {
                this.isShow = false;
            }

            private bool InvalidTargetPath() => 
                ((this.m_Directory.IndexOfAny(this.kInvalidPathChars) >= 0) || this.TargetDir().Split(this.kPathSepChars, StringSplitOptions.None).Contains<string>(string.Empty));

            public override bool IsShow() => 
                this.isShow;

            public override bool OnGUI(bool selected)
            {
                if (!this.IsShow())
                {
                    bool flag = base.OnGUI(selected);
                    if (Event.current.type == EventType.Repaint)
                    {
                        Rect lastRect = GUILayoutUtility.GetLastRect();
                        Rect position = new Rect((lastRect.x + lastRect.width) - 13f, lastRect.y + 4f, 13f, 13f);
                        AddStateMachineBehaviourComponentWindow.s_Styles.rightArrow.Draw(position, false, false, false, false);
                    }
                    return flag;
                }
                GUILayout.Label("Name", EditorStyles.label, new GUILayoutOption[0]);
                EditorGUI.FocusTextInControl("NewScriptName");
                GUI.SetNextControlName("NewScriptName");
                AddStateMachineBehaviourComponentWindow.className = EditorGUILayout.TextField(AddStateMachineBehaviourComponentWindow.className, new GUILayoutOption[0]);
                EditorGUILayout.Space();
                bool flag3 = this.CanCreate();
                if (!flag3 && (AddStateMachineBehaviourComponentWindow.className != ""))
                {
                    GUILayout.Label(this.GetError(), EditorStyles.helpBox, new GUILayoutOption[0]);
                }
                GUILayout.FlexibleSpace();
                using (new EditorGUI.DisabledScope(!flag3))
                {
                    if (GUILayout.Button("Create and Add", new GUILayoutOption[0]))
                    {
                        this.Create();
                    }
                }
                EditorGUILayout.Space();
                return false;
            }

            public override void Show()
            {
                this.isShow = true;
            }

            private string TargetDir() => 
                Path.Combine("Assets", this.m_Directory.Trim(this.kPathSepChars));

            public string TargetPath() => 
                Path.Combine(this.TargetDir(), AddStateMachineBehaviourComponentWindow.className + "." + this.extension);

            private string extension =>
                "cs";

            private string templatePath
            {
                get
                {
                    string str = Path.Combine(EditorApplication.applicationContentsPath, "Resources/ScriptTemplates");
                    if (this.isStateMachine)
                    {
                        return Path.Combine(str, "86-C# Script-NewSubStateMachineBehaviourScript.cs.txt");
                    }
                    return Path.Combine(str, "86-C# Script-NewStateMachineBehaviourScript.cs.txt");
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

        private class ScriptElement : AddStateMachineBehaviourComponentWindow.Element
        {
            private MonoScript script;

            public ScriptElement(string scriptName, MonoScript scriptObject) : base(scriptName)
            {
                this.script = scriptObject;
            }

            public override void Create()
            {
                AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.AddBehaviour(this.script);
                AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.Close();
            }
        }

        private class Styles
        {
            public GUIStyle background = "grey_border";
            public GUIContent behaviourContent = new GUIContent("Behaviour");
            public GUIStyle componentButton = new GUIStyle("PR Label");
            public GUIStyle header = new GUIStyle(EditorStyles.inspectorBig);
            public GUIStyle leftArrow = "AC LeftArrow";
            public GUIStyle rightArrow = "AC RightArrow";
            public GUIContent searchContent = new GUIContent("Search");

            public Styles()
            {
                this.header.font = EditorStyles.boldLabel.font;
                this.componentButton.alignment = TextAnchor.MiddleLeft;
                RectOffset padding = this.componentButton.padding;
                padding.left -= 15;
                this.componentButton.fixedHeight = 20f;
            }
        }
    }
}

