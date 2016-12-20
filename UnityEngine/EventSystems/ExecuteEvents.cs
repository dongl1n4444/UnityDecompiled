namespace UnityEngine.EventSystems
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    /// <summary>
    /// <para>Helper class that can be used to send IEventSystemHandler events to GameObjects.</para>
    /// </summary>
    public static class ExecuteEvents
    {
        [CompilerGenerated]
        private static EventFunction<IPointerEnterHandler> <>f__mg$cache0;
        [CompilerGenerated]
        private static EventFunction<IPointerExitHandler> <>f__mg$cache1;
        [CompilerGenerated]
        private static EventFunction<ICancelHandler> <>f__mg$cache10;
        [CompilerGenerated]
        private static EventFunction<IPointerDownHandler> <>f__mg$cache2;
        [CompilerGenerated]
        private static EventFunction<IPointerUpHandler> <>f__mg$cache3;
        [CompilerGenerated]
        private static EventFunction<IPointerClickHandler> <>f__mg$cache4;
        [CompilerGenerated]
        private static EventFunction<IInitializePotentialDragHandler> <>f__mg$cache5;
        [CompilerGenerated]
        private static EventFunction<IBeginDragHandler> <>f__mg$cache6;
        [CompilerGenerated]
        private static EventFunction<IDragHandler> <>f__mg$cache7;
        [CompilerGenerated]
        private static EventFunction<IEndDragHandler> <>f__mg$cache8;
        [CompilerGenerated]
        private static EventFunction<IDropHandler> <>f__mg$cache9;
        [CompilerGenerated]
        private static EventFunction<IScrollHandler> <>f__mg$cacheA;
        [CompilerGenerated]
        private static EventFunction<IUpdateSelectedHandler> <>f__mg$cacheB;
        [CompilerGenerated]
        private static EventFunction<ISelectHandler> <>f__mg$cacheC;
        [CompilerGenerated]
        private static EventFunction<IDeselectHandler> <>f__mg$cacheD;
        [CompilerGenerated]
        private static EventFunction<IMoveHandler> <>f__mg$cacheE;
        [CompilerGenerated]
        private static EventFunction<ISubmitHandler> <>f__mg$cacheF;
        private static readonly EventFunction<IBeginDragHandler> s_BeginDragHandler;
        private static readonly EventFunction<ICancelHandler> s_CancelHandler;
        private static readonly EventFunction<IDeselectHandler> s_DeselectHandler;
        private static readonly EventFunction<IDragHandler> s_DragHandler;
        private static readonly EventFunction<IDropHandler> s_DropHandler;
        private static readonly EventFunction<IEndDragHandler> s_EndDragHandler;
        private static readonly ObjectPool<List<IEventSystemHandler>> s_HandlerListPool;
        private static readonly EventFunction<IInitializePotentialDragHandler> s_InitializePotentialDragHandler;
        private static readonly List<Transform> s_InternalTransformList;
        private static readonly EventFunction<IMoveHandler> s_MoveHandler;
        private static readonly EventFunction<IPointerClickHandler> s_PointerClickHandler;
        private static readonly EventFunction<IPointerDownHandler> s_PointerDownHandler;
        private static readonly EventFunction<IPointerEnterHandler> s_PointerEnterHandler;
        private static readonly EventFunction<IPointerExitHandler> s_PointerExitHandler;
        private static readonly EventFunction<IPointerUpHandler> s_PointerUpHandler;
        private static readonly EventFunction<IScrollHandler> s_ScrollHandler;
        private static readonly EventFunction<ISelectHandler> s_SelectHandler;
        private static readonly EventFunction<ISubmitHandler> s_SubmitHandler;
        private static readonly EventFunction<IUpdateSelectedHandler> s_UpdateSelectedHandler;

        static ExecuteEvents()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new EventFunction<IPointerEnterHandler>(ExecuteEvents.Execute);
            }
            s_PointerEnterHandler = <>f__mg$cache0;
            if (<>f__mg$cache1 == null)
            {
                <>f__mg$cache1 = new EventFunction<IPointerExitHandler>(ExecuteEvents.Execute);
            }
            s_PointerExitHandler = <>f__mg$cache1;
            if (<>f__mg$cache2 == null)
            {
                <>f__mg$cache2 = new EventFunction<IPointerDownHandler>(ExecuteEvents.Execute);
            }
            s_PointerDownHandler = <>f__mg$cache2;
            if (<>f__mg$cache3 == null)
            {
                <>f__mg$cache3 = new EventFunction<IPointerUpHandler>(ExecuteEvents.Execute);
            }
            s_PointerUpHandler = <>f__mg$cache3;
            if (<>f__mg$cache4 == null)
            {
                <>f__mg$cache4 = new EventFunction<IPointerClickHandler>(ExecuteEvents.Execute);
            }
            s_PointerClickHandler = <>f__mg$cache4;
            if (<>f__mg$cache5 == null)
            {
                <>f__mg$cache5 = new EventFunction<IInitializePotentialDragHandler>(ExecuteEvents.Execute);
            }
            s_InitializePotentialDragHandler = <>f__mg$cache5;
            if (<>f__mg$cache6 == null)
            {
                <>f__mg$cache6 = new EventFunction<IBeginDragHandler>(ExecuteEvents.Execute);
            }
            s_BeginDragHandler = <>f__mg$cache6;
            if (<>f__mg$cache7 == null)
            {
                <>f__mg$cache7 = new EventFunction<IDragHandler>(ExecuteEvents.Execute);
            }
            s_DragHandler = <>f__mg$cache7;
            if (<>f__mg$cache8 == null)
            {
                <>f__mg$cache8 = new EventFunction<IEndDragHandler>(ExecuteEvents.Execute);
            }
            s_EndDragHandler = <>f__mg$cache8;
            if (<>f__mg$cache9 == null)
            {
                <>f__mg$cache9 = new EventFunction<IDropHandler>(ExecuteEvents.Execute);
            }
            s_DropHandler = <>f__mg$cache9;
            if (<>f__mg$cacheA == null)
            {
                <>f__mg$cacheA = new EventFunction<IScrollHandler>(ExecuteEvents.Execute);
            }
            s_ScrollHandler = <>f__mg$cacheA;
            if (<>f__mg$cacheB == null)
            {
                <>f__mg$cacheB = new EventFunction<IUpdateSelectedHandler>(ExecuteEvents.Execute);
            }
            s_UpdateSelectedHandler = <>f__mg$cacheB;
            if (<>f__mg$cacheC == null)
            {
                <>f__mg$cacheC = new EventFunction<ISelectHandler>(ExecuteEvents.Execute);
            }
            s_SelectHandler = <>f__mg$cacheC;
            if (<>f__mg$cacheD == null)
            {
                <>f__mg$cacheD = new EventFunction<IDeselectHandler>(ExecuteEvents.Execute);
            }
            s_DeselectHandler = <>f__mg$cacheD;
            if (<>f__mg$cacheE == null)
            {
                <>f__mg$cacheE = new EventFunction<IMoveHandler>(ExecuteEvents.Execute);
            }
            s_MoveHandler = <>f__mg$cacheE;
            if (<>f__mg$cacheF == null)
            {
                <>f__mg$cacheF = new EventFunction<ISubmitHandler>(ExecuteEvents.Execute);
            }
            s_SubmitHandler = <>f__mg$cacheF;
            if (<>f__mg$cache10 == null)
            {
                <>f__mg$cache10 = new EventFunction<ICancelHandler>(ExecuteEvents.Execute);
            }
            s_CancelHandler = <>f__mg$cache10;
            s_HandlerListPool = new ObjectPool<List<IEventSystemHandler>>(null, new UnityAction<List<IEventSystemHandler>>(ExecuteEvents.<s_HandlerListPool>m__0));
            s_InternalTransformList = new List<Transform>(30);
        }

        [CompilerGenerated]
        private static void <s_HandlerListPool>m__0(List<IEventSystemHandler> l)
        {
            l.Clear();
        }

        public static bool CanHandleEvent<T>(GameObject go) where T: IEventSystemHandler
        {
            List<IEventSystemHandler> results = s_HandlerListPool.Get();
            GetEventList<T>(go, results);
            int count = results.Count;
            s_HandlerListPool.Release(results);
            return (count != 0);
        }

        private static void Execute(IBeginDragHandler handler, BaseEventData eventData)
        {
            handler.OnBeginDrag(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(ICancelHandler handler, BaseEventData eventData)
        {
            handler.OnCancel(eventData);
        }

        private static void Execute(IDeselectHandler handler, BaseEventData eventData)
        {
            handler.OnDeselect(eventData);
        }

        private static void Execute(IDragHandler handler, BaseEventData eventData)
        {
            handler.OnDrag(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IDropHandler handler, BaseEventData eventData)
        {
            handler.OnDrop(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IEndDragHandler handler, BaseEventData eventData)
        {
            handler.OnEndDrag(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IInitializePotentialDragHandler handler, BaseEventData eventData)
        {
            handler.OnInitializePotentialDrag(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IMoveHandler handler, BaseEventData eventData)
        {
            handler.OnMove(ValidateEventData<AxisEventData>(eventData));
        }

        private static void Execute(IPointerClickHandler handler, BaseEventData eventData)
        {
            handler.OnPointerClick(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IPointerDownHandler handler, BaseEventData eventData)
        {
            handler.OnPointerDown(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IPointerEnterHandler handler, BaseEventData eventData)
        {
            handler.OnPointerEnter(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IPointerExitHandler handler, BaseEventData eventData)
        {
            handler.OnPointerExit(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IPointerUpHandler handler, BaseEventData eventData)
        {
            handler.OnPointerUp(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(IScrollHandler handler, BaseEventData eventData)
        {
            handler.OnScroll(ValidateEventData<PointerEventData>(eventData));
        }

        private static void Execute(ISelectHandler handler, BaseEventData eventData)
        {
            handler.OnSelect(eventData);
        }

        private static void Execute(ISubmitHandler handler, BaseEventData eventData)
        {
            handler.OnSubmit(eventData);
        }

        private static void Execute(IUpdateSelectedHandler handler, BaseEventData eventData)
        {
            handler.OnUpdateSelected(eventData);
        }

        public static bool Execute<T>(GameObject target, BaseEventData eventData, EventFunction<T> functor) where T: IEventSystemHandler
        {
            List<IEventSystemHandler> results = s_HandlerListPool.Get();
            GetEventList<T>(target, results);
            for (int i = 0; i < results.Count; i++)
            {
                T local;
                try
                {
                    local = results[i];
                }
                catch (Exception exception)
                {
                    IEventSystemHandler handler = results[i];
                    Debug.LogException(new Exception(string.Format("Type {0} expected {1} received.", typeof(T).Name, handler.GetType().Name), exception));
                    continue;
                }
                try
                {
                    functor(local, eventData);
                }
                catch (Exception exception2)
                {
                    Debug.LogException(exception2);
                }
            }
            int count = results.Count;
            s_HandlerListPool.Release(results);
            return (count > 0);
        }

        public static GameObject ExecuteHierarchy<T>(GameObject root, BaseEventData eventData, EventFunction<T> callbackFunction) where T: IEventSystemHandler
        {
            GetEventChain(root, s_InternalTransformList);
            for (int i = 0; i < s_InternalTransformList.Count; i++)
            {
                Transform transform = s_InternalTransformList[i];
                if (Execute<T>(transform.gameObject, eventData, callbackFunction))
                {
                    return transform.gameObject;
                }
            }
            return null;
        }

        private static void GetEventChain(GameObject root, IList<Transform> eventChain)
        {
            eventChain.Clear();
            if (root != null)
            {
                for (Transform transform = root.transform; transform != null; transform = transform.parent)
                {
                    eventChain.Add(transform);
                }
            }
        }

        public static GameObject GetEventHandler<T>(GameObject root) where T: IEventSystemHandler
        {
            if (root != null)
            {
                for (Transform transform = root.transform; transform != null; transform = transform.parent)
                {
                    if (CanHandleEvent<T>(transform.gameObject))
                    {
                        return transform.gameObject;
                    }
                }
            }
            return null;
        }

        private static void GetEventList<T>(GameObject go, IList<IEventSystemHandler> results) where T: IEventSystemHandler
        {
            if (results == null)
            {
                throw new ArgumentException("Results array is null", "results");
            }
            if ((go != null) && go.activeInHierarchy)
            {
                List<Component> list = ListPool<Component>.Get();
                go.GetComponents<Component>(list);
                for (int i = 0; i < list.Count; i++)
                {
                    if (ShouldSendToComponent<T>(list[i]))
                    {
                        results.Add(list[i] as IEventSystemHandler);
                    }
                }
                ListPool<Component>.Release(list);
            }
        }

        private static bool ShouldSendToComponent<T>(Component component) where T: IEventSystemHandler
        {
            if (!(component is T))
            {
                return false;
            }
            Behaviour behaviour = component as Behaviour;
            if (behaviour != null)
            {
                return behaviour.isActiveAndEnabled;
            }
            return true;
        }

        public static T ValidateEventData<T>(BaseEventData data) where T: class
        {
            if (!(data is T))
            {
                throw new ArgumentException(string.Format("Invalid type: {0} passed to event expecting {1}", data.GetType(), typeof(T)));
            }
            return (data as T);
        }

        /// <summary>
        /// <para>IBeginDragHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IBeginDragHandler> beginDragHandler
        {
            get
            {
                return s_BeginDragHandler;
            }
        }

        /// <summary>
        /// <para>ICancelHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<ICancelHandler> cancelHandler
        {
            get
            {
                return s_CancelHandler;
            }
        }

        /// <summary>
        /// <para>IDeselectHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IDeselectHandler> deselectHandler
        {
            get
            {
                return s_DeselectHandler;
            }
        }

        /// <summary>
        /// <para>IDragHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IDragHandler> dragHandler
        {
            get
            {
                return s_DragHandler;
            }
        }

        /// <summary>
        /// <para>IDropHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IDropHandler> dropHandler
        {
            get
            {
                return s_DropHandler;
            }
        }

        /// <summary>
        /// <para>IEndDragHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IEndDragHandler> endDragHandler
        {
            get
            {
                return s_EndDragHandler;
            }
        }

        /// <summary>
        /// <para>IInitializePotentialDragHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IInitializePotentialDragHandler> initializePotentialDrag
        {
            get
            {
                return s_InitializePotentialDragHandler;
            }
        }

        /// <summary>
        /// <para>IMoveHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IMoveHandler> moveHandler
        {
            get
            {
                return s_MoveHandler;
            }
        }

        /// <summary>
        /// <para>IPointerClickHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IPointerClickHandler> pointerClickHandler
        {
            get
            {
                return s_PointerClickHandler;
            }
        }

        /// <summary>
        /// <para>IPointerDownHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IPointerDownHandler> pointerDownHandler
        {
            get
            {
                return s_PointerDownHandler;
            }
        }

        /// <summary>
        /// <para>IPointerEnterHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IPointerEnterHandler> pointerEnterHandler
        {
            get
            {
                return s_PointerEnterHandler;
            }
        }

        /// <summary>
        /// <para>IPointerExitHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IPointerExitHandler> pointerExitHandler
        {
            get
            {
                return s_PointerExitHandler;
            }
        }

        /// <summary>
        /// <para>IPointerUpHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IPointerUpHandler> pointerUpHandler
        {
            get
            {
                return s_PointerUpHandler;
            }
        }

        /// <summary>
        /// <para>IScrollHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IScrollHandler> scrollHandler
        {
            get
            {
                return s_ScrollHandler;
            }
        }

        /// <summary>
        /// <para>ISelectHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<ISelectHandler> selectHandler
        {
            get
            {
                return s_SelectHandler;
            }
        }

        /// <summary>
        /// <para>ISubmitHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<ISubmitHandler> submitHandler
        {
            get
            {
                return s_SubmitHandler;
            }
        }

        /// <summary>
        /// <para>IUpdateSelectedHandler execute helper function.</para>
        /// </summary>
        public static EventFunction<IUpdateSelectedHandler> updateSelectedHandler
        {
            get
            {
                return s_UpdateSelectedHandler;
            }
        }

        public delegate void EventFunction<T1>(T1 handler, BaseEventData eventData);
    }
}

