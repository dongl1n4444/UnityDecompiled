namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;

    internal class SceneViewPicking
    {
        [CompilerGenerated]
        private static Action <>f__mg$cache0;
        private static int s_PreviousPrefixHash = 0;
        private static int s_PreviousTopmostHash = 0;
        private static bool s_RetainHashes = false;

        static SceneViewPicking()
        {
            if (<>f__mg$cache0 == null)
            {
                <>f__mg$cache0 = new Action(SceneViewPicking.ResetHashes);
            }
            Selection.selectionChanged = (Action) Delegate.Combine(Selection.selectionChanged, <>f__mg$cache0);
        }

        [DebuggerHidden]
        private static IEnumerable<GameObject> GetAllOverlapping(Vector2 position) => 
            new <GetAllOverlapping>c__Iterator0 { 
                position = position,
                $PC = -2
            };

        public static GameObject PickGameObject(Vector2 mousePosition)
        {
            s_RetainHashes = true;
            IEnumerator<GameObject> enumerator = GetAllOverlapping(mousePosition).GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return null;
            }
            GameObject current = enumerator.Current;
            GameObject obj4 = HandleUtility.FindSelectionBase(current);
            GameObject obj5 = (obj4 != null) ? obj4 : current;
            int hashCode = current.GetHashCode();
            int hash = hashCode;
            if (Selection.activeGameObject == null)
            {
                s_PreviousTopmostHash = hashCode;
                s_PreviousPrefixHash = hash;
                return obj5;
            }
            if (hashCode != s_PreviousTopmostHash)
            {
                s_PreviousTopmostHash = hashCode;
                s_PreviousPrefixHash = hash;
                return ((Selection.activeGameObject != obj4) ? obj5 : current);
            }
            s_PreviousTopmostHash = hashCode;
            if (Selection.activeGameObject == obj4)
            {
                if (hash == s_PreviousPrefixHash)
                {
                    return current;
                }
                s_PreviousPrefixHash = hash;
                return obj4;
            }
            GameObject[] filter = new GameObject[] { Selection.activeGameObject };
            if (HandleUtility.PickGameObject(mousePosition, false, null, filter) == Selection.activeGameObject)
            {
                while (enumerator.Current != Selection.activeGameObject)
                {
                    if (!enumerator.MoveNext())
                    {
                        s_PreviousPrefixHash = hashCode;
                        return obj5;
                    }
                    UpdateHash(ref hash, enumerator.Current);
                }
            }
            if (hash != s_PreviousPrefixHash)
            {
                s_PreviousPrefixHash = hashCode;
                return obj5;
            }
            if (!enumerator.MoveNext())
            {
                s_PreviousPrefixHash = hashCode;
                return obj5;
            }
            UpdateHash(ref hash, enumerator.Current);
            if (enumerator.Current == obj4)
            {
                if (!enumerator.MoveNext())
                {
                    s_PreviousPrefixHash = hashCode;
                    return obj5;
                }
                UpdateHash(ref hash, enumerator.Current);
            }
            s_PreviousPrefixHash = hash;
            return enumerator.Current;
        }

        private static void ResetHashes()
        {
            if (!s_RetainHashes)
            {
                s_PreviousTopmostHash = 0;
                s_PreviousPrefixHash = 0;
            }
            s_RetainHashes = false;
        }

        private static void UpdateHash(ref int hash, object obj)
        {
            hash = (hash * 0x21) + obj.GetHashCode();
        }

        [CompilerGenerated]
        private sealed class <GetAllOverlapping>c__Iterator0 : IEnumerable, IEnumerable<GameObject>, IEnumerator, IDisposable, IEnumerator<GameObject>
        {
            internal GameObject $current;
            internal bool $disposing;
            internal int $PC;
            internal List<GameObject> <allOverlapping>__1;
            internal GameObject <go>__2;
            internal Vector2 position;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$disposing = true;
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<allOverlapping>__1 = new List<GameObject>();
                        break;

                    case 1:
                        this.<allOverlapping>__1.Add(this.<go>__2);
                        break;

                    default:
                        goto Label_00F1;
                }
                this.<go>__2 = HandleUtility.PickGameObject(this.position, false, this.<allOverlapping>__1.ToArray());
                if (this.<go>__2 != null)
                {
                    if ((this.<allOverlapping>__1.Count <= 0) || (this.<go>__2 != this.<allOverlapping>__1.Last<GameObject>()))
                    {
                        this.$current = this.<go>__2;
                        if (!this.$disposing)
                        {
                            this.$PC = 1;
                        }
                        return true;
                    }
                    Debug.LogError("GetAllOverlapping failed, could not ignore game object '" + this.<go>__2.name + "' when picking");
                }
                this.$PC = -1;
            Label_00F1:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new SceneViewPicking.<GetAllOverlapping>c__Iterator0 { position = this.position };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<UnityEngine.GameObject>.GetEnumerator();

            GameObject IEnumerator<GameObject>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

