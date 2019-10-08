using System.Windows.Forms;

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
            this.MenuIPs = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Menu_IP = new System.Windows.Forms.ToolStripTextBox();
            this.menu_Desconectar = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menu_Configurar = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Sobre = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_Sair = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.Text_MSI_Diretorio = new System.Windows.Forms.TextBox();
            this.BT_Buscar_MSI = new System.Windows.Forms.Button();
            this.FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.BT_Buscar_Steam = new System.Windows.Forms.Button();
            this.Text_Steam_Diretorio = new System.Windows.Forms.TextBox();
            this.BT_Aplicar = new System.Windows.Forms.Button();
            this.TCP_Listener = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.MenuContexto.SuspendLayout();
            this.MenuIPs.SuspendLayout();
            this.SuspendLayout();
            // 
            // IconeNotificacao
            // 
            this.IconeNotificacao.ContextMenuStrip = this.MenuContexto;
            this.IconeNotificacao.Icon = ((System.Drawing.Icon)(resources.GetObject("IconeNotificacao.Icon")));
            this.IconeNotificacao.Text = "LCDHM";
            this.IconeNotificacao.Visible = true;
            // 
            // MenuContexto
            // 
            this.MenuContexto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.MenuContexto.Font = new System.Drawing.Font("Tw Cen MT", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuContexto.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuContexto.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_Conectar,
            this.menu_Desconectar,
            this.toolStripSeparator1,
            this.menu_Configurar,
            this.menu_Sobre,
            this.menu_Sair});
            this.MenuContexto.Name = "menu";
            this.MenuContexto.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.MenuContexto.ShowItemToolTips = false;
            this.MenuContexto.Size = new System.Drawing.Size(142, 120);
            // 
            // menu_Conectar
            //    
            this.menu_Conectar.DropDown = this.MenuIPs;
            this.menu_Conectar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menu_Conectar.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.menu_Conectar.Image = global::LCDHM.Properties.Resources.conectar;
            this.menu_Conectar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menu_Conectar.Name = "menu_Conectar";
            this.menu_Conectar.Size = new System.Drawing.Size(141, 22);
            this.menu_Conectar.Text = "Conectar";
            this.menu_Conectar.Click += new System.EventHandler(this.Menu_Conectar_Click);
            // 
            // MenuIPs
            // 
            this.MenuIPs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.MenuIPs.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.MenuIPs.Font = new System.Drawing.Font("Tw Cen MT", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuIPs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_IP});
            this.MenuIPs.Name = "menu";
            this.MenuIPs.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.MenuIPs.ShowImageMargin = false;
            this.MenuIPs.ShowItemToolTips = false;
            this.MenuIPs.Size = new System.Drawing.Size(156, 42);
            // 
            // Menu_IP
            // 
            this.Menu_IP.AutoCompleteCustomSource.AddRange(new string[] {
            "192.168.0.XXX"});
            this.Menu_IP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(52)))));
            this.Menu_IP.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Menu_IP.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Menu_IP.Margin = new System.Windows.Forms.Padding(0);
            this.Menu_IP.MaxLength = 15;
            this.Menu_IP.Name = "Menu_IP";
            this.Menu_IP.Size = new System.Drawing.Size(100, 16);
            this.Menu_IP.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Menu_IP.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Menu_IP_EnterClick);
            this.Menu_IP.TextChanged += new System.EventHandler(this.Menu_IP_TextChanged);
            // 
            // menu_Desconectar
            // 
            this.menu_Desconectar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu_Desconectar.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.menu_Desconectar.Image = global::LCDHM.Properties.Resources.desconectar;
            this.menu_Desconectar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menu_Desconectar.Name = "menu_Desconectar";
            this.menu_Desconectar.Size = new System.Drawing.Size(141, 22);
            this.menu_Desconectar.Text = "Desconectar";
            this.menu_Desconectar.Visible = false;
            this.menu_Desconectar.Click += new System.EventHandler(this.Menu_Desconectar_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(138, 6);
            // 
            // menu_Configurar
            // 
            this.menu_Configurar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu_Configurar.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.menu_Configurar.Image = global::LCDHM.Properties.Resources.configuracoes;
            this.menu_Configurar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menu_Configurar.Name = "menu_Configurar";
            this.menu_Configurar.Size = new System.Drawing.Size(141, 22);
            this.menu_Configurar.Text = "Configurar";
            this.menu_Configurar.Click += new System.EventHandler(this.Menu_Configurar_Click);
            // 
            // menu_Sobre
            // 
            this.menu_Sobre.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu_Sobre.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.menu_Sobre.Image = global::LCDHM.Properties.Resources.info;
            this.menu_Sobre.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menu_Sobre.Name = "menu_Sobre";
            this.menu_Sobre.Size = new System.Drawing.Size(141, 22);
            this.menu_Sobre.Text = "Sobre";
            this.menu_Sobre.Click += new System.EventHandler(this.Menu_Sobre_Click);
            // 
            // menu_Sair
            // 
            this.menu_Sair.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menu_Sair.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.menu_Sair.Image = global::LCDHM.Properties.Resources.sair;
            this.menu_Sair.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menu_Sair.Name = "menu_Sair";
            this.menu_Sair.Size = new System.Drawing.Size(141, 22);
            this.menu_Sair.Text = "Sair";
            this.menu_Sair.Click += new System.EventHandler(this.Menu_Sair_Click);
            // 
            // timer
            // 
            this.timer.Interval = 800;
            this.timer.Tick += new System.EventHandler(this.MSI_EnviarTimer);
            // 
            // Text_MSI_Diretorio
            // 
            this.Text_MSI_Diretorio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.Text_MSI_Diretorio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Text_MSI_Diretorio.Font = new System.Drawing.Font("Tw Cen MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Text_MSI_Diretorio.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Text_MSI_Diretorio.Location = new System.Drawing.Point(161, 12);
            this.Text_MSI_Diretorio.Name = "Text_MSI_Diretorio";
            this.Text_MSI_Diretorio.ReadOnly = true;
            this.Text_MSI_Diretorio.Size = new System.Drawing.Size(460, 22);
            this.Text_MSI_Diretorio.TabIndex = 3;
            // 
            // BT_Buscar_MSI
            // 
            this.BT_Buscar_MSI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.BT_Buscar_MSI.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.BT_Buscar_MSI.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Buscar_MSI.Font = new System.Drawing.Font("Tw Cen MT", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Buscar_MSI.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.BT_Buscar_MSI.Location = new System.Drawing.Point(627, 12);
            this.BT_Buscar_MSI.Name = "BT_Buscar_MSI";
            this.BT_Buscar_MSI.Size = new System.Drawing.Size(87, 22);
            this.BT_Buscar_MSI.TabIndex = 4;
            this.BT_Buscar_MSI.Text = "Buscar";
            this.BT_Buscar_MSI.UseVisualStyleBackColor = false;
            this.BT_Buscar_MSI.Click += new System.EventHandler(this.BT_Buscar_MSI_Click);
            // 
            // FileDialog
            // 
            this.FileDialog.Filter = "Programa|*.exe";
            // 
            // BT_Buscar_Steam
            // 
            this.BT_Buscar_Steam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.BT_Buscar_Steam.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.BT_Buscar_Steam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Buscar_Steam.Font = new System.Drawing.Font("Tw Cen MT", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Buscar_Steam.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.BT_Buscar_Steam.Location = new System.Drawing.Point(627, 41);
            this.BT_Buscar_Steam.Name = "BT_Buscar_Steam";
            this.BT_Buscar_Steam.Size = new System.Drawing.Size(87, 22);
            this.BT_Buscar_Steam.TabIndex = 4;
            this.BT_Buscar_Steam.Text = "Buscar";
            this.BT_Buscar_Steam.UseVisualStyleBackColor = false;
            this.BT_Buscar_Steam.Click += new System.EventHandler(this.BT_Buscar_Steam_Click);
            // 
            // Text_Steam_Diretorio
            // 
            this.Text_Steam_Diretorio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.Text_Steam_Diretorio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Text_Steam_Diretorio.Font = new System.Drawing.Font("Tw Cen MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Text_Steam_Diretorio.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Text_Steam_Diretorio.Location = new System.Drawing.Point(161, 40);
            this.Text_Steam_Diretorio.Name = "Text_Steam_Diretorio";
            this.Text_Steam_Diretorio.ReadOnly = true;
            this.Text_Steam_Diretorio.Size = new System.Drawing.Size(460, 22);
            this.Text_Steam_Diretorio.TabIndex = 3;
            // 
            // BT_Aplicar
            // 
            this.BT_Aplicar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(28)))));
            this.BT_Aplicar.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.BT_Aplicar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Aplicar.Font = new System.Drawing.Font("Tw Cen MT", 10F);
            this.BT_Aplicar.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.BT_Aplicar.Location = new System.Drawing.Point(16, 68);
            this.BT_Aplicar.Name = "BT_Aplicar";
            this.BT_Aplicar.Size = new System.Drawing.Size(698, 26);
            this.BT_Aplicar.TabIndex = 5;
            this.BT_Aplicar.Text = "Aplicar";
            this.BT_Aplicar.UseVisualStyleBackColor = false;
            this.BT_Aplicar.Click += new System.EventHandler(this.BT_Aplicar_Click);
            // 
            // TCP_Listener
            // 
            this.TCP_Listener.Interval = 50;
            this.TCP_Listener.Tick += new System.EventHandler(this.TCP_Listener_Tick);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBox1.Enabled = false;
            this.textBox1.Font = new System.Drawing.Font("Tw Cen MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.textBox1.Location = new System.Drawing.Point(16, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(139, 22);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "Diretório do MSI";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBox2.Enabled = false;
            this.textBox2.Font = new System.Drawing.Font("Tw Cen MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.textBox2.Location = new System.Drawing.Point(16, 41);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(139, 22);
            this.textBox2.TabIndex = 9;
            this.textBox2.Text = "Diretório da Steam";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FormPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(52)))));
            this.ClientSize = new System.Drawing.Size(726, 106);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.BT_Buscar_MSI);
            this.Controls.Add(this.BT_Buscar_Steam);
            this.Controls.Add(this.Text_MSI_Diretorio);
            this.Controls.Add(this.Text_Steam_Diretorio);
            this.Controls.Add(this.BT_Aplicar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormPrincipal";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "LCDHM";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MenuContexto.ResumeLayout(false);
            this.MenuIPs.ResumeLayout(false);
            this.MenuIPs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NotifyIcon IconeNotificacao;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ContextMenuStrip MenuContexto;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menu_Sair;
        private System.Windows.Forms.ToolStripMenuItem menu_Conectar;
        private System.Windows.Forms.ContextMenuStrip MenuIPs;
        private System.Windows.Forms.ToolStripMenuItem menu_Desconectar;
        private System.Windows.Forms.ToolStripMenuItem menu_Sobre;
        private System.Windows.Forms.TextBox Text_MSI_Diretorio;
        private System.Windows.Forms.Button BT_Buscar_MSI;
        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.Windows.Forms.Button BT_Buscar_Steam;
        private System.Windows.Forms.TextBox Text_Steam_Diretorio;
        private System.Windows.Forms.Button BT_Aplicar;
        private System.Windows.Forms.ToolStripMenuItem menu_Configurar;
        private System.Windows.Forms.ToolStripTextBox Menu_IP;
        private System.Windows.Forms.Timer TCP_Listener;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
    }
}

