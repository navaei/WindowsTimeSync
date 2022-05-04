using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WindowsTimeSync
{
    public static class WinDateTime
    {
        [DllImport("kernel32.dll", EntryPoint = "SetSystemTime", SetLastError = true)]
        private static extern bool Win32SetSystemTime(ref SystemTime sysTime);

        [DllImport("kernel32.dll", EntryPoint = "SetLocalTime", SetLastError = true)]
        private static extern bool Win32SetLocalTime(ref SystemTime sysTime);


        [DllImport("w32time.dll")]
        public static extern uint W32TimeSyncNow([MarshalAs(UnmanagedType.LPWStr)] String computername, bool wait, uint flag);

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort Year;
            public ushort Month;
            public ushort DayOfWeek;
            public ushort Day;
            public ushort Hour;
            public ushort Minute;
            public ushort Second;
            public ushort Millisecond;
        };

        public static void SetDateTime(int year, int month, int day, int hour,
            int minute, int second, int millisecond, string local = null)
        {
            SystemTime updatedTime = new SystemTime
            {
                Year = (ushort)year,
                Month = (ushort)month,
                Day = (ushort)day,
                Hour = (ushort)hour,
                Minute = (ushort)minute,
                Second = (ushort)second,
                Millisecond = (ushort)millisecond
            };

            if (!string.IsNullOrEmpty(local))
            {
                if (!Win32SetLocalTime(ref updatedTime))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else
            {
                if (!Win32SetSystemTime(ref updatedTime))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public static void SetDateTime(DateTime datetime, string local = null)
        {
            SetDateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute,
                datetime.Second, datetime.Millisecond, local);
        }

        public static string GetSyncDate()
        {
            return W32TimeSyncNow("computername", true, 8).ToString();
        }
    }
}
