namespace UnityEditor.Collaboration
{
    using System;
    using System.Collections.Generic;

    internal class CollabTesting
    {
        private static IEnumerator<bool> _enumerator = null;
        private static Action<bool> _runAfter = null;

        public static void End(bool success)
        {
            _runAfter(success);
            _enumerator = null;
        }

        public static void Execute()
        {
            if ((_enumerator != null) && !Collab.instance.AnyJobRunning())
            {
                try
                {
                    if (!_enumerator.MoveNext())
                    {
                        End(true);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static void OnCompleteJob()
        {
            Execute();
        }

        public static Action<bool> AfterRun
        {
            set
            {
                _runAfter = value;
            }
        }

        public static Func<IEnumerable<bool>> Tick
        {
            set
            {
                _enumerator = value().GetEnumerator();
            }
        }
    }
}

