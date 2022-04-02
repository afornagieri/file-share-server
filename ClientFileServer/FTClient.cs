using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientFileServer
{
    class FTClient
    {
        static IPEndPoint localhost;
        static Socket client;
        public static string clientAddress = "127.0.0.1";
        public static int clientPort = 1000;
        public static string PastaRecepcaoArquivos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\";
        public static Label lblMessage;

        public static void SendFile(string file)
        {
            try
            {
                localhost = new IPEndPoint(IPAddress.Parse(clientAddress), clientPort);
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                string folder = "";

                folder += file.Substring(0, file.LastIndexOf(@"\") + 1);
                file = file.Substring(file.LastIndexOf(@"\") + 1);

                byte[] fileNameBytes = Encoding.UTF8.GetBytes(file);

                if(fileNameBytes.Length > (50000 * 1024))
                {
                    lblMessage.Invoke(new Action(() => {
                        lblMessage.ForeColor = Color.Red;
                        lblMessage.Text = "File bigger than 50mb !";
                    }));
                    
                    return;
                }

                string fullPath = folder + file;
                byte[] fileData = File.ReadAllBytes(fullPath);
                byte[] clientData = new byte[4 + fileNameBytes.Length + fileData.Length];
                byte[] fileNameLength = BitConverter.GetBytes(fileNameBytes.Length);

                fileNameLength.CopyTo(clientData, 0);
                fileNameBytes.CopyTo(clientData, 4);
                fileData.CopyTo(clientData, 4 + fileNameBytes.Length);
                client.Connect(localhost);
                client.Send(clientData, 0, clientData.Length, 0);
                client.Close();

                lblMessage.Invoke(new Action(() => {
                    lblMessage.ForeColor = Color.Green;
                    lblMessage.Text = $"{file} transfered !";
                }));
            }
            catch (Exception ex)
            {
                lblMessage.Invoke(new Action(() => {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "Server Error:" + ex;
                }));
            }
            finally
            {
                client.Close();
            }
        }
    }
}
