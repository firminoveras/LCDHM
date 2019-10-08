namespace LCDHM {
    partial class SobreForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BT_Fechar_Info = new System.Windows.Forms.Button();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.label1);
            this.groupBox.Font = new System.Drawing.Font("Tw Cen MT", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.ForeColor = System.Drawing.Color.White;
            this.groupBox.Location = new System.Drawing.Point(13, 127);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(330, 149);
            this.groupBox.TabIndex = 2;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Sobre";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(283, 114);
            this.label1.TabIndex = 0;
            this.label1.Text = "Desenvolvido por\r\nFirmino Veras\r\ngithub.com/firminoveras/LCDHM\r\n\r\nÍcones por \r\nMi" +
    "crosoft Visual Studio 2017 Image Library";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::LCDHM.Properties.Resources.Logo;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(331, 108);
            this.panel1.TabIndex = 1;
            // 
            // BT_Fechar_Info
            // 
            this.BT_Fechar_Info.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.BT_Fechar_Info.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Fechar_Info.Font = new System.Drawing.Font("Tw Cen MT", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Fechar_Info.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.BT_Fechar_Info.Location = new System.Drawing.Point(13, 282);
            this.BT_Fechar_Info.Name = "BT_Fechar_Info";
            this.BT_Fechar_Info.Size = new System.Drawing.Size(330, 52);
            this.BT_Fechar_Info.TabIndex = 5;
            this.BT_Fechar_Info.Text = "Sair";
            this.BT_Fechar_Info.UseVisualStyleBackColor = false;
            this.BT_Fechar_Info.Click += new System.EventHandler(this.BT_Fechar_Info_Click);
            // 
            // SobreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(355, 346);
            this.Controls.Add(this.BT_Fechar_Info);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SobreForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "SobreForm";
            this.TopMost = true;
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BT_Fechar_Info;
    }
}