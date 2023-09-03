using System;
using System.IO;

// ReSharper disable once CheckNamespace
namespace TimeLogger
{
    public static class Consts
    {
        public static Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version ?? new Version();

        public static string AppDataPath =>
#if DEBUG
            AppPath;
        // Path.GetDirectoryName(Application.ResourceAssembly.Location)!;
#else
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TimeLogger");
#endif
        public static string AppPath => AppDomain.CurrentDomain.BaseDirectory;
    }
}
