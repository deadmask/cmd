using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

        public Form1 frm;
        TcpListener server = null;
        public Int32 port;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        Thread listener;
        public bool runed;

        public List<TcpClient> clients = new List<TcpClient>();

     

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


        void command(string s, object client)
        {
           string [] str = s.Split('|');
            try
            {
                TcpClient tcpClient = (TcpClient)client;
                switch (str[0])
                {
                    case "create repo":
                        {
                          
                            if (frm.create_repo(str[1].Replace("\0",""), 0))
                            {
                                frm.log("Репозиторий создан успешно", frm.c.path_to_repo + "\\" + str[1].Replace("\0", ""));
                                frm.BeginInvoke((Action)(() =>
                                {
                                    frm.memoEdit1.Text += "Репозиторий пользователя " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString() + "  создан успешно" + Environment.NewLine;
                                }));
                            }
                            break;
                        }
                    case "send to repo":
                        {
                          //  MessageBox.Show(str[0]);
                          //  File.WriteAllBytes(@"C:\репо\HOME\Data\" + str[1]);
                            break;
                        }
                        
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
          //  string s = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
            clients.Add(tcpClient);
            NetworkStream clientStream = tcpClient.GetStream();
            byte[] message = new byte[4096];
          
            int bytesRead;
            ASCIIEncoding encoder = new ASCIIEncoding();   
            while (true)
            {
                bytesRead = 0;
                try
                {
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    break;
                }
                if (bytesRead == 0)
                {
                    break;
                }                    
             command(System.Text.ASCIIEncoding.Default.GetString(message), client);
            }
            clientStream.Close();
            tcpClient.Close();
            clients.Remove(tcpClient);

        }

    }
}


