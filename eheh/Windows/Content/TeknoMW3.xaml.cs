using eheh.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for TeknoMW3.xaml
    /// </summary>
    public partial class TeknoMW3 : UserControl
    {
        public TeknoMW3()
        {
            InitializeComponent();
        }

        private void BtnLan_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("LAN");
            string ip = GetIp("127.0.0.0");
            string port = "8080";
            GameHelper.Play("+server " + ip + ":" + port, Enums.Game.TeknoMW3);
        }

        public string GetIp(string domain)
        {
            try
            {
                switch (Uri.CheckHostName(domain))
                {
                    case UriHostNameType.Unknown:
                    case UriHostNameType.Basic:
                        MessageBox.Show("Invalid hostname!");
                        return string.Empty;
                    case UriHostNameType.IPv4:
                        return domain;
                    default:
                        return Dns.GetHostAddresses(domain)[0].ToString();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid hostname!");
                return string.Empty;
            }
        }

        private void BtnMultiplayer_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("MP");
            GameHelper.Play(string.Empty, Enums.Game.TeknoMW3);
        }
    }
}
