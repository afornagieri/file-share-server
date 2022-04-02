using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareFileServer
{
    public partial class Form1 : Form
    {
        Task task;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            openFileSearchFolder.Text = SFServer.destinationFolder;
            SFServer.listOfMessages = listLogs;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            int port = (int)txtPort.Value;
            string ipAddress = txtAddress.Text;

            try
            {
                SFServer.localhost = ipAddress;
                SFServer.port = port;

                task = Task.Factory.StartNew(() => {
                    SFServer.Start();
                });
            }
            catch (Exception ex)
            {
                listLogs.Invoke(new Action(() => {
                    listLogs.Items.Add("Erro while trying to connect to the server: " + ex.Message);
                    listLogs.SetSelected(listLogs.Items.Count - 1, true);
                }));
            }
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Restart();
            }
            catch (Exception ex)
            {
                listLogs.Invoke(new Action(() => {
                    listLogs.Items.Add("Error: " + ex.Message);
                    listLogs.SetSelected(listLogs.Items.Count - 1, true);
                }));
            }
        }

        private void openFileSearchFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if(dialog.ShowDialog() != DialogResult.Cancel)
            {
                SFServer.destinationFolder = dialog.SelectedPath + @"\";
                openFileSearchFolder.Text = SFServer.destinationFolder;
            }
        }
    }
}
