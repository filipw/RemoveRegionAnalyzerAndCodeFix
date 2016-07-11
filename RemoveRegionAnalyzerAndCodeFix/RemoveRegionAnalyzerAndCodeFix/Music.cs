using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace RemoveRegionAnalyzerAndCodeFix
{
    public static class Music
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern uint GetShortPathName(
            [MarshalAs(UnmanagedType.LPTStr)] string lpszLongPath,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszShortPath,
            uint cchBuffer);

        [DllImport("winmm.dll")]
        static extern int mciSendString(string mciCommand, StringBuilder buffer, int bufferSize, IntPtr callback);

        private static void Send(string mciCommand)
        {
            mciSendString(mciCommand, null, 0, IntPtr.Zero);
        }

        public static void Play(string fileName)
        {
            var assembly = typeof(Music).GetTypeInfo().Assembly;
            var codeBaseProp = assembly.GetType().GetRuntimeProperty("Location");
            var path = codeBaseProp.GetValue(assembly) as string;
            var filePath = Path.Combine(path.Replace("RemoveRegionAnalyzerAndCodeFix.dll", ""), fileName);

            uint bufferSize = 256;
            var shortNameBuffer = new StringBuilder((int)bufferSize);
            GetShortPathName(filePath, shortNameBuffer, bufferSize);

            var shortPath = shortNameBuffer.ToString();

            Send("open " + shortPath);
            Send("play " + shortPath);
        }
    }
}
