using eheh.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace eheh.Utils
{
    static class Variables
    {
        public const string SiteURL = "https://www.teknomw3.pw/";

        public const string ForumURL = "https://forum.teknomw3.pw/";

        public const string DiscordURL = "https://discord.gg/ytV9nPw";

        public static string UpdateSiteURL = "http://alpaa.fr/update/TeknoMW3/";

        public const string TeknoExePath = "iw5mp.exe";

        public const string LocalChecksumFile = "teknomw3.checksum";

        public const string RemotePatchFile = "patch.xml";

        public const string ExeReplaceTempPath = "temp_upl.exe";

        public const string IniFile = "teknogods.ini";

        public const string CfgFile = "players2\\config_mp.cfg";

        public static string CurrentExePath => Assembly.GetExecutingAssembly().Location;

        public static string ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static string ApplicationName => "Tekno Launcher";

        internal static Dictionary<string, string> TeknoMW3Settings = new Dictionary<string, string>()
        {
            { "ID", "" },
            { "Name", "" },
            { "FOV", "" },
            { "ClanTag", "" },
            { "Title", "" },
            { "Maxfps", "" },
            { "ClanTitle", "" },
            { "ClanEmblem", "" }
        };

        internal static void ChangeDvar(string dvarName, string value)
        {
            if (!File.Exists("players2\\config_mp.cfg"))
            {
                return;
            }
            if (new FileInfo("players2\\config_mp.cfg").IsReadOnly)
            {
                //App.AMainWindow.SetState(string.Format("The file {0} is read-only, dvar can't be changed", "players2\\config_mp.cfg"));
                return;
            }
            string[] array = File.ReadAllLines("players2\\config_mp.cfg");
            string sensitivityLine = array.FirstOrDefault((string x) => x.StartsWith($"seta {dvarName}"));
            if (sensitivityLine != null)
            {
                int num = Array.FindIndex(array, (string x) => x == sensitivityLine);
                array[num] = $"seta {dvarName} \"" + value + "\"";
                File.WriteAllLines("players2\\config_mp.cfg", array);
            }
        }

        internal static bool LoadSettings()
        {
            if (File.Exists("teknogods.ini"))
            {
                IniFile iniFile = new IniFile("teknogods.ini");
                PlayerSettings.Instance.Name = iniFile.Read("Name", "Settings");
                StatsHelper.SteamId = iniFile.Read("ID", "Settings");
                PlayerSettings.Instance.FOV = iniFile.Read("FOV", "Settings");
                PlayerSettings.Instance.ClanTag = iniFile.Read("Clantag", "Settings");
                PlayerSettings.Instance.Title = iniFile.Read("Title", "Settings");
                PlayerSettings.Instance.MaxFPS = iniFile.Read("Maxfps", "Settings");
                PlayerSettings.Instance.ClanTile = iniFile.Read("ClanTile", "Settings");
                PlayerSettings.Instance.ClanEmblem = iniFile.Read("ClanEmblem", "Settings");
                PlayerSettings.Instance.Sensitivity = float.Parse(LoadDvarSettings("sensitivity").Replace(",", "."), CultureInfo.InvariantCulture);
                return true;
            }
            return false;
        }

        internal static void SaveSettings(string sensitivity, string name, string clanTag, string title, string fov, string clanemblem, string clanTitle, string level, string prestige, string maxFps)
        {
            if (float.Parse(sensitivity.Replace(",", "."), CultureInfo.InvariantCulture) > 10) sensitivity = "10";
            if (int.Parse(fov) > 300) fov = "300";
            if (int.Parse(clanemblem) > 350) clanemblem = "350";
            if (int.Parse(clanTitle) > 511) clanTitle = "511";
            if (int.Parse(level) > 80) level = "80";
            if (int.Parse(prestige) > 20) prestige = "20";
            if (int.Parse(maxFps) > 500) maxFps = "0";
            PlayerSettings.Instance.Sensitivity = float.Parse(sensitivity.Replace(",", "."), CultureInfo.InvariantCulture);
            PlayerSettings.Instance.Name = name;
            PlayerSettings.Instance.ClanTag = clanTag;
            PlayerSettings.Instance.Title = title;
            PlayerSettings.Instance.FOV = fov;
            PlayerSettings.Instance.ClanEmblem = clanemblem;
            PlayerSettings.Instance.Level = int.Parse(level);
            PlayerSettings.Instance.Prestige = int.Parse(prestige);
            PlayerSettings.Instance.ClanTile = clanTitle;
            PlayerSettings.Instance.MaxFPS = maxFps;
            if (File.Exists("teknogods.ini"))
            {
                IniFile iniFile = new IniFile("teknogods.ini");
                iniFile.Write("Name", PlayerSettings.Instance.Name, "Settings");
                string steamId = StatsHelper.GetSteamId();
                if (!string.IsNullOrEmpty(steamId) && steamId != "0")
                {
                    iniFile.Write("ID", steamId, "Settings");
                }
                iniFile.Write("FOV", PlayerSettings.Instance.FOV, "Settings");
                iniFile.Write("Clantag", PlayerSettings.Instance.ClanTag, "Settings");
                iniFile.Write("Title", PlayerSettings.Instance.Title, "Settings");
                iniFile.Write("Maxfps", PlayerSettings.Instance.MaxFPS, "Settings");
                iniFile.Write("ClanTile", PlayerSettings.Instance.ClanTile, "Settings");
                iniFile.Write("ClanEmblem", PlayerSettings.Instance.ClanEmblem, "Settings");
                MessageBox.Show("Saved");
            }
            ChangeDvar("sensitivity", PlayerSettings.Instance.Sensitivity.ToString());
        }

        public static string LoadDvarSettings(string dvar)
        {
            if (File.Exists("players2\\config_mp.cfg"))
            {
                if (new FileInfo("players2\\config_mp.cfg").IsReadOnly)
                {
                    //Program.MainForm.SetState(string.Format("The file {0} is read-only, dvar can't be changed", "players2\\config_mp.cfg"));
                    return "0";
                }
                string text = File.ReadAllLines("players2\\config_mp.cfg").FirstOrDefault((string x) => x.StartsWith($"seta {dvar}"));
                if (text != null)
                {
                    return text.Split('"')[1];
                }
            }
            return "0";
        }

        public static void CheckPath()
        {
            if (!GameHelper.IsPathCorrect())
            { 
                MessageBox.Show("It appears you are not running launcher from a valid MW3 installation directory." + Environment.NewLine +
                    "Move it to a valid MW3 installation directory." + Environment.NewLine + "If you believe this is an error, please contact a TeknoGods Staff.", "TeknoGods", MessageBoxButton.OK);
                Environment.Exit(0);
            }
        }
    }
}
