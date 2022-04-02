using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientFileServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FTClient.lblMessage = statusLabel;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Send file...";

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                linkLabel1.Text = dialog.FileName;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAddress.Text) ||
                string.IsNullOrEmpty(txtPort.Value.ToString()) ||
                linkLabel1.Text == "Select a file to send...")
            {
                statusLabel.ForeColor = Color.Red;
                statusLabel.Text = "Invalid data !";
                return;
            }

            string IPAddress = txtAddress.Text;
            int port = (int)txtPort.Value;
            string fileName = linkLabel1.Text;

            FTClient.clientAddress = IPAddress;
            FTClient.clientPort = port;

            try
            {
                Task.Factory.StartNew(() =>
                {
                    FTClient.SendFile(fileName);
                });
            }
            catch (Exception ex)
            {
                statusLabel.ForeColor = Color.Red;
                statusLabel.Text = "Error: " + ex;
            }
        }
    }
}
