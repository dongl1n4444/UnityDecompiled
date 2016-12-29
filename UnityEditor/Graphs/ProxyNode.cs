namespace UnityEditor.Graphs
{
    using System;
    using UnityEngine;

    internal class ProxyNode : Node
    {
        [SerializeField]
        private bool m_IsIn;

        public void Init(bool isIn)
        {
            this.m_IsIn = isIn;
        }

        public static ProxyNode Instance(bool isIn)
        {
            ProxyNode node = Node.Instance<ProxyNode>();
            node.Init(isIn);
            return node;
        }

        public bool isIn =>
            this.m_IsIn;
    }
}

