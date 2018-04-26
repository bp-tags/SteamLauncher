using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SteamLauncher
{
    class Program
    {
        static string GetSteamPath()
        {
            var processes = Process.GetProcessesByName("Steam");
            if (processes.Length == 0)
                throw new Exception("Steam isn't running");

            return Path.GetDirectoryName(processes[0].MainModule.FileName);
        }

        static void StartGameOverlay(int processId)
        {
            string steamPath = GetSteamPath();
            Process process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    FileName = Path.Combine(steamPath, "GameOverlayUI.exe"),
                    WorkingDirectory = steamPath,
                    Arguments = $"{processId}"
                }
            };

            process.Start();
        }

        static void StartGame(string fileName)
        {
            // Resolve symlink target
            fileName = NativeMethods.GetFinalPathName(fileName);

            var process = new Process
            {
                StartInfo =
                {
                    FileName = fileName
                }
            };

            if (Path.GetExtension(fileName).ToLower() == "exe")
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(fileName);

            process.Start();
            StartGameOverlay(process.Id);

            process.WaitForExit();
        }

        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                    StartGame(args[0]);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK);
            }
        }
    }
}
