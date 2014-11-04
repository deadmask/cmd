using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

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



        private void HandleClientComm(object client)
        {
            try
            {
                string tmp = RandomString(30);
                TcpClient tcpClient = (TcpClient)client;
                string s = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                clients.Add(tcpClient);
                NetworkStream clientStream = tcpClient.GetStream();
                byte[] message = new byte[4096 * 8];

                int bytesRead;
                while (true)
                {
                    bytesRead = 0;
                    try
                    {
                        bytesRead = clientStream.Read(message, 0, message.Length);                      
                    }
                    catch
                    {
                        break;
                    }
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    AppendAllBytes(@"Temp\" + tmp, message);
                }
                clientStream.Close();
                tcpClient.Close();

                string[] str = File.ReadAllLines(@"Temp\" + tmp, Encoding.Default);

                string st = "";
                foreach (var v in str)
                {
                    st += v + Environment.NewLine;
                    if (v == "</header>")
                    { break; }
                }
                header h = new header();
                try
                {
                    StreamReader txtReader = new StreamReader(new MemoryStream(ASCIIEncoding.Default.GetBytes(st)), Encoding.Default);
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(header));
                    h = (header)xmlSerializer.Deserialize(txtReader);
                    txtReader.Close();
                }
                catch (Exception er)
                {
                }
                #region

                switch (h.command)
                {
                    case "-cr":
                        {
                            if (frm.create_repo(h.author, h.filecount, h.date_creation))
                            {
                                frm.log("Репозиторий создан успешно", frm.c.path_to_repo + "\\" + h.name);
                                frm.BeginInvoke((Action)(() =>
                                {
                                    frm.memoEdit1.Text += "Репозиторий пользователя " + s + "  создан успешно" + Environment.NewLine;
                                }));
                            }
                            break;
                        }
                    case "-sf":
                        {
                            XMLFile<repo> file = new XMLFile<repo>(frm.c.path_to_repo + "\\" + h.name + "\\info");
                            repo r = new repo();
                            r = file.Load();
                            r.file_info.Add
                                (
                            new file()
                            {
                                Name = h.file_name,
                                Length = h.file_size,
                                Extension = Path.GetExtension(h.file_path),
                                crc = h.crc,
                                FullName = h.file_path,
                                Attributes = h.atribute
                            }
                            );
                            r.filecount++;
                            file.Save(r);
                            byte[] b = File.ReadAllBytes(@"Temp\" + tmp);
                            FileStream fw = new FileStream(createpath(frm.c.path_to_repo + "\\" + h.name + "\\Data\\", h.file_path), FileMode.Create, FileAccess.ReadWrite);
                            fw.Write(b, ASCIIEncoding.Default.GetBytes(st).Length, b.Length - ASCIIEncoding.Default.GetBytes(st).Length);
                            fw.Close();
                            file.Save(r);
                            frm.BeginInvoke((Action)(() => { frm.log("Принят файл " + h.file_name + " от клиента " + s + " размер " + h.file_size + " байт ", frm.c.path_to_repo + "\\" + h.name); }));
                            frm.BeginInvoke((Action)(() => { frm.memoEdit1.Text += " Принят файл " + h.file_name + " от клиента " + s + " размер " + h.file_size + " байт " + Environment.NewLine; }));
                            break;
                        }
                    default:
                        {
                            frm.BeginInvoke((Action)(() => { frm.memoEdit1.Text += System.Text.ASCIIEncoding.Default.GetString(message) + Environment.NewLine; }));
                            break;
                        }
                }
                #endregion
                File.Delete(@"Temp\" + tmp);

                clients.Remove(tcpClient);
            }
            catch(Exception){}
        }

        private string createpath(string p1, string p2)
        {
            Directory.CreateDirectory(p1 + "\\" + Path.GetDirectoryName(p2).Replace(":", ""));
            return p1 + "\\" + p2.Replace(":", "");
        }

  
        public static void AppendAllBytes(string path, byte[] bytes)
        {
            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
        }
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                //Генерируем число являющееся латинским символом в юникоде
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                //Конструируем строку со случайно сгенерированными символами
                builder.Append(ch);
            }
            return builder.ToString();
        }
        private void command(byte[] message, object client)
        {
            TcpClient tcpClient = (TcpClient)client;            
            XmlSerializer serializer = new XmlSerializer(typeof(header));
            StreamReader reader = new StreamReader(new MemoryStream(message), System.Text.Encoding.Default);
            header h  = (header)serializer.Deserialize(reader);
           switch(h.command)
      {
          case "-cr":
              {          
     
                  if (frm.create_repo(h.author,h.filecount,h.date_creation))
                  {
                      frm.log("Репозиторий создан успешно", frm.c.path_to_repo + "\\" + h.name);
                      frm.BeginInvoke((Action)(() =>
                      {
                          frm.memoEdit1.Text += "Репозиторий пользователя " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString() + "  создан успешно" + Environment.NewLine;
                      }));
                  }
                  break;
              }
          case "-sf":
              {
               
                  frm.BeginInvoke((Action)(() => { frm.memoEdit1.Text += h.file_name + Environment.NewLine; }));
                  break; 
              }
           }
        }

    }
}

public class header
{ 
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
    public int rev { set; get; }
    public DateTime date_creation { set; get; }
    public DateTime updated { set; get; }
    public int filecount { set; get; }
}