using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaGis_IP_Blacklister
{
    public partial class Update : Form
    {
        string newV;
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
            System.Diagnostics.Process.Start(dlLink);
        }        

        private void Update_Load(object sender, EventArgs e)
        {            
            try
            {
                WebRequest req = WebRequest.Create(@"https://pastebin.com/raw/PeTLeE0y");
                using (WebResponse res = req.GetResponse())
                {
                    res.Dispose();
                }

                req = WebRequest.Create(@"https://pastebin.com/raw/h8x2MN1H");
                using (WebResponse res = req.GetResponse())
                {
                    res.Dispose();
                }

                using (WebClient client = new WebClient())
                {
                    newV = client.DownloadString(@"https://pastebin.com/raw/PeTLeE0y");
                    client.Dispose();
                }

                using (WebClient client = new WebClient())
                {
                    dlLink = client.DownloadString(@"https://pastebin.com/raw/h8x2MN1H");
                    client.Dispose();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong!\nSeems the Link to the Download is down.\nCheckout again later", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
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


        }
    }
}
