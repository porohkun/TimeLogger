using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;


public class TestVersion
{
    private const string subkey32 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
    private const string subkey64 = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
    private static string subkey { get { return Environment.Is64BitOperatingSystem ? subkey64 : subkey32; } }
    private static RegistryKey baseRegistryKey = Registry.LocalMachine;
    private static RegistryKey unistallKey { get { return baseRegistryKey.OpenSubKey(subkey); } }
    
    public static string GetVersion(string nameToSearch)
    {
        using (unistallKey)
        {
            string[] allApplications = unistallKey.GetSubKeyNames();
            foreach (string name in allApplications)
            {
                using (RegistryKey appKey = baseRegistryKey.OpenSubKey(Path.Combine(subkey, name)))
                {
                    string appName = (string)appKey.GetValue("DisplayName");
                    if (appName == nameToSearch)
                        return (string)appKey.GetValue("DisplayVersion");
                }
            }
        }
        return null;
    }

    public static bool SetVersion(string nameToSearch, string version)
    {
        using (unistallKey)
        {
            string[] allApplications = unistallKey.GetSubKeyNames();
            foreach (string name in allApplications)
            {
                using (RegistryKey appKey = baseRegistryKey.OpenSubKey(Path.Combine(subkey, name), true))
                {
                    string appName = (string)appKey.GetValue("DisplayName");
                    if (appName == nameToSearch)
                    {
                        appKey.SetValue("DisplayVersion", version, RegistryValueKind.String);
                        appKey.Close();
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
