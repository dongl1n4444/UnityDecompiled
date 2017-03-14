namespace UnityEditor.TestTools.TestRunner
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    internal class PlatformSpecificSetup
    {
        private IDictionary<BuildTarget, IPlatformSetup> m_SetupTypes;
        [SerializeField]
        private BuildTarget m_Target;

        public PlatformSpecificSetup()
        {
        }

        public PlatformSpecificSetup(BuildTarget target)
        {
            this.m_Target = target;
        }

        public void CleanUp()
        {
            IDictionary<BuildTarget, IPlatformSetup> setup = this.GetSetup();
            if (setup.ContainsKey(this.m_Target))
            {
                setup[this.m_Target].CleanUp();
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

        public void Setup()
        {
            IDictionary<BuildTarget, IPlatformSetup> setup = this.GetSetup();
            if (setup.ContainsKey(this.m_Target))
            {
                setup[this.m_Target].Setup();
            }
        }
    }
}

