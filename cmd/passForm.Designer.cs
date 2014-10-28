namespace cmd
{
    partial class passForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.login = new DevExpress.XtraEditors.TextEdit();
            this.panel1 = new System.Windows.Forms.Panel();
            this.password = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.login.Properties)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.password.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // login
            // 
            this.login.EditValue = "login";
            this.login.Location = new System.Drawing.Point(23, 19);
            this.login.Name = "login";
            this.login.Size = new System.Drawing.Size(205, 20);
            this.login.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.password);
            this.panel1.Controls.Add(this.login);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(249, 79);
            this.panel1.TabIndex = 1;
            // 
            // password
            // 
            this.password.EditValue = "password";
            this.password.Location = new System.Drawing.Point(22, 40);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(205, 20);
            this.password.TabIndex = 1;
            // 
            // passForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 108);
            this.Controls.Add(this.panel1);
            this.Name = "passForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Подтверждение";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.passForm_FormClosing);
            
            ((System.ComponentModel.ISupportInitialize)(this.login.Properties)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.password.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit login;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.TextEdit password;
    }
}