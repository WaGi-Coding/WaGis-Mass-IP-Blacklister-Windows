using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using NetFwTypeLib;
using Newtonsoft.Json.Linq;

namespace WaGis_IP_Blacklister
{
    public partial class MainForm : Form
    {
        INetFwPolicy2 firewallPolicy;
        List<string> allips = new List<string>();
        List<List<string>> fiveklists = new List<List<string>>();
        int numLines;
        int listindex = 0;
        string newV;

        int BlockSize = 1000;

        int protocolNumber;
        string protDesc;

        //[DllImport("user32.dll", EntryPoint = "ShowCaret")] //
        //public static extern long ShowCaret(IntPtr hwnd);  //  
        //[DllImport("user32.dll", EntryPoint = "HideCaret")] // To hide the Cursor in Logbox
        //public static extern long HideCaret(IntPtr hwnd);  //
        
        public MainForm()
        {
            InitializeComponent();
            lblInfo.Text = string.Empty;
            
            notifyIcon1.Icon = new Icon(this.Icon, 40, 40);
            
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            protocolNumber = Properties.Settings.Default.Protocol_Number;
            protDesc = Properties.Settings.Default.Protocol;
        }

        static bool Win10orWinServer()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            string productName = (string)reg.GetValue("ProductName");

            return ((productName.Contains("Windows 10") || productName.Contains("Windows Server")) && !productName.Contains("2008"));
        }

        protected String WaGiRequest(string url)
        {            
            HttpWebRequest webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.UserAgent = "WaGis-Mass-IP-Blacklister-Windows";
            webRequest.ServicePoint.Expect100Continue = false;
            try
            {
                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                    return responseReader.ReadToEnd();
            }
            catch
            {
                return String.Empty;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Administrator check
            //moved to Program.cs
            //////////////////////

            if (IsAdministrator())
            {                
                this.Text = $"WaGi's IP-Blacklister - v{Application.ProductVersion}";
                btnDeleteAll.BackColor = Color.FromArgb(255, 180, 0, 0);

                ///////LOAD HERE////
                btnLoadSettings.PerformClick();
                ///////////////////            

                ///////////////////// WORKAROUND FOR DIFFERENT WINDOWS VERSIONS
                if (Win10orWinServer()) // [DIRTY FIX] If we use Windows 10 or Server, you can add 10k Rules instead of 1k like in Windows 7. I am not sure about versions between so i leave them at 1000 which is quite low. I need to find a better method to detect the BlockSize-Limit (Maximum IP's allowed per FireWall-Rule on different Windows Versions)
                {
                    BlockSize = 10000;                   
                }
                else
                {
                    BlockSize = 1000;
                    MessageBox.Show("Seems you're not using Windows 10 or Windows Server.\nMaximum IP's per Rule will be reduced from 10k to 1k.\nNo worries, you can still use this Tool.");
                }

                // Maybe Check for Updates - NOT DONE
                CheckForUpdate();
                timerCheckForUpdate.Start();
            }
                        
        }

        private void CheckForUpdate()
        {
            try
            {
                newV = WaGiRequest("https://api.github.com/repos/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/releases/latest");

                JObject jObject = JObject.Parse(newV);
                newV = Convert.ToString(jObject.SelectToken("tag_name"));

                var versionNow = new Version(Application.ProductVersion);
                var versionWeb = new Version(newV);

                var result = versionNow.CompareTo(versionWeb);

                if (result < 0)
                {
                    //MessageBox.Show($"New version {versionWeb} available! Your version: {Application.ProductVersion}");
                    Update updateform = new Update();
                    Show();
                    updateform.ShowDialog();
                    updateToolStripMenuItem.Visible = true;
                    timerUpdateBlink.Start();
                    timerCheckForUpdate.Stop();
                }

                /////////////////////////

                //HideCaret(richTextBox2.Handle);
                richtbList.SelectionStart = richtbList.Text.Length;
                firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            }
            catch (Exception)
            {
            }
        }

        private void richtbList_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(richtbList.Text))
            {
                numLines = 0;
            }
            else
	        {
                numLines = richtbList.Text.Split('\n').Length;
            }

            lblLines.Text = $"Lines: {numLines}";
        }

        private void ProtocolSwitch(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            {
                switch (rb.Name)
                {
                    case "rbTCP":
                        protocolNumber = 6;
                        protDesc = "TCP";
                        break;

                    case "rbUDP":
                        protocolNumber = 17;
                        protDesc = "UDP";
                        break;

                    case "rbALL":
                        protocolNumber = 256;
                        protDesc = "ALL";
                        break;                    

                    default:
                        protocolNumber = 6; // bc we start with TCP checked. TCP = 6                                
                        break;
                }
            }
        }

        private void btnADD_Click(object sender, EventArgs e)
        {
            try
            {
                if (!cbInbound.Checked && !cbOutbound.Checked)
                {
                    MessageBox.Show("You have to choose at least one Rule Direction.\n\nINBOUND and/or OUTBOUND", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lblInfo.Text = "Adding Rules - Please wait...";
                EnableDisableControls(false);
                cbAutoOnOff.Enabled = false;



                //Only grab all IPv4/6 IPs/ranges and put them in a List

                //IPv6 CIDR aka. IP-Range support REGEX by WaGi-Coding--- (\b((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:)))(%.+)?s*(\/([0-9]|[1-9][0-9]|1[0-1][0-9]|12[0-8]))?\b)(?!\/)
                //IPv4 CIDR aka IP-Range support by WaGi-Coding---- (\b([0-9]{1,3}\.){3}[0-9]{1,3}(\/([0-9]|[1-2][0-9]|3[0-2]))?\b)(?!\/)
                allips.AddRange(Regex.Matches(richtbList.Text, @"((\b((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]d|1dd|[1-9]?d)(.(25[0-5]|2[0-4]d|1dd|[1-9]?d)){3}))|:)))(%.+)?s*(\/([0-9]|[1-9][0-9]|1[0-1][0-9]|12[0-8]))?\b)(?!\/))|((\b([0-9]{1,3}\.){3}[0-9]{1,3}(\/([0-9]|[1-2][0-9]|3[0-2]))?\b)(?!\/))", RegexOptions.IgnoreCase)
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToList());

                ///////////////////////////////////////////////


                //Excluding IPs here
                if (listBoxExcludeIPs.Items.Count >= 1)
                {
                    for (int i = 0; i < listBoxExcludeIPs.Items.Count; i++)
                    {
                        allips.RemoveAll(ex => ex.Contains(listBoxExcludeIPs.Items[i].ToString()));
                    }
                }
                ////////////////////


                if (!String.IsNullOrWhiteSpace(richtbList.Text) && allips.Count > 0)
                {
                    while (allips.Count >= 1)
                    {
                        List<string> list5k = new List<string>();
                        list5k.AddRange(allips.Take(BlockSize));
                        fiveklists.Add(list5k);
                        if (allips.Count >= BlockSize)
                        {
                            allips.RemoveRange(0, BlockSize);
                        }
                        else if (allips.Count > 0 && allips.Count < BlockSize)
                        {
                            allips.RemoveRange(0, allips.Count);
                        }

                        string chain = "";
                        foreach (string ip in list5k)
                        {
                            chain += ip + ',';
                        }
                        chain = chain.Remove(chain.Length - 1);

                        if (cbInbound.Checked)
                        {
                            MakeRule(chain, protocolNumber, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN, "WaGi IP-Blacklist IN " + protDesc);
                        }

                        if (cbOutbound.Checked)
                        {
                            MakeRule(chain, protocolNumber, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT, "WaGi IP-Blacklist OUT " + protDesc);
                        }

                        Thread.Sleep(250);
                    }
                }

                btnADD.Enabled = true;
                btnDeleteAll.Enabled = true;
                richtbList.Text = string.Empty;
                lblInfo.Text = "";
                EnableDisableControls(true);
                cbAutoOnOff.Enabled = true;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private void btnDEL_Click(object sender, EventArgs e)
        {
            if (!cbInbound.Checked && !cbOutbound.Checked)
            {
                MessageBox.Show("You have to choose at least one Rule Direction.\n\nINBOUND and/or OUTBOUND", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            EnableDisableControls(false);
            cbAutoOnOff.Enabled = false;

            if (cbInbound.Checked)
            {
                RemoveRules("IN", protDesc);
            }
            if (cbOutbound.Checked)
            {
                RemoveRules("OUT", protDesc);
            }

            EnableDisableControls(true);
            cbAutoOnOff.Enabled = true;
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            EnableDisableControls(false);
            cbAutoOnOff.Enabled = false;
            RemoveRules("IN", "TCP");
            RemoveRules("IN", "UDP");
            RemoveRules("IN", "ALL");

            RemoveRules("OUT", "TCP");
            RemoveRules("OUT", "UDP");
            RemoveRules("OUT", "ALL");
            EnableDisableControls(true);
            cbAutoOnOff.Enabled = true;
        }

        private void RemoveRules(string INorOUT, string protTCPorUDPorALL)
        {
            btnADD.Enabled = false;
            btnDeleteAll.Enabled = false;
            lblInfo.Text = $"Removing {protTCPorUDPorALL} {INorOUT}";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";           
            startInfo.Arguments = $"/C C:\\Windows\\System32\\netsh.exe advfirewall firewall delete rule name=\"WaGi IP-Blacklist {INorOUT} {protTCPorUDPorALL}\"";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            Thread.Sleep(100);
            lblInfo.Text = "";
            btnADD.Enabled = true;
            btnDeleteAll.Enabled = true;            
        }

        private void MakeRule(string str, int protNumber, NET_FW_RULE_DIRECTION_ ruleDirection, string ruleName)
        {
            Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);            

            // Let's create a new rule
            INetFwRule2 Rule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            Rule.Enabled = true;


            NET_FW_RULE_DIRECTION_ direction = ruleDirection;
            Rule.Direction = direction; //Inbound
            Rule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;

            Rule.Protocol = protNumber; // ANY/TCP/UDP

            try
            {
                Rule.RemoteAddresses = str;
            }
            catch (Exception)
            {
                MessageBox.Show("Can't add Rules. Maybe a Format failure?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            //Rule.LocalPorts = "81"; //Port 81

            //Name of rule
            Rule.Name = ruleName;
            // ...//
            //Rule.Profiles = (int)NET_FW_PROFILE_TYPE_.NET_FW_PROFILE_TYPE_MAX;

            // Now add the rule
            //INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            try
            {
                firewallPolicy.Rules.Add(Rule);
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            
        }


        private void CopyAction(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(richtbList.SelectedText))
            {
                Clipboard.SetText(richtbList.SelectedText);
            }
        }

        private void PasteAction(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                richtbList.Paste();
            }
        }

        private void CutAction(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(richtbList.SelectedText))
            {
                richtbList.Cut();
            }
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void listBoxExcludeIPs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                listindex = listBoxExcludeIPs.IndexFromPoint(e.Location);
                {
                    listBoxExcludeIPs.SelectedIndex = listindex;

                    if (listindex == listBoxExcludeIPs.SelectedIndex)
                    {
                        contextMenuStrip2.Show(Cursor.Position);
                    }
                }
            }
        }

        private void ToolstripMenuItemREMOVE_Click(object sender, EventArgs e)
        {
            if (listBoxExcludeIPs.Items.Count >= 1)
            {
                if (listBoxExcludeIPs.SelectedItem != null)
                {
                    listBoxExcludeIPs.Items.RemoveAt(listindex);

                }
            }            
        }

        private void toolStriptbListURL_Click(object sender, EventArgs e)
        {
            toolStriptbListURL.SelectAll();
            toolStriptbListURL.Focus();
        }

        private void ToolstripMenuItemOK_Click(object sender, EventArgs e)
        {
            //Check if exists already before adding
            if (listBoxExcludeIPs.Items.Contains(toolStriptbListURL.Text))
            {
                MessageBox.Show("This IP is already in the List!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ///////////////////////////////////////
            try
            {
                IPAddress.Parse(toolStriptbListURL.Text);

            }
            catch (Exception)
            {
                MessageBox.Show("This seems not to be a valid IPv4 Address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            listBoxExcludeIPs.Items.Add(toolStriptbListURL.Text);
        }

        private void numericHours_ValueChanged(object sender, EventArgs e)
        {
            if (numericHours.Value == 24)
            {
                numericMinutes.Value = 0;
            }
            
        }

        private void numericMinutes_ValueChanged(object sender, EventArgs e)
        {
            if (numericHours.Value == 0 && numericMinutes.Value == 0)
            {
                numericMinutes.Value = 1;
            }

            if (numericHours.Value == 24)
            {
                numericMinutes.Value = 0;
            }

            if (numericHours.Value == 0 && numericMinutes.Value < 1)
            {
                numericMinutes.Value = 1;
            }
        }

        private void tbListURL_Enter(object sender, EventArgs e)
        {
            TextBox TB = (TextBox)sender;
            int VisibleTime = 5000;  //in milliseconds

            ToolTip tt = new ToolTip();
            tt.Show("List must have some seperation between IPs, not matter which ones - URL must end with .txt", TB, -150, 22, VisibleTime);
        }

        private void AutomaticProcedure()
        {
            //GET NEW LIST
            btnGetList.PerformClick();

            Thread.Sleep(250);

            //Delete
            btnDEL.PerformClick();

            Thread.Sleep(250);

            //Add
            btnADD.PerformClick();
        }

        private void IntervalTimer_Tick(object sender, EventArgs e)
        {
            AutomaticProcedure();
        }

        private void EnableDisableControls(bool truefalse)
        {
            numericHours.Enabled = truefalse;
            numericMinutes.Enabled = truefalse;
            pnlDirection.Enabled = truefalse;
            pnlProtocol.Enabled = truefalse;
            cbAutoOnDoOnce.Enabled = truefalse;
            pnlListURL.Enabled = truefalse;
            pnlExcludeIPs.Enabled = truefalse;
            btnSaveSettings.Enabled = truefalse;
            btnLoadSettings.Enabled = truefalse;
            btnADD.Enabled = truefalse;
            btnDEL.Enabled = truefalse;
            btnDeleteAll.Enabled = truefalse;
            richtbList.Enabled = truefalse;
        }

        private void cbAutoOnOff_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAutoOnOff.Checked)
            {
                if (cbAutoOnDoOnce.Checked)
                {
                    AutomaticProcedure();
                }

                EnableDisableControls(false);

                int frequency = ((int)numericHours.Value * 60000 * 60) + ((int)numericMinutes.Value * 60000);
                IntervalTimer.Interval = frequency;

                IntervalTimer.Start();
            }
            else
            {
                IntervalTimer.Stop();
                EnableDisableControls(true);
            }
        }


        private void richTextBox2_Click(object sender, EventArgs e)
        {
            //HideCaret(richTextBox2.Handle);
        }

        private void btnGetList_Click(object sender, EventArgs e)
        {
            lblInfo.Text = "Getting List from URL";
            richtbList.Text = string.Empty;
            string extension = string.Empty;

            if (tbListURL.Text.Length >= 10)
            {
                extension = tbListURL.Text.Substring(tbListURL.Text.Length - 4).ToLower();               
            }
            else
            {
                if (!cbAutoOnOff.Checked)
                {
                    MessageBox.Show("Please put in a valid URL where the IP-List ist located", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            if (extension == ".txt")
            {
                try
                {
                    WebRequest req = WebRequest.Create(tbListURL.Text);
                    using (WebResponse res = req.GetResponse())
                    {
                        res.Dispose();
                    }
                }
                catch (Exception)
                {
                    if (!cbAutoOnOff.Checked)
                    {
                        MessageBox.Show("URL could not be resolved!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    lblInfo.Text = "";
                    return;
                }

                string s;
                using (WebClient client = new WebClient())
                {
                    s = client.DownloadString(tbListURL.Text);
                    client.Dispose();
                }
                richtbList.Text = s;

                // set the current caret position to the end
                richtbList.SelectionStart = richtbList.Text.Length;
                // scroll it automatically
                richtbList.ScrollToCaret();
                lblInfo.Text = "";
            }
            else
            {
                MessageBox.Show("Invalid Extension!\n\nOnly use URLs ending with .txt\n\nFor example www.domain.com/blacklist.txt", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnDeleteAll_MouseEnter(object sender, EventArgs e)
        {
            btnDeleteAll.BackColor = Color.FromArgb(255, 255, 0, 0);
        }

        private void btnDeleteAll_MouseLeave(object sender, EventArgs e)
        {
            btnDeleteAll.BackColor = Color.FromArgb(255, 180, 0, 0);
        }
        
        private string GetDirection()
        {
            if (cbOutbound.Checked && !cbInbound.Checked)
            {
                return "OUT";
            }
            else if(!cbOutbound.Checked && cbInbound.Checked)
            {
                return "IN";
            }
            else
            {
                return "BOTH";
            }
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Logging = false; // removed logging functionality totally for now
            Properties.Settings.Default.Protocol = protDesc;
            Properties.Settings.Default.Protocol_Number = protocolNumber;
            Properties.Settings.Default.Direction = GetDirection();

            string[] excludeArray = new string[listBoxExcludeIPs.Items.Count];
            listBoxExcludeIPs.Items.CopyTo(excludeArray, 0);
            StringCollection myCol = new StringCollection();
            myCol.AddRange(excludeArray);
            Properties.Settings.Default.Excluded_IPs = myCol;

            Properties.Settings.Default.Hours = numericHours.Value;
            Properties.Settings.Default.Minutes = numericMinutes.Value;

            Properties.Settings.Default.DoOnce = cbAutoOnDoOnce.Checked;

            Properties.Settings.Default.List_URL = Regex.Replace(tbListURL.Text, @"\s+", "");
            //saveExIpList();
            //saveUrl();

            Properties.Settings.Default.Save();
        }

        private void btnLoadSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            //if (Properties.Settings.Default.Logging)
            //{
            //    cbLogging.Checked = true;
            //}
            //else
            //{
            //    cbLogging.Checked = false;
            //}


            switch (Properties.Settings.Default.Protocol)
            {
                case "TCP":
                    rbTCP.Checked = true;
                    break;
                case "UDP":
                    rbUDP.Checked = true;
                    break;
                default:
                    rbALL.Checked = true;
                    break;
            }

            protocolNumber = Properties.Settings.Default.Protocol_Number;

            if (Properties.Settings.Default.Direction == "IN")
            {
                cbInbound.Checked = true;
                cbOutbound.Checked = false;
            }
            else if (Properties.Settings.Default.Direction == "OUT")
            {
                cbOutbound.Checked = true;
                cbInbound.Checked = false;
            }
            else if(Properties.Settings.Default.Direction == "BOTH")
            {
                cbInbound.Checked = true;
                cbOutbound.Checked = true;
            }

            listBoxExcludeIPs.Items.Clear();
            foreach (var item in Properties.Settings.Default.Excluded_IPs)
            {
                listBoxExcludeIPs.Items.Add(item.ToString());
            }

            numericHours.Value = Properties.Settings.Default.Hours;
            numericMinutes.Value = Properties.Settings.Default.Minutes;

            cbAutoOnDoOnce.Checked = Properties.Settings.Default.DoOnce;

            tbListURL.Text = Properties.Settings.Default.List_URL;

            //listBoxExcludeIPs.Items.Clear();
            //loadExIpList();
            //loadUrl();
        }

        private void cbInbound_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbInbound.Checked && !cbOutbound.Checked)
            {
                cbOutbound.Checked = true;
            }
        }

        private void cbOutbound_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbOutbound.Checked && !cbInbound.Checked)
            {
                cbInbound.Checked = true;
            }
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoForm infoFrm = new InfoForm();
            infoFrm.ShowDialog();
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Update updateform = new Update();
            updateform.ShowDialog();
        }

        private void timerUpdateBlink_Tick(object sender, EventArgs e)
        {
            if (updateToolStripMenuItem.BackColor == SystemColors.Control)
            {
                updateToolStripMenuItem.BackColor = Color.Crimson;
            }
            else
            {
                updateToolStripMenuItem.BackColor = SystemColors.Control;
            }
        }

        private void notifyIcon1_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
                notifyIcon1.Visible = false;
            }
        }

        private void minimizeToSystemTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hide();
            notifyIcon1.Visible = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            notifyIcon1.Visible = false;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timerCheckForUpdate_Tick(object sender, EventArgs e)
        {
            CheckForUpdate();
        }
    }
}
