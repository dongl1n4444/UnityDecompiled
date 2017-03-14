namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.iOS.Xcode;

    internal class FileTypeUtils
    {
        private static readonly Dictionary<PBXSourceTree, string> sourceTree;
        private static readonly Dictionary<string, PBXSourceTree> stringToSourceTreeMap;
        private static readonly Dictionary<string, FileTypeDesc> types;

        static FileTypeUtils()
        {
            Dictionary<string, FileTypeDesc> dictionary = new Dictionary<string, FileTypeDesc> {
                { 
                    "a",
                    new FileTypeDesc("archive.ar", PBXFileType.Framework)
                },
                { 
                    "app",
                    new FileTypeDesc("wrapper.application", PBXFileType.NotBuildable, true)
                },
                { 
                    "appex",
                    new FileTypeDesc("wrapper.app-extension", PBXFileType.CopyFile)
                },
                { 
                    "bin",
                    new FileTypeDesc("archive.macbinary", PBXFileType.Resource)
                },
                { 
                    "s",
                    new FileTypeDesc("sourcecode.asm", PBXFileType.Source)
                },
                { 
                    "c",
                    new FileTypeDesc("sourcecode.c.c", PBXFileType.Source)
                },
                { 
                    "cc",
                    new FileTypeDesc("sourcecode.cpp.cpp", PBXFileType.Source)
                },
                { 
                    "cpp",
                    new FileTypeDesc("sourcecode.cpp.cpp", PBXFileType.Source)
                },
                { 
                    "swift",
                    new FileTypeDesc("sourcecode.swift", PBXFileType.Source)
                },
                { 
                    "dll",
                    new FileTypeDesc("file", PBXFileType.NotBuildable)
                },
                { 
                    "framework",
                    new FileTypeDesc("wrapper.framework", PBXFileType.Framework)
                },
                { 
                    "h",
                    new FileTypeDesc("sourcecode.c.h", PBXFileType.NotBuildable)
                },
                { 
                    "pch",
                    new FileTypeDesc("sourcecode.c.h", PBXFileType.NotBuildable)
                },
                { 
                    "icns",
                    new FileTypeDesc("image.icns", PBXFileType.Resource)
                },
                { 
                    "xcassets",
                    new FileTypeDesc("folder.assetcatalog", PBXFileType.Resource)
                },
                { 
                    "inc",
                    new FileTypeDesc("sourcecode.inc", PBXFileType.NotBuildable)
                },
                { 
                    "m",
                    new FileTypeDesc("sourcecode.c.objc", PBXFileType.Source)
                },
                { 
                    "mm",
                    new FileTypeDesc("sourcecode.cpp.objcpp", PBXFileType.Source)
                },
                { 
                    "nib",
                    new FileTypeDesc("wrapper.nib", PBXFileType.Resource)
                },
                { 
                    "plist",
                    new FileTypeDesc("text.plist.xml", PBXFileType.Resource)
                },
                { 
                    "png",
                    new FileTypeDesc("image.png", PBXFileType.Resource)
                },
                { 
                    "rtf",
                    new FileTypeDesc("text.rtf", PBXFileType.Resource)
                },
                { 
                    "tiff",
                    new FileTypeDesc("image.tiff", PBXFileType.Resource)
                },
                { 
                    "txt",
                    new FileTypeDesc("text", PBXFileType.Resource)
                },
                { 
                    "json",
                    new FileTypeDesc("text.json", PBXFileType.Resource)
                },
                { 
                    "xcodeproj",
                    new FileTypeDesc("wrapper.pb-project", PBXFileType.NotBuildable)
                },
                { 
                    "xib",
                    new FileTypeDesc("file.xib", PBXFileType.Resource)
                },
                { 
                    "strings",
                    new FileTypeDesc("text.plist.strings", PBXFileType.Resource)
                },
                { 
                    "storyboard",
                    new FileTypeDesc("file.storyboard", PBXFileType.Resource)
                },
                { 
                    "bundle",
                    new FileTypeDesc("wrapper.plug-in", PBXFileType.Resource)
                },
                { 
                    "dylib",
                    new FileTypeDesc("compiled.mach-o.dylib", PBXFileType.Framework)
                },
                { 
                    "tbd",
                    new FileTypeDesc("sourcecode.text-based-dylib-definition", PBXFileType.Framework)
                }
            };
            types = dictionary;
            Dictionary<PBXSourceTree, string> dictionary2 = new Dictionary<PBXSourceTree, string> {
                { 
                    PBXSourceTree.Absolute,
                    "<absolute>"
                },
                { 
                    PBXSourceTree.Group,
                    "<group>"
                },
                { 
                    PBXSourceTree.Build,
                    "BUILT_PRODUCTS_DIR"
                },
                { 
                    PBXSourceTree.Developer,
                    "DEVELOPER_DIR"
                },
                { 
                    PBXSourceTree.Sdk,
                    "SDKROOT"
                },
                { 
                    PBXSourceTree.Source,
                    "SOURCE_ROOT"
                }
            };
            sourceTree = dictionary2;
            Dictionary<string, PBXSourceTree> dictionary3 = new Dictionary<string, PBXSourceTree> {
                { 
                    "<absolute>",
                    PBXSourceTree.Absolute
                },
                { 
                    "<group>",
                    PBXSourceTree.Group
                },
                { 
                    "BUILT_PRODUCTS_DIR",
                    PBXSourceTree.Build
                },
                { 
                    "DEVELOPER_DIR",
                    PBXSourceTree.Developer
                },
                { 
                    "SDKROOT",
                    PBXSourceTree.Sdk
                },
                { 
                    "SOURCE_ROOT",
                    PBXSourceTree.Source
                }
            };
            stringToSourceTreeMap = dictionary3;
        }

        internal static List<PBXSourceTree> AllAbsoluteSourceTrees() => 
            new List<PBXSourceTree> { 
                PBXSourceTree.Absolute,
                PBXSourceTree.Build,
                PBXSourceTree.Developer,
                PBXSourceTree.Sdk,
                PBXSourceTree.Source
            };

        public static PBXFileType GetFileType(string ext, bool isFolderRef)
        {
            ext = TrimExtension(ext);
            if (isFolderRef)
            {
                return PBXFileType.Resource;
            }
            if (!types.ContainsKey(ext))
            {
                return PBXFileType.Resource;
            }
            return types[ext].type;
        }

        public static string GetTypeName(string ext)
        {
            ext = TrimExtension(ext);
            if (types.ContainsKey(ext))
            {
                return types[ext].name;
            }
            return "file";
        }

        public static bool IsBuildable(string ext, bool isFolderReference)
        {
            ext = TrimExtension(ext);
            return (isFolderReference || IsBuildableFile(ext));
        }

        public static bool IsBuildableFile(string ext)
        {
            ext = TrimExtension(ext);
            return (!types.ContainsKey(ext) || (types[ext].type != PBXFileType.NotBuildable));
        }

        internal static bool IsFileTypeExplicit(string ext)
        {
            ext = TrimExtension(ext);
            return (types.ContainsKey(ext) && types[ext].isExplicit);
        }

        public static bool IsKnownExtension(string ext)
        {
            ext = TrimExtension(ext);
            return types.ContainsKey(ext);
        }

        internal static PBXSourceTree ParseSourceTree(string tree)
        {
            if (stringToSourceTreeMap.ContainsKey(tree))
            {
                return stringToSourceTreeMap[tree];
            }
            return PBXSourceTree.Source;
        }

        internal static string SourceTreeDesc(PBXSourceTree tree) => 
            sourceTree[tree];

        public static string TrimExtension(string ext)
        {
            char[] trimChars = new char[] { '.' };
            return ext.TrimStart(trimChars);
        }

        internal class FileTypeDesc
        {
            public bool isExplicit;
            public string name;
            public PBXFileType type;

            public FileTypeDesc(string typeName, PBXFileType type)
            {
                this.name = typeName;
                this.type = type;
                this.isExplicit = false;
            }

            public FileTypeDesc(string typeName, PBXFileType type, bool isExplicit)
            {
                this.name = typeName;
                this.type = type;
                this.isExplicit = isExplicit;
            }
        }
    }
}

