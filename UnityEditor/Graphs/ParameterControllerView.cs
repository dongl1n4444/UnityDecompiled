namespace UnityEditor.Graphs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEditorInternal;
    using UnityEngine;

    internal class ParameterControllerView : IAnimatorControllerSubEditor
    {
        private const float kElementHeight = 24f;
        private bool m_HadKeyFocusAtMouseDown = false;
        private IAnimatorControllerEditor m_Host;
        protected int m_LastSelectedIndex;
        protected ReorderableList m_ParameterList;
        protected RenameOverlay m_RenameOverlay;
        protected Vector2 m_ScrollPosition;
        private string m_Search = "";
        private int m_SearchMode;
        private Element[] m_SearchTree;
        private Element[] m_Tree;
        private static Styles s_Styles;

        private void AddParameterMenu(object value)
        {
            Undo.RegisterCompleteObjectUndo(this.m_Host.animatorController, "Parameter Added");
            UnityEngine.AnimatorControllerParameterType type = (UnityEngine.AnimatorControllerParameterType) value;
            string name = "New " + type.ToString();
            this.m_Host.animatorController.AddParameter(name, type);
            this.RebuildList();
            this.m_ParameterList.index = this.m_Tree.Length - 1;
            if (this.renameOverlay.IsRenaming())
            {
                this.RenameEnd();
            }
            this.renameOverlay.BeginRename(this.m_Host.animatorController.parameters[this.m_ParameterList.index].name, this.m_ParameterList.index, 0.1f);
        }

        private Element CreateElement(UnityEngine.AnimatorControllerParameter parameter)
        {
            switch (parameter.type)
            {
                case UnityEngine.AnimatorControllerParameterType.Float:
                    return new FloatElement(parameter, this);

                case UnityEngine.AnimatorControllerParameterType.Int:
                    return new IntElement(parameter, this);

                case UnityEngine.AnimatorControllerParameterType.Bool:
                    return new BoolElement(parameter, this);

                case UnityEngine.AnimatorControllerParameterType.Trigger:
                    return new TriggerElement(parameter, this);
            }
            return null;
        }

        protected void CreateParameterList()
        {
            List<Element> list = new List<Element>();
            if (this.m_Host.animatorController != null)
            {
                UnityEngine.AnimatorControllerParameter[] parameters = this.m_Host.animatorController.parameters;
                for (int i = 0; i < parameters.Length; i++)
                {
                    list.Add(this.CreateElement(parameters[i]));
                }
            }
            this.m_Tree = list.ToArray();
        }

        protected void CreateSearchParameterList(string newSearch, int searchMode)
        {
            char[] separator = new char[] { ' ' };
            string[] strArray = newSearch.ToLower().Split(separator);
            List<Element> collection = new List<Element>();
            List<Element> list2 = new List<Element>();
            foreach (Element element in this.m_Tree)
            {
                if (searchMode != 0)
                {
                    string str = element.m_Parameter.type.ToString();
                    SearchMode mode = (SearchMode) searchMode;
                    if (str != mode.ToString())
                    {
                        continue;
                    }
                }
                string str2 = element.name.ToLower().Replace(" ", "");
                bool flag = true;
                bool flag2 = false;
                for (int i = 0; i < strArray.Length; i++)
                {
                    string str3 = strArray[i];
                    if (str2.Contains(str3))
                    {
                        if ((i == 0) && str2.StartsWith(str3))
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
            this.m_SearchTree = list3.ToArray();
        }

        private void DeleteParameter()
        {
            this.OnRemoveParameter(this.m_ParameterList.index);
        }

        private void DoParameterList()
        {
            int index = this.m_ParameterList.index;
            this.m_ParameterList.list = this.activeTree;
            if (index >= this.activeTree.Length)
            {
                index = this.activeTree.Length - 1;
            }
            this.m_ParameterList.index = index;
            this.m_ParameterList.draggable = !this.m_Host.liveLink && (this.activeTree == this.m_Tree);
            string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
            EditorGUI.kFloatFieldFormatString = "f1";
            this.m_ScrollPosition = GUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            this.m_ParameterList.DoLayoutList();
            if (EditorGUI.EndChangeCheck() && !this.m_Host.liveLink)
            {
                Element[] tree = this.m_Tree;
                UnityEngine.AnimatorControllerParameter[] parameterArray = new UnityEngine.AnimatorControllerParameter[tree.Length];
                for (int i = 0; i < tree.Length; i++)
                {
                    parameterArray[i] = tree[i].m_Parameter;
                }
                this.m_Host.animatorController.parameters = parameterArray;
            }
            GUILayout.EndScrollView();
            EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
        }

        public void GrabKeyboardFocus()
        {
            this.m_ParameterList.GrabKeyboardFocus();
            this.m_Host.Repaint();
        }

        public bool HasKeyboardControl() => 
            this.m_ParameterList.HasKeyboardControl();

        public void Init(IAnimatorControllerEditor host)
        {
            this.m_Host = host;
        }

        private void KeyboardHandling()
        {
            if ((this.m_ParameterList != null) && this.m_ParameterList.HasKeyboardControl())
            {
                Event current = Event.current;
                switch (current.type)
                {
                    case EventType.ExecuteCommand:
                        if ((current.commandName == "SoftDelete") || (current.commandName == "Delete"))
                        {
                            current.Use();
                            this.OnRemoveParameter(this.m_ParameterList.index);
                        }
                        break;

                    case EventType.KeyDown:
                    {
                        KeyCode keyCode = Event.current.keyCode;
                        if (keyCode != KeyCode.Home)
                        {
                            if (keyCode == KeyCode.End)
                            {
                                current.Use();
                                this.m_ParameterList.index = this.m_ParameterList.count - 1;
                                break;
                            }
                            if (keyCode == KeyCode.Delete)
                            {
                                current.Use();
                                this.OnRemoveParameter(this.m_ParameterList.index);
                                break;
                            }
                        }
                        else
                        {
                            current.Use();
                            this.m_ParameterList.index = 0;
                        }
                        break;
                    }
                }
            }
        }

        private void OnAddParameter(Rect buttonRect)
        {
            GenericMenu menu = new GenericMenu();
            IEnumerator enumerator = Enum.GetValues(typeof(UnityEngine.AnimatorControllerParameterType)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    menu.AddItem(new GUIContent(current.ToString()), false, new GenericMenu.MenuFunction2(this.AddParameterMenu), current);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            menu.DropDown(buttonRect);
        }

        public void OnDestroy()
        {
        }

        public void OnDisable()
        {
            if (this.renameOverlay != null)
            {
                if (this.renameOverlay.IsRenaming())
                {
                    this.RenameEnd();
                }
                Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            }
        }

        private void OnDrawBackgroundParameter(Rect rect, int index, bool selected, bool focused)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIStyle style = ((index % 2) != 0) ? s_Styles.evenBackground : s_Styles.oddBackground;
                ((!selected && !focused) ? style : s_Styles.elementBackground).Draw(rect, false, selected, selected, focused);
            }
        }

        private void OnDrawParameter(Rect rect, int index, bool selected, bool focused)
        {
            Event current = Event.current;
            if (((current.type == EventType.MouseUp) && (current.button == 1)) && rect.Contains(current.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction(this.DeleteParameter));
                menu.ShowAsContext();
                Event.current.Use();
            }
            if (index < this.m_ParameterList.list.Count)
            {
                Element element = this.m_ParameterList.list[index] as Element;
                rect.yMin += 2f;
                rect.yMax -= 3f;
                element.OnGUI(rect, index);
            }
        }

        public void OnEnable()
        {
            this.m_Search = "";
            this.m_Tree = null;
            this.m_SearchTree = null;
            this.m_SearchMode = 0;
            this.m_ScrollPosition = new Vector2(0f, 0f);
            this.m_RenameOverlay = new RenameOverlay();
            this.m_LastSelectedIndex = -1;
            this.m_ParameterList = new ReorderableList(this.m_Tree, typeof(Element), true, false, false, false);
            this.m_ParameterList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.OnReorderParameter);
            this.m_ParameterList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.OnDrawParameter);
            this.m_ParameterList.drawElementBackgroundCallback = new ReorderableList.ElementCallbackDelegate(this.OnDrawBackgroundParameter);
            this.m_ParameterList.onMouseUpCallback = new ReorderableList.SelectCallbackDelegate(this.OnMouseUpParameter);
            this.m_ParameterList.index = 0;
            this.m_ParameterList.headerHeight = 0f;
            this.m_ParameterList.elementHeight = 24f;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public void OnEvent()
        {
            if (this.renameOverlay != null)
            {
                this.renameOverlay.OnEvent();
            }
        }

        public void OnFocus()
        {
        }

        public void OnGUI(Rect rect)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            Event current = Event.current;
            if ((current.type == EventType.MouseDown) && rect.Contains(current.mousePosition))
            {
                this.m_HadKeyFocusAtMouseDown = this.m_ParameterList.HasKeyboardControl();
            }
            this.KeyboardHandling();
            if (this.m_Host.animatorController != null)
            {
                this.DoParameterList();
            }
        }

        public void OnLostFocus()
        {
            if ((this.renameOverlay != null) && this.renameOverlay.IsRenaming())
            {
                this.renameOverlay.EndRename(true);
                this.RenameEnd();
            }
        }

        private void OnMouseUpParameter(ReorderableList list)
        {
            if ((!this.m_Host.liveLink && this.m_HadKeyFocusAtMouseDown) && ((list.index == this.m_LastSelectedIndex) && (Event.current.button == 0)))
            {
                Element element = list.list[list.index] as Element;
                this.renameOverlay.BeginRename(element.name, list.index, 0.1f);
            }
            this.m_LastSelectedIndex = list.index;
        }

        private void OnRemoveParameter(int index)
        {
            if (!this.m_Host.liveLink && (this.m_ParameterList.list.Count > 0))
            {
                Element element = this.m_ParameterList.list[index] as Element;
                List<UnityEngine.Object> list = this.m_Host.animatorController.CollectObjectsUsingParameter(element.name).ToList<UnityEngine.Object>();
                bool flag = false;
                if (list.Count > 0)
                {
                    string title = "Delete parameter " + element.name + "?";
                    string message = "It is used by : \n";
                    foreach (UnityEngine.Object obj2 in list)
                    {
                        AnimatorTransitionBase base2 = obj2 as AnimatorTransitionBase;
                        if ((base2 != null) && (base2.destinationState != null))
                        {
                            message = message + "Transition to " + base2.destinationState.name + "\n";
                        }
                        else if ((base2 != null) && (base2.destinationStateMachine != null))
                        {
                            message = message + "Transition to " + base2.destinationStateMachine.name + "\n";
                        }
                        else
                        {
                            message = message + obj2.name + "\n";
                        }
                    }
                    if (EditorUtility.DisplayDialog(title, message, "Delete", "Cancel"))
                    {
                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    this.ResetTextFields();
                    list.Add(this.m_Host.animatorController);
                    Undo.RegisterCompleteObjectUndo(list.ToArray(), "Parameter removed");
                    this.m_Host.animatorController.RemoveParameter(element.m_Parameter);
                    this.RebuildList();
                    this.m_ParameterList.GrabKeyboardFocus();
                }
            }
        }

        private void OnReorderParameter(ReorderableList reorderablelist)
        {
            Element[] array = new Element[reorderablelist.list.Count];
            reorderablelist.list.CopyTo(array, 0);
            UnityEngine.AnimatorControllerParameter[] parameterArray = new UnityEngine.AnimatorControllerParameter[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                if ((array[i] == null) || (array[i].m_Parameter == null))
                {
                    return;
                }
                parameterArray[i] = array[i].m_Parameter;
            }
            Undo.RegisterCompleteObjectUndo(this.m_Host.animatorController, "Parameter reordering");
            this.m_Host.animatorController.parameters = parameterArray;
            this.RebuildList();
        }

        public void OnToolbarGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            if (this.m_Host != null)
            {
                using (new EditorGUI.DisabledScope(this.m_Host.animatorController == null))
                {
                    string[] names = Enum.GetNames(typeof(SearchMode));
                    int searchMode = this.m_SearchMode;
                    GUI.SetNextControlName("ParameterSearch");
                    if (((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape)) && (GUI.GetNameOfFocusedControl() == "ParameterSearch"))
                    {
                        this.m_Search = "";
                        this.CreateSearchParameterList(this.m_Search, this.m_SearchMode);
                    }
                    EditorGUI.BeginChangeCheck();
                    string str = EditorGUILayout.ToolbarSearchField(this.m_Search, names, ref searchMode, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.m_Search = str;
                        this.m_SearchMode = searchMode;
                        this.CreateSearchParameterList(this.m_Search, this.m_SearchMode);
                    }
                    GUILayout.Space(10f);
                    using (new EditorGUI.DisabledScope(this.m_Host.liveLink))
                    {
                        Rect position = GUILayoutUtility.GetRect(s_Styles.iconToolbarPlusMore, s_Styles.invisibleButton);
                        if (GUI.Button(position, s_Styles.iconToolbarPlusMore, s_Styles.invisibleButton))
                        {
                            this.OnAddParameter(position);
                        }
                    }
                }
            }
        }

        protected void RebuildList()
        {
            this.CreateParameterList();
            this.CreateSearchParameterList(this.m_Search, this.m_SearchMode);
        }

        public void ReleaseKeyboardFocus()
        {
            this.m_ParameterList.ReleaseKeyboardFocus();
            this.m_Host.Repaint();
        }

        protected void RenameEnd()
        {
            if (this.renameOverlay.userAcceptedRename)
            {
                string name = !string.IsNullOrEmpty(this.renameOverlay.name) ? this.renameOverlay.name : this.renameOverlay.originalName;
                if (name != this.renameOverlay.originalName)
                {
                    int userData = this.renameOverlay.userData;
                    Element element = this.m_ParameterList.list[userData] as Element;
                    name = this.m_Host.animatorController.MakeUniqueParameterName(name);
                    Undo.RegisterCompleteObjectUndo(this.m_Host.animatorController, "Parameter renamed");
                    this.m_Host.animatorController.RenameParameter(element.name, name);
                    element.name = name;
                    UnityEngine.AnimatorControllerParameter[] parameters = this.m_Host.animatorController.parameters;
                    parameters[userData] = element.m_Parameter;
                    this.m_Host.animatorController.parameters = parameters;
                }
            }
            this.m_ParameterList.GrabKeyboardFocus();
            this.renameOverlay.Clear();
        }

        private void ResetTextFields()
        {
            EditorGUI.EndEditingActiveTextField();
            GUIUtility.keyboardControl = 0;
        }

        public void ResetUI()
        {
            this.RebuildList();
        }

        public void UndoRedoPerformed()
        {
            this.RebuildList();
            this.m_Host.Repaint();
        }

        private Element[] activeTree
        {
            get
            {
                if (this.m_Tree == null)
                {
                    this.CreateParameterList();
                }
                return (((this.m_Search != "") || (this.m_SearchMode != 0)) ? this.m_SearchTree : this.m_Tree);
            }
        }

        public RenameOverlay renameOverlay =>
            this.m_RenameOverlay;

        private class BoolElement : ParameterControllerView.Element
        {
            public BoolElement(UnityEngine.AnimatorControllerParameter parameter, ParameterControllerView host) : base(parameter, host)
            {
            }

            public override void OnSpecializedGUI(Rect rect)
            {
                EditorGUI.BeginChangeCheck();
                bool flag = GUI.Toggle(rect, this.value, "");
                if (EditorGUI.EndChangeCheck())
                {
                    this.value = flag;
                }
            }

            public bool value
            {
                get
                {
                    if (base.m_Host.m_Host.liveLink)
                    {
                        return base.m_Host.m_Host.previewAnimator.GetBool(base.name);
                    }
                    return base.m_Parameter.defaultBool;
                }
                set
                {
                    if (base.m_Host.m_Host.liveLink)
                    {
                        base.m_Host.m_Host.previewAnimator.SetBool(base.name, value);
                    }
                    else
                    {
                        Undo.RegisterCompleteObjectUndo(base.m_Host.m_Host.animatorController, "Parameter default value changed");
                        base.m_Parameter.defaultBool = value;
                    }
                }
            }
        }

        private class Element : IComparable
        {
            protected const float kParameterValueWidth = 48f;
            protected const float kSpacing = 2f;
            public ParameterControllerView m_Host;
            public UnityEngine.AnimatorControllerParameter m_Parameter;

            public Element(UnityEngine.AnimatorControllerParameter parameter, ParameterControllerView host)
            {
                this.m_Host = host;
                this.m_Parameter = parameter;
            }

            public int CompareTo(object o) => 
                this.name.CompareTo((o as ParameterControllerView.Element).name);

            public virtual void OnGUI(Rect rect, int index)
            {
                Rect rect2 = new Rect((rect.xMax - 48f) - 2f, rect.y, 48f, rect.height);
                Rect position = Rect.MinMaxRect(rect.x, rect.yMin, rect2.xMin - 2f, rect.yMax);
                if ((this.m_Host.renameOverlay.IsRenaming() && (this.m_Host.renameOverlay.userData == index)) && !this.m_Host.renameOverlay.isWaitingForDelay)
                {
                    if ((position.width >= 0f) && (position.height >= 0f))
                    {
                        position.x -= 2f;
                        this.m_Host.renameOverlay.editFieldRect = position;
                    }
                    if (!this.m_Host.renameOverlay.OnGUI())
                    {
                        this.m_Host.RenameEnd();
                    }
                }
                else
                {
                    GUI.Label(position, this.name);
                }
                bool flag2 = this.m_Host.m_Host.liveLink && this.m_Host.m_Host.previewAnimator.IsParameterControlledByCurve(this.name);
                using (new EditorGUI.DisabledScope(this.m_Host.m_Host.liveLink && flag2))
                {
                    this.OnSpecializedGUI(rect2);
                }
            }

            public virtual void OnSpecializedGUI(Rect rect)
            {
            }

            public string name
            {
                get => 
                    this.m_Parameter.name;
                set
                {
                    this.m_Parameter.name = value;
                }
            }
        }

        private class FloatElement : ParameterControllerView.Element
        {
            public FloatElement(UnityEngine.AnimatorControllerParameter parameter, ParameterControllerView host) : base(parameter, host)
            {
            }

            public override void OnSpecializedGUI(Rect rect)
            {
                EditorGUI.BeginChangeCheck();
                float num = EditorGUI.FloatField(rect, this.value);
                if (EditorGUI.EndChangeCheck())
                {
                    this.value = num;
                }
            }

            public float value
            {
                get
                {
                    if (base.m_Host.m_Host.liveLink)
                    {
                        return base.m_Host.m_Host.previewAnimator.GetFloat(base.name);
                    }
                    return base.m_Parameter.defaultFloat;
                }
                set
                {
                    if (base.m_Host.m_Host.liveLink)
                    {
                        base.m_Host.m_Host.previewAnimator.SetFloat(base.name, value);
                    }
                    else
                    {
                        Undo.RegisterCompleteObjectUndo(base.m_Host.m_Host.animatorController, "Parameter default value changed");
                        base.m_Parameter.defaultFloat = value;
                    }
                }
            }
        }

        private class IntElement : ParameterControllerView.Element
        {
            public IntElement(UnityEngine.AnimatorControllerParameter parameter, ParameterControllerView host) : base(parameter, host)
            {
            }

            public override void OnSpecializedGUI(Rect rect)
            {
                EditorGUI.BeginChangeCheck();
                int num = EditorGUI.IntField(rect, this.value);
                if (EditorGUI.EndChangeCheck())
                {
                    this.value = num;
                }
            }

            public int value
            {
                get
                {
                    if (base.m_Host.m_Host.liveLink)
                    {
                        return base.m_Host.m_Host.previewAnimator.GetInteger(base.name);
                    }
                    return base.m_Parameter.defaultInt;
                }
                set
                {
                    if (base.m_Host.m_Host.liveLink)
                    {
                        base.m_Host.m_Host.previewAnimator.SetInteger(base.name, value);
                    }
                    else
                    {
                        Undo.RegisterCompleteObjectUndo(base.m_Host.m_Host.animatorController, "Parameter default value changed");
                        base.m_Parameter.defaultInt = value;
                    }
                }
            }
        }

        private enum SearchMode
        {
            Name,
            Float,
            Int,
            Bool,
            Trigger
        }

        private class Styles
        {
            public readonly GUIStyle elementBackground = new GUIStyle("RL Element");
            public readonly GUIStyle evenBackground = "CN EntryBackEven";
            public GUIContent iconToolbarPlusMore = EditorGUIUtility.IconContent("Toolbar Plus More", "Choose to add a new parameter");
            public readonly GUIStyle invisibleButton = "InvisibleButton";
            public readonly GUIStyle oddBackground = "CN EntryBackodd";
            public GUIContent searchContent = new GUIContent("Search");
            public readonly GUIStyle triggerButton = "Radio";
        }

        private class TriggerElement : ParameterControllerView.BoolElement
        {
            public TriggerElement(UnityEngine.AnimatorControllerParameter parameter, ParameterControllerView host) : base(parameter, host)
            {
            }

            public override void OnSpecializedGUI(Rect rect)
            {
                EditorGUI.BeginChangeCheck();
                bool flag = GUI.Toggle(rect, base.value, "", ParameterControllerView.s_Styles.triggerButton);
                if (EditorGUI.EndChangeCheck())
                {
                    base.value = flag;
                }
            }
        }
    }
}

