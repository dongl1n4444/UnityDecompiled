using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal class LibraryCollection : KeyedCollection<string, Library>
{
    [CompilerGenerated]
    private static Func<PluginData, string> <>f__am$cache0;
    [CompilerGenerated]
    private static Func<PluginData, string> <>f__am$cache1;

    public LibraryCollection() : base(StringComparer.InvariantCultureIgnoreCase)
    {
    }

    protected override string GetKeyForItem(Library library)
    {
        return library.Name;
    }

    public void RegisterManagedDll(string name, bool process, [Optional, DefaultParameterValue(null)] string directory)
    {
        if (base.Contains(name))
        {
            throw new Exception(string.Format("Duplicate library {0}.", name));
        }
        Library item = new Library(name, false, false, process, directory);
        item.RegisterArch("AnyCPU");
        base.Add(item);
    }

    public void RegisterPlayerWinMd(string name, [Optional, DefaultParameterValue(null)] string directory)
    {
        if (base.Contains(name))
        {
            throw new Exception(string.Format("Duplicate library {0}.", name));
        }
        Library item = new Library(name, true, true, false, directory);
        item.RegisterArch("Player");
        base.Add(item);
    }

    public void RegisterPlugins(IEnumerable<PluginData> plugins, IEnumerable<string> incompatiblePlugins)
    {
        this.RemoveIncompatible(incompatiblePlugins);
        this.RemovePlaceholders(plugins);
        if (<>f__am$cache0 == null)
        {
            <>f__am$cache0 = new Func<PluginData, string>(null, (IntPtr) <RegisterPlugins>m__0);
        }
        foreach (IGrouping<string, PluginData> grouping in Enumerable.GroupBy<PluginData, string>(plugins, <>f__am$cache0, StringComparer.InvariantCultureIgnoreCase))
        {
            Library library;
            string key = grouping.Key;
            if (base.Contains(key))
            {
                throw new Exception(string.Format("Duplicate plugin {0}.", key));
            }
            List<PluginData> list = new List<PluginData>();
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<PluginData, string>(null, (IntPtr) <RegisterPlugins>m__1);
            }
            foreach (IGrouping<string, PluginData> grouping2 in Enumerable.GroupBy<PluginData, string>(grouping, <>f__am$cache1, StringComparer.InvariantCultureIgnoreCase))
            {
                bool? nullable2 = null;
                bool? nullable = nullable2;
                bool flag = false;
                bool flag2 = false;
                nullable2 = null;
                bool? nullable3 = nullable2;
                foreach (PluginData data in grouping2)
                {
                    if ((!data.AnySdk && nullable.HasValue) && nullable.Value)
                    {
                        throw new Exception(string.Format("{0} plugin has conflicting SDK values.", data.Name));
                    }
                    nullable = new bool?(data.AnySdk);
                    if (data.Native)
                    {
                        if (flag)
                        {
                            throw new Exception(string.Format("Duplicate plugin {0}.", data.Name));
                        }
                        flag = true;
                    }
                    if (data.WinMd)
                    {
                        if (flag2)
                        {
                            throw new Exception(string.Format("Duplicate plugin {0}.", data.Name));
                        }
                        flag2 = true;
                    }
                    if (!nullable3.HasValue)
                    {
                        nullable3 = new bool?(data.Process);
                    }
                    else if (nullable3.Value != data.Process)
                    {
                        throw new Exception(string.Format("{0} plugin has conflicting \"Don't process\" values.", data.Name));
                    }
                }
                PluginData item = new PluginData {
                    Name = key,
                    Cpu = grouping2.Key,
                    AnySdk = !nullable.HasValue ? true : nullable.Value,
                    Native = flag,
                    WinMd = flag2,
                    Process = !nullable3.HasValue ? true : nullable3.Value
                };
                list.Add(item);
            }
            if ((list.Count == 1) && Library.IsAnyCpuTag(list[0].Cpu))
            {
                string directory = null;
                PluginData data3 = list[0];
                library = new Library(key, data3.Native, data3.WinMd, data3.Process, directory);
                library.RegisterArch("AnyCPU");
            }
            else
            {
                PluginData data4 = list[0];
                bool native = data4.Native;
                bool winMd = data4.WinMd;
                for (int i = 1; i < list.Count; i++)
                {
                    PluginData data5 = list[i];
                    if (data5.AnySdk != data4.AnySdk)
                    {
                        throw new Exception(string.Format("Plugin {0} must have identical settings for all SDKs.", key));
                    }
                    if ((data5.Native != data4.Native) && (data5.WinMd == data4.WinMd))
                    {
                        throw new Exception(string.Format("Plugin {0}.{1} cannot have both native and managed copies.", key, !data4.WinMd ? "dll" : "winmd"));
                    }
                    if (data5.Process != data4.Process)
                    {
                        throw new Exception(string.Format("All copies of plugin {0} must have the same \"Don't process\" setting.", key));
                    }
                    native |= list[i].Native;
                    winMd |= list[i].WinMd;
                }
                string str3 = "Plugins";
                str3 = str3 + @"\$(PlatformShortName)";
                library = new Library(key, native, winMd, !winMd, str3);
                foreach (PluginData data6 in list)
                {
                    library.RegisterArch(data6.Cpu);
                }
            }
            base.Add(library);
        }
    }

    private void RemoveIncompatible(IEnumerable<string> plugins)
    {
        foreach (string str in plugins)
        {
            if (base.Contains(str))
            {
                base.Remove(str);
            }
        }
    }

    private void RemovePlaceholders(IEnumerable<PluginData> plugins)
    {
        foreach (PluginData data in plugins)
        {
            string key = !string.IsNullOrEmpty(data.PlaceholderPath) ? Path.GetFileNameWithoutExtension(data.PlaceholderPath) : data.Name;
            if (base.Contains(key))
            {
                Library item = base[key];
                if (!item.AnyCpu)
                {
                    throw new Exception(string.Format("Plugin placeholder {0} cannot be architecture specific.", key));
                }
                base.Remove(item);
            }
        }
    }
}

