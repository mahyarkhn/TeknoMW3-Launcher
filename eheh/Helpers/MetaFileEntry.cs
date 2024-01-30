using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Windows;
using System.Xml.Serialization;
using eheh.Utils;

namespace eheh.Helpers
{
    [XmlType("Entry")]
    public class MetaFileEntry
    {
        [XmlAttribute("url")]
        public string RelativeURL
        {
            get;
            set;
        }

        [XmlAttribute("local")]
        public string LocalURL
        {
            get;
            set;
        }

        [XmlAttribute("MD5")]
        public string FileMD5
        {
            get;
            set;
        }

        [XmlAttribute("size")]
        public long FileSize
        {
            get;
            set;
        }

        public event Action<MetaFileEntry> Downloaded;

        protected void OnApplied()
        {
            this.Downloaded?.Invoke(this);
        }

        public void Download()
        {
            string fullPath = Path.GetFullPath("./" + LocalURL);
            bool flag = fullPath.Equals(Path.GetFullPath(Variables.CurrentExePath), StringComparison.InvariantCultureIgnoreCase);
            GameHelper.UpdateControl.SetExtra($"Check if {RelativeURL} already exists ...");
            if (File.Exists(fullPath) && Cryptography.GetFileMD5HashBase64(fullPath) == FileMD5)
            {
                GameHelper.UpdateControl.SetExtra($"File {RelativeURL} already exists... Next !");
                OnApplied();
                return;
            }
            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }
            GameHelper.UpdateControl.SetExtra($"Download {RelativeURL} ...");
            if (flag)
            {
                GameHelper.WebClient.DownloadFileCompleted += OnUplauncherDownloaded;
                GameHelper.WebClient.DownloadFileAsync(new Uri(Variables.UpdateSiteURL + RelativeURL), "./temp_upl.exe", "temp_upl.exe");
            }
            else
            {
                GameHelper.WebClient.DownloadFileCompleted += OnFileDownloaded;
                GameHelper.WebClient.DownloadFileAsync(new Uri(Variables.UpdateSiteURL + RelativeURL), "./" + LocalURL, LocalURL);
            }
        }

        private void OnFileDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            ((WebClient)sender).DownloadFileCompleted -= OnFileDownloaded;
            OnApplied();
        }

        private static void OnUplauncherDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            ((WebClient)sender).DownloadFileCompleted -= OnUplauncherDownloaded;
            string text = Path.GetTempFileName() + ".exe";
            //File.WriteAllBytes(text, Resources.TeknoMW3_Client_LauncherReplacer);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = text,
                Arguments = string.Format("{0} \"{1}\" \"{2}\"", Process.GetCurrentProcess().Id, Path.GetFullPath("temp_upl.exe"), Path.GetFullPath(Variables.CurrentExePath)),
                Verb = "runas"
            };
            try
            {
                MessageBox.Show("A new version of the Launcher is available, this application will close.");
                Process.Start(startInfo);
                Environment.Exit(1);
            }
            catch (Exception)
            {
            }
            MessageBox.Show("Error replacing Launcher");
        }
    }
}
