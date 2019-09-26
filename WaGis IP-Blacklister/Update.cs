using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaGis_IP_Blacklister
{
    public partial class Update : Form
    {
        string jsonData;
        string newV;
        string body;
        string dlLink;
        Version newVersion;
        Version curVersion;

        public Update()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DialogResult openBrowserResult = MessageBox.Show("Click OK to open the Link in the Browser & closing the Programm!", "Open Browser?", MessageBoxButtons.OKCancel);

            if (openBrowserResult == DialogResult.OK)
            {
                System.Diagnostics.Process.Start(dlLink);
                Application.Exit();
            }
            else
            {
                return;
            }
        }

        protected String WaGiRequest(string url)
        {
            url += (String.IsNullOrEmpty(new Uri(url).Query) ? "?" : "&");
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

        List<string> fullInfo;
        private void Update_Load(object sender, EventArgs e)
        {            
            try
            {
                Version curV = new Version(Application.ProductVersion);

                jsonData = WaGiRequest("https://api.github.com/repos/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/releases/latest");
                
                var jsonArr = JArray.Parse(WaGiRequest("https://api.github.com/repos/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/releases"));
                fullInfo = new List<string>();

                foreach (JObject jO in jsonArr)
                {
                    Version tempVersion = new Version(jO.SelectToken("tag_name").ToString());

                    if (curV.CompareTo(tempVersion) < 0)
                    {
                        Console.WriteLine(tempVersion);                        
                        fullInfo.Add(((jsonArr.First == jO) ? "" : "\n\n") + tempVersion.ToString() + ':');
                        fullInfo.Add(jO.SelectToken("body").ToString().Split(new string[] { "---" }, StringSplitOptions.None)[1].Replace('*', '●'));                        
                    }
                    else
                    {
                        break;
                    }


                }

                JObject jObject = JObject.Parse(jsonData);
                newV = Convert.ToString(jObject.SelectToken("tag_name"));
                body = Convert.ToString(jObject.SelectToken("body"));



                dlLink = "https://sourceforge.net/projects/wagi-ip-blacklister/";
            }
            catch (Exception)
            {
                MessageBox.Show("Seems there is an Update, but a Problem with resolving the newest Version Data. Checkout yourself for the new Version on Github or SourceForge", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            newVersion = new Version(newV);            
            curVersion = new Version(Application.ProductVersion);

            lblCurrent.Text = "Current Version: " + curVersion;
            lblNew.Text = "New Version: " + newVersion;

            var result = curVersion.CompareTo(newVersion);

            if (result > 0 || result == 0)
            {
                btnDownload.Enabled = false;
            }
            else
            {
                btnDownload.Enabled = true;
            }

            //richTextBox1.Text = body.Split(new string[] { "---" }, StringSplitOptions.None)[1].Replace('*', '●');
            richTextBox1.Text = string.Join("\n", fullInfo);

        }
    }
}
