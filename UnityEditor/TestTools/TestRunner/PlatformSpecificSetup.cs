namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;

    [Serializable]
    internal class PlatformSpecificSetup
    {
        private IDictionary<BuildTarget, IPlatformSetup> m_SetupTypes;

        public void CleanUp(BuildTarget target)
        {
            IDictionary<BuildTarget, IPlatformSetup> setup = this.GetSetup();
            if (setup.ContainsKey(target))
            {
                setup[target].CleanUp();
            }
        }

        private IDictionary<BuildTarget, IPlatformSetup> GetSetup()
        {
            if (this.m_SetupTypes == null)
            {
                Dictionary<BuildTarget, IPlatformSetup> dictionary2 = new Dictionary<BuildTarget, IPlatformSetup> {
                    { 
                        BuildTarget.iOS,
                        new ApplePlatformSetup(BuildTarget.iOS)
                    },
                    { 
                        BuildTarget.tvOS,
                        new ApplePlatformSetup(BuildTarget.tvOS)
                    },
                    { 
                        BuildTarget.XboxOne,
                        new XboxOnePlatformSetup()
                    }
                };
                this.m_SetupTypes = dictionary2;
            }
            return this.m_SetupTypes;
        }

        public void Setup(BuildTarget target)
        {
            IDictionary<BuildTarget, IPlatformSetup> setup = this.GetSetup();
            if (setup.ContainsKey(target))
            {
                setup[target].Setup();
            }
        }
    }
}

