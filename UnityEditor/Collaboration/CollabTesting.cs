namespace UnityEditor.Collaboration
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class CollabTesting
    {
        private static IEnumerator<bool> _enumerator = null;
        private static Action _runAfter = null;

        public static void End()
        {
            if (_enumerator != null)
            {
                _runAfter();
                _enumerator = null;
            }
        }

        public static void Execute()
        {
            if ((_enumerator != null) && !Collab.instance.AnyJobRunning())
            {
                try
                {
                    if (!_enumerator.MoveNext())
                    {
                        End();
                    }
                }
                catch (Exception)
                {
                    Debug.LogError("Something Went wrong with the test framework itself");
                    throw;
                }
            }
        }

        public static void OnCompleteJob()
        {
            Execute();
        }

        public static Action AfterRun
        {
            set
            {
                _runAfter = value;
            }
        }

        public static bool IsRunning =>
            (_enumerator != null);

        public static Func<IEnumerable<bool>> Tick
        {
            set
            {
                _enumerator = value().GetEnumerator();
            }
        }
    }
}

