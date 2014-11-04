using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace cmd
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {

        List<string> repolist = new List<string>();
        public  settings c = new settings();
        serv s =new serv();

        public Form1()
        {
            InitializeComponent();
            s.frm = this;
        }
             

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            notifyIcon1.Visible = true;
            this.Hide();          
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Show(); 
            notifyIcon1.Visible = false; 
        }

        private void вЫходИзПриложенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save_settings();
            Environment.Exit(0);
        }

        void save_settings()
        {
            XMLFile<settings> file = new XMLFile<settings>("settings");
            c.app_autostart = sw1.IsOn;
            c.form_closing = sw2.IsOn;
            c.login = repologin.Text;
            c.password = repopass.Text;
            c.path_to_repo = workpath.Text;
            c.protect_repo = sw4.IsOn;
            c.server_autostart = sw2.IsOn;
            c.server_port = s_port.Text;
            file.Save(c);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                XMLFile<settings> file = new XMLFile<settings>("settings");
                c = file.Load();
                sw1.IsOn = c.app_autostart;
                sw2.IsOn = c.form_closing;
                repologin.Text = c.login;
                repopass.Text = c.password;
                workpath.Text = c.path_to_repo;
                sw4.IsOn = c.protect_repo;
                sw2.IsOn = c.server_autostart;
                s_port.Text = c.server_port;

                if (sw4.IsOn != false)
                {
                    repologin.Enabled = false;
                    repopass.Enabled = false;
                }
                else
                {
                    repologin.Enabled = true;
                    repopass.Enabled = true;
                }
                XMLFile<List<string>> list = new XMLFile<List<string>>(c.path_to_repo + "\\lst");
                repolist = list.Load();
                if (!Directory.Exists("Temp")) 
                {
                    Directory.CreateDirectory("Temp");
                }
         
                    
            }
            catch (Exception) {    ; save_settings(); }
        }


        private void sw4_EditValueChanged(object sender, EventArgs e)
        {
            if (sw4.IsOn == false)
            {
                repologin.Enabled = false;
                repopass.Enabled = false;
            }
            else
            {
                repologin.Enabled = true;
                repopass.Enabled = true;
            }
        }
        
        private void xtraTabControl1_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            listView1.View = View.LargeIcon;
            if ((e.Page.Name == "settings") && (sw4.IsOn == true))
            {
                passForm p = new passForm();
                p.p = c.password;
                p.l = c.login;
                p.ShowDialog();
              
                if (p.DialogResult == DialogResult.OK)
                {
                  
                    if (p.ReturnData() == false)
                    {
                        e.Cancel = true;
                    }
                }
            }
            if ((e.Page.Name == "settings") && (sw4.IsOn == false))
            {

            }
            if (e.Page.Name == "repobrowser")
            {
                listView1.Items.Clear();
                loadrepo();
            }
        }

        public void loadrepo()
        {
                for (int i = 0; i < repolist.Count;i++ )
                {
                    ListViewItem v= new ListViewItem(Text=repolist[i]);
                    listView1.Items.Add(v);
                    v.ImageIndex = 0;
                }
        }


        private void workpath_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                c.path_to_repo = folderBrowserDialog1.SelectedPath;
                workpath.Text = c.path_to_repo;
                if (!File.Exists(workpath.Text+"\\lst.xml"))
                { 
                    XMLFile<List<string>> list = new XMLFile<List<string>>(c.path_to_repo + "\\lst");
                    list.Save(repolist);
                }
                else
                {
                    XMLFile<List<string>> list = new XMLFile<List<string>>(c.path_to_repo + "\\lst");
                    repolist = list.Load();
                }
            }
        }

        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (create_repo(System.Environment.MachineName, 0, DateTime.Now))
            {  
                log("Репозиторий создан успешно", c.path_to_repo + "\\" + System.Environment.MachineName);
                memoEdit1.Text +=  "Репозиторий создан успешно" + Environment.NewLine;
            }
            else 
            {
                log("Ошибка при создании репозитория", c.path_to_repo);
                memoEdit1.Text += "Ошибка при создании репозитория" + Environment.NewLine;
            }

        }

       public void log(string s, string  p)
        {
            try
            {        
                if (!Directory.Exists(p + "\\Log")) { Directory.CreateDirectory(p + "\\Log"); }
                if (File.Exists(p + "\\Log\\log.txt"))
                {
                    long length = new System.IO.FileInfo(p + "\\Log\\log.txt").Length;
                    if (length > 100000000)
                    {
                        if (File.Exists(p + "\\Log\\log_old.txt"))
                        {
                            File.Delete(p + "\\Log\\log_old.txt");
                        }
                        File.Move(p + "\\Log\\log.txt", p + "\\Log\\log_old.txt");
                        File.AppendAllText(p + "\\Log\\log.txt", "[" + DateTime.Now + "] Лог превысил длинну 100МБ и был укорочен" + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText(p + "\\Log\\log.txt", "[" + DateTime.Now + "] " + s + Environment.NewLine);
                    }
                }
                else
                {
                    File.AppendAllText(p + "\\Log\\log.txt", "[" + DateTime.Now + "] " + s + Environment.NewLine);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


       public bool create_repo(string author, int fc, DateTime date_creation)
        {
            try
            {
                repo r = new repo();
                r.author = author;
                r.date_creation = DateTime.Now;
                r.filecount = fc;
                r.name = author;

                if (!Directory.Exists(c.path_to_repo + "\\" + r.name))
                {
                    Directory.CreateDirectory(c.path_to_repo + "\\" + r.name);
                }
                if (!Directory.Exists(c.path_to_repo + "\\" + r.name + "\\Log"))
                {
                    Directory.CreateDirectory(c.path_to_repo + "\\" + r.name + "\\Log");
                }
                if (!Directory.Exists(c.path_to_repo + "\\" + r.name + "\\Data"))
                {
                    Directory.CreateDirectory(c.path_to_repo + "\\" + r.name + "\\Data");
                }

                XMLFile<repo> file = new XMLFile<repo>(c.path_to_repo + "\\" + r.name + "\\info");
                file.Save(r);
               

                if (repolist.Contains(r.name)) 
                {
                    log("Репозиторий для вашего ПК был создан ранее", c.path_to_repo);
                    this.BeginInvoke((Action)(() =>
                      {
                          memoEdit1.Text = memoEdit1.Text + "Репозиторий для  ПК с именем\" " + r.name + "\" был создан ранее" + Environment.NewLine;

                      }));
                    return false; 
                }
                else
                {
                    repolist.Add(r.name);
                    XMLFile<List<string>> list = new XMLFile<List<string>>(c.path_to_repo + "\\lst");
                    list.Save(repolist);
                   
                    this.BeginInvoke((Action)(() =>
                    {
                        listView1.Items.Clear();
                      loadrepo();  
                    }));
                      
                   
                }
            return true;
            }
            catch (Exception ex) {   MessageBox.Show(ex.Message);   return false; }
        }

         private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            repo r = new repo();
            XMLFile<repo> file = new XMLFile<repo>(c.path_to_repo + "\\" + e.Item.Text + "\\info");
            r = file.Load();
            label6.Text = r.author;
            label7.Text = r.date_creation.ToString();
            label8.Text = r.updated.ToString();
            label10.Text = r.filecount.ToString();
        }

        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            delrepo();
        }
        void delrepo ()
        {
            try
            {
                string text = listView1.SelectedItems[0].Text;
                repolist.Remove(text);
                XMLFile<List<string>> list = new XMLFile<List<string>>(c.path_to_repo + "\\lst");
                list.Save(repolist);
                listView1.Items.Clear();
                loadrepo();
                log("Репозиторий " + text + " Удален администратором", c.path_to_repo);
                memoEdit1.Text = memoEdit1.Text + "Репозиторий " + text + " Удален администратором" + Environment.NewLine;

                if (!Directory.Exists(c.path_to_repo + "\\deleted\\" + text))
                {
                    Directory.CreateDirectory(c.path_to_repo + "\\deleted\\");
                    Directory.Move(c.path_to_repo + "\\" + text, c.path_to_repo + "\\deleted\\" + text);
                }
                else
                {
                    Directory.Delete(c.path_to_repo + "\\deleted\\" + text, true);
                    Directory.Move(c.path_to_repo + "\\" + text, c.path_to_repo + "\\deleted\\" + text);
                }


            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void EditRepoBtn_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            get_repo_info();
        }
        void get_repo_info()
        {
            try
            {
                string text = listView1.SelectedItems[0].Text;
                repo r = new repo();
                XMLFile<repo> file = new XMLFile<repo>(c.path_to_repo + "\\" + text + "\\info");
                r = file.Load();
                memoEdit1.Text += "----------------------------------REPO INFO----------------------------" + Environment.NewLine;
                memoEdit1.Text += "Автор :" + r.author + Environment.NewLine;
                memoEdit1.Text += "Дата создания :" + r.date_creation + Environment.NewLine;
                memoEdit1.Text += "Количество файлов :" + r.filecount + Environment.NewLine;           
                memoEdit1.Text += "Последнее обновление :" + r.updated + Environment.NewLine;
                memoEdit1.Text += "Пользователи :" + Environment.NewLine;
                foreach (var v in r.user_list)
                {
                    memoEdit1.Text += v + Environment.NewLine;
                }
                memoEdit1.Text += "----------------------------------------------------------------------------" + Environment.NewLine;

                xtraTabControl1.SelectedTabPage = xtraTabPage1;

                log("----------------------------------REPO INFO----------------------------", c.path_to_repo);
                log("Автор :" + r.author, c.path_to_repo);
                log("Дата создания :" + r.date_creation, c.path_to_repo);
                log("Количество файлов :" + r.filecount, c.path_to_repo);  
                log("Последнее обновление :" + r.updated, c.path_to_repo);
                log("Пользователи :" + r.user_list, c.path_to_repo);
                log("----------------------------------------------------------------------------", c.path_to_repo);


            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                s.port = 3333;
                s.runed = true;
                s.start();
            }
            catch(Exception){}

        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry("localhost");
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
                Socket send = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                       
                send.Connect(ipEndPoint);

                if (send.Connected) { memoEdit1.Text += "подключен" + Environment.NewLine; } else { memoEdit1.Text += "не подключен" + Environment.NewLine; }
                send.Disconnect(true);
           
            
            }
            catch (Exception ex) { memoEdit1.Text += ex.Message  + Environment.NewLine; }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {        
            s.stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                labelControl3.Text = s.runed.ToString();
                if (s.clients.Count != checkedListBoxControl1.Items.Count)
                {
                    checkedListBoxControl1.Items.Clear();
                    foreach (var v in s.clients)
                    {
                         
                        checkedListBoxControl1.Items.Add(  ((IPEndPoint)v.Client.RemoteEndPoint).Address.ToString()  );
                    }

                }
            }
            catch (Exception) { }
        }

        private void xtraTabControl1_Click(object sender, EventArgs e)
        {




        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    repobrowsermenu.Show(Cursor.Position);
                }
            } 
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            delrepo();
        }

        private void информацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            get_repo_info();
        }

        private void открытьДиректориюВПроводникеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(c.path_to_repo + "\\" + listView1.SelectedItems[0].Text))
            { 
                Process.Start(c.path_to_repo + "\\" + listView1.SelectedItems[0].Text); 
            }
        }

        private void отключитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TcpClient tcpClient = (TcpClient)s.clients[0]; 
            tcpClient.Close();
        }

        private void GetDirectories(DirectoryInfo[] subDirs,
           TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
          listView1.View = View.List;      
            string path = c.path_to_repo + "\\"+listView1.SelectedItems[0].Text;;           
            try
            {
                XMLFile<repo> file = new XMLFile<repo>(path + "\\info");
                repo r = file.Load();
                listView1.Items.Clear();

                foreach (var v in r.file_info)
                {
                    listView1.Items.Add(v.FullName);  
                }
             }
            
            catch (Exception) { }
        }


      
    }
}
