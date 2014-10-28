using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cmd
{
    public partial class createrepoform : DevExpress.XtraEditors.XtraForm
    {
        public createrepoform()
        {
            InitializeComponent();           
        }
        public string author
        {
            get { return textEdit1.Text; }
        }
        public string name
        {
            get { return textEdit3.Text; }
        }
        private void createrepoform_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(textEdit3.Text==string.Empty){}
            else
            {
             this.DialogResult = DialogResult.OK;
            }
          
        }

        private void createrepoform_Load(object sender, EventArgs e)
        {
            textEdit1.Text = System.Environment.MachineName;
        }      
    }
}
