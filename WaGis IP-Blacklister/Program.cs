using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaGis_IP_Blacklister
{
    static class Program
    {
        private static Mutex _mutex = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]        
        static void Main()
        {

            bool IsAdministrator()
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            _mutex = new Mutex(true, @"Global\" + "WaGi-IP-Blacklister", out bool aIsNewInstance);
            GC.KeepAlive(_mutex);

            if (!aIsNewInstance)
            {
                MessageBox.Show("An instance of WaGi-IP-Blacklister is already running!\nIf you can't find the opened instance, checkout the System Tray or Task-Manager", "WaGi - ERROR");
                return;
            }

            //Administrator check
            if (!IsAdministrator())
            {
                MessageBox.Show("This Application makes changes in the Firewall. You need to run it as Administrator!", "WaGi's IP-Blacklister", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());

            }
            //////////////////////

        }
    }
}
