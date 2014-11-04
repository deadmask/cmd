using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DXApplication1
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<TcpClient> c = new List<TcpClient>();
        private void button1_Click(object sender, EventArgs e)
        {
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
         
         
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {           
             
        }
        protected string md5(string fn)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fn))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            string[] dir = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "*.*", SearchOption.AllDirectories);
             foreach (var v in dir)
             {               
                 FileInfo fi = new FileInfo(v);
                 header h = new header();
                 h.name = Environment.MachineName;
                 h.file_name = fi.Name;
                 h.file_path = fi.FullName;
                 h.file_size = fi.Length;
                 h.updated = DateTime.Now;
                 h.command = "-sf";
                 h.crc = md5(v);
                 h.atribute = File.GetAttributes(v);
              
                 IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
                 IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 3333);                 
                 Socket client = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);      
                 client.Connect(ipEndPoint);
                 byte[] preBuf = ASCIIEncoding.Default.GetBytes(h.getstring() + Environment.NewLine);
                 byte[] Buf = ASCIIEncoding.Default.GetBytes("");
                 client.SendFile(v, preBuf, Buf, TransmitFileOptions.UseDefaultWorkerThread);
                 client.Shutdown(SocketShutdown.Both);
                 client.Close();
                 client.Close();
           
             }
        }             

        private void button4_Click(object sender, EventArgs e)//-cr
        {
            header h = new header();          
            h.command = "-cr";
            h.author = System.Environment.MachineName;
            h.date_creation = DateTime.Now;
            h.filecount = 0;
            h.name = System.Environment.MachineName;     
            h.updated = DateTime.Now;
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 3333);   
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(ipEndPoint);
            byte[] preBuf = ASCIIEncoding.Default.GetBytes(h.getstring() + Environment.NewLine);
            byte[] Buf = ASCIIEncoding.Default.GetBytes("");
            client.SendFile(null, preBuf, Buf, TransmitFileOptions.UseDefaultWorkerThread);
            client.Shutdown(SocketShutdown.Both);
            client.Close();
            client.Close();
           
        }

        private void button6_Click(object sender, EventArgs e)
        {

            byte[] b = new byte[1000000000];
         
            File.WriteAllBytes(@"C:\base\1Cv7\2014\Август\file",b);
        }
 


    
    }

 
}
[Serializable]
public class header
{
 //   object Obj 
    public string getstring()
    {
        StringWriter sw = new StringWriter();
        XmlSerializer writer = new XmlSerializer(typeof(header));
        writer.Serialize(sw, this);
        return sw.ToString();
    }
    public FileAttributes atribute { set; get; }
    public string crc { set; get; }
    public string command { set; get; }
    public string file_name { set; get; }
    public long file_size { set; get; }
    public string file_path { set; get; }
    public string name { set; get; }
    public string author { set; get; }
    public DateTime date_creation { set; get; }
    public DateTime updated { set; get; }
    public int filecount { set; get; }
}