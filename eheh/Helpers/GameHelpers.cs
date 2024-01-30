using eheh.Enums;
using eheh.Extensions;
using eheh.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace eheh.Helpers
{
    public static class GameHelper
    {
        private static Stack<MetaFileEntry> m_currentTasks;

        private static DateTime? m_lastUpdateCheck;

        private static readonly FileSizeFormatProvider m_bytesFormatProvider = new FileSizeFormatProvider();

        private static MetaFile m_metaFile;

        private static readonly BackgroundWorker m_MD5Worker = new BackgroundWorker();

        private static DateTime? m_lastProgressChange;

        private static long m_lastGlobalDownloadedBytes;

        private static long m_lastFileDownloadedBytes;

        private static WebClient m_client = new WebClient();

        private static string m_progressDownloadSpeedInfo;

        public static bool GameIsRunning = false;

        private static string m_lastServerName = "";

        private static string m_lastServerMap = "";

        public static MemoryHelper Memory
        {
            get;
        } = new MemoryHelper();


        public static eheh.Windows.Content.Update UpdateControl
        {
            get;
            set;
        }

        public static WebClient WebClient => m_client;

        public static bool IsUpdating
        {
            get;
            set;
        }

        public static bool IsUpToDate
        {
            get;
            set;
        }

        public static string LocalChecksum
        {
            get;
            private set;
        }

        public static DateTime? LastVote
        {
            get;
            private set;
        }

        public static double DownloadProgress
        {
            get;
            set;
        }

        public static Color StateMessageColor
        {
            get;
            set;
        }

        public static string StateMessage
        {
            get;
            set;
        }

        public static long TotalBytesToDownload
        {
            get;
            set;
        }

        public static long TotalDownloadedBytes
        {
            get;
            set;
        }

        public static bool GlobalDownloadProgress
        {
            get;
            set;
        }

        public static string ProgressDownloadSpeedInfo
        {
            get
            {
                return m_progressDownloadSpeedInfo;
            }
            set
            {
                m_progressDownloadSpeedInfo = value;
                UpdateControl.SetSpeed(value);
            }
        }

        public static bool TeknoMW3IsRunning { get; set; }
        public static bool TeknoBoIsRunning { get; set; }

        public static Process TeknoMW3Process { get; set; }
        public static Process TeknoBoProcess { get; set; }

        public static bool IsPathCorrect()
        {
            return File.Exists("localization.txt");
        }

        private static bool CanPlay()
        {
            if (!IsUpdating)
            {
                return IsUpToDate;
            }
            return false;
        }

        public static void Play(string arguments, Game game)
        {
            if (!CanPlay())
            {
                return;
            }
            if (m_lastUpdateCheck.HasValue)
            {
                DateTime now = DateTime.Now;
                DateTime? lastUpdateCheck = m_lastUpdateCheck;
                if (!(now - lastUpdateCheck > TimeSpan.FromMinutes(5.0)))
                {
                    goto IL_008d;
                }
            }
            CheckUpdates();
        IL_008d:
            if (game == Game.TeknoMW3)
            {
                if (!File.Exists("iw5mp.exe"))
                {
                    UpdateControl.SetStatus("iw5mp.exe doesn't exist, run updater or contact TeknoGods Staff");
                    return;
                }
                else if (TeknoMW3IsRunning)
                {
                    MessageBox.Show("Game is already running");
                    return;
                }

                //TeknoMW3Process = Process.Start(new ProcessStartInfo("iw5mp", arguments));
                //ProcessExecute("iw5mp.exe", arguments);
                TeknoMW3IsRunning = true;
                //if (PlayerSettings.Instance.ClanTile == "512")
                //{
                //    if (!Memory.Process_Handle("iw5mp")) return;
                //    Memory.WriteInteger(0x1328D50, 512); // null title background
                //}
            }
            else
            {
                return;
            }
        }

        private static bool CheckURL()
        {
            if (!WebSiteIsAvailable(Variables.UpdateSiteURL + "patch.xml"))
            {
                Variables.UpdateSiteURL = "http://downloads.teknomw3.pw/client/";
                if (!WebSiteIsAvailable(Variables.UpdateSiteURL + "patch.xml"))
                {
                    Variables.UpdateSiteURL = "http://updates.teknogods.com/client/";
                    if (!WebSiteIsAvailable(Variables.UpdateSiteURL + "patch.xml"))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool WebSiteIsAvailable(string Url)
        {
            string text = string.Empty;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
            httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            httpWebRequest.Method = "GET";
            try
            {
                using ((HttpWebResponse)httpWebRequest.GetResponse())
                {
                }
            }
            catch (WebException ex)
            {
                text = text + ((text.Length > 0) ? "\n" : "") + ex.Message;
            }
            return text.Length == 0;
        }

        public static void CheckUpdates()
        {
            if (!IsUpdating)
            {
                if (!CheckURL())
                {
                    UpdateControl.SetStatus("Update servers are currently unavailable.");
                }
                IsUpdating = true;
                UpdateControl.SetStatus("Downloading informations ...");
                m_client = new WebClient();
                m_client.DownloadProgressChanged += OnDownloadProgressChanged;
                m_client.DownloadStringCompleted += OnPatchDownloaded;
                try
                {
                    m_client.DownloadStringAsync(new Uri(Variables.UpdateSiteURL + "patch.xml"), "patch.xml");
                }
                catch (SocketException)
                {
                    UpdateControl.SetStatus("Server offline");
                }
            }
        }

        private static void OnPatchDownloaded(object sender, DownloadStringCompletedEventArgs e)
        {
            ProgressDownloadSpeedInfo = string.Empty;
            m_client.DownloadStringCompleted -= OnPatchDownloaded;
            try
            {
                m_metaFile = XmlUtils.Deserialize<MetaFile>(new StringReader(e.Result));
                m_MD5Worker.WorkerReportsProgress = true;
                m_MD5Worker.DoWork += MD5Worker_DoWork;
                m_MD5Worker.ProgressChanged += MD5Worker_ProgressChanged;
                m_MD5Worker.RunWorkerCompleted += MD5Worker_RunWorkerCompleted;
                if (!File.Exists("teknomw3.checksum"))
                {
                    m_MD5Worker.RunWorkerAsync();
                }
                else
                {
                    LocalChecksum = File.ReadAllText("teknomw3.checksum");
                    if (string.IsNullOrEmpty(LocalChecksum))
                    {
                        m_MD5Worker.RunWorkerAsync();
                    }
                    else
                    {
                        CompareChecksums();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleDownloadError(cancelled: false, ex, (string)e.UserState);
            }
        }

        private static void MD5Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            m_MD5Worker.DoWork -= MD5Worker_DoWork;
            string currentDirectory = Directory.GetCurrentDirectory();
            HashSet<string> filesNames = new HashSet<string>(from x in m_metaFile.Tasks
                                                             select x.LocalURL);
            List<string> list = (from x in Directory.GetFiles(currentDirectory, "*.*", SearchOption.AllDirectories)
                                 where filesNames.Contains(GetRelativePath(Path.GetFullPath(x), Path.GetFullPath("./")))
                                 select x into p
                                 orderby p
                                 select p).ToList();
            MD5 mD = MD5.Create();
            DateTime now = DateTime.Now;
            long location = 0L;
            int location2 = 0;
            foreach (string item in list.Take(list.Count - 1))
            {
                string text = item.Substring(currentDirectory.Length + 1);
                byte[] bytes = Encoding.UTF8.GetBytes(text.ToLower());
                mD.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
                Interlocked.Add(ref location, bytes.Length);
                byte[] array = File.ReadAllBytes(item);
                mD.TransformBlock(array, 0, array.Length, array, 0);
                Interlocked.Add(ref location, array.Length);
                Interlocked.Increment(ref location2);
                int percentProgress = location2 * 100 / list.Count;
                m_MD5Worker.ReportProgress(percentProgress, (double)location / (DateTime.Now - now).TotalSeconds);
            }
            if (list.Count > 0)
            {
                byte[] array2 = File.ReadAllBytes(list.Last());
                mD.TransformFinalBlock(array2, 0, array2.Length);
            }
            LocalChecksum = ((list.Count > 0) ? BitConverter.ToString(mD.Hash).Replace("-", "").ToLower() : string.Empty);
            File.WriteAllText("teknomw3.checksum", LocalChecksum);
            m_lastUpdateCheck = DateTime.Now;
        }

        private static void MD5Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateControl.SetExtra(string.Format(m_bytesFormatProvider, "Checking file validity... ({0} % done) ({1:fs}/s)", new object[2]
            {
                e.ProgressPercentage,
                (double)e.UserState
            }));
            UpdateControl.SetPercentage(e.ProgressPercentage);
        }

        private static void MD5Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            m_MD5Worker.RunWorkerCompleted -= MD5Worker_RunWorkerCompleted;
            DownloadProgress = 100.0;
            UpdateControl.SetPercentage((int)DownloadProgress);
            UpdateControl.SetExtra("Game files verification done.");
            CompareChecksums();
        }

        private static void CompareChecksums()
        {
            try
            {
                if (m_metaFile != null && m_metaFile.FolderChecksum != LocalChecksum)
                {
                    IsUpToDate = false;
                    m_currentTasks = new Stack<MetaFileEntry>(m_metaFile.Tasks);
                    GlobalDownloadProgress = true;
                    TotalBytesToDownload = m_metaFile.Tasks.Sum((MetaFileEntry x) => x.FileSize);
                    DownloadProgress = 0.0;
                    ProgressDownloadSpeedInfo = string.Empty;
                    UpdateControl.SetPercentage((int)DownloadProgress);
                    ProcessTask();
                }
                else
                {
                    File.WriteAllText("teknomw3.checksum", LocalChecksum);
                    UpdateControl.SetStatus($"Game is up to date");
                    IsUpdating = false;
                    IsUpToDate = true;
                    UpdateControl.MainWindow.EnableThings(true);
                }
            }
            catch (Exception ex)
            {
                HandleDownloadError(cancelled: false, ex, Variables.UpdateSiteURL + "patch.xml");
            }
        }

        private static void ProcessTask()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                if (m_currentTasks.Count == 0)
                {
                    OnUpdateEnded(success: true);
                }
                else
                {
                    MetaFileEntry metaFileEntry = m_currentTasks.Pop();
                    metaFileEntry.Downloaded += OnTaskApplied;
                    metaFileEntry.Download();
                }
            });
        }

        private static void OnTaskApplied(MetaFileEntry x)
        {
            TotalDownloadedBytes += x.FileSize;
            DownloadProgress = (double)TotalDownloadedBytes / (double)TotalBytesToDownload * 100.0;
            UpdateControl.SetPercentage((int)DownloadProgress);
            ProcessTask();
        }

        private static void OnUpdateEnded(bool success)
        {
            UpdateControl.SetTime("0 s");
            UpdateControl.SetSpeed("0");
            if (success)
            {
                UpdateControl.SetStatus($"Game is up to date");
                LocalChecksum = m_metaFile.FolderChecksum;
                File.WriteAllText("teknomw3.checksum", LocalChecksum);
                UpdateControl.MainWindow.EnableThings(true);
            }
            IsUpToDate = true;
            IsUpdating = false;
            GlobalDownloadProgress = false;
            ProgressDownloadSpeedInfo = string.Empty;
            UpdateControl.MainWindow.EnableThings(true);
        }

        private static void HandleDownloadError(bool cancelled, Exception ex, string url)
        {
            if (cancelled)
            {
                UpdateControl.SetStatus("Update failled");
                UpdateControl.SetExtra("Update failed");
            }
            else
            {
                IsUpToDate = true;
                UpdateControl.SetStatus("Update failled");
                UpdateControl.SetExtra($"Error when updating : {ex.InnerException.Message}");
                UpdateControl.MainWindow.EnableThings(true);
            }
            OnUpdateEnded(success: false);
        }

        private static void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!GlobalDownloadProgress)
            {
                DownloadProgress = (double)e.BytesReceived / (double)e.TotalBytesToReceive * 100.0;
                if (m_lastProgressChange.HasValue && DateTime.Now - m_lastProgressChange.Value > TimeSpan.FromSeconds(1.0))
                {
                    ProgressDownloadSpeedInfo = string.Format(m_bytesFormatProvider, "{0:fs} / {1:fs} ({2:fs}/s)", new object[3]
                    {
                        e.BytesReceived,
                        e.TotalBytesToReceive,
                        (double)(e.BytesReceived - m_lastFileDownloadedBytes) / (DateTime.Now - m_lastProgressChange.Value).TotalSeconds
                    });
                    m_lastProgressChange = DateTime.Now;
                    m_lastFileDownloadedBytes = e.BytesReceived;
                    UpdateControl.SetSpeed(ProgressDownloadSpeedInfo);
                }
            }
            else if (m_lastProgressChange.HasValue && DateTime.Now - m_lastProgressChange.Value > TimeSpan.FromSeconds(1.0))
            {
                ProgressDownloadSpeedInfo = string.Format(m_bytesFormatProvider, "{0:fs} / {1:fs} ({2:fs}/s)", new object[3]
                {
                    TotalDownloadedBytes + e.BytesReceived,
                    TotalBytesToDownload,
                    (double)(TotalDownloadedBytes + e.BytesReceived - m_lastGlobalDownloadedBytes) / (DateTime.Now - m_lastProgressChange.Value).TotalSeconds
                });
                UpdateControl.SetSpeed(ProgressDownloadSpeedInfo);
                m_lastProgressChange = DateTime.Now;
                m_lastGlobalDownloadedBytes = TotalDownloadedBytes + e.BytesReceived;
            }
            if (!m_lastProgressChange.HasValue)
            {
                m_lastProgressChange = DateTime.Now;
            }
        }

        private static string GetRelativePath(string fullPath, string relativeTo)
        {
            string[] array = fullPath.Split(new string[1]
            {
                relativeTo.Replace("/", "\\").Replace("\\\\", "\\")
            }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length == 0)
            {
                return "";
            }
            return array.Last();
        }

        //public static void UpdateDiscordInfos()
        //{
        //    System.Timers.Timer timer = new System.Timers.Timer(5000.0);
        //    timer.AutoReset = true;
        //    timer.Elapsed += delegate
        //    {
        //        string serverName = GetServerName();
        //        if (serverName == "offline")
        //        {
        //            DiscordHelper.Presence = null;
        //        }
        //        else
        //        {
        //            string serverMap = GetServerMap();
        //            string serverImgMap = GetServerImgMap();
        //            GetServerIP();
        //            string serverCurrentGametype = GetServerCurrentGametype();
        //            byte[] serverPlayerCount = GetServerPlayerCount();
        //            byte[] serverSlots = GetServerSlots();
        //            if (serverPlayerCount[0] == 0)
        //            {
        //                DiscordHelper.Presence = new RichPresence
        //                {
        //                    Details = "Main Menu",
        //                    State = "Browsing ServerList",
        //                    Assets = new Assets
        //                    {
        //                        LargeImageText = "TeknoMW3",
        //                        LargeImageKey = "mw3_logo"
        //                    },
        //                    Timestamps = new Timestamps
        //                    {
        //                        Start = DiscordHelper.DateStarted
        //                    }
        //                };
        //            }
        //            else
        //            {
        //                DiscordHelper.Presence = new RichPresence
        //                {
        //                    Details = $"{serverName}",
        //                    State = $"{serverCurrentGametype} ({serverPlayerCount[0]}/{serverSlots[0]})",
        //                    Assets = new Assets
        //                    {
        //                        LargeImageText = serverMap,
        //                        LargeImageKey = serverImgMap
        //                    },
        //                    Timestamps = new Timestamps
        //                    {
        //                        Start = DiscordHelper.DateStarted
        //                    }
        //                };
        //                if (serverPlayerCount[0] == 0)
        //                {
        //                    DiscordHelper.Presence.Details = "Main Menu";
        //                    DiscordHelper.Presence.State = "Browsing Server List";
        //                    DiscordHelper.Presence.Assets = new Assets
        //                    {
        //                        LargeImageText = "TeknoMW3",
        //                        LargeImageKey = "mw3_logo"
        //                    };
        //                }
        //                else
        //                {
        //                    DiscordHelper.Presence.Details = $"{serverName}";
        //                    DiscordHelper.Presence.State = $"{serverCurrentGametype} ({serverPlayerCount[0]} of {serverSlots[0]})";
        //                    m_lastServerName = serverName;
        //                    DiscordHelper.Presence.Assets = new Assets
        //                    {
        //                        LargeImageText = serverMap,
        //                        LargeImageKey = serverImgMap
        //                    };
        //                    m_lastServerMap = serverMap;
        //                }
        //            }
        //        }
        //    };
        //    timer.Start();
        //}

        private static string GetServerName()
        {
            if (!Memory.Process_Handle("iw5mp"))
            {
                return "offline";
            }
            return Memory.ReadString(9415652, 30).Replace("\0", "").RemoveColors();
        }

        private static string GetServerIP()
        {
            if (!Memory.Process_Handle("iw5mp"))
            {
                return "offline";
            }
            return Memory.ReadString(12200440, 30);
        }

        private static string GetServerCurrentGametype()
        {
            if (!Memory.Process_Handle("iw5mp"))
            {
                return "offline";
            }
            string text = Memory.ReadString(9415620, 10);
            if (text.Contains("sd"))
            {
                text = "Search & Destroy";
            }
            else if (text.Contains("dm"))
            {
                text = "Free-For-All";
            }
            else if (text.Contains("ctf"))
            {
                text = "Capture the Flag";
            }
            else if (text.Contains("dom"))
            {
                text = "Domination";
            }
            else if (text.Contains("war"))
            {
                text = "Team Deathmatch";
            }
            else if (text.Contains("grnd"))
            {
                text = "Drop Zone";
            }
            else if (text.Contains("koth"))
            {
                text = "Headquarters";
            }
            else if (text.Contains("gun"))
            {
                text = "Gun Game";
            }
            else if (text.Contains("conf"))
            {
                text = "Kill Confirmed";
            }
            else if (text.Contains("oic"))
            {
                text = "One in the Chamber";
            }
            else if (text.Contains("sab"))
            {
                text = "Sabotage";
            }
            else if (text.Contains("tdef"))
            {
                text = "Team Defender";
            }
            else if (text.Contains("tjugg"))
            {
                text = "Team Juggernaut";
            }
            else if (text.Contains("infect"))
            {
                text = "Infected";
            }
            return text.RemoveColors();
        }

        private static byte[] GetServerPlayerCount()
        {
            return Memory.ReadBytes(9459592, 2);
        }

        private static byte[] GetServerSlots()
        {
            Memory.Process_Handle("iw5mp");
            return Memory.ReadBytes(9415912, 2);
        }

        private static string GetServerMap()
        {
            if (!Memory.Process_Handle("iw5mp"))
            {
                return "offline";
            }
            switch (Memory.ReadString(9415928, 20).Split('.')[0])
            {
                case "mp_dome":
                    return "Dome";
                case "mp_seatown":
                    return "Seatown";
                case "mp_paris":
                    return "Resistance";
                case "mp_hardhat":
                    return "Hardhat";
                case "mp_alpha":
                    return "Lockdown";
                case "mp_bravo":
                    return "Mission";
                case "mp_exchange":
                    return "Downturn";
                case "mp_interchange":
                    return "Interchange";
                case "mp_migadishu":
                    return "Bakaara";
                case "mp_plaza2":
                    return "Arkaden";
                case "mp_carbon":
                    return "Carbon";
                case "mp_outpost":
                    return "Outpost";
                case "mp_underground":
                    return "Underground";
                case "mp_village":
                    return "Village";
                case "mp_radar":
                    return "Outpost";
                default:
                    return "DLC Map";
            }
        }

        private static string GetServerImgMap()
        {
            return Memory.ReadString(9415928, 20).Split('.')[0];
        }
    }
}
