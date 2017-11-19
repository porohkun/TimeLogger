using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimiJson;
using System.IO;

namespace Installer
{
	class Program
	{
		public static string AppDataPath { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Porohkun", "TimeLogger"); } }
		static string UpdatePath { get { return Path.Combine(AppDataPath, "Update"); } }

		static void Main(string[] args)
		{
			try
			{
				var path = args[0];
				var procId = int.Parse(args[1]);
				bool exceptions = false;
				Console.WriteLine("Waiting for app closing...");
				Process proc = null;
				do
				{
					try
					{
						proc = Process.GetProcessById(procId);
					}
					catch
					{
						proc = null;
					}
				} while (proc != null);
				Console.WriteLine("done.");

				var updFilePath = Path.Combine(UpdatePath, "update-config.json");
				var json = JsonValue.ParseFile(updFilePath);
				var version = Version.Parse(json["version"]);

				foreach (var jfile in json["files"])
				{
					var updPath = Path.Combine(UpdatePath, jfile["path"], jfile["name"]);
					var filePath = Path.Combine(path, jfile["path"], jfile["name"]);
					switch (jfile["action"].String)
					{
						case "replace":
						case "add":
							Console.WriteLine("Copying file " + jfile["name"]);
							exceptions = exceptions || CatchAction(() => Directory.CreateDirectory(Path.GetDirectoryName(filePath)));
							exceptions = exceptions || CatchAction(() => File.Copy(updPath, filePath, true));
							break;
						case "del":
						case "delete":
							Console.WriteLine("Deleting file " + jfile["name"]);
							exceptions = exceptions || CatchAction(() => File.Delete(filePath));
							break;
					}
				}

				Console.WriteLine("updating reg version");
				exceptions = exceptions || CatchAction(() => TestVersion.SetVersion("Time Logger", string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build)));

				Console.WriteLine("all done.");
				if (exceptions)
					Console.ReadKey();

				try
				{
					ProcessStartInfo info = new ProcessStartInfo(Path.Combine(path, "TimeLogger.exe"));
					info.Arguments = "-delete";
					Process.Start(info);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
					Console.ReadLine();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.ReadLine();
			}
		}

		public static bool CatchAction(Action action)
		{
			try
			{
				action.Invoke();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				return true;
			}
			return false;
		}
	}
}
