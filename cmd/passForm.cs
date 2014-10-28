using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cmd
{
    public partial class passForm : DevExpress.XtraEditors.XtraForm
    {
        public passForm()
        {
            InitializeComponent();
        }

        public string p="admin"; 
        public string l="admin";
        bool rez;

        private void passForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if ((login.Text != l) && (password.Text != p)) { rez = false; } else { rez = true; }
            this.DialogResult = DialogResult.OK;          
        }

        public bool ReturnData()
        {
            return rez;
        } 
      


    }
}
