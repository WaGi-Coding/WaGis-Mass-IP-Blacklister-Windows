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

        int protocolNumber = Properties.Settings.Default.Protocol_Number;
        string protDesc = Properties.Settings.Default.Protocol;

        //[DllImport("user32.dll", EntryPoint = "ShowCaret")] //
        //public static extern long ShowCaret(IntPtr hwnd);  //  
        [DllImport("user32.dll", EntryPoint = "HideCaret")] // To hide the Cursor in Logbox
        public static extern long HideCaret(IntPtr hwnd);  //
        
        public MainForm()
        {
            InitializeComponent();            
            lblInfo.Text = string.Empty;
        }

        protected String WaGiRequest(string url)
        {
            url += (String.IsNullOrEmpty(new Uri(url).Query) ? "?" : "&") + "access_token=" + "7485b9319e4251a7e5e74fb122c21d56e4b8d215";
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
            if (!IsAdministrator())
            {
                MessageBox.Show("This Application makes changes in the Firewall. You need to run it as Administrator!", "WaGi's IP-Blacklister", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();                
            }
            //////////////////////

            this.Text = $"WaGi's IP-Blacklister - v{Application.ProductVersion}";
            btnDeleteAll.BackColor = Color.FromArgb(255, 180, 0, 0);

            ///////LOAD HERE////
            btnLoadSettings.PerformClick();
            ///////////////////            

            // Maybe Check for Updates - NOT DONE
            try
            {
                newV = WaGiRequest("https://api.github.com/repos/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/releases/latest");

                JObject jObject = JObject.Parse(newV);
                newV = Convert.ToString(jObject.SelectToken("tag_name"));
            }
            catch (Exception)
            {
                throw;
            }

            var versionNow = new Version(Application.ProductVersion);
            var versionWeb = new Version(newV);

            var result = versionNow.CompareTo(versionWeb);

            if (result < 0)
            {
                //MessageBox.Show($"New version {versionWeb} available! Your version: {Application.ProductVersion}");
                Update updateform = new Update();
                updateform.ShowDialog();
                updateToolStripMenuItem.Visible = true;
                timerUpdateBlink.Start();
            }

            /////////////////////////

            HideCaret(richTextBox2.Handle);
            richtbList.SelectionStart = richtbList.Text.Length;
            firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));            
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
                        protocolNumber = 0;
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



                //Only grab all IPv4 IPs/ranges and put them in a List

                allips.AddRange(Regex.Matches(richtbList.Text, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")
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
                        list5k.AddRange(allips.Take(5000));
                        fiveklists.Add(list5k);
                        if (allips.Count >= 5000)
                        {
                            allips.RemoveRange(0, 5000);
                        }
                        else if (allips.Count > 0 && allips.Count < 5000)
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
            
            //Rule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_ANY; // ANY/TCP/UDP

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
            firewallPolicy.Rules.Add(Rule);
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

        private void cbLogging_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLogging.Checked)
            {
                richTextBox2.Text = string.Empty;
                this.Size = new Size(800, 590);
            }
            else
            {
                richTextBox2.Text = string.Empty;
                this.Size = new Size(370, 590);                
            }
        }

        private void ToolstripMenuItemCOPY2_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(richTextBox2.SelectedText))
            {
                Clipboard.SetText(richTextBox2.SelectedText);
            }
        }

        private void richTextBox2_Click(object sender, EventArgs e)
        {
            HideCaret(richTextBox2.Handle);
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

        private void lblLogging_Click(object sender, EventArgs e)
        {
            cbLogging.Checked = !cbLogging.Checked;
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
            Properties.Settings.Default.Logging = cbLogging.Checked;
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
            if (Properties.Settings.Default.Logging)
            {
                cbLogging.Checked = true;
            }
            else
            {
                cbLogging.Checked = false;
            }


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
    }
}
