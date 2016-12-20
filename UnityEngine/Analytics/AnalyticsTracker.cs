namespace UnityEngine.Analytics
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Analytics/AnalyticsTracker")]
    public class AnalyticsTracker : MonoBehaviour
    {
        private Dictionary<string, object> m_Dict = new Dictionary<string, object>();
        [SerializeField]
        private string m_EventName;
        private int m_PrevDictHash = 0;
        [SerializeField]
        private TrackableProperty m_TrackableProperty = new TrackableProperty();
        [SerializeField]
        internal Trigger m_Trigger = Trigger.External;

        private void Awake()
        {
            if (this.m_Trigger == Trigger.Awake)
            {
                this.TriggerEvent();
            }
        }

        private void BuildParameters()
        {
            int hashCode = this.m_TrackableProperty.GetHashCode();
            if (hashCode != this.m_PrevDictHash)
            {
                this.m_Dict.Clear();
            }
            this.m_PrevDictHash = hashCode;
            int num2 = 0;
            int count = this.m_TrackableProperty.fields.Count;
            while (num2 < count)
            {
                TrackableProperty.FieldWithTarget target = this.m_TrackableProperty.fields[num2];
                if ((target.target != null) || target.doStatic)
                {
                    string str = target.GetValue().ToString();
                    this.m_Dict[target.paramName] = str;
                }
                num2++;
            }
        }

        private void OnApplicationPause()
        {
            if (this.m_Trigger == Trigger.OnApplicationPause)
            {
                this.TriggerEvent();
            }
        }

        private void OnDestroy()
        {
            if (this.m_Trigger == Trigger.OnDestroy)
            {
                this.TriggerEvent();
            }
        }

        private void OnDisable()
        {
            if (this.m_Trigger == Trigger.OnDisable)
            {
                this.TriggerEvent();
            }
        }

        private void OnEnable()
        {
            if (this.m_Trigger == Trigger.OnEnable)
            {
                this.TriggerEvent();
            }
        }

        private void SendEvent()
        {
            UnityEngine.Analytics.Analytics.CustomEvent(this.m_EventName, this.m_Dict);
        }

        private void Start()
        {
            if (this.m_Trigger == Trigger.Start)
            {
                this.TriggerEvent();
            }
        }

        /// <summary>
        /// <para>Trigger the instrumented event.</para>
        /// </summary>
        public void TriggerEvent()
        {
            this.BuildParameters();
            this.SendEvent();
        }

        /// <summary>
        /// <para>Name of the event.</para>
        /// </summary>
        public string eventName
        {
            get
            {
                return this.m_EventName;
            }
            set
            {
                this.m_EventName = value;
            }
        }

        internal TrackableProperty TP
        {
            get
            {
                return this.m_TrackableProperty;
            }
            set
            {
                this.m_TrackableProperty = value;
            }
        }

        [Serializable]
        internal enum Trigger
        {
            External,
            Awake,
            Start,
            OnEnable,
            OnDisable,
            OnApplicationPause,
            OnDestroy
        }
    }
}

