using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Dropper
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string currentName = Process.GetCurrentProcess().ProcessName;
            foreach (var process in Process.GetProcessesByName(currentName))
            {
                if (process.Id != Process.GetCurrentProcess().Id)
                    process.Kill();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
