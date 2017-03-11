namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [InitializeOnLoad]
    internal class UISystemProfilerRenderService : IDisposable
    {
        private LRUCache m_Cache = new LRUCache(10);
        private bool m_Disposed;

        public void Dispose()
        {
            this.m_Disposed = true;
            this.m_Cache.Clear();
        }

        private Texture2D Generate(int renderDataIndex, int renderDataCount, bool overdraw) => 
            (!this.m_Disposed ? ProfilerProperty.UISystemProfilerRender(renderDataIndex, renderDataCount, overdraw) : null);

        public Texture2D GetThumbnail(int renderDataIndex, int infoRenderDataCount, bool overdraw)
        {
            if (this.m_Disposed)
            {
                return null;
            }
            long key = ((((ushort) renderDataIndex) << 0x20) | ((ushort) (((ushort) infoRenderDataCount) & 0x7fff))) | (!overdraw ? ((ushort) 0) : ((ushort) 0x8000));
            Texture2D data = this.m_Cache.Get(key);
            if (data == null)
            {
                data = this.Generate(renderDataIndex, infoRenderDataCount, overdraw);
                if (data != null)
                {
                    this.m_Cache.Add(key, data);
                }
            }
            return data;
        }

        private class LRUCache
        {
            [CompilerGenerated]
            private static Func<long, long> <>f__am$cache0;
            [CompilerGenerated]
            private static Func<long, long> <>f__am$cache1;
            private Dictionary<long, Texture2D> m_Cache;
            private List<long> m_CacheQueue;
            private int m_CacheQueueFront;
            private int m_Capacity;

            public LRUCache(int capacity)
            {
                if (capacity <= 0)
                {
                    capacity = 0x10;
                }
                this.m_Capacity = capacity;
                this.m_Cache = new Dictionary<long, Texture2D>(this.m_Capacity);
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = new Func<long, long>(UISystemProfilerRenderService.LRUCache.<LRUCache>m__0);
                }
                IEnumerable<long> collection = Enumerable.Select<long, long>(Enumerable.Repeat<long>(-1L, this.m_Capacity), <>f__am$cache0);
                this.m_CacheQueue = new List<long>(collection);
                this.m_CacheQueueFront = 0;
            }

            [CompilerGenerated]
            private static long <LRUCache>m__0(long value) => 
                value;

            public void Add(long key, Texture2D data)
            {
                if (this.Get(key) == null)
                {
                    if (this.m_CacheQueue[this.m_CacheQueueFront] != -1L)
                    {
                        long num = this.m_CacheQueue[this.m_CacheQueueFront];
                        Texture2D t = this.m_Cache[num];
                        this.m_Cache.Remove(num);
                        ProfilerProperty.ReleaseUISystemProfilerRender(t);
                    }
                    this.m_CacheQueue[this.m_CacheQueueFront] = key;
                    this.m_Cache[key] = data;
                    this.m_CacheQueueFront++;
                    if (this.m_CacheQueueFront == this.m_Capacity)
                    {
                        this.m_CacheQueueFront = 0;
                    }
                }
            }

            public void Clear()
            {
                foreach (long num in this.m_CacheQueue)
                {
                    Texture2D textured;
                    if (this.m_Cache.TryGetValue(num, out textured))
                    {
                        ProfilerProperty.ReleaseUISystemProfilerRender(textured);
                    }
                }
                this.m_Cache.Clear();
                this.m_CacheQueue.Clear();
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = value => value;
                }
                IEnumerable<long> collection = Enumerable.Select<long, long>(Enumerable.Repeat<long>(-1L, this.m_Capacity), <>f__am$cache1);
                this.m_CacheQueue.AddRange(collection);
                this.m_CacheQueueFront = 0;
            }

            public Texture2D Get(long key)
            {
                Texture2D textured;
                if (this.m_Cache.TryGetValue(key, out textured))
                {
                    this.m_CacheQueue[this.m_CacheQueue.IndexOf(key)] = this.m_CacheQueue[this.m_CacheQueueFront];
                    this.m_CacheQueue[this.m_CacheQueueFront] = key;
                    this.m_CacheQueueFront++;
                    if (this.m_CacheQueueFront == this.m_Capacity)
                    {
                        this.m_CacheQueueFront = 0;
                    }
                    return textured;
                }
                return null;
            }
        }
    }
}

