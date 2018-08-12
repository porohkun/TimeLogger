using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharpSetup.Dialogs;
using File = WixSharp.File;
using SFAct = WixSharpSetup.SourceFile.Action;
// ReSharper disable CoVariantArrayConversion

namespace WixSharpSetup
{
    class Program
    {
        private static string Product = "TimeLogger";
        private static string Manufacturer = "Porohkun";

        private static string SourcePath = @"..\..\..\TimeLogger\bin\Release\";
        private static List<SourceFile> SourceFiles = SourceFile.NewList(
            new SourceFile(@"..\..\..\TimeLogger\bin\Release\TimeLogger.exe",
                SFAct.Install | SFAct.CopyToFtp | SFAct.ShortCut | SFAct.Version),
            new SourceFile(@"..\..\..\TimeLogger\bin\Release\MimiJson.dll",
                SFAct.Install | SFAct.CopyToFtp),
            new SourceFile(@"..\..\..\TimeLogger\bin\Release\System.Windows.Controls.Input.Toolkit.dll",
                SFAct.Install | SFAct.CopyToFtp),
            new SourceFile(@"..\..\..\TimeLogger\bin\Release\System.Windows.Controls.Layout.Toolkit.dll",
                SFAct.Install | SFAct.CopyToFtp),
            new SourceFile(@"..\..\..\TimeLogger\bin\Release\WPFControls.dll",
                SFAct.Install | SFAct.CopyToFtp),
            new SourceFile(@"..\..\..\TimeLogger\bin\Release\WPFToolkit.dll",
                SFAct.Install | SFAct.CopyToFtp),
            new SourceFile(@"..\..\..\TimeLogger\bin\Release\update-config.json",
                SFAct.CopyToFtp),
            new SourceFile(@"..\..\..\Installer\bin\Release\Installer.exe",
                SFAct.CopyToFtp),
            new SourceFile(@"..\..\..\WixSharpSetup\bin\Release\TimeLoggerSetup.msi",
                SFAct.CopyToFtp)
            );

        static void Main()
        {
            try
            {
                var project = new ManagedProject(Product,
                    new Dir($"%ProgramFiles%\\{Manufacturer}\\{Product}",
                        SourceFiles.Where(f => f.HaveAction(SFAct.Install)).Select(f => new File(f.FullPath) as WixEntity).ToArray())
                    );

                project.OutFileName = "TimeLoggerSetup";

                var versionFile = SourceFiles.FirstOrDefault(f => f.HaveAction(SFAct.Version));
                if (versionFile == null)
                {
                    Console.WriteLine("No files with 'version' flag");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine(versionFile.FullPath);
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(versionFile.FullPath);
                var version = new Version(fileVersionInfo.FileVersion);

                project.Version = version;
                project.UpgradeCode = new Guid("F9B694E9-4951-4A01-B6CD-9C19A1F70D79");
                project.GUID = Guid.NewGuid();
                project.MajorUpgrade = MajorUpgrade.Default;
                project.MajorUpgrade.AllowSameVersionUpgrades = true;
                project.MajorUpgrade.AllowDowngrades = false;

                project.ManagedUI = new ManagedUI();

                project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                                .Add<InstallDirDialog>()
                                                .Add<ProgressDialog>()
                                                .Add<ExitDialog>();

                project.ManagedUI.ModifyDialogs.Add<MaintenanceTypeDialog>()
                                               .Add<ProgressDialog>()
                                               .Add<ExitDialog>();

                //project.SourceBaseDir = "<input dir path>";
                //project.OutDir = "<output dir path>";

                ValidateAssemblyCompatibility();

                project.BuildMsi();

                if (!Directory.Exists("Upload"))
                    Directory.CreateDirectory("Upload");
                foreach (var file in Directory.GetFiles("Upload"))
                    System.IO.File.Delete(file);
                foreach (var file in SourceFiles.Where(f => f.HaveAction(SFAct.CopyToFtp)))
                    System.IO.File.Copy(file.FullPath, Path.Combine("Upload", file.Name));
                
                foreach (var file in SourceFiles.Where(f => f.Name == "update-config.json"))
                    System.IO.File.Copy(file.FullPath, Path.Combine("../../../docs", file.Name));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }

        static void ValidateAssemblyCompatibility()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            if (!assembly.ImageRuntimeVersion.StartsWith("v2."))
            {
                Console.WriteLine("Warning: assembly '{0}' is compiled for {1} runtime, which may not be compatible with the CLR version hosted by MSI. " +
                                  "The incompatibility is particularly possible for the EmbeddedUI scenarios. " +
                                   "The safest way to solve the problem is to compile the assembly for v3.5 Target Framework.",
                                   assembly.GetName().Name, assembly.ImageRuntimeVersion);
            }
        }
    }

}