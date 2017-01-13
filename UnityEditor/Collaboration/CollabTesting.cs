namespace UnityEditor.Collaboration
{
    using System;
    using System.Collections.Generic;

    internal class CollabTesting
    {
        private static readonly Queue<Action> m_Actions = new Queue<Action>();

        public static void AddAction(Action action)
        {
            m_Actions.Enqueue(action);
        }

        public static void DropAll()
        {
            m_Actions.Clear();
        }

        public static void Execute()
        {
            if (m_Actions.Count != 0)
            {
                m_Actions.Dequeue()();
            }
        }

        public static void OnCompleteJob()
        {
            Execute();
        }

        public static int ActionsCount =>
            m_Actions.Count;
    }
}

