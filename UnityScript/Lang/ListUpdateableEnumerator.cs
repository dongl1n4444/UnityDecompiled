namespace UnityScript.Lang
{
    using System;
    using System.Collections;

    [Serializable]
    internal class ListUpdateableEnumerator : IEnumerator
    {
        protected int _current = -1;
        protected IList _list;

        public ListUpdateableEnumerator(IList list)
        {
            this._list = list;
        }

        public override bool MoveNext()
        {
            this._current++;
            return (this._current < this._list.Count);
        }

        public override void Reset()
        {
            this._current = -1;
        }

        public void Update(object newValue)
        {
            this._list[this._current] = newValue;
        }

        public override object Current
        {
            get
            {
                return this._list[this._current];
            }
        }
    }
}

