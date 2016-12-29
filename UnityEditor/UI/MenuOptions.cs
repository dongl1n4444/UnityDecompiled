namespace UnityEditor.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    internal static class MenuOptions
    {
        private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
        private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
        private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
        private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string kKnobPath = "UI/Skin/Knob.psd";
        private const string kMaskPath = "UI/Skin/UIMask.psd";
        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        private const string kUILayerName = "UI";
        private static DefaultControls.Resources s_StandardResources;

        [UnityEditor.MenuItem("GameObject/UI/Button", false, 0x7ee)]
        public static void AddButton(MenuCommand menuCommand)
        {
            PlaceUIElementRoot(DefaultControls.CreateButton(GetStandardResources()), menuCommand);
        }

        [UnityEditor.MenuItem("GameObject/UI/Canvas", false, 0x80c)]
        public static void AddCanvas(MenuCommand menuCommand)
        {
            GameObject child = CreateNewUI();
            GameObjectUtility.SetParentAndAlign(child, menuCommand.context as GameObject);
            if (child.transform.parent is RectTransform)
            {
                RectTransform transform = child.transform as RectTransform;
                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.anchoredPosition = Vector2.zero;
                transform.sizeDelta = Vector2.zero;
            }
            Selection.activeGameObject = child;
        }

        [UnityEditor.MenuItem("GameObject/UI/Dropdown", false, 0x7f3)]
        public static void AddDropdown(MenuCommand menuCommand)
        {
            PlaceUIElementRoot(DefaultControls.CreateDropdown(GetStandardResources()), menuCommand);
        }

        [UnityEditor.MenuItem("GameObject/UI/Image", false, 0x7d1)]
        public static void AddImage(MenuCommand menuCommand)
        {
            PlaceUIElementRoot(DefaultControls.CreateImage(GetStandardResources()), menuCommand);
        }

        [UnityEditor.MenuItem("GameObject/UI/Input Field", false, 0x7f4)]
        public static void AddInputField(MenuCommand menuCommand)
        {
            PlaceUIElementRoot(DefaultControls.CreateInputField(GetStandardResources()), menuCommand);
        }

        [UnityEditor.MenuItem("GameObject/UI/Panel", false, 0x80d)]
        public static void AddPanel(MenuCommand menuCommand)
        {
            GameObject element = DefaultControls.CreatePanel(GetStandardResources());
            PlaceUIElementRoot(element, menuCommand);
            RectTransform component = element.GetComponent<RectTransform>();
            component.anchoredPosition = Vector2.zero;
            component.sizeDelta = Vector2.zero;
        }

        [UnityEditor.MenuItem("GameObject/UI/Raw Image", false, 0x7d2)]
        public static void AddRawImage(MenuCommand menuCommand)
        {
            PlaceUIElementRoot(DefaultControls.CreateRawImage(GetStandardResources()), menuCommand);
        }

        [UnityEditor.MenuItem("GameObject/UI/Scrollbar", false, 0x7f2)]
        public static void AddScrollbar(MenuCommand menuCommand)
        {
            PlaceUIElementRoot(DefaultControls.CreateScrollbar(GetStandardResources()), menuCommand);
        }

        [UnityEditor.MenuItem("GameObject/UI/Scroll View", false, 0x80e)]
        public static void AddScrollView(MenuCommand menuCommand)
        {
            PlaceUIElementRoot(DefaultControls.CreateScrollView(GetStandardResources()), menuCommand);
        }

        [UnityEditor.MenuItem("GameObject/UI/Slider", false, 0x7f1)]
        public static void AddSlider(MenuCommand menuCommand)
        {
            PlaceUIElementRoot(DefaultControls.CreateSlider(GetStandardResources()), menuCommand);
        }

        [UnityEditor.MenuItem("GameObject/UI/Text", false, 0x7d0)]
        public static void AddText(MenuCommand menuCommand)
        {
            PlaceUIElementRoot(DefaultControls.CreateText(GetStandardResources()), menuCommand);
        }

        [UnityEditor.MenuItem("GameObject/UI/Toggle", false, 0x7ef)]
        public static void AddToggle(MenuCommand menuCommand)
        {
            PlaceUIElementRoot(DefaultControls.CreateToggle(GetStandardResources()), menuCommand);
        }

        private static void CreateEventSystem(bool select)
        {
            CreateEventSystem(select, null);
        }

        [UnityEditor.MenuItem("GameObject/UI/Event System", false, 0x834)]
        public static void CreateEventSystem(MenuCommand menuCommand)
        {
            GameObject context = menuCommand.context as GameObject;
            CreateEventSystem(true, context);
        }

        private static void CreateEventSystem(bool select, GameObject parent)
        {
            EventSystem system = UnityEngine.Object.FindObjectOfType<EventSystem>();
            if (system == null)
            {
                GameObject child = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(child, parent);
                system = child.AddComponent<EventSystem>();
                child.AddComponent<StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(child, "Create " + child.name);
            }
            if (select && (system != null))
            {
                Selection.activeGameObject = system.gameObject;
            }
        }

        public static GameObject CreateNewUI()
        {
            GameObject objectToUndo = new GameObject("Canvas") {
                layer = LayerMask.NameToLayer("UI")
            };
            objectToUndo.AddComponent<Canvas>().renderMode = UnityEngine.RenderMode.ScreenSpaceOverlay;
            objectToUndo.AddComponent<CanvasScaler>();
            objectToUndo.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(objectToUndo, "Create " + objectToUndo.name);
            CreateEventSystem(false);
            return objectToUndo;
        }

        public static GameObject GetOrCreateCanvasGameObject()
        {
            Canvas componentInParent = Selection.activeGameObject?.GetComponentInParent<Canvas>();
            if ((componentInParent != null) && componentInParent.gameObject.activeInHierarchy)
            {
                return componentInParent.gameObject;
            }
            componentInParent = UnityEngine.Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if ((componentInParent != null) && componentInParent.gameObject.activeInHierarchy)
            {
                return componentInParent.gameObject;
            }
            return CreateNewUI();
        }

        private static DefaultControls.Resources GetStandardResources()
        {
            if (s_StandardResources.standard == null)
            {
                s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
                s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
                s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd");
                s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
            }
            return s_StandardResources;
        }

        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            GameObject context = menuCommand.context as GameObject;
            if ((context == null) || (context.GetComponentInParent<Canvas>() == null))
            {
                context = GetOrCreateCanvasGameObject();
            }
            string uniqueNameForSibling = GameObjectUtility.GetUniqueNameForSibling(context.transform, element.name);
            element.name = uniqueNameForSibling;
            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
            Undo.SetTransformParent(element.transform, context.transform, "Parent " + element.name);
            GameObjectUtility.SetParentAndAlign(element, context);
            if (context != menuCommand.context)
            {
                SetPositionVisibleinSceneView(context.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());
            }
            Selection.activeGameObject = element;
        }

        private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            SceneView lastActiveSceneView = SceneView.lastActiveSceneView;
            if ((lastActiveSceneView == null) && (SceneView.sceneViews.Count > 0))
            {
                lastActiveSceneView = SceneView.sceneViews[0] as SceneView;
            }
            if ((lastActiveSceneView != null) && (lastActiveSceneView.camera != null))
            {
                Vector2 vector;
                Camera cam = lastActiveSceneView.camera;
                Vector3 zero = Vector3.zero;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2((float) (cam.pixelWidth / 2), (float) (cam.pixelHeight / 2)), cam, out vector))
                {
                    Vector3 vector13;
                    Vector3 vector22;
                    vector.x += canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                    vector.y += canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;
                    vector.x = Mathf.Clamp(vector.x, 0f, canvasRTransform.sizeDelta.x);
                    vector.y = Mathf.Clamp(vector.y, 0f, canvasRTransform.sizeDelta.y);
                    zero.x = vector.x - (canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x);
                    zero.y = vector.y - (canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y);
                    vector13.x = (canvasRTransform.sizeDelta.x * (0f - canvasRTransform.pivot.x)) + (itemTransform.sizeDelta.x * itemTransform.pivot.x);
                    vector13.y = (canvasRTransform.sizeDelta.y * (0f - canvasRTransform.pivot.y)) + (itemTransform.sizeDelta.y * itemTransform.pivot.y);
                    vector22.x = (canvasRTransform.sizeDelta.x * (1f - canvasRTransform.pivot.x)) - (itemTransform.sizeDelta.x * itemTransform.pivot.x);
                    vector22.y = (canvasRTransform.sizeDelta.y * (1f - canvasRTransform.pivot.y)) - (itemTransform.sizeDelta.y * itemTransform.pivot.y);
                    zero.x = Mathf.Clamp(zero.x, vector13.x, vector22.x);
                    zero.y = Mathf.Clamp(zero.y, vector13.y, vector22.y);
                }
                itemTransform.anchoredPosition = zero;
                itemTransform.localRotation = Quaternion.identity;
                itemTransform.localScale = Vector3.one;
            }
        }
    }
}

