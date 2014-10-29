using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace DXApplication1
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TcpClient client = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3333);
            client.Connect(serverEndPoint);
            NetworkStream clientStream = client.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes("create repo");
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        //    client.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
         
           string[] dir = Directory.GetFiles(@"C:\base\1Cv7\2014\Август","*.*");
           foreach(var v in dir)
           {              
             FileInfo fi = new FileInfo(v);
             TcpClient client = new TcpClient();
             IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3333);
             client.Connect(serverEndPoint);
             NetworkStream clientStream = client.GetStream();
             ASCIIEncoding encoder = new ASCIIEncoding();
             byte[] buffer = encoder.GetBytes("send to repo"+"|"+fi.Name);
             clientStream.Write(buffer, 0, buffer.Length);
             clientStream.Flush();

             buffer = encoder.GetBytes(v);
             clientStream.Write(buffer, 0, buffer.Length);
             clientStream.Flush();

             client.Close();

          //  client.Client.SendFile();
           }
           
        }
    }
}
