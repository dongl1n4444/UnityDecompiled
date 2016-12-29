namespace UnityEngine.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI.CoroutineTween;

    /// <summary>
    /// <para>A standard dropdown that presents a list of options when clicked, of which one can be chosen.</para>
    /// </summary>
    [AddComponentMenu("UI/Dropdown", 0x23), RequireComponent(typeof(RectTransform))]
    public class Dropdown : Selectable, IPointerClickHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
    {
        private TweenRunner<FloatTween> m_AlphaTweenRunner;
        private GameObject m_Blocker;
        [SerializeField]
        private Image m_CaptionImage;
        [SerializeField]
        private Text m_CaptionText;
        private GameObject m_Dropdown;
        [SerializeField]
        private Image m_ItemImage;
        private List<DropdownItem> m_Items = new List<DropdownItem>();
        [Space, SerializeField]
        private Text m_ItemText;
        [Space, SerializeField]
        private DropdownEvent m_OnValueChanged = new DropdownEvent();
        [Space, SerializeField]
        private OptionDataList m_Options = new OptionDataList();
        [SerializeField]
        private RectTransform m_Template;
        [Space, SerializeField]
        private int m_Value;
        private static OptionData s_NoOptionData = new OptionData();
        private bool validTemplate = false;

        protected Dropdown()
        {
        }

        private DropdownItem AddItem(OptionData data, bool selected, DropdownItem itemTemplate, List<DropdownItem> items)
        {
            DropdownItem item = this.CreateItem(itemTemplate);
            item.rectTransform.SetParent(itemTemplate.rectTransform.parent, false);
            item.gameObject.SetActive(true);
            item.gameObject.name = "Item " + items.Count + ((data.text == null) ? "" : (": " + data.text));
            if (item.toggle != null)
            {
                item.toggle.isOn = false;
            }
            if (item.text != null)
            {
                item.text.text = data.text;
            }
            if (item.image != null)
            {
                item.image.sprite = data.image;
                item.image.enabled = item.image.sprite != null;
            }
            items.Add(item);
            return item;
        }

        public void AddOptions(List<string> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                this.options.Add(new OptionData(options[i]));
            }
            this.RefreshShownValue();
        }

        public void AddOptions(List<Sprite> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                this.options.Add(new OptionData(options[i]));
            }
            this.RefreshShownValue();
        }

        public void AddOptions(List<OptionData> options)
        {
            this.options.AddRange(options);
            this.RefreshShownValue();
        }

        private void AlphaFadeList(float duration, float alpha)
        {
            CanvasGroup component = this.m_Dropdown.GetComponent<CanvasGroup>();
            this.AlphaFadeList(duration, component.alpha, alpha);
        }

        private void AlphaFadeList(float duration, float start, float end)
        {
            if (!end.Equals(start))
            {
                FloatTween info = new FloatTween {
                    duration = duration,
                    startValue = start,
                    targetValue = end
                };
                info.AddOnChangedCallback(new UnityAction<float>(this.SetAlpha));
                info.ignoreTimeScale = true;
                this.m_AlphaTweenRunner.StartTween(info);
            }
        }

        protected override void Awake()
        {
            if (Application.isPlaying)
            {
                this.m_AlphaTweenRunner = new TweenRunner<FloatTween>();
                this.m_AlphaTweenRunner.Init(this);
                if (this.m_CaptionImage != null)
                {
                    this.m_CaptionImage.enabled = this.m_CaptionImage.sprite != null;
                }
                if (this.m_Template != null)
                {
                    this.m_Template.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// <para>Clear the list of options in the Dropdown.</para>
        /// </summary>
        public void ClearOptions()
        {
            this.options.Clear();
            this.RefreshShownValue();
        }

        /// <summary>
        /// <para>Override this method to implement a different way to obtain a blocker GameObject.</para>
        /// </summary>
        /// <param name="rootCanvas">The root canvas the dropdown is under.</param>
        /// <returns>
        /// <para>The obtained blocker.</para>
        /// </returns>
        protected virtual GameObject CreateBlocker(Canvas rootCanvas)
        {
            GameObject obj2 = new GameObject("Blocker");
            RectTransform transform = obj2.AddComponent<RectTransform>();
            transform.SetParent(rootCanvas.transform, false);
            transform.anchorMin = Vector3.zero;
            transform.anchorMax = Vector3.one;
            transform.sizeDelta = Vector2.zero;
            Canvas canvas = obj2.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            Canvas component = this.m_Dropdown.GetComponent<Canvas>();
            canvas.sortingLayerID = component.sortingLayerID;
            canvas.sortingOrder = component.sortingOrder - 1;
            obj2.AddComponent<GraphicRaycaster>();
            obj2.AddComponent<Image>().color = Color.clear;
            obj2.AddComponent<Button>().onClick.AddListener(new UnityAction(this.Hide));
            return obj2;
        }

        /// <summary>
        /// <para>Override this method to implement a different way to obtain a dropdown list GameObject.</para>
        /// </summary>
        /// <param name="template">The template to create the dropdown list from.</param>
        /// <returns>
        /// <para>The obtained dropdown list.</para>
        /// </returns>
        protected virtual GameObject CreateDropdownList(GameObject template) => 
            UnityEngine.Object.Instantiate<GameObject>(template);

        protected virtual DropdownItem CreateItem(DropdownItem itemTemplate) => 
            UnityEngine.Object.Instantiate<DropdownItem>(itemTemplate);

        [DebuggerHidden]
        private IEnumerator DelayedDestroyDropdownList(float delay) => 
            new <DelayedDestroyDropdownList>c__Iterator0 { 
                delay = delay,
                $this = this
            };

        /// <summary>
        /// <para>Override this method to implement a different way to dispose of a blocker GameObject that blocks clicks to other controls while the dropdown list is open.</para>
        /// </summary>
        /// <param name="blocker">The blocker to dispose of.</param>
        protected virtual void DestroyBlocker(GameObject blocker)
        {
            UnityEngine.Object.Destroy(blocker);
        }

        /// <summary>
        /// <para>Override this method to implement a different way to dispose of a dropdown list GameObject.</para>
        /// </summary>
        /// <param name="dropdownList">The dropdown list to dispose of.</param>
        protected virtual void DestroyDropdownList(GameObject dropdownList)
        {
            UnityEngine.Object.Destroy(dropdownList);
        }

        protected virtual void DestroyItem(DropdownItem item)
        {
        }

        private static T GetOrAddComponent<T>(GameObject go) where T: Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// <para>Hide the dropdown list.</para>
        /// </summary>
        public void Hide()
        {
            if (this.m_Dropdown != null)
            {
                this.AlphaFadeList(0.15f, 0f);
                if (this.IsActive())
                {
                    base.StartCoroutine(this.DelayedDestroyDropdownList(0.15f));
                }
            }
            if (this.m_Blocker != null)
            {
                this.DestroyBlocker(this.m_Blocker);
            }
            this.m_Blocker = null;
            this.Select();
        }

        /// <summary>
        /// <para>Called by a BaseInputModule when a Cancel event occurs.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnCancel(BaseEventData eventData)
        {
            this.Hide();
        }

        /// <summary>
        /// <para>Handling for when the dropdown is 'clicked'.</para>
        /// </summary>
        /// <param name="eventData">Current event.</param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            this.Show();
        }

        private void OnSelectItem(Toggle toggle)
        {
            if (!toggle.isOn)
            {
                toggle.isOn = true;
            }
            int num = -1;
            Transform transform = toggle.transform;
            Transform parent = transform.parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i) == transform)
                {
                    num = i - 1;
                    break;
                }
            }
            if (num >= 0)
            {
                this.value = num;
                this.Hide();
            }
        }

        /// <summary>
        /// <para>What to do when the event system sends a submit Event.</para>
        /// </summary>
        /// <param name="eventData">Current event.</param>
        public virtual void OnSubmit(BaseEventData eventData)
        {
            this.Show();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (this.IsActive())
            {
                this.RefreshShownValue();
            }
        }

        /// <summary>
        /// <para>Refreshes the text and image (if available) of the currently selected option.
        /// 
        /// If you have modified the list of options, you should call this method afterwards to ensure that the visual state of the dropdown corresponds to the updated options.</para>
        /// </summary>
        public void RefreshShownValue()
        {
            OptionData data = s_NoOptionData;
            if (this.options.Count > 0)
            {
                data = this.options[Mathf.Clamp(this.m_Value, 0, this.options.Count - 1)];
            }
            if (this.m_CaptionText != null)
            {
                if ((data != null) && (data.text != null))
                {
                    this.m_CaptionText.text = data.text;
                }
                else
                {
                    this.m_CaptionText.text = "";
                }
            }
            if (this.m_CaptionImage != null)
            {
                if (data != null)
                {
                    this.m_CaptionImage.sprite = data.image;
                }
                else
                {
                    this.m_CaptionImage.sprite = null;
                }
                this.m_CaptionImage.enabled = this.m_CaptionImage.sprite != null;
            }
        }

        private void SetAlpha(float alpha)
        {
            if (this.m_Dropdown != null)
            {
                this.m_Dropdown.GetComponent<CanvasGroup>().alpha = alpha;
            }
        }

        private void SetupTemplate()
        {
            this.validTemplate = false;
            if (this.m_Template == null)
            {
                UnityEngine.Debug.LogError("The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", this);
            }
            else
            {
                GameObject gameObject = this.m_Template.gameObject;
                gameObject.SetActive(true);
                Toggle componentInChildren = this.m_Template.GetComponentInChildren<Toggle>();
                this.validTemplate = true;
                if ((componentInChildren == null) || (componentInChildren.transform == this.template))
                {
                    this.validTemplate = false;
                    UnityEngine.Debug.LogError("The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", this.template);
                }
                else if (componentInChildren.transform.parent is RectTransform)
                {
                    if ((this.itemText != null) && !this.itemText.transform.IsChildOf(componentInChildren.transform))
                    {
                        this.validTemplate = false;
                        UnityEngine.Debug.LogError("The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", this.template);
                    }
                    else if ((this.itemImage != null) && !this.itemImage.transform.IsChildOf(componentInChildren.transform))
                    {
                        this.validTemplate = false;
                        UnityEngine.Debug.LogError("The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", this.template);
                    }
                }
                else
                {
                    this.validTemplate = false;
                    UnityEngine.Debug.LogError("The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", this.template);
                }
                if (!this.validTemplate)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    DropdownItem item = componentInChildren.gameObject.AddComponent<DropdownItem>();
                    item.text = this.m_ItemText;
                    item.image = this.m_ItemImage;
                    item.toggle = componentInChildren;
                    item.rectTransform = (RectTransform) componentInChildren.transform;
                    Canvas orAddComponent = GetOrAddComponent<Canvas>(gameObject);
                    orAddComponent.overrideSorting = true;
                    orAddComponent.sortingOrder = 0x7530;
                    GetOrAddComponent<GraphicRaycaster>(gameObject);
                    GetOrAddComponent<CanvasGroup>(gameObject);
                    gameObject.SetActive(false);
                    this.validTemplate = true;
                }
            }
        }

        /// <summary>
        /// <para>Show the dropdown list.</para>
        /// </summary>
        public void Show()
        {
            if ((this.IsActive() && this.IsInteractable()) && (this.m_Dropdown == null))
            {
                if (!this.validTemplate)
                {
                    this.SetupTemplate();
                    if (!this.validTemplate)
                    {
                        return;
                    }
                }
                List<Canvas> results = ListPool<Canvas>.Get();
                base.gameObject.GetComponentsInParent<Canvas>(false, results);
                if (results.Count != 0)
                {
                    Canvas rootCanvas = results[0];
                    ListPool<Canvas>.Release(results);
                    this.m_Template.gameObject.SetActive(true);
                    this.m_Dropdown = this.CreateDropdownList(this.m_Template.gameObject);
                    this.m_Dropdown.name = "Dropdown List";
                    this.m_Dropdown.SetActive(true);
                    RectTransform transform = this.m_Dropdown.transform as RectTransform;
                    transform.SetParent(this.m_Template.transform.parent, false);
                    DropdownItem componentInChildren = this.m_Dropdown.GetComponentInChildren<DropdownItem>();
                    RectTransform transform2 = componentInChildren.rectTransform.parent.gameObject.transform as RectTransform;
                    componentInChildren.rectTransform.gameObject.SetActive(true);
                    Rect rect = transform2.rect;
                    Rect rect2 = componentInChildren.rectTransform.rect;
                    Vector2 vector = (rect2.min - rect.min) + componentInChildren.rectTransform.localPosition;
                    Vector2 vector2 = (rect2.max - rect.max) + componentInChildren.rectTransform.localPosition;
                    Vector2 size = rect2.size;
                    this.m_Items.Clear();
                    Toggle toggle = null;
                    for (int i = 0; i < this.options.Count; i++)
                    {
                        <Show>c__AnonStorey1 storey = new <Show>c__AnonStorey1 {
                            $this = this
                        };
                        OptionData data = this.options[i];
                        storey.item = this.AddItem(data, this.value == i, componentInChildren, this.m_Items);
                        if (storey.item != null)
                        {
                            storey.item.toggle.isOn = this.value == i;
                            storey.item.toggle.onValueChanged.AddListener(new UnityAction<bool>(storey.<>m__0));
                            if (storey.item.toggle.isOn)
                            {
                                storey.item.toggle.Select();
                            }
                            if (toggle != null)
                            {
                                Navigation navigation = toggle.navigation;
                                Navigation navigation2 = storey.item.toggle.navigation;
                                navigation.mode = Navigation.Mode.Explicit;
                                navigation2.mode = Navigation.Mode.Explicit;
                                navigation.selectOnDown = storey.item.toggle;
                                navigation.selectOnRight = storey.item.toggle;
                                navigation2.selectOnLeft = toggle;
                                navigation2.selectOnUp = toggle;
                                toggle.navigation = navigation;
                                storey.item.toggle.navigation = navigation2;
                            }
                            toggle = storey.item.toggle;
                        }
                    }
                    Vector2 sizeDelta = transform2.sizeDelta;
                    sizeDelta.y = ((size.y * this.m_Items.Count) + vector.y) - vector2.y;
                    transform2.sizeDelta = sizeDelta;
                    float num2 = transform.rect.height - transform2.rect.height;
                    if (num2 > 0f)
                    {
                        transform.sizeDelta = new Vector2(transform.sizeDelta.x, transform.sizeDelta.y - num2);
                    }
                    Vector3[] fourCornersArray = new Vector3[4];
                    transform.GetWorldCorners(fourCornersArray);
                    RectTransform transform3 = rootCanvas.transform as RectTransform;
                    Rect rect5 = transform3.rect;
                    for (int j = 0; j < 2; j++)
                    {
                        bool flag = false;
                        for (int m = 0; m < 4; m++)
                        {
                            Vector3 vector7 = transform3.InverseTransformPoint(fourCornersArray[m]);
                            if ((vector7[j] < rect5.min[j]) || (vector7[j] > rect5.max[j]))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            RectTransformUtility.FlipLayoutOnAxis(transform, j, false, false);
                        }
                    }
                    for (int k = 0; k < this.m_Items.Count; k++)
                    {
                        RectTransform rectTransform = this.m_Items[k].rectTransform;
                        rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 0f);
                        rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 0f);
                        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, (vector.y + (size.y * ((this.m_Items.Count - 1) - k))) + (size.y * rectTransform.pivot.y));
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, size.y);
                    }
                    this.AlphaFadeList(0.15f, 0f, 1f);
                    this.m_Template.gameObject.SetActive(false);
                    componentInChildren.gameObject.SetActive(false);
                    this.m_Blocker = this.CreateBlocker(rootCanvas);
                }
            }
        }

        /// <summary>
        /// <para>The Image component to hold the image of the currently selected option.</para>
        /// </summary>
        public Image captionImage
        {
            get => 
                this.m_CaptionImage;
            set
            {
                this.m_CaptionImage = value;
                this.RefreshShownValue();
            }
        }

        /// <summary>
        /// <para>The Text component to hold the text of the currently selected option.</para>
        /// </summary>
        public Text captionText
        {
            get => 
                this.m_CaptionText;
            set
            {
                this.m_CaptionText = value;
                this.RefreshShownValue();
            }
        }

        /// <summary>
        /// <para>The Image component to hold the image of the item.</para>
        /// </summary>
        public Image itemImage
        {
            get => 
                this.m_ItemImage;
            set
            {
                this.m_ItemImage = value;
                this.RefreshShownValue();
            }
        }

        /// <summary>
        /// <para>The Text component to hold the text of the item.</para>
        /// </summary>
        public Text itemText
        {
            get => 
                this.m_ItemText;
            set
            {
                this.m_ItemText = value;
                this.RefreshShownValue();
            }
        }

        /// <summary>
        /// <para>A UnityEvent that is invoked when when a user has clicked one of the options in the dropdown list.</para>
        /// </summary>
        public DropdownEvent onValueChanged
        {
            get => 
                this.m_OnValueChanged;
            set
            {
                this.m_OnValueChanged = value;
            }
        }

        /// <summary>
        /// <para>The list of possible options. A text string and an image can be specified for each option.</para>
        /// </summary>
        public List<OptionData> options
        {
            get => 
                this.m_Options.options;
            set
            {
                this.m_Options.options = value;
                this.RefreshShownValue();
            }
        }

        /// <summary>
        /// <para>The Rect Transform of the template for the dropdown list.</para>
        /// </summary>
        public RectTransform template
        {
            get => 
                this.m_Template;
            set
            {
                this.m_Template = value;
                this.RefreshShownValue();
            }
        }

        /// <summary>
        /// <para>The index of the currently selected option. 0 is the first option, 1 is the second, and so on.</para>
        /// </summary>
        public int value
        {
            get => 
                this.m_Value;
            set
            {
                if (!Application.isPlaying || ((value != this.m_Value) && (this.options.Count != 0)))
                {
                    this.m_Value = Mathf.Clamp(value, 0, this.options.Count - 1);
                    this.RefreshShownValue();
                    this.m_OnValueChanged.Invoke(this.m_Value);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DelayedDestroyDropdownList>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal Dropdown $this;
            internal float delay;

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
                        this.$current = new WaitForSecondsRealtime(this.delay);
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                        for (int i = 0; i < this.$this.m_Items.Count; i++)
                        {
                            if (this.$this.m_Items[i] != null)
                            {
                                this.$this.DestroyItem(this.$this.m_Items[i]);
                            }
                        }
                        this.$this.m_Items.Clear();
                        if (this.$this.m_Dropdown != null)
                        {
                            this.$this.DestroyDropdownList(this.$this.m_Dropdown);
                        }
                        this.$this.m_Dropdown = null;
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }

        [CompilerGenerated]
        private sealed class <Show>c__AnonStorey1
        {
            internal Dropdown $this;
            internal Dropdown.DropdownItem item;

            internal void <>m__0(bool x)
            {
                this.$this.OnSelectItem(this.item.toggle);
            }
        }

        /// <summary>
        /// <para>UnityEvent callback for when a dropdown current option is changed.</para>
        /// </summary>
        [Serializable]
        public class DropdownEvent : UnityEvent<int>
        {
        }

        internal protected class DropdownItem : MonoBehaviour, IPointerEnterHandler, ICancelHandler, IEventSystemHandler
        {
            [SerializeField]
            private Image m_Image;
            [SerializeField]
            private RectTransform m_RectTransform;
            [SerializeField]
            private Text m_Text;
            [SerializeField]
            private Toggle m_Toggle;

            public virtual void OnCancel(BaseEventData eventData)
            {
                Dropdown componentInParent = base.GetComponentInParent<Dropdown>();
                if (componentInParent != null)
                {
                    componentInParent.Hide();
                }
            }

            public virtual void OnPointerEnter(PointerEventData eventData)
            {
                EventSystem.current.SetSelectedGameObject(base.gameObject);
            }

            public Image image
            {
                get => 
                    this.m_Image;
                set
                {
                    this.m_Image = value;
                }
            }

            public RectTransform rectTransform
            {
                get => 
                    this.m_RectTransform;
                set
                {
                    this.m_RectTransform = value;
                }
            }

            public Text text
            {
                get => 
                    this.m_Text;
                set
                {
                    this.m_Text = value;
                }
            }

            public Toggle toggle
            {
                get => 
                    this.m_Toggle;
                set
                {
                    this.m_Toggle = value;
                }
            }
        }

        /// <summary>
        /// <para>Class to store the text and/or image of a single option in the dropdown list.</para>
        /// </summary>
        [Serializable]
        public class OptionData
        {
            [SerializeField]
            private Sprite m_Image;
            [SerializeField]
            private string m_Text;

            /// <summary>
            /// <para>Create an object representing a single option for the dropdown list.</para>
            /// </summary>
            /// <param name="text">Optional text for the option.</param>
            /// <param name="image">Optional image for the option.</param>
            public OptionData()
            {
            }

            /// <summary>
            /// <para>Create an object representing a single option for the dropdown list.</para>
            /// </summary>
            /// <param name="text">Optional text for the option.</param>
            /// <param name="image">Optional image for the option.</param>
            public OptionData(string text)
            {
                this.text = text;
            }

            /// <summary>
            /// <para>Create an object representing a single option for the dropdown list.</para>
            /// </summary>
            /// <param name="text">Optional text for the option.</param>
            /// <param name="image">Optional image for the option.</param>
            public OptionData(Sprite image)
            {
                this.image = image;
            }

            /// <summary>
            /// <para>Create an object representing a single option for the dropdown list.</para>
            /// </summary>
            /// <param name="text">Optional text for the option.</param>
            /// <param name="image">Optional image for the option.</param>
            public OptionData(string text, Sprite image)
            {
                this.text = text;
                this.image = image;
            }

            /// <summary>
            /// <para>The image associated with the option.</para>
            /// </summary>
            public Sprite image
            {
                get => 
                    this.m_Image;
                set
                {
                    this.m_Image = value;
                }
            }

            /// <summary>
            /// <para>The text associated with the option.</para>
            /// </summary>
            public string text
            {
                get => 
                    this.m_Text;
                set
                {
                    this.m_Text = value;
                }
            }
        }

        /// <summary>
        /// <para>Class used internally to store the list of options for the dropdown list.</para>
        /// </summary>
        [Serializable]
        public class OptionDataList
        {
            [SerializeField]
            private List<Dropdown.OptionData> m_Options;

            public OptionDataList()
            {
                this.options = new List<Dropdown.OptionData>();
            }

            /// <summary>
            /// <para>The list of options for the dropdown list.</para>
            /// </summary>
            public List<Dropdown.OptionData> options
            {
                get => 
                    this.m_Options;
                set
                {
                    this.m_Options = value;
                }
            }
        }
    }
}

