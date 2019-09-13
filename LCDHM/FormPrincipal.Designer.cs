namespace LCDHM {
    partial class FormPrincipal {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrincipal));
            this.IconeNotificacao = new System.Windows.Forms.NotifyIcon(this.components);
            this.MenuContexto = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menu_Conectar = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_portas = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menu_Desconectar = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Atualizar = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menu_Configurar = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Sobre = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Sair = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.Text_MSI_Diretorio = new System.Windows.Forms.TextBox();
            this.BT_Buscar_MSI = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BT_Buscar_Steam = new System.Windows.Forms.Button();
            this.Text_Steam_Diretorio = new System.Windows.Forms.TextBox();
            this.BT_Aplicar = new System.Windows.Forms.Button();
            this.serial = new System.IO.Ports.SerialPort(this.components);
            this.MenuContexto.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // IconeNotificacao
            // 
            this.IconeNotificacao.ContextMenuStrip = this.MenuContexto;
            this.IconeNotificacao.Icon = ((System.Drawing.Icon)(resources.GetObject("IconeNotificacao.Icon")));
            this.IconeNotificacao.Text = "LCDhm";
            this.IconeNotificacao.Visible = true;
            // 
            // MenuContexto
            // 
            this.MenuContexto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MenuContexto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.MenuContexto.Font = new System.Drawing.Font("Tw Cen MT", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuContexto.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_Conectar,
            this.menu_Desconectar,
            this.menu_Atualizar,
            this.toolStripSeparator1,
            this.menu_Configurar,
            this.menu_Sobre,
            this.menu_Sair});
            this.MenuContexto.Name = "menu";
            this.MenuContexto.ShowImageMargin = false;
            this.MenuContexto.Size = new System.Drawing.Size(126, 142);
            // 
            // menu_Conectar
            // 
            this.menu_Conectar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menu_Conectar.DropDown = this.Menu_portas;
            this.menu_Conectar.Font = new System.Drawing.Font("Tw Cen MT", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu_Conectar.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.menu_Conectar.Name = "menu_Conectar";
            this.menu_Conectar.Size = new System.Drawing.Size(125, 22);
            this.menu_Conectar.Text = "Conectar";
            // 
            // Menu_portas
            // 
            this.Menu_portas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Menu_portas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Menu_portas.Font = new System.Drawing.Font("Tw Cen MT", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Menu_portas.Name = "menu";
            this.Menu_portas.ShowImageMargin = false;
            this.Menu_portas.Size = new System.Drawing.Size(36, 4);
            // 
            // menu_Desconectar
            // 
            this.menu_Desconectar.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.menu_Desconectar.Name = "menu_Desconectar";
            this.menu_Desconectar.Size = new System.Drawing.Size(125, 22);
            this.menu_Desconectar.Text = "Desconectar";
            this.menu_Desconectar.Visible = false;
            this.menu_Desconectar.Click += new System.EventHandler(this.Menu_Desconectar_Click);
            // 
            // menu_Atualizar
            // 
            this.menu_Atualizar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menu_Atualizar.Font = new System.Drawing.Font("Tw Cen MT", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu_Atualizar.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.menu_Atualizar.Name = "menu_Atualizar";
            this.menu_Atualizar.Size = new System.Drawing.Size(125, 22);
            this.menu_Atualizar.Text = "Atualizar";
            this.menu_Atualizar.Click += new System.EventHandler(this.Menu_Atualizar_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(122, 6);
            // 
            // menu_Configurar
            // 
            this.menu_Configurar.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.menu_Configurar.Name = "menu_Configurar";
            this.menu_Configurar.Size = new System.Drawing.Size(125, 22);
            this.menu_Configurar.Text = "Configurar";
            this.menu_Configurar.Click += new System.EventHandler(this.Menu_Configurar_Click);
            // 
            // menu_Sobre
            // 
            this.menu_Sobre.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.menu_Sobre.Name = "menu_Sobre";
            this.menu_Sobre.Size = new System.Drawing.Size(125, 22);
            this.menu_Sobre.Text = "Sobre";
            // 
            // menu_Sair
            // 
            this.menu_Sair.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.menu_Sair.Font = new System.Drawing.Font("Tw Cen MT", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu_Sair.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.menu_Sair.Name = "menu_Sair";
            this.menu_Sair.Size = new System.Drawing.Size(125, 22);
            this.menu_Sair.Text = "Sair";
            this.menu_Sair.Click += new System.EventHandler(this.Menu_Sair_Click);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // Text_MSI_Diretorio
            // 
            this.Text_MSI_Diretorio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Text_MSI_Diretorio.Font = new System.Drawing.Font("Tw Cen MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Text_MSI_Diretorio.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Text_MSI_Diretorio.Location = new System.Drawing.Point(6, 30);
            this.Text_MSI_Diretorio.Name = "Text_MSI_Diretorio";
            this.Text_MSI_Diretorio.ReadOnly = true;
            this.Text_MSI_Diretorio.Size = new System.Drawing.Size(587, 22);
            this.Text_MSI_Diretorio.TabIndex = 3;
            // 
            // BT_Buscar_MSI
            // 
            this.BT_Buscar_MSI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BT_Buscar_MSI.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Buscar_MSI.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.BT_Buscar_MSI.Location = new System.Drawing.Point(599, 24);
            this.BT_Buscar_MSI.Name = "BT_Buscar_MSI";
            this.BT_Buscar_MSI.Size = new System.Drawing.Size(75, 30);
            this.BT_Buscar_MSI.TabIndex = 4;
            this.BT_Buscar_MSI.Text = "Buscar";
            this.BT_Buscar_MSI.UseVisualStyleBackColor = false;
            this.BT_Buscar_MSI.Click += new System.EventHandler(this.BT_Buscar_MSI_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BT_Buscar_MSI);
            this.groupBox1.Controls.Add(this.Text_MSI_Diretorio);
            this.groupBox1.Font = new System.Drawing.Font("Tw Cen MT", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(680, 68);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Localização do MSIAfterburner";
            // 
            // FileDialog
            // 
            this.FileDialog.FileName = "FileDialog";
            this.FileDialog.Filter = "Programa|*.exe";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BT_Buscar_Steam);
            this.groupBox2.Controls.Add(this.Text_Steam_Diretorio);
            this.groupBox2.Font = new System.Drawing.Font("Tw Cen MT", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.groupBox2.Location = new System.Drawing.Point(12, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(680, 68);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Localização da Steam";
            // 
            // BT_Buscar_Steam
            // 
            this.BT_Buscar_Steam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BT_Buscar_Steam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Buscar_Steam.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.BT_Buscar_Steam.Location = new System.Drawing.Point(599, 24);
            this.BT_Buscar_Steam.Name = "BT_Buscar_Steam";
            this.BT_Buscar_Steam.Size = new System.Drawing.Size(75, 30);
            this.BT_Buscar_Steam.TabIndex = 4;
            this.BT_Buscar_Steam.Text = "Buscar";
            this.BT_Buscar_Steam.UseVisualStyleBackColor = false;
            this.BT_Buscar_Steam.Click += new System.EventHandler(this.BT_Buscar_Steam_Click);
            // 
            // Text_Steam_Diretorio
            // 
            this.Text_Steam_Diretorio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Text_Steam_Diretorio.Font = new System.Drawing.Font("Tw Cen MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Text_Steam_Diretorio.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Text_Steam_Diretorio.Location = new System.Drawing.Point(6, 30);
            this.Text_Steam_Diretorio.Name = "Text_Steam_Diretorio";
            this.Text_Steam_Diretorio.ReadOnly = true;
            this.Text_Steam_Diretorio.Size = new System.Drawing.Size(587, 22);
            this.Text_Steam_Diretorio.TabIndex = 3;
            // 
            // BT_Aplicar
            // 
            this.BT_Aplicar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BT_Aplicar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Aplicar.Font = new System.Drawing.Font("Tw Cen MT", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Aplicar.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.BT_Aplicar.Location = new System.Drawing.Point(12, 160);
            this.BT_Aplicar.Name = "BT_Aplicar";
            this.BT_Aplicar.Size = new System.Drawing.Size(680, 30);
            this.BT_Aplicar.TabIndex = 5;
            this.BT_Aplicar.Text = "Aplicar";
            this.BT_Aplicar.UseVisualStyleBackColor = false;
            this.BT_Aplicar.Click += new System.EventHandler(this.BT_Aplicar_Click);
            // 
            // serial
            // 
            this.serial.BaudRate = 115200;
            this.serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Serial_Recebeu);
            // 
            // FormPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(704, 200);
            this.Controls.Add(this.BT_Aplicar);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormPrincipal";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.Text = "LCDHM";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MenuContexto.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.NotifyIcon IconeNotificacao;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ContextMenuStrip MenuContexto;
        private System.Windows.Forms.ToolStripMenuItem menu_Atualizar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menu_Sair;
        private System.Windows.Forms.ToolStripMenuItem menu_Conectar;
        private System.Windows.Forms.ContextMenuStrip Menu_portas;
        private System.Windows.Forms.ToolStripMenuItem menu_Desconectar;
        private System.Windows.Forms.ToolStripMenuItem menu_Sobre;
        private System.Windows.Forms.TextBox Text_MSI_Diretorio;
        private System.Windows.Forms.Button BT_Buscar_MSI;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button BT_Buscar_Steam;
        private System.Windows.Forms.TextBox Text_Steam_Diretorio;
        private System.Windows.Forms.Button BT_Aplicar;
        private System.Windows.Forms.ToolStripMenuItem menu_Configurar;
        private System.IO.Ports.SerialPort serial;
    }
}

