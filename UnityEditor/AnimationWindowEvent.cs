namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class AnimationWindowEvent : ScriptableObject
    {
        public AnimationClip clip;
        public AnimationClipInfoProperties clipInfo;
        public int eventIndex;
        public GameObject root;

        public static AnimationWindowEvent CreateAndEdit(GameObject root, AnimationClip clip, float time)
        {
            AnimationEvent evt = new AnimationEvent {
                time = time
            };
            int num = InsertAnimationEvent(ref AnimationUtility.GetAnimationEvents(clip), clip, evt);
            AnimationWindowEvent event3 = ScriptableObject.CreateInstance<AnimationWindowEvent>();
            event3.hideFlags = HideFlags.HideInHierarchy;
            event3.name = "Animation Event";
            event3.root = root;
            event3.clip = clip;
            event3.clipInfo = null;
            event3.eventIndex = num;
            return event3;
        }

        public static AnimationWindowEvent Edit(AnimationClipInfoProperties clipInfo, int eventIndex)
        {
            AnimationWindowEvent event2 = ScriptableObject.CreateInstance<AnimationWindowEvent>();
            event2.hideFlags = HideFlags.HideInHierarchy;
            event2.name = "Animation Event";
            event2.root = null;
            event2.clip = null;
            event2.clipInfo = clipInfo;
            event2.eventIndex = eventIndex;
            return event2;
        }

        public static AnimationWindowEvent Edit(GameObject root, AnimationClip clip, int eventIndex)
        {
            AnimationWindowEvent event2 = ScriptableObject.CreateInstance<AnimationWindowEvent>();
            event2.hideFlags = HideFlags.HideInHierarchy;
            event2.name = "Animation Event";
            event2.root = root;
            event2.clip = clip;
            event2.clipInfo = null;
            event2.eventIndex = eventIndex;
            return event2;
        }

        private static int InsertAnimationEvent(ref AnimationEvent[] events, AnimationClip clip, AnimationEvent evt)
        {
            Undo.RegisterCompleteObjectUndo(clip, "Add Event");
            int length = events.Length;
            for (int i = 0; i < events.Length; i++)
            {
                if (events[i].time > evt.time)
                {
                    length = i;
                    break;
                }
            }
            ArrayUtility.Insert<AnimationEvent>(ref events, length, evt);
            AnimationUtility.SetAnimationEvents(clip, events);
            events = AnimationUtility.GetAnimationEvents(clip);
            if ((events[length].time != evt.time) || (events[length].functionName != events[length].functionName))
            {
                Debug.LogError("Failed insertion");
            }
            return length;
        }
    }
}

