namespace UnityEngine.Purchasing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// <para>Maps store-specific product identifiers to one or more store names.</para>
    /// </summary>
    public class IDs : IEnumerable<KeyValuePair<string, string>>, IEnumerable
    {
        private Dictionary<string, string> m_Dic = new Dictionary<string, string>();

        /// <summary>
        /// <para>Add a product ID which is supported by a list of store platform names.</para>
        /// </summary>
        /// <param name="id">Platform specific Product ID.</param>
        /// <param name="stores">Array of platform names using this Product ID.</param>
        public void Add(string id, params object[] stores)
        {
            foreach (object obj2 in stores)
            {
                this.m_Dic[obj2.ToString()] = id;
            }
        }

        /// <summary>
        /// <para>Add a product ID which is supported by a list of store platform names.</para>
        /// </summary>
        /// <param name="id">Platform specific Product ID.</param>
        /// <param name="stores">Array of platform names using this Product ID.</param>
        public void Add(string id, params string[] stores)
        {
            foreach (string str in stores)
            {
                this.m_Dic[str] = id;
            }
        }

        /// <summary>
        /// <para>Enumerator for IDs.</para>
        /// </summary>
        /// <returns>
        /// <para>The first string of the pair is the store-specific product ID. The second is one of the mapped store names.</para>
        /// </returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => 
            this.m_Dic.GetEnumerator();

        internal string SpecificIDForStore(string store, string defaultValue)
        {
            if (this.m_Dic.ContainsKey(store))
            {
                return this.m_Dic[store];
            }
            return defaultValue;
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.m_Dic.GetEnumerator();
    }
}

