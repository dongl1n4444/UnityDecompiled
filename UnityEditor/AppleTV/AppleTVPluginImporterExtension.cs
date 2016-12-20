namespace UnityEditor.AppleTV
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.iOS;
    using UnityEditor.Modules;

    internal class AppleTVPluginImporterExtension : DefaultPluginImporterExtension
    {
        internal static List<string> m_defaultFrameworks;
        internal static FrameworkListProperty m_frameworks;
        internal static List<string> m_frequentFrameworks;
        internal static List<string> m_rareFrameworks;

        static AppleTVPluginImporterExtension()
        {
            List<string> list = new List<string> { 
                "AudioToolbox",
                "AVFoundation",
                "CFNetwork",
                "CoreGraphics",
                "CoreLocation",
                "CoreMedia",
                "CoreVideo",
                "CoreText",
                "Foundation",
                "MediaPlayer",
                "OpenAL",
                "OpenGLES",
                "QuartzCore",
                "SystemConfiguration",
                "UIKit"
            };
            m_defaultFrameworks = list;
            list = new List<string> { 
                "AdSupport",
                "CoreData",
                "CoreFoundation",
                "GameKit",
                "MobileCoreServices",
                "StoreKit",
                "Security"
            };
            m_frequentFrameworks = list;
            list = new List<string> { 
                "Accelerate",
                "AudioUnit",
                "AVKit",
                "CloudKit",
                "CoreAudio",
                "CoreAuthentication",
                "CoreBluetooth",
                "CoreImage",
                "CoreSpotlight",
                "GameController",
                "GamePlaykit",
                "GLKit",
                "ImageIO",
                "JavaScriptCore",
                "MediaAccessibility",
                "MediaToolbox",
                "Metal",
                "MetalKit",
                "MetalPerformanceShaders",
                "ModelIO",
                "SceneKit",
                "SpriteKit",
                "TVMLKit",
                "TVServices"
            };
            m_rareFrameworks = list;
            m_frameworks = new FrameworkListProperty("FrameworkDependencies", Plugin.frameworkDependenciesKey, m_defaultFrameworks, m_frequentFrameworks, m_rareFrameworks, "tvOS");
        }

        public AppleTVPluginImporterExtension() : base(GetProperties())
        {
        }

        private static DefaultPluginImporterExtension.Property[] GetProperties()
        {
            return new DefaultPluginImporterExtension.Property[] { m_frameworks, new DefaultPluginImporterExtension.Property(EditorGUIUtility.TextContent("Compile flags"), Plugin.compileFlagsKey, "", "tvOS") };
        }
    }
}

