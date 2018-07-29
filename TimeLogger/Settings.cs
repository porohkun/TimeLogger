using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MimiJson;
using System.Windows;
using System.Security;
using System.Net;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace TimeLogger
{
	public static class Settings
	{
		public static Version CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version;
		public static string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Porohkun", "TimeLogger");
		public static string UpdatePath => Path.Combine(AppDataPath, "Update");
		public static string AppPath => AppDomain.CurrentDomain.BaseDirectory;
		public static string SettingsPath => Path.Combine(AppDataPath, "settings.json");
#if DEBUG
        public static string TasksPath => "tasks.json";
#else
		public static string TasksPath => Path.Combine(AppDataPath, "tasks.json");
#endif
        public static string UpdateConfigUrl => "https://porohkun.github.io/TimeLogger/update-config.json";

		public static string Title => "TimeLogger" +
#if DEBUG
		                              " [DEBUG]" +
#endif
		                              " v." + CurrentVersion;

		private static string ShortcutPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "TimeLogger.lnk");

		private static bool _autoUpdate = true;
		public static event EventHandler AutoUpdateChanged;
		public static bool AutoUpdate
		{
			get => _autoUpdate;
			set
			{
				if (value != _autoUpdate)
				{
					_autoUpdate = value;
					AutoUpdateChanged?.Invoke(null, EventArgs.Empty);
					Save();
				}
			}
		}

		public static event EventHandler LaunchOnStartupChanged;
		public static bool LaunchOnStartup
		{
			get => File.Exists(ShortcutPath);
			set
			{
				if (value != LaunchOnStartup)
				{
					if (value)
					{
						WshShell shell = new WshShell();
						IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(ShortcutPath);
						shortcut.TargetPath = Path.Combine(AppPath, "TimeLogger.exe") + " -hidden";
						shortcut.IconLocation = Path.Combine(AppPath, "TimeLogger.exe") + ",0";
						shortcut.Save();
					}
					else
					{
						File.Delete(ShortcutPath);
					}
					LaunchOnStartupChanged?.Invoke(null, EventArgs.Empty);
				}
			}
		}

		private static void CreateShortcut()
		{
			WshShell shell = new WshShell();
			IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(ShortcutPath);
			shortcut.Description = "New shortcut for a Notepad";
			shortcut.Hotkey = "Ctrl+Shift+N";
			shortcut.TargetPath = Path.Combine(AppPath,"TimeLogger.exe")+" -hidden";
			shortcut.Save();
		}

		static Settings()
		{
			if (!Directory.Exists(AppDataPath))
				Directory.CreateDirectory(AppDataPath);
			if (File.Exists(SettingsPath))
			{
				try
				{
					var json = JsonValue.ParseFile(SettingsPath);
					AutoUpdate = json["auto_update"];
				}
				catch (Exception)
				{
					Save();
				}
			}
			else
			{
				Save();
			}
		}

		public static void Save()
		{
			try
			{
				var json = new JsonValue(new JsonObject(
					new JOPair("auto_update", AutoUpdate)
				));
				json.ToFile(SettingsPath);
			}
			catch (Exception e)
			{
				MessageBox.Show("Cant save settings file:\r\n" + e.Message);
			}
		}
	}
}
