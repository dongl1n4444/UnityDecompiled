namespace UnityEngine.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    /// <summary>
    /// <para>Turn a simple label into a interactable input field.</para>
    /// </summary>
    [AddComponentMenu("UI/Input Field", 0x1f)]
    public class InputField : Selectable, IUpdateSelectedHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, ISubmitHandler, ICanvasElement, ILayoutElement, IEventSystemHandler
    {
        private RectTransform caretRectTrans = null;
        private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";
        private const float kHScrollSpeed = 0.05f;
        private static readonly char[] kSeparators = new char[] { ' ', '.', ',', '\t', '\r', '\n' };
        private const float kVScrollSpeed = 0.1f;
        private bool m_AllowInput = false;
        [FormerlySerializedAs("asteriskChar"), SerializeField]
        private char m_AsteriskChar = '*';
        private Coroutine m_BlinkCoroutine = null;
        private float m_BlinkStartTime = 0f;
        private CanvasRenderer m_CachedInputRenderer;
        [SerializeField, Range(0f, 4f)]
        private float m_CaretBlinkRate = 0.85f;
        [SerializeField]
        private Color m_CaretColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f);
        protected int m_CaretPosition = 0;
        protected int m_CaretSelectPosition = 0;
        protected bool m_CaretVisible;
        [SerializeField, Range(1f, 5f)]
        private int m_CaretWidth = 1;
        [FormerlySerializedAs("characterLimit"), SerializeField]
        private int m_CharacterLimit = 0;
        [FormerlySerializedAs("validation"), SerializeField]
        private CharacterValidation m_CharacterValidation = CharacterValidation.None;
        [SerializeField]
        private ContentType m_ContentType = ContentType.Standard;
        protected UIVertex[] m_CursorVerts = null;
        [SerializeField]
        private bool m_CustomCaretColor = false;
        private Coroutine m_DragCoroutine = null;
        private bool m_DragPositionOutOfBounds = false;
        protected int m_DrawEnd = 0;
        protected int m_DrawStart = 0;
        private bool m_HasDoneFocusTransition = false;
        [FormerlySerializedAs("hideMobileInput"), SerializeField]
        private bool m_HideMobileInput = false;
        private TextGenerator m_InputTextCache;
        [FormerlySerializedAs("inputType"), SerializeField]
        private InputType m_InputType = InputType.Standard;
        protected TouchScreenKeyboard m_Keyboard;
        [FormerlySerializedAs("keyboardType"), SerializeField]
        private TouchScreenKeyboardType m_KeyboardType = TouchScreenKeyboardType.Default;
        [SerializeField]
        private LineType m_LineType = LineType.SingleLine;
        [NonSerialized]
        protected Mesh m_Mesh;
        [FormerlySerializedAs("onSubmit"), FormerlySerializedAs("m_OnSubmit"), FormerlySerializedAs("m_EndEdit"), SerializeField]
        private SubmitEvent m_OnEndEdit = new SubmitEvent();
        [FormerlySerializedAs("onValidateInput"), SerializeField]
        private OnValidateInput m_OnValidateInput;
        [FormerlySerializedAs("onValueChange"), FormerlySerializedAs("m_OnValueChange"), SerializeField]
        private OnChangeEvent m_OnValueChanged = new OnChangeEvent();
        private string m_OriginalText = "";
        [SerializeField]
        protected Graphic m_Placeholder;
        private bool m_PreventFontCallback = false;
        private Event m_ProcessingEvent = new Event();
        [SerializeField]
        private bool m_ReadOnly = false;
        [FormerlySerializedAs("selectionColor"), SerializeField]
        private Color m_SelectionColor = new Color(0.6588235f, 0.8078431f, 1f, 0.7529412f);
        private bool m_ShouldActivateNextUpdate = false;
        [SerializeField, FormerlySerializedAs("mValue")]
        protected string m_Text = string.Empty;
        [SerializeField, FormerlySerializedAs("text")]
        protected Text m_TextComponent;
        private bool m_UpdateDrag = false;
        private bool m_WasCanceled = false;

        protected InputField()
        {
            this.EnforceTextHOverflow();
        }

        /// <summary>
        /// <para>Function to activate the InputField to begin processing Events.</para>
        /// </summary>
        public void ActivateInputField()
        {
            if ((((this.m_TextComponent != null) && (this.m_TextComponent.font != null)) && this.IsActive()) && this.IsInteractable())
            {
                if (this.isFocused && ((this.m_Keyboard != null) && !this.m_Keyboard.active))
                {
                    this.m_Keyboard.active = true;
                    this.m_Keyboard.text = this.m_Text;
                }
                this.m_ShouldActivateNextUpdate = true;
            }
        }

        private void ActivateInputFieldInternal()
        {
            if (EventSystem.current != null)
            {
                if (EventSystem.current.currentSelectedGameObject != base.gameObject)
                {
                    EventSystem.current.SetSelectedGameObject(base.gameObject);
                }
                if (TouchScreenKeyboard.isSupported)
                {
                    if (this.input.touchSupported)
                    {
                        TouchScreenKeyboard.hideInput = this.shouldHideMobileInput;
                    }
                    this.m_Keyboard = (this.inputType != InputType.Password) ? TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, this.inputType == InputType.AutoCorrect, this.multiLine) : TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, false, this.multiLine, true);
                    this.MoveTextEnd(false);
                }
                else
                {
                    this.input.imeCompositionMode = IMECompositionMode.On;
                    this.OnFocus();
                }
                this.m_AllowInput = true;
                this.m_OriginalText = this.text;
                this.m_WasCanceled = false;
                this.SetCaretVisible();
                this.UpdateLabel();
            }
        }

        /// <summary>
        /// <para>Append a character to the input field.</para>
        /// </summary>
        /// <param name="input">Character / string to append.</param>
        protected virtual void Append(char input)
        {
            if (!this.m_ReadOnly && this.InPlaceEditing())
            {
                int charIndex = Math.Min(this.selectionFocusPosition, this.selectionAnchorPosition);
                if (this.onValidateInput != null)
                {
                    input = this.onValidateInput(this.text, charIndex, input);
                }
                else if (this.characterValidation != CharacterValidation.None)
                {
                    input = this.Validate(this.text, charIndex, input);
                }
                if (input != '\0')
                {
                    this.Insert(input);
                }
            }
        }

        /// <summary>
        /// <para>Append a character to the input field.</para>
        /// </summary>
        /// <param name="input">Character / string to append.</param>
        protected virtual void Append(string input)
        {
            if (!this.m_ReadOnly && this.InPlaceEditing())
            {
                int num = 0;
                int length = input.Length;
                while (num < length)
                {
                    char ch = input[num];
                    if (((ch >= ' ') || (ch == '\t')) || (((ch == '\r') || (ch == '\n')) || (ch == '\n')))
                    {
                        this.Append(ch);
                    }
                    num++;
                }
            }
        }

        private void AssignPositioningIfNeeded()
        {
            if (((this.m_TextComponent != null) && (this.caretRectTrans != null)) && ((((this.caretRectTrans.localPosition != this.m_TextComponent.rectTransform.localPosition) || (this.caretRectTrans.localRotation != this.m_TextComponent.rectTransform.localRotation)) || ((this.caretRectTrans.localScale != this.m_TextComponent.rectTransform.localScale) || (this.caretRectTrans.anchorMin != this.m_TextComponent.rectTransform.anchorMin))) || (((this.caretRectTrans.anchorMax != this.m_TextComponent.rectTransform.anchorMax) || (this.caretRectTrans.anchoredPosition != this.m_TextComponent.rectTransform.anchoredPosition)) || ((this.caretRectTrans.sizeDelta != this.m_TextComponent.rectTransform.sizeDelta) || (this.caretRectTrans.pivot != this.m_TextComponent.rectTransform.pivot)))))
            {
                this.caretRectTrans.localPosition = this.m_TextComponent.rectTransform.localPosition;
                this.caretRectTrans.localRotation = this.m_TextComponent.rectTransform.localRotation;
                this.caretRectTrans.localScale = this.m_TextComponent.rectTransform.localScale;
                this.caretRectTrans.anchorMin = this.m_TextComponent.rectTransform.anchorMin;
                this.caretRectTrans.anchorMax = this.m_TextComponent.rectTransform.anchorMax;
                this.caretRectTrans.anchoredPosition = this.m_TextComponent.rectTransform.anchoredPosition;
                this.caretRectTrans.sizeDelta = this.m_TextComponent.rectTransform.sizeDelta;
                this.caretRectTrans.pivot = this.m_TextComponent.rectTransform.pivot;
            }
        }

        private void Backspace()
        {
            if (!this.m_ReadOnly)
            {
                if (this.hasSelection)
                {
                    this.Delete();
                    this.SendOnValueChangedAndUpdateLabel();
                }
                else if (this.caretPositionInternal > 0)
                {
                    this.m_Text = this.text.Remove(this.caretPositionInternal - 1, 1);
                    int num = this.caretPositionInternal - 1;
                    this.caretPositionInternal = num;
                    this.caretSelectPositionInternal = num;
                    this.SendOnValueChangedAndUpdateLabel();
                }
            }
        }

        public virtual void CalculateLayoutInputHorizontal()
        {
        }

        public virtual void CalculateLayoutInputVertical()
        {
        }

        [DebuggerHidden]
        private IEnumerator CaretBlink() => 
            new <CaretBlink>c__Iterator0 { $this = this };

        protected void ClampPos(ref int pos)
        {
            if (pos < 0)
            {
                pos = 0;
            }
            else if (pos > this.text.Length)
            {
                pos = this.text.Length;
            }
        }

        private void CreateCursorVerts()
        {
            this.m_CursorVerts = new UIVertex[4];
            for (int i = 0; i < this.m_CursorVerts.Length; i++)
            {
                this.m_CursorVerts[i] = UIVertex.simpleVert;
                this.m_CursorVerts[i].uv0 = Vector2.zero;
            }
        }

        /// <summary>
        /// <para>Function to deactivate the InputField to stop the processing of Events and send OnSubmit if not canceled.</para>
        /// </summary>
        public void DeactivateInputField()
        {
            if (this.m_AllowInput)
            {
                this.m_HasDoneFocusTransition = false;
                this.m_AllowInput = false;
                if (this.m_Placeholder != null)
                {
                    this.m_Placeholder.enabled = string.IsNullOrEmpty(this.m_Text);
                }
                if ((this.m_TextComponent != null) && this.IsInteractable())
                {
                    if (this.m_WasCanceled)
                    {
                        this.text = this.m_OriginalText;
                    }
                    if (this.m_Keyboard != null)
                    {
                        this.m_Keyboard.active = false;
                        this.m_Keyboard = null;
                    }
                    this.m_CaretPosition = this.m_CaretSelectPosition = 0;
                    this.SendOnSubmit();
                    this.input.imeCompositionMode = IMECompositionMode.Auto;
                }
                this.MarkGeometryAsDirty();
            }
        }

        private void Delete()
        {
            if (!this.m_ReadOnly && (this.caretPositionInternal != this.caretSelectPositionInternal))
            {
                if (this.caretPositionInternal < this.caretSelectPositionInternal)
                {
                    this.m_Text = this.text.Substring(0, this.caretPositionInternal) + this.text.Substring(this.caretSelectPositionInternal, this.text.Length - this.caretSelectPositionInternal);
                    this.caretSelectPositionInternal = this.caretPositionInternal;
                }
                else
                {
                    this.m_Text = this.text.Substring(0, this.caretSelectPositionInternal) + this.text.Substring(this.caretPositionInternal, this.text.Length - this.caretPositionInternal);
                    this.caretPositionInternal = this.caretSelectPositionInternal;
                }
            }
        }

        private int DetermineCharacterLine(int charPos, TextGenerator generator)
        {
            for (int i = 0; i < (generator.lineCount - 1); i++)
            {
                UILineInfo info = generator.lines[i + 1];
                if (info.startCharIdx > charPos)
                {
                    return i;
                }
            }
            return (generator.lineCount - 1);
        }

        protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
        {
            if (this.m_HasDoneFocusTransition)
            {
                state = Selectable.SelectionState.Highlighted;
            }
            else if (state == Selectable.SelectionState.Pressed)
            {
                this.m_HasDoneFocusTransition = true;
            }
            base.DoStateTransition(state, instant);
        }

        private void EnforceContentType()
        {
            switch (this.contentType)
            {
                case ContentType.Standard:
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.Default;
                    this.m_CharacterValidation = CharacterValidation.None;
                    break;

                case ContentType.Autocorrected:
                    this.m_InputType = InputType.AutoCorrect;
                    this.m_KeyboardType = TouchScreenKeyboardType.Default;
                    this.m_CharacterValidation = CharacterValidation.None;
                    break;

                case ContentType.IntegerNumber:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
                    this.m_CharacterValidation = CharacterValidation.Integer;
                    break;

                case ContentType.DecimalNumber:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
                    this.m_CharacterValidation = CharacterValidation.Decimal;
                    break;

                case ContentType.Alphanumeric:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.ASCIICapable;
                    this.m_CharacterValidation = CharacterValidation.Alphanumeric;
                    break;

                case ContentType.Name:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.Default;
                    this.m_CharacterValidation = CharacterValidation.Name;
                    break;

                case ContentType.EmailAddress:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Standard;
                    this.m_KeyboardType = TouchScreenKeyboardType.EmailAddress;
                    this.m_CharacterValidation = CharacterValidation.EmailAddress;
                    break;

                case ContentType.Password:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Password;
                    this.m_KeyboardType = TouchScreenKeyboardType.Default;
                    this.m_CharacterValidation = CharacterValidation.None;
                    break;

                case ContentType.Pin:
                    this.m_LineType = LineType.SingleLine;
                    this.m_InputType = InputType.Password;
                    this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
                    this.m_CharacterValidation = CharacterValidation.Integer;
                    break;
            }
            this.EnforceTextHOverflow();
        }

        private void EnforceTextHOverflow()
        {
            if (this.m_TextComponent != null)
            {
                if (this.multiLine)
                {
                    this.m_TextComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
                }
                else
                {
                    this.m_TextComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
                }
            }
        }

        private int FindtNextWordBegin()
        {
            if ((this.caretSelectPositionInternal + 1) >= this.text.Length)
            {
                return this.text.Length;
            }
            int length = this.text.IndexOfAny(kSeparators, this.caretSelectPositionInternal + 1);
            if (length == -1)
            {
                length = this.text.Length;
            }
            else
            {
                length++;
            }
            return length;
        }

        private int FindtPrevWordBegin()
        {
            if ((this.caretSelectPositionInternal - 2) < 0)
            {
                return 0;
            }
            int num2 = this.text.LastIndexOfAny(kSeparators, this.caretSelectPositionInternal - 2);
            if (num2 == -1)
            {
                num2 = 0;
            }
            else
            {
                num2++;
            }
            return num2;
        }

        /// <summary>
        /// <para>Force the label to update immediatly. This will recalculate the positioning of the caret and the visible text.</para>
        /// </summary>
        public void ForceLabelUpdate()
        {
            this.UpdateLabel();
        }

        private void ForwardSpace()
        {
            if (!this.m_ReadOnly)
            {
                if (this.hasSelection)
                {
                    this.Delete();
                    this.SendOnValueChangedAndUpdateLabel();
                }
                else if (this.caretPositionInternal < this.text.Length)
                {
                    this.m_Text = this.text.Remove(this.caretPositionInternal, 1);
                    this.SendOnValueChangedAndUpdateLabel();
                }
            }
        }

        private void GenerateCaret(VertexHelper vbo, Vector2 roundingOffset)
        {
            if (this.m_CaretVisible)
            {
                if (this.m_CursorVerts == null)
                {
                    this.CreateCursorVerts();
                }
                float caretWidth = this.m_CaretWidth;
                int charPos = Mathf.Max(0, this.caretPositionInternal - this.m_DrawStart);
                TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
                if ((cachedTextGenerator != null) && (cachedTextGenerator.lineCount != 0))
                {
                    Vector2 zero = Vector2.zero;
                    if (charPos < cachedTextGenerator.characters.Count)
                    {
                        UICharInfo info = cachedTextGenerator.characters[charPos];
                        zero.x = info.cursorPos.x;
                    }
                    zero.x /= this.m_TextComponent.pixelsPerUnit;
                    if (zero.x > this.m_TextComponent.rectTransform.rect.xMax)
                    {
                        zero.x = this.m_TextComponent.rectTransform.rect.xMax;
                    }
                    int num3 = this.DetermineCharacterLine(charPos, cachedTextGenerator);
                    UILineInfo info2 = cachedTextGenerator.lines[num3];
                    zero.y = info2.topY / this.m_TextComponent.pixelsPerUnit;
                    UILineInfo info3 = cachedTextGenerator.lines[num3];
                    float num4 = ((float) info3.height) / this.m_TextComponent.pixelsPerUnit;
                    for (int i = 0; i < this.m_CursorVerts.Length; i++)
                    {
                        this.m_CursorVerts[i].color = this.caretColor;
                    }
                    this.m_CursorVerts[0].position = new Vector3(zero.x, zero.y - num4, 0f);
                    this.m_CursorVerts[1].position = new Vector3(zero.x + caretWidth, zero.y - num4, 0f);
                    this.m_CursorVerts[2].position = new Vector3(zero.x + caretWidth, zero.y, 0f);
                    this.m_CursorVerts[3].position = new Vector3(zero.x, zero.y, 0f);
                    if (roundingOffset != Vector2.zero)
                    {
                        for (int j = 0; j < this.m_CursorVerts.Length; j++)
                        {
                            UIVertex vertex = this.m_CursorVerts[j];
                            vertex.position.x += roundingOffset.x;
                            vertex.position.y += roundingOffset.y;
                        }
                    }
                    vbo.AddUIVertexQuad(this.m_CursorVerts);
                    int height = Screen.height;
                    int targetDisplay = this.m_TextComponent.canvas.targetDisplay;
                    if ((targetDisplay > 0) && (targetDisplay < Display.displays.Length))
                    {
                        height = Display.displays[targetDisplay].renderingHeight;
                    }
                    zero.y = height - zero.y;
                    this.input.compositionCursorPos = zero;
                }
            }
        }

        private void GenerateHightlight(VertexHelper vbo, Vector2 roundingOffset)
        {
            int charPos = Mathf.Max(0, this.caretPositionInternal - this.m_DrawStart);
            int num2 = Mathf.Max(0, this.caretSelectPositionInternal - this.m_DrawStart);
            if (charPos > num2)
            {
                int num3 = charPos;
                charPos = num2;
                num2 = num3;
            }
            num2--;
            TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
            if (cachedTextGenerator.lineCount > 0)
            {
                int line = this.DetermineCharacterLine(charPos, cachedTextGenerator);
                int lineEndPosition = GetLineEndPosition(cachedTextGenerator, line);
                UIVertex simpleVert = UIVertex.simpleVert;
                simpleVert.uv0 = Vector2.zero;
                simpleVert.color = this.selectionColor;
                for (int i = charPos; (i <= num2) && (i < cachedTextGenerator.characterCount); i++)
                {
                    if ((i == lineEndPosition) || (i == num2))
                    {
                        UICharInfo info = cachedTextGenerator.characters[charPos];
                        UICharInfo info2 = cachedTextGenerator.characters[i];
                        UILineInfo info3 = cachedTextGenerator.lines[line];
                        Vector2 vector = new Vector2(info.cursorPos.x / this.m_TextComponent.pixelsPerUnit, info3.topY / this.m_TextComponent.pixelsPerUnit);
                        UILineInfo info4 = cachedTextGenerator.lines[line];
                        Vector2 vector2 = new Vector2((info2.cursorPos.x + info2.charWidth) / this.m_TextComponent.pixelsPerUnit, vector.y - (((float) info4.height) / this.m_TextComponent.pixelsPerUnit));
                        if ((vector2.x > this.m_TextComponent.rectTransform.rect.xMax) || (vector2.x < this.m_TextComponent.rectTransform.rect.xMin))
                        {
                            vector2.x = this.m_TextComponent.rectTransform.rect.xMax;
                        }
                        int currentVertCount = vbo.currentVertCount;
                        simpleVert.position = new Vector3(vector.x, vector2.y, 0f) + roundingOffset;
                        vbo.AddVert(simpleVert);
                        simpleVert.position = new Vector3(vector2.x, vector2.y, 0f) + roundingOffset;
                        vbo.AddVert(simpleVert);
                        simpleVert.position = new Vector3(vector2.x, vector.y, 0f) + roundingOffset;
                        vbo.AddVert(simpleVert);
                        simpleVert.position = new Vector3(vector.x, vector.y, 0f) + roundingOffset;
                        vbo.AddVert(simpleVert);
                        vbo.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
                        vbo.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
                        charPos = i + 1;
                        line++;
                        lineEndPosition = GetLineEndPosition(cachedTextGenerator, line);
                    }
                }
            }
        }

        /// <summary>
        /// <para>The character that is under the mouse.</para>
        /// </summary>
        /// <param name="pos">Mouse position.</param>
        /// <returns>
        /// <para>Character index with in value.</para>
        /// </returns>
        protected int GetCharacterIndexFromPosition(Vector2 pos)
        {
            TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
            if (cachedTextGenerator.lineCount == 0)
            {
                return 0;
            }
            int unclampedCharacterLineFromPosition = this.GetUnclampedCharacterLineFromPosition(pos, cachedTextGenerator);
            if (unclampedCharacterLineFromPosition < 0)
            {
                return 0;
            }
            if (unclampedCharacterLineFromPosition >= cachedTextGenerator.lineCount)
            {
                return cachedTextGenerator.characterCountVisible;
            }
            UILineInfo info = cachedTextGenerator.lines[unclampedCharacterLineFromPosition];
            int startCharIdx = info.startCharIdx;
            int lineEndPosition = GetLineEndPosition(cachedTextGenerator, unclampedCharacterLineFromPosition);
            for (int i = startCharIdx; i < lineEndPosition; i++)
            {
                if (i >= cachedTextGenerator.characterCountVisible)
                {
                    return lineEndPosition;
                }
                UICharInfo info2 = cachedTextGenerator.characters[i];
                Vector2 vector = (Vector2) (info2.cursorPos / this.m_TextComponent.pixelsPerUnit);
                float num6 = pos.x - vector.x;
                float num7 = (vector.x + (info2.charWidth / this.m_TextComponent.pixelsPerUnit)) - pos.x;
                if (num6 < num7)
                {
                    return i;
                }
            }
            return lineEndPosition;
        }

        private static int GetLineEndPosition(TextGenerator gen, int line)
        {
            line = Mathf.Max(line, 0);
            if ((line + 1) < gen.lines.Count)
            {
                UILineInfo info = gen.lines[line + 1];
                return (info.startCharIdx - 1);
            }
            return gen.characterCountVisible;
        }

        private static int GetLineStartPosition(TextGenerator gen, int line)
        {
            line = Mathf.Clamp(line, 0, gen.lines.Count - 1);
            UILineInfo info = gen.lines[line];
            return info.startCharIdx;
        }

        private string GetSelectedString()
        {
            if (!this.hasSelection)
            {
                return "";
            }
            int caretPositionInternal = this.caretPositionInternal;
            int caretSelectPositionInternal = this.caretSelectPositionInternal;
            if (caretPositionInternal > caretSelectPositionInternal)
            {
                int num3 = caretPositionInternal;
                caretPositionInternal = caretSelectPositionInternal;
                caretSelectPositionInternal = num3;
            }
            return this.text.Substring(caretPositionInternal, caretSelectPositionInternal - caretPositionInternal);
        }

        private int GetUnclampedCharacterLineFromPosition(Vector2 pos, TextGenerator generator)
        {
            if (!this.multiLine)
            {
                return 0;
            }
            float num2 = pos.y * this.m_TextComponent.pixelsPerUnit;
            float num3 = 0f;
            for (int i = 0; i < generator.lineCount; i++)
            {
                UILineInfo info = generator.lines[i];
                float topY = info.topY;
                UILineInfo info2 = generator.lines[i];
                float num6 = topY - info2.height;
                if (num2 > topY)
                {
                    float num7 = topY - num3;
                    if (num2 > (topY - (0.5f * num7)))
                    {
                        return (i - 1);
                    }
                    return i;
                }
                if (num2 > num6)
                {
                    return i;
                }
                num3 = num6;
            }
            return generator.lineCount;
        }

        /// <summary>
        /// <para>See ICanvasElement.GraphicUpdateComplete.</para>
        /// </summary>
        public virtual void GraphicUpdateComplete()
        {
        }

        private bool InPlaceEditing() => 
            !TouchScreenKeyboard.isSupported;

        private void Insert(char c)
        {
            if (!this.m_ReadOnly)
            {
                string str = c.ToString();
                this.Delete();
                if ((this.characterLimit <= 0) || (this.text.Length < this.characterLimit))
                {
                    this.m_Text = this.text.Insert(this.m_CaretPosition, str);
                    this.caretSelectPositionInternal = this.caretPositionInternal += str.Length;
                    this.SendOnValueChanged();
                }
            }
        }

        private bool IsSelectionVisible()
        {
            if ((this.m_DrawStart > this.caretPositionInternal) || (this.m_DrawStart > this.caretSelectPositionInternal))
            {
                return false;
            }
            if ((this.m_DrawEnd < this.caretPositionInternal) || (this.m_DrawEnd < this.caretSelectPositionInternal))
            {
                return false;
            }
            return true;
        }

        private bool IsValidChar(char c)
        {
            if (c == '\x007f')
            {
                return false;
            }
            return (((c == '\t') || (c == '\n')) || this.m_TextComponent.font.HasCharacter(c));
        }

        /// <summary>
        /// <para>Process the Event and perform the appropriate action for that key.</para>
        /// </summary>
        /// <param name="evt">The Event that is currently being processed.</param>
        /// <returns>
        /// <para>If we should continue processing events or we have hit an end condition.</para>
        /// </returns>
        protected EditState KeyPressed(Event evt)
        {
            char ch;
            EventModifiers modifiers = evt.modifiers;
            bool ctrl = (SystemInfo.operatingSystemFamily != OperatingSystemFamily.MacOSX) ? ((modifiers & EventModifiers.Control) != EventModifiers.None) : ((modifiers & EventModifiers.Command) != EventModifiers.None);
            bool shift = (modifiers & EventModifiers.Shift) != EventModifiers.None;
            bool flag3 = (modifiers & EventModifiers.Alt) != EventModifiers.None;
            bool flag4 = (ctrl && !flag3) && !shift;
            KeyCode keyCode = evt.keyCode;
            switch (keyCode)
            {
                case KeyCode.KeypadEnter:
                    break;

                case KeyCode.UpArrow:
                    this.MoveUp(shift);
                    return EditState.Continue;

                case KeyCode.DownArrow:
                    this.MoveDown(shift);
                    return EditState.Continue;

                case KeyCode.RightArrow:
                    this.MoveRight(shift, ctrl);
                    return EditState.Continue;

                case KeyCode.LeftArrow:
                    this.MoveLeft(shift, ctrl);
                    return EditState.Continue;

                case KeyCode.Home:
                    this.MoveTextStart(shift);
                    return EditState.Continue;

                case KeyCode.End:
                    this.MoveTextEnd(shift);
                    return EditState.Continue;

                case KeyCode.A:
                    if (!flag4)
                    {
                        goto Label_024D;
                    }
                    this.SelectAll();
                    return EditState.Continue;

                case KeyCode.C:
                    if (!flag4)
                    {
                        goto Label_024D;
                    }
                    if (this.inputType == InputType.Password)
                    {
                        clipboard = "";
                    }
                    else
                    {
                        clipboard = this.GetSelectedString();
                    }
                    return EditState.Continue;

                case KeyCode.V:
                    if (!flag4)
                    {
                        goto Label_024D;
                    }
                    this.Append(clipboard);
                    return EditState.Continue;

                case KeyCode.X:
                    if (!flag4)
                    {
                        goto Label_024D;
                    }
                    if (this.inputType == InputType.Password)
                    {
                        clipboard = "";
                    }
                    else
                    {
                        clipboard = this.GetSelectedString();
                    }
                    this.Delete();
                    this.SendOnValueChangedAndUpdateLabel();
                    return EditState.Continue;

                default:
                    if (keyCode != KeyCode.Backspace)
                    {
                        if (keyCode == KeyCode.Return)
                        {
                            break;
                        }
                        if (keyCode == KeyCode.Escape)
                        {
                            this.m_WasCanceled = true;
                            return EditState.Finish;
                        }
                        if (keyCode == KeyCode.Delete)
                        {
                            this.ForwardSpace();
                            return EditState.Continue;
                        }
                        goto Label_024D;
                    }
                    this.Backspace();
                    return EditState.Continue;
            }
            if (this.lineType != LineType.MultiLineNewline)
            {
                return EditState.Finish;
            }
        Label_024D:
            ch = evt.character;
            if (this.multiLine || (((ch != '\t') && (ch != '\r')) && (ch != '\n')))
            {
                if ((ch == '\r') || (ch == '\x0003'))
                {
                    ch = '\n';
                }
                if (this.IsValidChar(ch))
                {
                    this.Append(ch);
                }
                if ((ch == '\0') && (this.compositionString.Length > 0))
                {
                    this.UpdateLabel();
                }
            }
            return EditState.Continue;
        }

        protected virtual void LateUpdate()
        {
            if (this.m_ShouldActivateNextUpdate)
            {
                if (!this.isFocused)
                {
                    this.ActivateInputFieldInternal();
                    this.m_ShouldActivateNextUpdate = false;
                    return;
                }
                this.m_ShouldActivateNextUpdate = false;
            }
            if (!this.InPlaceEditing() && this.isFocused)
            {
                this.AssignPositioningIfNeeded();
                if ((this.m_Keyboard == null) || this.m_Keyboard.done)
                {
                    if (this.m_Keyboard != null)
                    {
                        if (!this.m_ReadOnly)
                        {
                            this.text = this.m_Keyboard.text;
                        }
                        if (this.m_Keyboard.wasCanceled)
                        {
                            this.m_WasCanceled = true;
                        }
                    }
                    this.OnDeselect(null);
                }
                else
                {
                    string text = this.m_Keyboard.text;
                    if (this.m_Text != text)
                    {
                        if (this.m_ReadOnly)
                        {
                            this.m_Keyboard.text = this.m_Text;
                        }
                        else
                        {
                            this.m_Text = "";
                            for (int i = 0; i < text.Length; i++)
                            {
                                char addedChar = text[i];
                                switch (addedChar)
                                {
                                    case '\r':
                                    case '\x0003':
                                        addedChar = '\n';
                                        break;
                                }
                                if (this.onValidateInput != null)
                                {
                                    addedChar = this.onValidateInput(this.m_Text, this.m_Text.Length, addedChar);
                                }
                                else if (this.characterValidation != CharacterValidation.None)
                                {
                                    addedChar = this.Validate(this.m_Text, this.m_Text.Length, addedChar);
                                }
                                if ((this.lineType == LineType.MultiLineSubmit) && (addedChar == '\n'))
                                {
                                    this.m_Keyboard.text = this.m_Text;
                                    this.OnDeselect(null);
                                    return;
                                }
                                if (addedChar != '\0')
                                {
                                    this.m_Text = this.m_Text + addedChar;
                                }
                            }
                            if ((this.characterLimit > 0) && (this.m_Text.Length > this.characterLimit))
                            {
                                this.m_Text = this.m_Text.Substring(0, this.characterLimit);
                            }
                            if (this.m_Keyboard.canGetSelection)
                            {
                                this.UpdateCaretFromKeyboard();
                            }
                            else
                            {
                                int length = this.m_Text.Length;
                                this.caretSelectPositionInternal = length;
                                this.caretPositionInternal = length;
                            }
                            if (this.m_Text != text)
                            {
                                this.m_Keyboard.text = this.m_Text;
                            }
                            this.SendOnValueChangedAndUpdateLabel();
                        }
                    }
                    else if (this.m_Keyboard.canGetSelection)
                    {
                        this.UpdateCaretFromKeyboard();
                    }
                    if (this.m_Keyboard.done)
                    {
                        if (this.m_Keyboard.wasCanceled)
                        {
                            this.m_WasCanceled = true;
                        }
                        this.OnDeselect(null);
                    }
                }
            }
        }

        /// <summary>
        /// <para>See ICanvasElement.LayoutComplete.</para>
        /// </summary>
        public virtual void LayoutComplete()
        {
        }

        private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
        {
            if (originalPos >= this.cachedInputTextGenerator.characterCountVisible)
            {
                return this.text.Length;
            }
            UICharInfo info = this.cachedInputTextGenerator.characters[originalPos];
            int num2 = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
            if ((num2 + 1) >= this.cachedInputTextGenerator.lineCount)
            {
                return (!goToLastChar ? originalPos : this.text.Length);
            }
            int lineEndPosition = GetLineEndPosition(this.cachedInputTextGenerator, num2 + 1);
            UILineInfo info2 = this.cachedInputTextGenerator.lines[num2 + 1];
            for (int i = info2.startCharIdx; i < lineEndPosition; i++)
            {
                UICharInfo info3 = this.cachedInputTextGenerator.characters[i];
                if (info3.cursorPos.x >= info.cursorPos.x)
                {
                    return i;
                }
            }
            return lineEndPosition;
        }

        private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
        {
            if (originalPos >= this.cachedInputTextGenerator.characters.Count)
            {
                return 0;
            }
            UICharInfo info = this.cachedInputTextGenerator.characters[originalPos];
            int num2 = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
            if (num2 <= 0)
            {
                return (!goToFirstChar ? originalPos : 0);
            }
            UILineInfo info2 = this.cachedInputTextGenerator.lines[num2];
            int num3 = info2.startCharIdx - 1;
            UILineInfo info3 = this.cachedInputTextGenerator.lines[num2 - 1];
            for (int i = info3.startCharIdx; i < num3; i++)
            {
                UICharInfo info4 = this.cachedInputTextGenerator.characters[i];
                if (info4.cursorPos.x >= info.cursorPos.x)
                {
                    return i;
                }
            }
            return num3;
        }

        private void MarkGeometryAsDirty()
        {
            if (Application.isPlaying && (PrefabUtility.GetPrefabObject(base.gameObject) == null))
            {
                CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
            }
        }

        private bool MayDrag(PointerEventData eventData) => 
            (((this.IsActive() && this.IsInteractable()) && ((eventData.button == PointerEventData.InputButton.Left) && (this.m_TextComponent != null))) && (this.m_Keyboard == null));

        [DebuggerHidden]
        private IEnumerator MouseDragOutsideRect(PointerEventData eventData) => 
            new <MouseDragOutsideRect>c__Iterator1 { 
                eventData = eventData,
                $this = this
            };

        private void MoveDown(bool shift)
        {
            this.MoveDown(shift, true);
        }

        private void MoveDown(bool shift, bool goToLastChar)
        {
            int num;
            if (this.hasSelection && !shift)
            {
                num = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal);
                this.caretSelectPositionInternal = num;
                this.caretPositionInternal = num;
            }
            int num2 = !this.multiLine ? this.text.Length : this.LineDownCharacterPosition(this.caretSelectPositionInternal, goToLastChar);
            if (shift)
            {
                this.caretSelectPositionInternal = num2;
            }
            else
            {
                num = num2;
                this.caretSelectPositionInternal = num;
                this.caretPositionInternal = num;
            }
        }

        private void MoveLeft(bool shift, bool ctrl)
        {
            int num;
            if (this.hasSelection && !shift)
            {
                num = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal);
                this.caretSelectPositionInternal = num;
                this.caretPositionInternal = num;
            }
            else
            {
                int num2;
                if (ctrl)
                {
                    num2 = this.FindtPrevWordBegin();
                }
                else
                {
                    num2 = this.caretSelectPositionInternal - 1;
                }
                if (shift)
                {
                    this.caretSelectPositionInternal = num2;
                }
                else
                {
                    num = num2;
                    this.caretPositionInternal = num;
                    this.caretSelectPositionInternal = num;
                }
            }
        }

        private void MoveRight(bool shift, bool ctrl)
        {
            int num;
            if (this.hasSelection && !shift)
            {
                num = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal);
                this.caretSelectPositionInternal = num;
                this.caretPositionInternal = num;
            }
            else
            {
                int num2;
                if (ctrl)
                {
                    num2 = this.FindtNextWordBegin();
                }
                else
                {
                    num2 = this.caretSelectPositionInternal + 1;
                }
                if (shift)
                {
                    this.caretSelectPositionInternal = num2;
                }
                else
                {
                    num = num2;
                    this.caretPositionInternal = num;
                    this.caretSelectPositionInternal = num;
                }
            }
        }

        /// <summary>
        /// <para>Move the caret index to end of text.</para>
        /// </summary>
        /// <param name="shift">Only move the selectionPosition.</param>
        public void MoveTextEnd(bool shift)
        {
            int length = this.text.Length;
            if (shift)
            {
                this.caretSelectPositionInternal = length;
            }
            else
            {
                this.caretPositionInternal = length;
                this.caretSelectPositionInternal = this.caretPositionInternal;
            }
            this.UpdateLabel();
        }

        /// <summary>
        /// <para>Move the caret index to start of text.</para>
        /// </summary>
        /// <param name="shift">Only move the selectionPosition.</param>
        public void MoveTextStart(bool shift)
        {
            int num = 0;
            if (shift)
            {
                this.caretSelectPositionInternal = num;
            }
            else
            {
                this.caretPositionInternal = num;
                this.caretSelectPositionInternal = this.caretPositionInternal;
            }
            this.UpdateLabel();
        }

        private void MoveUp(bool shift)
        {
            this.MoveUp(shift, true);
        }

        private void MoveUp(bool shift, bool goToFirstChar)
        {
            int num;
            if (this.hasSelection && !shift)
            {
                num = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal);
                this.caretSelectPositionInternal = num;
                this.caretPositionInternal = num;
            }
            int num2 = !this.multiLine ? 0 : this.LineUpCharacterPosition(this.caretSelectPositionInternal, goToFirstChar);
            if (shift)
            {
                this.caretSelectPositionInternal = num2;
            }
            else
            {
                num = num2;
                this.caretPositionInternal = num;
                this.caretSelectPositionInternal = num;
            }
        }

        /// <summary>
        /// <para>Capture the OnBeginDrag callback from the EventSystem and ensure we should listen to the drag events to follow.</para>
        /// </summary>
        /// <param name="eventData">The data passed by the EventSystem.</param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                this.m_UpdateDrag = true;
            }
        }

        /// <summary>
        /// <para>What to do when the event system sends a Deselect Event.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDeselect(BaseEventData eventData)
        {
            this.DeactivateInputField();
            base.OnDeselect(eventData);
        }

        /// <summary>
        /// <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            this.m_BlinkCoroutine = null;
            this.DeactivateInputField();
            if (this.m_TextComponent != null)
            {
                this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
                this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
                this.m_TextComponent.UnregisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
            }
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
            if (this.m_CachedInputRenderer != null)
            {
                this.m_CachedInputRenderer.Clear();
            }
            if (this.m_Mesh != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_Mesh);
            }
            this.m_Mesh = null;
            base.OnDisable();
        }

        /// <summary>
        /// <para>What to do when the event system sends a Drag Event.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                Vector2 vector;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera, out vector);
                this.caretSelectPositionInternal = this.GetCharacterIndexFromPosition(vector) + this.m_DrawStart;
                this.MarkGeometryAsDirty();
                this.m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera);
                if (this.m_DragPositionOutOfBounds && (this.m_DragCoroutine == null))
                {
                    this.m_DragCoroutine = base.StartCoroutine(this.MouseDragOutsideRect(eventData));
                }
                eventData.Use();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.m_Text == null)
            {
                this.m_Text = string.Empty;
            }
            this.m_DrawStart = 0;
            this.m_DrawEnd = this.m_Text.Length;
            if (this.m_CachedInputRenderer != null)
            {
                this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
            }
            if (this.m_TextComponent != null)
            {
                this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
                this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
                this.m_TextComponent.RegisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
                this.UpdateLabel();
            }
        }

        /// <summary>
        /// <para>Capture the OnEndDrag callback from the EventSystem and cancel the listening of drag events.</para>
        /// </summary>
        /// <param name="eventData">The eventData sent by the EventSystem.</param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                this.m_UpdateDrag = false;
            }
        }

        private void OnFillVBO(Mesh vbo)
        {
            using (VertexHelper helper = new VertexHelper())
            {
                if (!this.isFocused)
                {
                    helper.FillMesh(vbo);
                }
                else
                {
                    Vector2 roundingOffset = this.m_TextComponent.PixelAdjustPoint(Vector2.zero);
                    if (!this.hasSelection)
                    {
                        this.GenerateCaret(helper, roundingOffset);
                    }
                    else
                    {
                        this.GenerateHightlight(helper, roundingOffset);
                    }
                    helper.FillMesh(vbo);
                }
            }
        }

        /// <summary>
        /// <para>Focus the input field initializing properties.</para>
        /// </summary>
        protected void OnFocus()
        {
            this.SelectAll();
        }

        /// <summary>
        /// <para>What to do when the event system sends a pointer click Event.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.ActivateInputField();
            }
        }

        /// <summary>
        /// <para>What to do when the event system sends a pointer down Event.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (this.MayDrag(eventData))
            {
                EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
                bool allowInput = this.m_AllowInput;
                base.OnPointerDown(eventData);
                if (!this.InPlaceEditing() && ((this.m_Keyboard == null) || !this.m_Keyboard.active))
                {
                    this.OnSelect(eventData);
                }
                else
                {
                    if (allowInput)
                    {
                        Vector2 vector;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera, out vector);
                        int num = this.GetCharacterIndexFromPosition(vector) + this.m_DrawStart;
                        this.caretPositionInternal = num;
                        this.caretSelectPositionInternal = num;
                    }
                    this.UpdateLabel();
                    eventData.Use();
                }
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (this.shouldActivateOnSelect)
            {
                this.ActivateInputField();
            }
        }

        /// <summary>
        /// <para>What to do when the event system sends a submit Event.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSubmit(BaseEventData eventData)
        {
            if ((this.IsActive() && this.IsInteractable()) && !this.isFocused)
            {
                this.m_ShouldActivateNextUpdate = true;
            }
        }

        /// <summary>
        /// <para>What to do when the event system sends a Update selected Event.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnUpdateSelected(BaseEventData eventData)
        {
            if (this.isFocused)
            {
                bool flag = false;
                while (Event.PopEvent(this.m_ProcessingEvent))
                {
                    if (this.m_ProcessingEvent.rawType == EventType.KeyDown)
                    {
                        flag = true;
                        if (this.KeyPressed(this.m_ProcessingEvent) == EditState.Finish)
                        {
                            this.DeactivateInputField();
                            break;
                        }
                    }
                    EventType type = this.m_ProcessingEvent.type;
                    if ((type == EventType.ValidateCommand) || (type == EventType.ExecuteCommand))
                    {
                        string commandName = this.m_ProcessingEvent.commandName;
                        if ((commandName != null) && (commandName == "SelectAll"))
                        {
                            this.SelectAll();
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    this.UpdateLabel();
                }
                eventData.Use();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.EnforceContentType();
            this.EnforceTextHOverflow();
            this.m_CharacterLimit = Math.Max(0, this.m_CharacterLimit);
            if (this.IsActive())
            {
                this.UpdateLabel();
                if (this.m_AllowInput)
                {
                    this.SetCaretActive();
                }
            }
        }

        /// <summary>
        /// <para>Helper function to allow separate events to be processed by the InputField.</para>
        /// </summary>
        /// <param name="e">The Event to be processed.</param>
        public void ProcessEvent(Event e)
        {
            this.KeyPressed(e);
        }

        /// <summary>
        /// <para>Rebuild the input fields geometry. (caret and highlight).</para>
        /// </summary>
        /// <param name="update"></param>
        public virtual void Rebuild(CanvasUpdate update)
        {
            if (update == CanvasUpdate.LatePreRender)
            {
                this.UpdateGeometry();
            }
        }

        /// <summary>
        /// <para>Convert screen space into input field local space.</para>
        /// </summary>
        /// <param name="screen"></param>
        [Obsolete("This function is no longer used. Please use RectTransformUtility.ScreenPointToLocalPointInRectangle() instead.")]
        public Vector2 ScreenToLocal(Vector2 screen)
        {
            Canvas canvas = this.m_TextComponent.canvas;
            if (canvas == null)
            {
                return screen;
            }
            Vector3 zero = Vector3.zero;
            if (canvas.renderMode == UnityEngine.RenderMode.ScreenSpaceOverlay)
            {
                zero = this.m_TextComponent.transform.InverseTransformPoint((Vector3) screen);
            }
            else if (canvas.worldCamera != null)
            {
                float num;
                Ray ray = canvas.worldCamera.ScreenPointToRay((Vector3) screen);
                new Plane(this.m_TextComponent.transform.forward, this.m_TextComponent.transform.position).Raycast(ray, out num);
                zero = this.m_TextComponent.transform.InverseTransformPoint(ray.GetPoint(num));
            }
            return new Vector2(zero.x, zero.y);
        }

        /// <summary>
        /// <para>Highlight the whole InputField.</para>
        /// </summary>
        protected void SelectAll()
        {
            this.caretPositionInternal = this.text.Length;
            this.caretSelectPositionInternal = 0;
        }

        /// <summary>
        /// <para>Convenience function to make functionality to send the SubmitEvent easier.</para>
        /// </summary>
        protected void SendOnSubmit()
        {
            UISystemProfilerApi.AddMarker("InputField.onSubmit", this);
            if (this.onEndEdit != null)
            {
                this.onEndEdit.Invoke(this.m_Text);
            }
        }

        private void SendOnValueChanged()
        {
            UISystemProfilerApi.AddMarker("InputField.value", this);
            if (this.onValueChanged != null)
            {
                this.onValueChanged.Invoke(this.text);
            }
        }

        private void SendOnValueChangedAndUpdateLabel()
        {
            this.SendOnValueChanged();
            this.UpdateLabel();
        }

        private void SetCaretActive()
        {
            if (this.m_AllowInput)
            {
                if (this.m_CaretBlinkRate > 0f)
                {
                    if (this.m_BlinkCoroutine == null)
                    {
                        this.m_BlinkCoroutine = base.StartCoroutine(this.CaretBlink());
                    }
                }
                else
                {
                    this.m_CaretVisible = true;
                }
            }
        }

        private void SetCaretVisible()
        {
            if (this.m_AllowInput)
            {
                this.m_CaretVisible = true;
                this.m_BlinkStartTime = Time.unscaledTime;
                this.SetCaretActive();
            }
        }

        private void SetDrawRangeToContainCaretPosition(int caretPos)
        {
            if (this.cachedInputTextGenerator.lineCount > 0)
            {
                Vector2 size = this.cachedInputTextGenerator.rectExtents.size;
                if (!this.multiLine)
                {
                    IList<UICharInfo> characters = this.cachedInputTextGenerator.characters;
                    if (this.m_DrawEnd > this.cachedInputTextGenerator.characterCountVisible)
                    {
                        this.m_DrawEnd = this.cachedInputTextGenerator.characterCountVisible;
                    }
                    float num9 = 0f;
                    if ((caretPos <= this.m_DrawEnd) && ((caretPos != this.m_DrawEnd) || (this.m_DrawStart <= 0)))
                    {
                        if (caretPos < this.m_DrawStart)
                        {
                            this.m_DrawStart = caretPos;
                        }
                        this.m_DrawEnd = this.m_DrawStart;
                    }
                    else
                    {
                        this.m_DrawEnd = caretPos;
                        this.m_DrawStart = this.m_DrawEnd - 1;
                        while (this.m_DrawStart >= 0)
                        {
                            UICharInfo info13 = characters[this.m_DrawStart];
                            if ((num9 + info13.charWidth) > size.x)
                            {
                                break;
                            }
                            UICharInfo info14 = characters[this.m_DrawStart];
                            num9 += info14.charWidth;
                            this.m_DrawStart--;
                        }
                        this.m_DrawStart++;
                    }
                    while (this.m_DrawEnd < this.cachedInputTextGenerator.characterCountVisible)
                    {
                        UICharInfo info15 = characters[this.m_DrawEnd];
                        num9 += info15.charWidth;
                        if (num9 > size.x)
                        {
                            break;
                        }
                        this.m_DrawEnd++;
                    }
                }
                else
                {
                    IList<UILineInfo> lines = this.cachedInputTextGenerator.lines;
                    int line = this.DetermineCharacterLine(caretPos, this.cachedInputTextGenerator);
                    if (caretPos <= this.m_DrawEnd)
                    {
                        if (caretPos < this.m_DrawStart)
                        {
                            this.m_DrawStart = GetLineStartPosition(this.cachedInputTextGenerator, line);
                        }
                        int num5 = this.DetermineCharacterLine(this.m_DrawStart, this.cachedInputTextGenerator);
                        int num6 = num5;
                        UILineInfo info5 = lines[num5];
                        float topY = info5.topY;
                        UILineInfo info6 = lines[num6];
                        UILineInfo info7 = lines[num6];
                        float num8 = info6.topY - info7.height;
                        if (num6 == (lines.Count - 1))
                        {
                            UILineInfo info8 = lines[num6];
                            num8 += info8.leading;
                        }
                        while (num6 < (lines.Count - 1))
                        {
                            UILineInfo info9 = lines[num6 + 1];
                            UILineInfo info10 = lines[num6 + 1];
                            num8 = info9.topY - info10.height;
                            if ((num6 + 1) == (lines.Count - 1))
                            {
                                UILineInfo info11 = lines[num6 + 1];
                                num8 += info11.leading;
                            }
                            if ((topY - num8) > size.y)
                            {
                                break;
                            }
                            num6++;
                        }
                        this.m_DrawEnd = GetLineEndPosition(this.cachedInputTextGenerator, num6);
                        while (num5 > 0)
                        {
                            UILineInfo info12 = lines[num5 - 1];
                            if ((info12.topY - num8) > size.y)
                            {
                                break;
                            }
                            num5--;
                        }
                        this.m_DrawStart = GetLineStartPosition(this.cachedInputTextGenerator, num5);
                    }
                    else
                    {
                        this.m_DrawEnd = GetLineEndPosition(this.cachedInputTextGenerator, line);
                        UILineInfo info = lines[line];
                        UILineInfo info2 = lines[line];
                        float num2 = info.topY - info2.height;
                        if (line == (lines.Count - 1))
                        {
                            UILineInfo info3 = lines[line];
                            num2 += info3.leading;
                        }
                        int num3 = line;
                        while (num3 > 0)
                        {
                            UILineInfo info4 = lines[num3 - 1];
                            if ((info4.topY - num2) > size.y)
                            {
                                break;
                            }
                            num3--;
                        }
                        this.m_DrawStart = GetLineStartPosition(this.cachedInputTextGenerator, num3);
                    }
                }
            }
        }

        private void SetToCustom()
        {
            if (this.contentType != ContentType.Custom)
            {
                this.contentType = ContentType.Custom;
            }
        }

        private void SetToCustomIfContentTypeIsNot(params ContentType[] allowedContentTypes)
        {
            if (this.contentType != ContentType.Custom)
            {
                for (int i = 0; i < allowedContentTypes.Length; i++)
                {
                    if (this.contentType == allowedContentTypes[i])
                    {
                        return;
                    }
                }
                this.contentType = ContentType.Custom;
            }
        }

        Transform ICanvasElement.get_transform() => 
            base.transform;

        private void UpdateCaretFromKeyboard()
        {
            RangeInt selection = this.m_Keyboard.selection;
            int start = selection.start;
            int end = selection.end;
            bool flag = false;
            if (this.caretPositionInternal != start)
            {
                flag = true;
                this.caretPositionInternal = start;
            }
            if (this.caretSelectPositionInternal != end)
            {
                this.caretSelectPositionInternal = end;
                flag = true;
            }
            if (flag)
            {
                this.m_BlinkStartTime = Time.unscaledTime;
                this.UpdateLabel();
            }
        }

        private void UpdateCaretMaterial()
        {
            if ((this.m_TextComponent != null) && (this.m_CachedInputRenderer != null))
            {
                this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
            }
        }

        private void UpdateGeometry()
        {
            if (Application.isPlaying && this.shouldHideMobileInput)
            {
                if ((this.m_CachedInputRenderer == null) && (this.m_TextComponent != null))
                {
                    System.Type[] components = new System.Type[] { typeof(RectTransform), typeof(CanvasRenderer) };
                    GameObject obj2 = new GameObject(base.transform.name + " Input Caret", components) {
                        hideFlags = HideFlags.DontSave
                    };
                    obj2.transform.SetParent(this.m_TextComponent.transform.parent);
                    obj2.transform.SetAsFirstSibling();
                    obj2.layer = base.gameObject.layer;
                    this.caretRectTrans = obj2.GetComponent<RectTransform>();
                    this.m_CachedInputRenderer = obj2.GetComponent<CanvasRenderer>();
                    this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
                    obj2.AddComponent<LayoutElement>().ignoreLayout = true;
                    this.AssignPositioningIfNeeded();
                }
                if (this.m_CachedInputRenderer != null)
                {
                    this.OnFillVBO(this.mesh);
                    this.m_CachedInputRenderer.SetMesh(this.mesh);
                }
            }
        }

        /// <summary>
        /// <para>Update the Text associated with this input field.</para>
        /// </summary>
        protected void UpdateLabel()
        {
            if (((this.m_TextComponent != null) && (this.m_TextComponent.font != null)) && !this.m_PreventFontCallback)
            {
                string text;
                string str2;
                this.m_PreventFontCallback = true;
                if (this.compositionString.Length > 0)
                {
                    text = this.text.Substring(0, this.m_CaretPosition) + this.compositionString + this.text.Substring(this.m_CaretPosition);
                }
                else
                {
                    text = this.text;
                }
                if (this.inputType == InputType.Password)
                {
                    str2 = new string(this.asteriskChar, text.Length);
                }
                else
                {
                    str2 = text;
                }
                bool flag = string.IsNullOrEmpty(text);
                if (this.m_Placeholder != null)
                {
                    this.m_Placeholder.enabled = flag;
                }
                if (!this.m_AllowInput)
                {
                    this.m_DrawStart = 0;
                    this.m_DrawEnd = this.m_Text.Length;
                }
                if (!flag)
                {
                    Vector2 size = this.m_TextComponent.rectTransform.rect.size;
                    TextGenerationSettings generationSettings = this.m_TextComponent.GetGenerationSettings(size);
                    generationSettings.generateOutOfBounds = true;
                    this.cachedInputTextGenerator.PopulateWithErrors(str2, generationSettings, base.gameObject);
                    this.SetDrawRangeToContainCaretPosition(this.caretSelectPositionInternal);
                    str2 = str2.Substring(this.m_DrawStart, Mathf.Min(this.m_DrawEnd, str2.Length) - this.m_DrawStart);
                    this.SetCaretVisible();
                }
                this.m_TextComponent.text = str2;
                this.MarkGeometryAsDirty();
                this.m_PreventFontCallback = false;
            }
        }

        /// <summary>
        /// <para>Predefined validation functionality for different characterValidation types.</para>
        /// </summary>
        /// <param name="text">The whole text string to validate.</param>
        /// <param name="pos">The position at which the current character is being inserted.</param>
        /// <param name="ch">The character that is being inserted.</param>
        /// <returns>
        /// <para>The character that should be inserted.</para>
        /// </returns>
        protected char Validate(string text, int pos, char ch)
        {
            if ((this.characterValidation == CharacterValidation.None) || !base.enabled)
            {
                return ch;
            }
            if ((this.characterValidation == CharacterValidation.Integer) || (this.characterValidation == CharacterValidation.Decimal))
            {
                bool flag = ((pos == 0) && (text.Length > 0)) && (text[0] == '-');
                bool flag2 = (this.caretPositionInternal == 0) || (this.caretSelectPositionInternal == 0);
                if (!flag)
                {
                    if ((ch >= '0') && (ch <= '9'))
                    {
                        return ch;
                    }
                    if ((ch == '-') && ((pos == 0) || flag2))
                    {
                        return ch;
                    }
                    if (((ch == '.') && (this.characterValidation == CharacterValidation.Decimal)) && !text.Contains("."))
                    {
                        return ch;
                    }
                }
            }
            else if (this.characterValidation == CharacterValidation.Alphanumeric)
            {
                if ((ch >= 'A') && (ch <= 'Z'))
                {
                    return ch;
                }
                if ((ch >= 'a') && (ch <= 'z'))
                {
                    return ch;
                }
                if ((ch >= '0') && (ch <= '9'))
                {
                    return ch;
                }
            }
            else if (this.characterValidation == CharacterValidation.Name)
            {
                if (char.IsLetter(ch))
                {
                    if (char.IsLower(ch) && ((pos == 0) || (text[pos - 1] == ' ')))
                    {
                        return char.ToUpper(ch);
                    }
                    if ((char.IsUpper(ch) && (pos > 0)) && ((text[pos - 1] != ' ') && (text[pos - 1] != '\'')))
                    {
                        return char.ToLower(ch);
                    }
                    return ch;
                }
                if ((ch == '\'') && ((!text.Contains("'") && ((pos <= 0) || ((text[pos - 1] != ' ') && (text[pos - 1] != '\'')))) && ((pos >= text.Length) || ((text[pos] != ' ') && (text[pos] != '\'')))))
                {
                    return ch;
                }
                if ((ch == ' ') && (((pos <= 0) || ((text[pos - 1] != ' ') && (text[pos - 1] != '\''))) && ((pos >= text.Length) || ((text[pos] != ' ') && (text[pos] != '\'')))))
                {
                    return ch;
                }
            }
            else if (this.characterValidation == CharacterValidation.EmailAddress)
            {
                if ((ch >= 'A') && (ch <= 'Z'))
                {
                    return ch;
                }
                if ((ch >= 'a') && (ch <= 'z'))
                {
                    return ch;
                }
                if ((ch >= '0') && (ch <= '9'))
                {
                    return ch;
                }
                if ((ch == '@') && (text.IndexOf('@') == -1))
                {
                    return ch;
                }
                if ("!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1)
                {
                    return ch;
                }
                if (ch == '.')
                {
                    char ch3 = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
                    char ch4 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
                    if ((ch3 != '.') && (ch4 != '.'))
                    {
                        return ch;
                    }
                }
            }
            return '\0';
        }

        /// <summary>
        /// <para>The character used for password fields.</para>
        /// </summary>
        public char asteriskChar
        {
            get => 
                this.m_AsteriskChar;
            set
            {
                if (SetPropertyUtility.SetStruct<char>(ref this.m_AsteriskChar, value))
                {
                    this.UpdateLabel();
                }
            }
        }

        protected TextGenerator cachedInputTextGenerator
        {
            get
            {
                if (this.m_InputTextCache == null)
                {
                    this.m_InputTextCache = new TextGenerator();
                }
                return this.m_InputTextCache;
            }
        }

        /// <summary>
        /// <para>The blinking rate of the input caret, defined as the number of times the blink cycle occurs per second.</para>
        /// </summary>
        public float caretBlinkRate
        {
            get => 
                this.m_CaretBlinkRate;
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_CaretBlinkRate, value) && this.m_AllowInput)
                {
                    this.SetCaretActive();
                }
            }
        }

        /// <summary>
        /// <para>The custom caret color used if customCaretColor is set.</para>
        /// </summary>
        public Color caretColor
        {
            get => 
                (!this.customCaretColor ? this.textComponent.color : this.m_CaretColor);
            set
            {
                if (SetPropertyUtility.SetColor(ref this.m_CaretColor, value))
                {
                    this.MarkGeometryAsDirty();
                }
            }
        }

        /// <summary>
        /// <para>Current InputField caret position (also selection tail).</para>
        /// </summary>
        public int caretPosition
        {
            get => 
                (this.m_CaretSelectPosition + this.compositionString.Length);
            set
            {
                this.selectionAnchorPosition = value;
                this.selectionFocusPosition = value;
            }
        }

        protected int caretPositionInternal
        {
            get => 
                (this.m_CaretPosition + this.compositionString.Length);
            set
            {
                this.m_CaretPosition = value;
                this.ClampPos(ref this.m_CaretPosition);
            }
        }

        /// <summary>
        /// <para>Current InputField selection head.</para>
        /// </summary>
        [Obsolete("caretSelectPosition has been deprecated. Use selectionFocusPosition instead (UnityUpgradable) -> selectionFocusPosition", true)]
        public int caretSelectPosition
        {
            get => 
                this.selectionFocusPosition;
            protected set
            {
                this.selectionFocusPosition = value;
            }
        }

        protected int caretSelectPositionInternal
        {
            get => 
                (this.m_CaretSelectPosition + this.compositionString.Length);
            set
            {
                this.m_CaretSelectPosition = value;
                this.ClampPos(ref this.m_CaretSelectPosition);
            }
        }

        /// <summary>
        /// <para>The width of the caret in pixels.</para>
        /// </summary>
        public int caretWidth
        {
            get => 
                this.m_CaretWidth;
            set
            {
                if (SetPropertyUtility.SetStruct<int>(ref this.m_CaretWidth, value))
                {
                    this.MarkGeometryAsDirty();
                }
            }
        }

        /// <summary>
        /// <para>How many characters the input field is limited to. 0 = infinite.</para>
        /// </summary>
        public int characterLimit
        {
            get => 
                this.m_CharacterLimit;
            set
            {
                if (SetPropertyUtility.SetStruct<int>(ref this.m_CharacterLimit, Math.Max(0, value)))
                {
                    this.UpdateLabel();
                }
            }
        }

        /// <summary>
        /// <para>The type of validation to perform on a character.</para>
        /// </summary>
        public CharacterValidation characterValidation
        {
            get => 
                this.m_CharacterValidation;
            set
            {
                if (SetPropertyUtility.SetStruct<CharacterValidation>(ref this.m_CharacterValidation, value))
                {
                    this.SetToCustom();
                }
            }
        }

        private static string clipboard
        {
            get => 
                GUIUtility.systemCopyBuffer;
            set
            {
                GUIUtility.systemCopyBuffer = value;
            }
        }

        private string compositionString =>
            ((this.input == null) ? Input.compositionString : this.input.compositionString);

        /// <summary>
        /// <para>Specifies the type of the input text content.</para>
        /// </summary>
        public ContentType contentType
        {
            get => 
                this.m_ContentType;
            set
            {
                if (SetPropertyUtility.SetStruct<ContentType>(ref this.m_ContentType, value))
                {
                    this.EnforceContentType();
                }
            }
        }

        /// <summary>
        /// <para>Should a custom caret color be used or should the textComponent.color be used.</para>
        /// </summary>
        public bool customCaretColor
        {
            get => 
                this.m_CustomCaretColor;
            set
            {
                if (this.m_CustomCaretColor != value)
                {
                    this.m_CustomCaretColor = value;
                    this.MarkGeometryAsDirty();
                }
            }
        }

        public virtual float flexibleHeight =>
            -1f;

        public virtual float flexibleWidth =>
            -1f;

        private bool hasSelection =>
            (this.caretPositionInternal != this.caretSelectPositionInternal);

        private BaseInput input
        {
            get
            {
                if ((EventSystem.current != null) && (EventSystem.current.currentInputModule != null))
                {
                    return EventSystem.current.currentInputModule.input;
                }
                return null;
            }
        }

        /// <summary>
        /// <para>The type of input expected. See InputField.InputType.</para>
        /// </summary>
        public InputType inputType
        {
            get => 
                this.m_InputType;
            set
            {
                if (SetPropertyUtility.SetStruct<InputType>(ref this.m_InputType, value))
                {
                    this.SetToCustom();
                }
            }
        }

        /// <summary>
        /// <para>Does the InputField currently have focus and is able to process events.</para>
        /// </summary>
        public bool isFocused =>
            this.m_AllowInput;

        /// <summary>
        /// <para>They type of mobile keyboard that will be used.</para>
        /// </summary>
        public TouchScreenKeyboardType keyboardType
        {
            get => 
                this.m_KeyboardType;
            set
            {
                if ((EditorUserBuildSettings.activeBuildTarget != BuildTarget.WiiU) && (value == TouchScreenKeyboardType.NintendoNetworkAccount))
                {
                    UnityEngine.Debug.LogWarning("Invalid InputField.keyboardType value set. TouchScreenKeyboardType.NintendoNetworkAccount only applies to the Wii U. InputField.keyboardType will default to TouchScreenKeyboardType.Default .");
                }
                if (SetPropertyUtility.SetStruct<TouchScreenKeyboardType>(ref this.m_KeyboardType, value))
                {
                    this.SetToCustom();
                }
            }
        }

        public virtual int layoutPriority =>
            1;

        /// <summary>
        /// <para>The LineType used by the InputField.</para>
        /// </summary>
        public LineType lineType
        {
            get => 
                this.m_LineType;
            set
            {
                if (SetPropertyUtility.SetStruct<LineType>(ref this.m_LineType, value))
                {
                    ContentType[] allowedContentTypes = new ContentType[2];
                    allowedContentTypes[1] = ContentType.Autocorrected;
                    this.SetToCustomIfContentTypeIsNot(allowedContentTypes);
                    this.EnforceTextHOverflow();
                }
            }
        }

        protected Mesh mesh
        {
            get
            {
                if (this.m_Mesh == null)
                {
                    this.m_Mesh = new Mesh();
                }
                return this.m_Mesh;
            }
        }

        public virtual float minHeight =>
            0f;

        public virtual float minWidth =>
            0f;

        /// <summary>
        /// <para>If the input field supports multiple lines.</para>
        /// </summary>
        public bool multiLine =>
            ((this.m_LineType == LineType.MultiLineNewline) || (this.lineType == LineType.MultiLineSubmit));

        /// <summary>
        /// <para>The Unity Event to call when editing has ended.</para>
        /// </summary>
        public SubmitEvent onEndEdit
        {
            get => 
                this.m_OnEndEdit;
            set
            {
                SetPropertyUtility.SetClass<SubmitEvent>(ref this.m_OnEndEdit, value);
            }
        }

        /// <summary>
        /// <para>The function to call to validate the input characters.</para>
        /// </summary>
        public OnValidateInput onValidateInput
        {
            get => 
                this.m_OnValidateInput;
            set
            {
                SetPropertyUtility.SetClass<OnValidateInput>(ref this.m_OnValidateInput, value);
            }
        }

        /// <summary>
        /// <para>Accessor to the OnChangeEvent.</para>
        /// </summary>
        [Obsolete("onValueChange has been renamed to onValueChanged")]
        public OnChangeEvent onValueChange
        {
            get => 
                this.onValueChanged;
            set
            {
                this.onValueChanged = value;
            }
        }

        /// <summary>
        /// <para>Accessor to the OnChangeEvent.</para>
        /// </summary>
        public OnChangeEvent onValueChanged
        {
            get => 
                this.m_OnValueChanged;
            set
            {
                SetPropertyUtility.SetClass<OnChangeEvent>(ref this.m_OnValueChanged, value);
            }
        }

        /// <summary>
        /// <para>This is an optional ‘empty’ graphic to show that the InputField text field is empty. Note that this ‘empty' graphic still displays even when the InputField is selected (that is; when there is focus on it).
        /// 
        /// A placeholder graphic can be used to show subtle hints or make it more obvious that the control is an InputField.</para>
        /// </summary>
        public Graphic placeholder
        {
            get => 
                this.m_Placeholder;
            set
            {
                SetPropertyUtility.SetClass<Graphic>(ref this.m_Placeholder, value);
            }
        }

        public virtual float preferredHeight
        {
            get
            {
                if (this.textComponent == null)
                {
                    return 0f;
                }
                TextGenerationSettings generationSettings = this.textComponent.GetGenerationSettings(new Vector2(this.textComponent.rectTransform.rect.size.x, 0f));
                return (this.textComponent.cachedTextGeneratorForLayout.GetPreferredHeight(this.m_Text, generationSettings) / this.textComponent.pixelsPerUnit);
            }
        }

        public virtual float preferredWidth
        {
            get
            {
                if (this.textComponent == null)
                {
                    return 0f;
                }
                TextGenerationSettings generationSettings = this.textComponent.GetGenerationSettings(Vector2.zero);
                return (this.textComponent.cachedTextGeneratorForLayout.GetPreferredWidth(this.m_Text, generationSettings) / this.textComponent.pixelsPerUnit);
            }
        }

        /// <summary>
        /// <para>Set the InputField to be read only.</para>
        /// </summary>
        public bool readOnly
        {
            get => 
                this.m_ReadOnly;
            set
            {
                this.m_ReadOnly = value;
            }
        }

        /// <summary>
        /// <para>The beginning point of the selection.</para>
        /// </summary>
        public int selectionAnchorPosition
        {
            get => 
                (this.m_CaretPosition + this.compositionString.Length);
            set
            {
                if (this.compositionString.Length == 0)
                {
                    this.m_CaretPosition = value;
                    this.ClampPos(ref this.m_CaretPosition);
                }
            }
        }

        /// <summary>
        /// <para>The color of the highlight to show which characters are selected.</para>
        /// </summary>
        public Color selectionColor
        {
            get => 
                this.m_SelectionColor;
            set
            {
                if (SetPropertyUtility.SetColor(ref this.m_SelectionColor, value))
                {
                    this.MarkGeometryAsDirty();
                }
            }
        }

        /// <summary>
        /// <para>The the end point of the selection.</para>
        /// </summary>
        public int selectionFocusPosition
        {
            get => 
                (this.m_CaretSelectPosition + this.compositionString.Length);
            set
            {
                if (this.compositionString.Length == 0)
                {
                    this.m_CaretSelectPosition = value;
                    this.ClampPos(ref this.m_CaretSelectPosition);
                }
            }
        }

        private bool shouldActivateOnSelect =>
            (Application.platform != RuntimePlatform.tvOS);

        /// <summary>
        /// <para>Should the mobile keyboard input be hidden.</para>
        /// </summary>
        public bool shouldHideMobileInput
        {
            get
            {
                RuntimePlatform platform = Application.platform;
                switch (platform)
                {
                    case RuntimePlatform.IPhonePlayer:
                    case RuntimePlatform.Android:
                        break;

                    default:
                        if ((platform != RuntimePlatform.TizenPlayer) && (platform != RuntimePlatform.tvOS))
                        {
                            return true;
                        }
                        break;
                }
                return this.m_HideMobileInput;
            }
            set
            {
                SetPropertyUtility.SetStruct<bool>(ref this.m_HideMobileInput, value);
            }
        }

        /// <summary>
        /// <para>The current value of the input field.</para>
        /// </summary>
        public string text
        {
            get => 
                this.m_Text;
            set
            {
                if (this.text != value)
                {
                    if (value == null)
                    {
                        value = "";
                    }
                    value = value.Replace("\0", string.Empty);
                    if (this.m_LineType == LineType.SingleLine)
                    {
                        value = value.Replace("\n", "").Replace("\t", "");
                    }
                    if ((this.onValidateInput != null) || (this.characterValidation != CharacterValidation.None))
                    {
                        OnValidateInput input;
                        this.m_Text = "";
                        OnValidateInput onValidateInput = this.onValidateInput;
                        if (onValidateInput != null)
                        {
                            input = onValidateInput;
                        }
                        else
                        {
                            input = new OnValidateInput(this.Validate);
                        }
                        this.m_CaretPosition = this.m_CaretSelectPosition = value.Length;
                        int num2 = (this.characterLimit <= 0) ? value.Length : Math.Min(this.characterLimit, value.Length);
                        for (int i = 0; i < num2; i++)
                        {
                            char ch = input(this.m_Text, this.m_Text.Length, value[i]);
                            if (ch != '\0')
                            {
                                this.m_Text = this.m_Text + ch;
                            }
                        }
                    }
                    else
                    {
                        this.m_Text = ((this.characterLimit <= 0) || (value.Length <= this.characterLimit)) ? value : value.Substring(0, this.characterLimit);
                    }
                    if (!Application.isPlaying)
                    {
                        this.SendOnValueChangedAndUpdateLabel();
                    }
                    else
                    {
                        if (this.m_Keyboard != null)
                        {
                            this.m_Keyboard.text = this.m_Text;
                        }
                        if (this.m_CaretPosition > this.m_Text.Length)
                        {
                            this.m_CaretPosition = this.m_CaretSelectPosition = this.m_Text.Length;
                        }
                        else if (this.m_CaretSelectPosition > this.m_Text.Length)
                        {
                            this.m_CaretSelectPosition = this.m_Text.Length;
                        }
                        this.SendOnValueChangedAndUpdateLabel();
                    }
                }
            }
        }

        /// <summary>
        /// <para>The Text component that is going to be used to render the text to screen.</para>
        /// </summary>
        public Text textComponent
        {
            get => 
                this.m_TextComponent;
            set
            {
                if (SetPropertyUtility.SetClass<Text>(ref this.m_TextComponent, value))
                {
                    this.EnforceTextHOverflow();
                }
            }
        }

        /// <summary>
        /// <para>If the UI.InputField was canceled and will revert back to the original text upon DeactivateInputField.</para>
        /// </summary>
        public bool wasCanceled =>
            this.m_WasCanceled;

        [CompilerGenerated]
        private sealed class <CaretBlink>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal InputField $this;
            internal float <blinkPeriod>__1;
            internal bool <blinkState>__1;

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
                        this.$this.m_CaretVisible = true;
                        this.$current = null;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        goto Label_0130;

                    case 1:
                    case 2:
                        if (this.$this.isFocused && (this.$this.m_CaretBlinkRate > 0f))
                        {
                            this.<blinkPeriod>__1 = 1f / this.$this.m_CaretBlinkRate;
                            this.<blinkState>__1 = ((Time.unscaledTime - this.$this.m_BlinkStartTime) % this.<blinkPeriod>__1) < (this.<blinkPeriod>__1 / 2f);
                            if (this.$this.m_CaretVisible != this.<blinkState>__1)
                            {
                                this.$this.m_CaretVisible = this.<blinkState>__1;
                                if (!this.$this.hasSelection)
                                {
                                    this.$this.MarkGeometryAsDirty();
                                }
                            }
                            this.$current = null;
                            if (!this.$disposing)
                            {
                                this.$PC = 2;
                            }
                            goto Label_0130;
                        }
                        this.$this.m_BlinkCoroutine = null;
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0130:
                return true;
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
        private sealed class <MouseDragOutsideRect>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal bool $disposing;
            internal int $PC;
            internal InputField $this;
            internal float <delay>__1;
            internal Vector2 <localMousePos>__1;
            internal Rect <rect>__1;
            internal PointerEventData eventData;

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
                    case 1:
                        if (this.$this.m_UpdateDrag && this.$this.m_DragPositionOutOfBounds)
                        {
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.$this.textComponent.rectTransform, this.eventData.position, this.eventData.pressEventCamera, out this.<localMousePos>__1);
                            this.<rect>__1 = this.$this.textComponent.rectTransform.rect;
                            if (this.$this.multiLine)
                            {
                                if (this.<localMousePos>__1.y > this.<rect>__1.yMax)
                                {
                                    this.$this.MoveUp(true, true);
                                }
                                else if (this.<localMousePos>__1.y < this.<rect>__1.yMin)
                                {
                                    this.$this.MoveDown(true, true);
                                }
                            }
                            else if (this.<localMousePos>__1.x < this.<rect>__1.xMin)
                            {
                                this.$this.MoveLeft(true, false);
                            }
                            else if (this.<localMousePos>__1.x > this.<rect>__1.xMax)
                            {
                                this.$this.MoveRight(true, false);
                            }
                            this.$this.UpdateLabel();
                            this.<delay>__1 = !this.$this.multiLine ? 0.05f : 0.1f;
                            this.$current = new WaitForSecondsRealtime(this.<delay>__1);
                            if (!this.$disposing)
                            {
                                this.$PC = 1;
                            }
                            return true;
                        }
                        this.$this.m_DragCoroutine = null;
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

        /// <summary>
        /// <para>The type of characters that are allowed to be added to the string.</para>
        /// </summary>
        public enum CharacterValidation
        {
            None,
            Integer,
            Decimal,
            Alphanumeric,
            Name,
            EmailAddress
        }

        /// <summary>
        /// <para>Specifies the type of the input text content.</para>
        /// </summary>
        public enum ContentType
        {
            Standard,
            Autocorrected,
            IntegerNumber,
            DecimalNumber,
            Alphanumeric,
            Name,
            EmailAddress,
            Password,
            Pin,
            Custom
        }

        protected enum EditState
        {
            Continue,
            Finish
        }

        /// <summary>
        /// <para>Type of data expected by the input field.</para>
        /// </summary>
        public enum InputType
        {
            Standard,
            AutoCorrect,
            Password
        }

        /// <summary>
        /// <para>The LineType is used to describe the behavior of the InputField.</para>
        /// </summary>
        public enum LineType
        {
            SingleLine,
            MultiLineSubmit,
            MultiLineNewline
        }

        /// <summary>
        /// <para>The callback sent anytime the Inputfield is updated.</para>
        /// </summary>
        [Serializable]
        public class OnChangeEvent : UnityEvent<string>
        {
        }

        /// <summary>
        /// <para>Custom validation callback.</para>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="charIndex"></param>
        /// <param name="addedChar"></param>
        public delegate char OnValidateInput(string text, int charIndex, char addedChar);

        /// <summary>
        /// <para>Unity Event with a inputfield as a param.</para>
        /// </summary>
        [Serializable]
        public class SubmitEvent : UnityEvent<string>
        {
        }
    }
}

