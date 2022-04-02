using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareFileServer
{
    class SFServer
    {
        static IPEndPoint endpoint;
        static Socket server;
        public static string localhost = "127.0.0.1";
        public static int port = 1000;
        public static string destinationFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\";
        public static ListBox listOfMessages;

        public static void Start()
        {
            try
            {
                endpoint = new IPEndPoint(IPAddress.Parse(localhost), port);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                server.Bind(endpoint);
            }
            catch (Exception ex)
            {
                listOfMessages.Invoke(new Action(() => {
                    listOfMessages.Items.Add("Error while trying to initiate the server: \n" + ex.Message);
                    listOfMessages.SetSelected(listOfMessages.Items.Count - 1, true);
                }));

                return;
            }

            try
            {
                server.Listen(100);
                listOfMessages.Invoke(new Action(() => {
                    listOfMessages.Items.Add("Server ready to receive files !");
                    listOfMessages.SetSelected(listOfMessages.Items.Count - 1, true);
                }));
                
                Socket client = server.Accept();
                client.ReceiveBufferSize = 16384;

                byte[] data = new byte[1024 * 50000];

                int lengthOfReceivedBytes = client.Receive(data, data.Length, 0);
                int fileNameLength = BitConverter.ToInt32(data, 0);
                string fileName = Encoding.UTF8.GetString(data, 4, fileNameLength);

                BinaryWriter writer = new BinaryWriter(File.Open(destinationFolder + fileName, FileMode.Append));
                writer.Write(data, 4 + fileNameLength, lengthOfReceivedBytes - 4 - fileNameLength);

                while (lengthOfReceivedBytes > 0)
                {
                    lengthOfReceivedBytes = client.Receive(data, data.Length, 0);

                    if(lengthOfReceivedBytes == 0)
                    {
                        writer.Close();
                    }
                    else
                    {
                        writer.Write(data, 0, lengthOfReceivedBytes);
                    }

                    listOfMessages.Invoke(new Action(() => {
                        listOfMessages.Items.Add($"File received {fileName}\n");
                        listOfMessages.SetSelected(listOfMessages.Items.Count - 1, true);
                    }));

                    writer.Close();
                    client.Close();
                }

            }
            catch (SocketException ex)
            {
                listOfMessages.Invoke(new Action(() => {
                    listOfMessages.Items.Add($"Error while receiving a file: " + ex.Message);
                    listOfMessages.SetSelected(listOfMessages.Items.Count - 1, true);
                }));
            }
            finally
            {
                server.Close();
                server.Dispose();
                Start();
            }
        }

    }
}
