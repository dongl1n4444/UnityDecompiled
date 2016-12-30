namespace UnityEngine.U2D.Interface
{
    using System;
    using UnityEngine;

    internal interface IEvent
    {
        EventType GetTypeForControl(int id);
        void Use();

        bool alt { get; }

        int button { get; }

        string commandName { get; }

        bool control { get; }

        KeyCode keyCode { get; }

        EventModifiers modifiers { get; }

        Vector2 mousePosition { get; }

        bool shift { get; }

        EventType type { get; }
    }
}

