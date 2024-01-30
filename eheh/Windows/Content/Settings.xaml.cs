using eheh.Utils;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
            txtboxSensitivity.Text = PlayerSettings.Instance.Sensitivity.ToString();
            txtboxInGameName.Text = PlayerSettings.Instance.Name;
            txtboxClanTag.Text = PlayerSettings.Instance.ClanTag;
            txtboxTitle.Text = PlayerSettings.Instance.Title;
            txtboxFieldOfView.Text = PlayerSettings.Instance.FOV;
            txtboxClanEmblem.Text = PlayerSettings.Instance.ClanEmblem;
            txtboxClanTitle.Text = PlayerSettings.Instance.ClanTile;
            txtboxLevel.Text = PlayerSettings.Instance.Level.ToString();
            txtboxPrestige.Text = PlayerSettings.Instance.Prestige.ToString();
            txtboxMaxFps.Text = PlayerSettings.Instance.MaxFPS;
        }

        private void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Variables.SaveSettings(txtboxSensitivity.Text, txtboxInGameName.Text, txtboxClanTag.Text, txtboxTitle.Text, txtboxFieldOfView.Text, txtboxClanEmblem.Text, 
                tglbtnNullClanTitle.IsChecked.Value ? "512" : txtboxClanTitle.Text, txtboxLevel.Text, txtboxPrestige.Text, tglbtnUnlimitedFps.IsChecked.Value ? "0" : txtboxMaxFps.Text);
        }

        private void Tbbtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (tglbtnUnlimitedFps.IsChecked.Value)
            {
                txtboxMaxFps.IsEnabled = false;
                txtboxMaxFps.Text = "0";
            }
            else
            {
                txtboxMaxFps.IsEnabled = true;
                txtboxMaxFps.Text = PlayerSettings.Instance.MaxFPS;
            }
            if (tglbtnNullClanTitle.IsChecked.Value)
            {
                txtboxClanTitle.IsEnabled = false;
                txtboxClanTitle.Text = "512";
            }
            else
            {
                txtboxClanTitle.IsEnabled = true;
                txtboxClanTitle.Text = PlayerSettings.Instance.ClanTile;
            }
        }
    }
}
