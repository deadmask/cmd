using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace TCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3333);

            client.Connect(serverEndPoint);

            NetworkStream clientStream = client.GetStream();

            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes("Hello Server!");

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
          
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
            clientThread.Start(client);

            String s;
            s = Console.ReadLine();
        }

        public static void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            do
            {
                bytesRead = 0;

                try
                {
                    // Blocks until a server  sends a message                    
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch (Exception)
                {
                    // A socket error has occured
                    break;
                }
                if (bytesRead == 0)
                {
                     break;
                }

                // Message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();

                // Output message
                Console.Write(encoder.GetString(message, 0, bytesRead));
            } while (clientStream.DataAvailable);
        }
    }
}
