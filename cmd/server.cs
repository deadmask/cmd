using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cmd
{
    class serv
    {
        TcpListener server = null;
        public Int32 port;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        Thread listener;
        public bool runed;

        public List<string> clients = new List<string>();
        public void start()
        {
            try
            {
                server = new TcpListener(localAddr, port);
                this.listener = new Thread(new ThreadStart(ListenForClients));
                this.listener.Start();

            }
            catch (Exception) { }
        }
        public void stop()
        {
            runed = false;
            server.Stop();

        }
        private void ListenForClients()
        {
            try
            {
                server.Start();
                while (runed)
                {
                    TcpClient client = this.server.AcceptTcpClient();
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(client);
                }
            }
            catch (Exception) { }
        }
        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            string s = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
            clients.Add(s);

            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;
            ASCIIEncoding encoder = new ASCIIEncoding();
            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message 
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured 
                    break;
                }

                if (bytesRead == 0)
                {

                    //the client has disconnected from the server 
                    break;
                }

                //message has successfully been received 
                //ASCIIEncoding encoder = new ASCIIEncoding();
                System.Console.WriteLine(encoder.GetString(message, 0, bytesRead));
                MessageBox.Show("+");
                byte[] buffer = encoder.GetBytes("Hello Client!");

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
            }

            // clientStream.Close();
            tcpClient.Close();
            clients.Remove(s);

        }

    }
}
