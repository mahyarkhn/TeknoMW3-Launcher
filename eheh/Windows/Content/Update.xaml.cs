using eheh.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eheh.Windows.Content
{
    /// <summary>
    /// Interaction logic for Update.xaml
    /// </summary>
    public partial class Update : UserControl
    {
        internal MainWindow MainWindow;
        public Update(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            InitializeComponent();
        }

        public void SetExtra(string extra)
        {
            this.txtblockUpdateExtra.Text = extra;
        }
        public void SetStatus(string status)
        {
            this.txtblockUpdateStatus.Text = status;
        }
        public void SetPercentage(int value)
        {
            this.prgbarUpdateProgress.Value = value;
        }
        public void SetSpeed(string speed)
        {
            this.txtblockDownloadSpeed.Text = "Speed: " + speed;
        }
        public void SetTime(string time)
        {
            this.txtblockDownloadTime.Text = "Remaining Time: " + time;
        }
        public void SetDownloadSize(string downloaded)
        {
            this.txtblockDownloadSize.Text = "Downloaded: " + downloaded;
        }

        private void BtnCheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            SetStatus("Checking for updates . . .");
            SetExtra("Checking for updates . . .");
            MainWindow.EnableThings(false);
            GameHelper.UpdateControl = this;
            GameHelper.CheckUpdates();
        }
    }
}
