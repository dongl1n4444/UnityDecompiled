namespace UnityEngine.U2D.Interface
{
    using System;
    using UnityEngine;

    internal class Event : IEvent
    {
        private UnityEngine.Event m_Event = UnityEngine.Event.current;

        public EventType GetTypeForControl(int id) => 
            this.m_Event.GetTypeForControl(id);

        public void Use()
        {
            this.m_Event.Use();
        }

        public bool alt =>
            this.m_Event.alt;

        public int button =>
            this.m_Event.button;

        public string commandName =>
            this.m_Event.commandName;

        public bool control =>
            this.m_Event.control;

        public KeyCode keyCode =>
            this.m_Event.keyCode;

        public EventModifiers modifiers =>
            this.m_Event.modifiers;

        public Vector2 mousePosition =>
            this.m_Event.mousePosition;

        public bool shift =>
            this.m_Event.shift;

        public EventType type =>
            this.m_Event.type;
    }
}

