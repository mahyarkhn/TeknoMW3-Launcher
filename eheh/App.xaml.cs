using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

using eheh.Windows;

namespace eheh
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _handler = (EventHandler)Delegate.Combine(_handler, new EventHandler(Handler));
            SetConsoleCtrlHandler(_handler, add: true);
            AMainWindow = new MainWindow();
            AMainWindow.Show();
        }

        private delegate bool EventHandler(CtrlType sig);

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static EventHandler _handler;

        public static bool IsShutdown;

        public static MainWindow AMainWindow
        {
            get;
            set;
        }

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_CLOSE_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                    Close();
                    return true;
                default:
                    return false;
            }
        }

        public static void Close()
        {
            //if (DiscordHelper.m_client == null || DiscordHelper.m_client.Connection == null || DiscordHelper.m_client.Connection.State == RpcState.Disconnected)
            //{
            //    Environment.Exit(0);
            //}
            //DiscordHelper.m_client.Dispose();
            IsShutdown = true;
        }
    }
}
