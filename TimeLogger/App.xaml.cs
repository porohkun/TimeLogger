using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TimeLogger
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if (e.Args.Contains("-delete"))
			{
				System.IO.Directory.Delete(Settings.UpdatePath, true);
			}
			(new MainWindow(e.Args.Contains("-hidden"))).Show();
		}
	}
}
