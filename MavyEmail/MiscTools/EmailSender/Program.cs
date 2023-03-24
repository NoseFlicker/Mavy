using System;
using System.Threading;
using System.Windows.Forms;
using QiHe.MiscTools.EmailSender;

namespace Mavy.MiscTools.EmailSender
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(true);
                Application.Run(new MainForm());  
            }).Start();
        }
    }
}