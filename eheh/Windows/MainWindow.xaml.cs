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
using System.Windows.Shapes;

using MahApps.Metro.Controls;
using eheh.Windows.Content;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using eheh.Utils;

namespace eheh.Windows
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var configs = Variables.LoadSettings();
            if (!configs)
            {
                MessageBox.Show("teknogods.ini not found!");
            }
            
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex;
            MoveCursorMenu(index);

            switch (index)
            {
                case 0: // Home
                    MainContent.Children.Clear();
                    //MainContent.Children.Add(new Home());
                    break;
                case 1: // TeknoMW3
                    MainContent.Children.Clear();
                    MainContent.Children.Add(new TeknoMW3());
                    break;
                case 2: // TeknoBO
                    MainContent.Children.Clear();
                    //MainContent.Children.Add(new TeknoBO());
                    break;
                case 3: // Social
                    MainContent.Children.Clear();
                    //MainContent.Children.Add(new Social());
                    break;
                case 4: // Settings
                    MainContent.Children.Clear();
                    MainContent.Children.Add(new Settings());
                    break;
                case 5: // Update
                    MainContent.Children.Clear();
                    MainContent.Children.Add(new Update(this));
                    break;
                default:
                    break;
            }
        }

        private void MoveCursorMenu(int index)
        {
            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, (60 * index), 0, 0);
        }

        internal void EnableThings(bool enable)
        {
            ListViewMenu.IsEnabled = enable;
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == e.LeftButton)
            {
                ReleaseCapture();
                Window window = Window.GetWindow(this);
                var wih = new WindowInteropHelper(window); // using System.Windows.Interop;
                IntPtr hWnd = wih.Handle;
                SendMessage(hWnd, 0xA1, 0x2, 0);
            }
        }
    }
}
