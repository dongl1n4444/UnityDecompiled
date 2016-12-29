namespace UnityEditor.iOS
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Modules;

    internal class iOSPluginImporterExtension : DefaultPluginImporterExtension
    {
        internal static List<string> m_defaultFrameworks;
        internal static FrameworkListProperty m_frameworks;
        internal static List<string> m_frequentFrameworks;
        internal static List<string> m_rareFrameworks;

        static iOSPluginImporterExtension()
        {
            List<string> list = new List<string> { 
                "AudioToolbox",
                "AVFoundation",
                "CFNetwork",
                "CoreGraphics",
                "CoreLocation",
                "CoreMedia",
                "CoreMotion",
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
                "Accounts",
                "CoreData",
                "CoreFoundation",
                "CoreTelephony",
                "GameKit",
                "MapKit",
                "MobileCoreServices",
                "Social",
                "StoreKit",
                "Security",
                "Twitter"
            };
            m_frequentFrameworks = list;
            list = new List<string> { 
                "Accelerate",
                "AddressBook",
                "AddressBookUI",
                "AssetsLibrary",
                "AudioUnit",
                "AVKit",
                "CloudKit",
                "Contacts",
                "ContactsUI",
                "CoreAudio",
                "CoreAudioKit",
                "CoreAuthentication",
                "CoreBluetooth",
                "CoreImage",
                "CoreMIDI",
                "CoreSpotlight",
                "EventKit",
                "EventKitUI",
                "GameController",
                "GamePlaykit",
                "GLKit",
                "GSS",
                "HealthKit",
                "HomeKit",
                "ImageIO",
                "JavaScriptCore",
                "LocalAuthentication",
                "MediaAccessibility",
                "MediaToolbox",
                "MessageUI",
                "Metal",
                "MetalKit",
                "MetalPerformanceShaders",
                "ModelIO",
                "NetworkExtension",
                "NotificationCenter",
                "MultipeerConnectivity",
                "NewsstandKit",
                "PassKit",
                "Photos",
                "PhotosUI",
                "PushKit",
                "ReplayKit",
                "QuickLook",
                "SafariServices",
                "SceneKit",
                "SpriteKit",
                "WatchKit",
                "WatchConnectivity",
                "WebKit"
            };
            m_rareFrameworks = list;
            m_frameworks = new FrameworkListProperty("FrameworkDependencies", Plugin.frameworkDependenciesKey, m_defaultFrameworks, m_frequentFrameworks, m_rareFrameworks, "iOS");
        }

        public iOSPluginImporterExtension() : base(GetProperties())
        {
        }

        private static DefaultPluginImporterExtension.Property[] GetProperties() => 
            new DefaultPluginImporterExtension.Property[] { m_frameworks, new DefaultPluginImporterExtension.Property(EditorGUIUtility.TextContent("Compile flags"), Plugin.compileFlagsKey, "", "iOS") };
    }
}

