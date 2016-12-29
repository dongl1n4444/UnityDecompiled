namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    internal class AudioProfilerGroupInfoHelper
    {
        public const int AUDIOPROFILER_FLAGS_3D = 1;
        public const int AUDIOPROFILER_FLAGS_COMPRESSED = 0x100;
        public const int AUDIOPROFILER_FLAGS_GROUP = 0x40;
        public const int AUDIOPROFILER_FLAGS_ISSPATIAL = 2;
        public const int AUDIOPROFILER_FLAGS_LOOPED = 0x200;
        public const int AUDIOPROFILER_FLAGS_MUTED = 8;
        public const int AUDIOPROFILER_FLAGS_NONBLOCKING = 0x2000;
        public const int AUDIOPROFILER_FLAGS_ONESHOT = 0x20;
        public const int AUDIOPROFILER_FLAGS_OPENMEMORY = 0x400;
        public const int AUDIOPROFILER_FLAGS_OPENMEMORYPOINT = 0x800;
        public const int AUDIOPROFILER_FLAGS_OPENUSER = 0x1000;
        public const int AUDIOPROFILER_FLAGS_PAUSED = 4;
        public const int AUDIOPROFILER_FLAGS_STREAM = 0x80;
        public const int AUDIOPROFILER_FLAGS_VIRTUAL = 0x10;

        private static string FormatDb(float vol)
        {
            if (vol == 0f)
            {
                return "-∞ dB";
            }
            return $"{(20f * Mathf.Log10(vol)):0.00} dB";
        }

        public static string GetColumnString(AudioProfilerGroupInfoWrapper info, ColumnIndices index)
        {
            bool flag = (info.info.flags & 1) != 0;
            bool flag2 = (info.info.flags & 0x40) != 0;
            switch (index)
            {
                case ColumnIndices.ObjectName:
                    return info.objectName;

                case ColumnIndices.AssetName:
                    return info.assetName;

                case ColumnIndices.Volume:
                    return FormatDb(info.info.volume);

                case ColumnIndices.Audibility:
                    return (!flag2 ? FormatDb(info.info.audibility) : "");

                case ColumnIndices.PlayCount:
                    return (!flag2 ? info.info.playCount.ToString() : "");

                case ColumnIndices.Is3D:
                    return (!flag2 ? (!flag ? "NO" : (((info.info.flags & 2) == 0) ? "YES" : "Spatial")) : "");

                case ColumnIndices.IsPaused:
                    return (!flag2 ? (((info.info.flags & 4) == 0) ? "NO" : "YES") : "");

                case ColumnIndices.IsMuted:
                    return (!flag2 ? (((info.info.flags & 8) == 0) ? "NO" : "YES") : "");

                case ColumnIndices.IsVirtual:
                    return (!flag2 ? (((info.info.flags & 0x10) == 0) ? "NO" : "YES") : "");

                case ColumnIndices.IsOneShot:
                    return (!flag2 ? (((info.info.flags & 0x20) == 0) ? "NO" : "YES") : "");

                case ColumnIndices.IsLooped:
                    return (!flag2 ? (((info.info.flags & 0x200) == 0) ? "NO" : "YES") : "");

                case ColumnIndices.DistanceToListener:
                    return (!flag2 ? (flag ? ((info.info.distanceToListener < 1000f) ? $"{info.info.distanceToListener:0.00} m" : $"{(info.info.distanceToListener * 0.001f):0.00} km") : "N/A") : "");

                case ColumnIndices.MinDist:
                    return (!flag2 ? (flag ? ((info.info.minDist < 1000f) ? $"{info.info.minDist:0.00} m" : $"{(info.info.minDist * 0.001f):0.00} km") : "N/A") : "");

                case ColumnIndices.MaxDist:
                    return (!flag2 ? (flag ? ((info.info.maxDist < 1000f) ? $"{info.info.maxDist:0.00} m" : $"{(info.info.maxDist * 0.001f):0.00} km") : "N/A") : "");

                case ColumnIndices.Time:
                    return (!flag2 ? $"{info.info.time:0.00} s" : "");

                case ColumnIndices.Duration:
                    return (!flag2 ? $"{info.info.duration:0.00} s" : "");

                case ColumnIndices.Frequency:
                    return (!flag2 ? ((info.info.frequency < 1000f) ? $"{info.info.frequency:0.00} Hz" : $"{(info.info.frequency * 0.001f):0.00} kHz") : $"{info.info.frequency:0.00} x");

                case ColumnIndices.IsStream:
                    return (!flag2 ? (((info.info.flags & 0x80) == 0) ? "NO" : "YES") : "");

                case ColumnIndices.IsCompressed:
                    return (!flag2 ? (((info.info.flags & 0x100) == 0) ? "NO" : "YES") : "");

                case ColumnIndices.IsNonBlocking:
                    return (!flag2 ? (((info.info.flags & 0x2000) == 0) ? "NO" : "YES") : "");

                case ColumnIndices.IsOpenUser:
                    return (!flag2 ? (((info.info.flags & 0x1000) == 0) ? "NO" : "YES") : "");

                case ColumnIndices.IsOpenMemory:
                    return (!flag2 ? (((info.info.flags & 0x400) == 0) ? "NO" : "YES") : "");

                case ColumnIndices.IsOpenMemoryPoint:
                    return (!flag2 ? (((info.info.flags & 0x800) == 0) ? "NO" : "YES") : "");
            }
            return "Unknown";
        }

        public static int GetLastColumnIndex() => 
            (!Unsupported.IsDeveloperBuild() ? 15 : 0x16);

        public class AudioProfilerGroupInfoComparer : IComparer<AudioProfilerGroupInfoWrapper>
        {
            public AudioProfilerGroupInfoHelper.ColumnIndices primarySortKey;
            public AudioProfilerGroupInfoHelper.ColumnIndices secondarySortKey;
            public bool sortByDescendingOrder;

            public AudioProfilerGroupInfoComparer(AudioProfilerGroupInfoHelper.ColumnIndices primarySortKey, AudioProfilerGroupInfoHelper.ColumnIndices secondarySortKey, bool sortByDescendingOrder)
            {
                this.primarySortKey = primarySortKey;
                this.secondarySortKey = secondarySortKey;
                this.sortByDescendingOrder = sortByDescendingOrder;
            }

            public int Compare(AudioProfilerGroupInfoWrapper a, AudioProfilerGroupInfoWrapper b)
            {
                int num = this.CompareInternal(a, b, this.primarySortKey);
                return ((num != 0) ? num : this.CompareInternal(a, b, this.secondarySortKey));
            }

            private int CompareInternal(AudioProfilerGroupInfoWrapper a, AudioProfilerGroupInfoWrapper b, AudioProfilerGroupInfoHelper.ColumnIndices key)
            {
                int num = 0;
                switch (key)
                {
                    case AudioProfilerGroupInfoHelper.ColumnIndices.ObjectName:
                        num = a.objectName.CompareTo(b.objectName);
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.AssetName:
                        num = a.assetName.CompareTo(b.assetName);
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.Volume:
                        num = a.info.volume.CompareTo(b.info.volume);
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.Audibility:
                        num = a.info.audibility.CompareTo(b.info.audibility);
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.PlayCount:
                        num = a.info.playCount.CompareTo(b.info.playCount);
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.Is3D:
                    {
                        int introduced15 = (a.info.flags & 1).CompareTo((int) (b.info.flags & 1));
                        int num3 = a.info.flags & 2;
                        num = introduced15 + (num3.CompareTo((int) (b.info.flags & 2)) * 2);
                        break;
                    }
                    case AudioProfilerGroupInfoHelper.ColumnIndices.IsPaused:
                        num = (a.info.flags & 4).CompareTo((int) (b.info.flags & 4));
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.IsMuted:
                        num = (a.info.flags & 8).CompareTo((int) (b.info.flags & 8));
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.IsVirtual:
                        num = (a.info.flags & 0x10).CompareTo((int) (b.info.flags & 0x10));
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.IsOneShot:
                        num = (a.info.flags & 0x20).CompareTo((int) (b.info.flags & 0x20));
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.IsLooped:
                        num = (a.info.flags & 0x200).CompareTo((int) (b.info.flags & 0x200));
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.DistanceToListener:
                        num = a.info.distanceToListener.CompareTo(b.info.distanceToListener);
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.MinDist:
                        num = a.info.minDist.CompareTo(b.info.minDist);
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.MaxDist:
                        num = a.info.maxDist.CompareTo(b.info.maxDist);
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.Time:
                        num = a.info.time.CompareTo(b.info.time);
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.Duration:
                        num = a.info.duration.CompareTo(b.info.duration);
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.Frequency:
                        num = a.info.frequency.CompareTo(b.info.frequency);
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.IsStream:
                        num = (a.info.flags & 0x80).CompareTo((int) (b.info.flags & 0x80));
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.IsCompressed:
                        num = (a.info.flags & 0x100).CompareTo((int) (b.info.flags & 0x100));
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.IsNonBlocking:
                        num = (a.info.flags & 0x2000).CompareTo((int) (b.info.flags & 0x2000));
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.IsOpenUser:
                        num = (a.info.flags & 0x1000).CompareTo((int) (b.info.flags & 0x1000));
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.IsOpenMemory:
                        num = (a.info.flags & 0x400).CompareTo((int) (b.info.flags & 0x400));
                        break;

                    case AudioProfilerGroupInfoHelper.ColumnIndices.IsOpenMemoryPoint:
                        num = (a.info.flags & 0x800).CompareTo((int) (b.info.flags & 0x800));
                        break;
                }
                return (!this.sortByDescendingOrder ? num : -num);
            }
        }

        public enum ColumnIndices
        {
            ObjectName,
            AssetName,
            Volume,
            Audibility,
            PlayCount,
            Is3D,
            IsPaused,
            IsMuted,
            IsVirtual,
            IsOneShot,
            IsLooped,
            DistanceToListener,
            MinDist,
            MaxDist,
            Time,
            Duration,
            Frequency,
            IsStream,
            IsCompressed,
            IsNonBlocking,
            IsOpenUser,
            IsOpenMemory,
            IsOpenMemoryPoint,
            _LastColumn
        }
    }
}

