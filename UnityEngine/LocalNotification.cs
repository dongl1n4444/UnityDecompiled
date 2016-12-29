namespace UnityEngine
{
    using System;
    using System.Collections;

    [Obsolete("LocalNotification is deprecated. Please use iOS.LocalNotification instead (UnityUpgradable) -> UnityEngine.iOS.LocalNotification", true)]
    public sealed class LocalNotification
    {
        public string alertAction
        {
            get => 
                null;
            set
            {
            }
        }

        public string alertBody
        {
            get => 
                null;
            set
            {
            }
        }

        public string alertLaunchImage
        {
            get => 
                null;
            set
            {
            }
        }

        public int applicationIconBadgeNumber
        {
            get => 
                0;
            set
            {
            }
        }

        public static string defaultSoundName =>
            null;

        public DateTime fireDate
        {
            get => 
                new DateTime();
            set
            {
            }
        }

        public bool hasAction
        {
            get => 
                false;
            set
            {
            }
        }

        public CalendarIdentifier repeatCalendar
        {
            get => 
                CalendarIdentifier.GregorianCalendar;
            set
            {
            }
        }

        public CalendarUnit repeatInterval
        {
            get => 
                CalendarUnit.Era;
            set
            {
            }
        }

        public string soundName
        {
            get => 
                null;
            set
            {
            }
        }

        public string timeZone
        {
            get => 
                null;
            set
            {
            }
        }

        public IDictionary userInfo
        {
            get => 
                null;
            set
            {
            }
        }
    }
}

