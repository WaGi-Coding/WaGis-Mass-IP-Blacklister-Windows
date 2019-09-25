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

        private void Update_Load(object sender, EventArgs e)
        {            
            try
            {
                jsonData = WaGiRequest("https://api.github.com/repos/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/releases/latest");
                
                var jsonArr = JArray.Parse(WaGiRequest("https://api.github.com/repos/WaGi-Coding/WaGis-Mass-IP-Blacklister-Windows/releases"));
                
                foreach (JObject jO in jsonArr)
                {
                    if (true)
                    {

                    }
                    Console.WriteLine(jO.SelectToken("tag_name"));

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

            string[] bodyArray = body.Split(new string[] { "---" }, StringSplitOptions.None);

            richTextBox1.Text = bodyArray[1].Replace('*', '●');

        }
    }
}
