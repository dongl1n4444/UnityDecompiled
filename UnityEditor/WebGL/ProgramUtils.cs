namespace UnityEditor.WebGL
{
    using System;
    using UnityEditor.Utils;
    using UnityEditor.WebGL.Emscripten;
    using UnityEngine;

    internal static class ProgramUtils
    {
        internal static bool StartProgramChecked(Program p)
        {
            bool flag;
            if (EmccArguments.debugEnvironmentAndInvocations)
            {
                Debug.Log(p.GetProcessStartInfo().Arguments);
            }
            using (p)
            {
                p.LogProcessStartInfo();
                try
                {
                    p.Start();
                }
                catch
                {
                    throw new Exception("Could not start ");
                }
                p.WaitForExit();
                if (p.ExitCode == 0)
                {
                    flag = true;
                }
                else
                {
                    Debug.LogError("Failed running " + p.GetProcessStartInfo().FileName + " " + p.GetProcessStartInfo().Arguments + "\n\n" + p.GetAllOutput());
                    throw new Exception("Failed building WebGL Player.");
                }
            }
            return flag;
        }
    }
}

