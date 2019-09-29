using MSI.Afterburner;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LCDHM {
    public partial class FormPrincipal : Form {
        private int
                PAGINA = 0,
                IPLocalLenght,
                ANALISE_LINHA = 0,
                ANALISE_MIN_FPS = 60,
                FPS_MIN = 0,
                FPS_MAX = 0,
                CPU_CORES = 0,
                CPU_THREADS = 0,
                RAM_TOTAL = 0,
                HDD_INDEX = 1,
                NET_INDEX = 1,
                CORE_BOOST = 0,
                MEM_BOOST = 0,
                FAN_BOOST = 30;

        private readonly int
                TCP_PORTA = 1199,
                FAN_SENSIBILIDADE = 5,
                CLOCK_SENSIBILIDADE = 25;

        private MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG
                FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO;

        private ControlMemory
                CM;

        private HardwareMonitor
                HM;

        private TcpClient
                Cliente;

        private String
                IPLocal;


        public FormPrincipal() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e) {
            Ocultar_Configuracoes();
            Properties.Settings.Default.Reload();
            Text_MSI_Diretorio.Text = Properties.Settings.Default.MSI_Directory;
            Text_Steam_Diretorio.Text = Properties.Settings.Default.Steam_Directory;
            if (!(Text_MSI_Diretorio.Text.EndsWith("MSIAfterburner.exe") && Text_Steam_Diretorio.Text.EndsWith("Steam.exe"))) Mostrar_Configuracoes();
            AtualizarIPLocal();

        }

        private void AtualizarIPLocal() {
            IPLocal = Dns.GetHostAddresses(Dns.GetHostName()).Where(address => address.AddressFamily == AddressFamily.InterNetwork).First().ToString();
            String IPFormatado = IPLocal.Split('.')[0] + "." + IPLocal.Split('.')[1] + "." + IPLocal.Split('.')[2] + ".";
            Menu_IP.Text = IPFormatado;
            IPLocalLenght = IPFormatado.Length;
        }

        private void SerialRecebeu() {
            if (Cliente.Connected) {
                String entrada = TCPReadLine();
                Console.WriteLine(entrada);
                if (entrada.Contains("init")) ConectarMSI();
                if (entrada.Contains("MENU") || entrada.Contains("p0")) MudarPagina(0);
                if (entrada.Contains("p1")) MudarPagina(1);
                if (entrada.Contains("p2")) { AtualizarClocks(); MudarPagina(2); }
                if (entrada.Contains("p3")) MudarPagina(3);
                if (entrada.Contains("p4")) MudarPagina(4);
                if (entrada.Contains("p5")) MudarPagina(5);
                if (entrada.Contains("p6")) MudarPagina(6);
                if (entrada.Contains("p7")) { ANALISE_LINHA = 0; MudarPagina(7); }
                if (entrada.Contains("analise_gravar") && ANALISE_LINHA < 7) ANALISE_LINHA++;
                if (entrada.Contains("analise_limpar")) ANALISE_LINHA = 0;
                if (entrada.Contains("analise_auto")) ANALISE_MIN_FPS = Convert.ToInt32(entrada.Replace("analise_auto", ""));
                if (entrada.Contains("fps_reset")) { FPS_MAX = 0; FPS_MIN = 0; Enviar("GPU.t6", "-"); Enviar("GPU.t7", "-"); }
                if (entrada.Contains("core_min") && CORE_BOOST > -200) { CORE_BOOST -= CLOCK_SENSIBILIDADE; AtualizarClocks(); Enviar("Overclock.tcore", Color.Red); }
                if (entrada.Contains("core_max") && CORE_BOOST < 200) { CORE_BOOST += CLOCK_SENSIBILIDADE; AtualizarClocks(); Enviar("Overclock.tcore", Color.Red); }
                if (entrada.Contains("core_rst")) { CORE_BOOST = 0; AtualizarClocks(); Enviar("Overclock.tcore", Color.Red); }
                if (entrada.Contains("mem_min") && MEM_BOOST > -200) { MEM_BOOST -= CLOCK_SENSIBILIDADE; AtualizarClocks(); Enviar("Overclock.tmem", Color.Red); }
                if (entrada.Contains("mem_max") && MEM_BOOST < 200) { MEM_BOOST += CLOCK_SENSIBILIDADE; AtualizarClocks(); Enviar("Overclock.tmem", Color.Red); }
                if (entrada.Contains("mem_rst")) { MEM_BOOST = 0; AtualizarClocks(); Enviar("Overclock.tmem", Color.Red); }
                if (entrada.Contains("fan_min") && FAN_BOOST > 31 + FAN_SENSIBILIDADE) { FAN_BOOST -= FAN_SENSIBILIDADE; FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None; AtualizarClocks(); Enviar("Overclock.tfan", Color.Red); }
                if (entrada.Contains("fan_max") && FAN_BOOST < 100 - FAN_SENSIBILIDADE) { FAN_BOOST += FAN_SENSIBILIDADE; FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None; AtualizarClocks(); Enviar("Overclock.tfan", Color.Red); }
                if (entrada.Contains("fan_auto")) { FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO; FAN_BOOST = 30; AtualizarClocks(); Enviar("Overclock.tfan", Color.Red); }
                if (entrada.Contains("oc_aplicar")) {
                    CM.GpuEntries[0].CoreClockBoostCur = CORE_BOOST * 1000;
                    CM.GpuEntries[0].MemoryClockBoostCur = MEM_BOOST * 1000;
                    CM.GpuEntries[0].FanFlagsCur = FAN_FLAG;
                    if (FAN_FLAG == MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None) CM.GpuEntries[0].FanSpeedCur = uint.Parse(FAN_BOOST.ToString());
                    Enviar("Overclock.tcore", Color.FromArgb(255, 0, 184, 192));
                    Enviar("Overclock.tmem", Color.FromArgb(255, 0, 184, 192));
                    Enviar("Overclock.tfan", Color.FromArgb(255, 0, 184, 192));
                    CM.CommitChanges();
                    AtualizarClocks();
                }
                if (entrada.Contains("gerenciador")) Process.Start("Taskmgr.exe");
                if (entrada.Contains("HD_max")) if (HDD_INDEX < 9) HDD_INDEX++; else HDD_INDEX = 1;
                if (entrada.Contains("HD_min")) if (HDD_INDEX > 1) HDD_INDEX--; else HDD_INDEX = 9;
                if (entrada.Contains("NET_max")) if (NET_INDEX < 9) NET_INDEX++; else NET_INDEX = 1;
                if (entrada.Contains("NET_min")) if (NET_INDEX > 1) NET_INDEX--; else NET_INDEX = 9;
                if (entrada.Contains("Steam")) try { Process.Start(Properties.Settings.Default.Steam_Directory); } catch (Exception) { Mostrar_Configuracoes(); }

                //TODO: Não funciona.
                if (entrada.Contains("delay")) {
                    timer.Stop();
                    timer.Enabled = false;
                    timer.Interval = Convert.ToInt32(entrada.Replace("delay", ""));
                    timer.Enabled = true;
                    timer.Start();
                }
                if (entrada.Contains("dormir")) {
                    String tempo = entrada.Replace("dormir", "");
                    int Seg = (int.Parse(tempo.ToCharArray()[2].ToString() + tempo.ToCharArray()[3].ToString()) * 60) + (int.Parse(tempo.ToCharArray()[0].ToString() + tempo.ToCharArray()[1].ToString()) * 3600);
                    Process.Start("CMD.exe", "/c shutdown -a");
                    if (Seg != 0) Process.Start("CMD.exe", "/c shutdown -s -t " + Seg.ToString());
                }

            }
        }

        private void MudarPagina(int PG_Index) {
            PAGINA = PG_Index;
            EnviarAutomatico();
        }
        private void TimerTick(object sender, EventArgs e) {
            if (Cliente.Connected && Cliente.GetStream().DataAvailable) SerialRecebeu();
            EnviarAutomatico();
        }
        private void EnviarAutomatico() {
            int fps;
            switch (PAGINA) {
                default:
                    break;
                case 1:
                    fps = int.Parse(GetEntidade("Framerate").Data.ToString("N0"));
                    if (fps == 0) {
                        Enviar("GPU.t5", "-");
                        Enviar("GPU.t6", "-");
                        Enviar("GPU.t7", "-");
                        FPS_MAX = 0;
                        FPS_MIN = 0;
                    } else {
                        if (FPS_MIN == 0) FPS_MIN = fps; else if (fps < FPS_MIN) FPS_MIN = fps;
                        if (FPS_MAX == 0) FPS_MAX = fps; else if (fps > FPS_MAX) FPS_MAX = fps;
                        Enviar("GPU.t5", fps.ToString());
                        Enviar("GPU.t6", FPS_MIN.ToString());
                        Enviar("GPU.t7", FPS_MAX.ToString());
                    }
                    Enviar("GPU.t0", GetEntidade("GPU usage").Data.ToString());
                    Enviar("GPU.t1", GetEntidade("Core clock").Data.ToString());
                    Enviar("GPU.t2", GetEntidade("Memory clock").Data.ToString());
                    Enviar("GPU.t3", GetEntidade("Memory usage").Data.ToString("N0"));
                    Enviar("GPU.t4", GetEntidade("GPU temperature").Data.ToString());
                    Enviar("GPU.t8", GetEntidade("Fan tachometer").Data.ToString());
                    Enviar("GPU.t9", GetEntidade("Power").Data.ToString());
                    break;
                case 2:
                    fps = int.Parse(GetEntidade("Framerate").Data.ToString("N0"));
                    Enviar("gGPU", 0, fps, 0, 100, 0, 100);
                    Enviar("gGPU", 1, int.Parse(GetEntidade("GPU temperature").Data.ToString("N0")), 0, 100, 0, 100);
                    Enviar("Overclock.t11", GetEntidade("GPU temperature").Data.ToString() + " C");
                    Enviar("Overclock.t10", fps.ToString() + " FPS");
                    Enviar("gGPU", 2, 60, 0, 100, 0, 100);
                    break;
                case 3:
                    Enviar("CPU.t12", GetEntidade("CPU usage").Data.ToString("N0"));
                    Enviar("CPU.t13", GetEntidade("CPU fan speed").Data.ToString());
                    Enviar("CPU.t14", GetEntidade("CPU temperature").Data.ToString());
                    Enviar("CPU.t0", GetEntidade("CPU clock").Data.ToString("N0"));
                    Enviar("CPU.t15", CPU_CORES.ToString());
                    Enviar("CPU.t16", CPU_THREADS.ToString());
                    String cpus = "|";
                    for (int i = 1; i <= CPU_THREADS; i++) {
                        String porcentagem = GetEntidade("CPU" + i.ToString() + " usage").Data.ToString("N0");
                        switch (porcentagem.Length) {
                            case 1: porcentagem = porcentagem.Equals("0") ? "   " : " " + porcentagem + "%"; break;
                            case 2: porcentagem += "%"; break;
                            case 3: porcentagem = "MAX"; break;
                        }
                        cpus += " " + porcentagem + " |";
                    }
                    Enviar("CPU.t17", cpus);
                    Enviar("gCPU", 0, int.Parse(GetEntidade("CPU usage").Data.ToString("N0")), 0, 100, 0, 81);
                    Enviar("gCPU", 1, int.Parse(GetEntidade("CPU temperature").Data.ToString("N0")), 0, 100, 0, 81);
                    break;
                case 4:
                    float RAM_Uso = GetEntidade("RAM usage").Data;
                    float RAM_Porcentagem = (RAM_Uso / RAM_TOTAL) * 100;
                    Enviar("MEM.t18", RAM_Porcentagem.ToString("N1"));
                    Enviar("MEM.j1", int.Parse(RAM_Porcentagem.ToString("N0")));
                    Enviar("MEM.t19", RAM_Uso.ToString("N0"));
                    Enviar("MEM.t20", GetEntidade("HDD" + HDD_INDEX.ToString() + " read rate").Data.ToString("N3"));
                    Enviar("MEM.t21", GetEntidade("HDD" + HDD_INDEX.ToString() + " write rate").Data.ToString("N3"));
                    Enviar("MEM.t22", GetEntidade("HDD" + HDD_INDEX.ToString() + " temperature").Data.ToString("N1"));
                    Enviar("MEM.t0", GetEntidade("HDD" + HDD_INDEX.ToString() + " usage").Data.ToString("N1"));
                    Enviar("MEM.t23", HDD_INDEX.ToString());
                    Enviar("MEM.t25", GetEntidade("NET" + NET_INDEX.ToString() + " download rate").Data.ToString("N3"));
                    Enviar("MEM.t26", GetEntidade("NET" + NET_INDEX.ToString() + " download rate").SrcUnits.ToString());
                    Enviar("MEM.t27", GetEntidade("NET" + NET_INDEX.ToString() + " upload rate").Data.ToString("N3"));
                    Enviar("MEM.t28", GetEntidade("NET" + NET_INDEX.ToString() + " upload rate").SrcUnits.ToString());
                    Enviar("MEM.t29", NET_INDEX.ToString());
                    Enviar("gMEM", 0, int.Parse(RAM_Porcentagem.ToString("N0")), 0, 100, 0, 81);
                    break;
                case 5:
                    float RAM_Uso2 = GetEntidade("RAM usage").Data;
                    float RAM_Porcentagem2 = (RAM_Uso2 / RAM_TOTAL) * 100;
                    Enviar("GRAFICO.t30", GetEntidade("GPU usage").Data.ToString("N0") + " %");
                    Enviar("GRAFICO.t31", GetEntidade("CPU usage").Data.ToString("N0") + " %");
                    Enviar("GRAFICO.t32", GetEntidade("framerate").Data.ToString("N0") + " fps");
                    Enviar("s0", 0, int.Parse(GetEntidade("GPU usage").Data.ToString("N0")), 0, 100, 0, 71);
                    Enviar("s0", 1, int.Parse(GetEntidade("CPU usage").Data.ToString("N0")), 0, 100, 0, 71);
                    Enviar("s0", 2, int.Parse(GetEntidade("framerate").Data.ToString("N0")), 0, 71, 0, 71);
                    Enviar("GRAFICO.t33", GetEntidade("Memory usage").Data.ToString("N0") + " MB");
                    Enviar("GRAFICO.t34", GetEntidade("GPU temperature").Data.ToString() + " C");
                    //TODO: Obter MB da GPU
                    Enviar("s1", 0, int.Parse(((GetEntidade("Memory usage").Data / 4096) * 100).ToString("N0")), 0, 100, 0, 51);
                    Enviar("GRAFICO.t35", GetEntidade("CPU clock").Data.ToString("N0") + " MHz");
                    Enviar("GRAFICO.t36", GetEntidade("CPU temperature").Data.ToString() + " C");
                    //TODO: Obter Clock da CPU
                    Enviar("s2", 0, int.Parse(((GetEntidade("CPU clock").Data / 3900) * 100).ToString("N0")), 0, 100, 0, 51);
                    Enviar("GRAFICO.t37", RAM_Uso2.ToString("N0") + " MB");
                    Enviar("GRAFICO.t38", (RAM_TOTAL - RAM_Uso2).ToString("N0") + " MB");
                    Enviar("s3", 0, int.Parse(RAM_Porcentagem2.ToString("N0")), 0, 100, 0, 51);
                    break;
                case 7:
                    fps = Convert.ToInt32(GetEntidade("Framerate").Data);
                    Enviar("ANALISE.t" + ANALISE_LINHA + "0", fps.ToString("N0"));
                    //if (fps < ANALISE_MIN_FPS && fps != 0) Enviar("t" + ANALISE_LINHA + "0", Color.Red); else Enviar("t" + ANALISE_LINHA + "0", Color.FromArgb(255, 0, 184, 192));
                    Enviar("t" + ANALISE_LINHA + "0", fps < ANALISE_MIN_FPS && fps != 0 ? Color.Red : Color.FromArgb(255, 0, 184, 192));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "1", GetEntidade("GPU usage").Data.ToString("N0"));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "2", GetEntidade("Memory usage").Data.ToString("N0").Replace(".", ""));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "3", GetEntidade("GPU Temperature").Data.ToString("N0"));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "4", GetEntidade("CPU usage").Data.ToString("N0"));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "5", GetEntidade("CPU temperature").Data.ToString("N0"));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "6", GetEntidade("RAM usage").Data.ToString("N0").Replace(".", ""));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "7", HDD_INDEX + ":" + GetEntidade("HDD" + HDD_INDEX.ToString() + " usage").Data.ToString("N0"));
                    if (fps < ANALISE_MIN_FPS && ANALISE_LINHA < 7 && fps != 0) ANALISE_LINHA++;
                    break;

                case 6:
                    Enviar("PSU.t0", GetEntidade("Power").Data.ToString());
                    Enviar("PSU.t1", GetEntidade("CPU voltage").Data.ToString());

                    Enviar("PSU.t2", GetEntidade("PSU + 3.3V voltage").Data.ToString());
                    Enviar("PSU.t4", GetEntidade("PSU + 5V voltage").Data.ToString());
                    Enviar("PSU.t6", GetEntidade("PSU + 12V voltage").Data.ToString());



                    break;
            }
        }

        private void TCPPrintLine(String MensagemTCP) {
            if (Cliente.Connected) {
                if (!MensagemTCP.EndsWith("\n")) MensagemTCP += '\n';
                Byte[] MensagemTCPByte = Encoding.ASCII.GetBytes(MensagemTCP);
                Cliente.GetStream().Write(MensagemTCPByte, 0, MensagemTCPByte.Length);
            }
        }
        private String TCPReadLine() {
            String LinhaLida = "";
            while (Cliente.Connected && Cliente.GetStream().DataAvailable) {
                int b = Cliente.GetStream().ReadByte();
                if (b == -1 || b == '\n') break;
                LinhaLida += (char)b;
            }

            return LinhaLida;
        }
        private void ConectarTCP(String IP) {
            try {
                Cliente = new TcpClient();
                if (Cliente.ConnectAsync(IP, TCP_PORTA).Wait(1000)) {
                    menu_Conectar.Visible = false;
                    menu_Desconectar.Visible = true;
                    TCPPrintLine("page 0");
                    Properties.Settings.Default.IP_Favorito = IP;
                    Properties.Settings.Default.Save();
                    IconeNotificacao.ShowBalloonTip(1000, "LCDHM", "Conectado", ToolTipIcon.Info);
                    timer.Start();
                } else {
                    IconeNotificacao.ShowBalloonTip(1000, "LCDHM", "Erro ao tentar se conectar a " + IP + ":" + TCP_PORTA, ToolTipIcon.Error);
                }
            } catch (Exception ex) {
                IconeNotificacao.ShowBalloonTip(1000, "LCDHM", ex.Message, ToolTipIcon.Error);
            }
        }

        private void DesconectarTCP() {
            if (Cliente.Connected) {
                Enviar("page 0");
                timer.Stop();
                menu_Conectar.Visible = true;
                menu_Desconectar.Visible = false;
                if (HM != null) HM.Disconnect();
                if (CM != null) CM.Disconnect();
                Cliente.Close();
                IconeNotificacao.ShowBalloonTip(1000, "LCDHM", "Desconectado", ToolTipIcon.Info);
            }
        }
        private void ConectarMSI() {
            Enviar("j0", 10);
            Enviar("t", "Inicializando MSI");
            while (Process.GetProcessesByName("MSIAfterburner").Length == 0) {
                try {
                    Process.Start(Properties.Settings.Default.MSI_Directory);
                } catch (Exception) {
                    Mostrar_Configuracoes();
                }
            }
            Enviar("j0", 25);
            Enviar("t", "Criando Conectividade");
            while (true) {
                try {
                    HM = new HardwareMonitor();
                    CM = new ControlMemory();
                    break;
                } catch (Exception) {
                    Enviar("j0", 30);
                    Thread.Sleep(500);
                }
            }

            Enviar("j0", 35);
            Enviar("t", "Conectando ao MSI");
            while (true) {
                try {
                    HM.Connect();
                    CM.Connect();
                    break;
                } catch (Exception) {
                    Enviar("j0", 40);
                    Thread.Sleep(500);
                }
            }

            Enviar("j0", 50);
            Enviar("t", "Definindo Constantes");
            CPU_THREADS = Environment.ProcessorCount;
            CPU_CORES = 0;
            System.Management.ManagementObjectSearcher managementObjectSearcher = new System.Management.ManagementObjectSearcher("Select * from Win32_Processor");
            foreach (System.Management.ManagementBaseObject item in managementObjectSearcher.Get()) this.CPU_CORES += int.Parse(item["NumberOfCores"].ToString());
            managementObjectSearcher.Dispose();
            Enviar("j0", 60);
            RAM_TOTAL = Convert.ToInt32(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024));

            Enviar("j0", 70);
            Enviar("t", "Definindo Clocks");
            FAN_BOOST = int.Parse(CM.GpuEntries[0].FanSpeedCur.ToString("N0"));

            Enviar("j0", 100);
            Enviar("t", "Conectado");
            Thread.Sleep(600);
            Enviar("page Principal");
        }
        private HardwareMonitorEntry GetEntidade(String nome) {
            foreach (HardwareMonitorEntry e in HM.Entries) {
                if (e.SrcName.Equals(nome)) {
                    HM.ReloadEntry(e);
                    return e;
                }
            }
            return new HardwareMonitorEntry();
        }

        private void AtualizarClocks() {
            CM.ReloadAll();
            Enviar("Overclock.tcore", (GetEntidade("Core clock").Data + CM.GpuEntries[0].CoreClockBoostCur / 1000).ToString("N0").Replace(".", "") + " (" + (CORE_BOOST >= 0 ? "+" : "") + CORE_BOOST + ")");
            Enviar("Overclock.tmem", (GetEntidade("Memory clock").Data + CM.GpuEntries[0].MemoryClockBoostCur / 1000).ToString("N0").Replace(".", "") + " (" + (MEM_BOOST >= 0 ? "+" : "") + MEM_BOOST + ")");
            Enviar("Overclock.tfan", FAN_FLAG == MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO ? "AUTO" : FAN_BOOST.ToString());
        }

        
        private void Menu_Conectar_Click(object sender, EventArgs e) {
            MenuContexto.Hide();
            Properties.Settings.Default.Reload();
            String IP = Properties.Settings.Default.IP_Favorito;
            IPAddress RealIP;
            if (IPAddress.TryParse(IP, out RealIP)) ConectarTCP(IP);

        }
        private void Menu_IP_EnterClick(object sender, KeyEventArgs e) {
            IPAddress RealIP;
            if (e.KeyCode == Keys.Enter && IPAddress.TryParse(Menu_IP.Text, out RealIP)) {
                MenuContexto.Hide();
                ConectarTCP(Menu_IP.Text);

            }

        }
        private void Menu_IP_TextChanged(object sender, EventArgs e) {
            if (!Menu_IP.Text.StartsWith(IPLocal.Substring(0, IPLocal.Length - 3))) {
                Menu_IP.Text = IPLocal.Substring(0, IPLocal.Length - 3);
            }
            String IP = Menu_IP.Text;
            IPAddress RealIP;
            if (IPAddress.TryParse(IP, out RealIP)) {
                Menu_IP.ForeColor = Color.FromArgb(255, 0, 184, 192);
            } else {
                Menu_IP.ForeColor = Color.FromArgb(255, 100, 0, 0);
            }
        }
        private void Menu_Desconectar_Click(object sender, EventArgs e) => DesconectarTCP();
        private void Menu_Configurar_Click(object sender, EventArgs e) => Mostrar_Configuracoes();
        private void Menu_Sobre_Click(object sender, EventArgs e) => new SobreForm().Show();
        private void Menu_Sair_Click(object sender, EventArgs e) {
            DesconectarTCP();
            Application.Exit();
        }

        private void Mostrar_Configuracoes() {
            Enabled = true;
            CenterToScreen();
            this.Show();
            this.Opacity = 100;
            this.ShowInTaskbar = true;
        }
        private void Ocultar_Configuracoes() {
            this.Location = new Point(-10000, -10000);            
            Hide();
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            Enabled = false;
        }
        private void BT_Buscar_Steam_Click(object sender, EventArgs e) {
            if (FileDialog.ShowDialog().ToString() == "OK") Text_Steam_Diretorio.Text = FileDialog.FileName;
        }
        private void BT_Aplicar_Click(object sender, EventArgs e) {
            if (Text_MSI_Diretorio.Text.EndsWith("MSIAfterburner.exe") && Text_Steam_Diretorio.Text.EndsWith("Steam.exe")) {
                Properties.Settings.Default.MSI_Directory = Text_MSI_Diretorio.Text;
                Properties.Settings.Default.Steam_Directory = Text_Steam_Diretorio.Text;
                Properties.Settings.Default.Save();
                Ocultar_Configuracoes();
            } else {
                IconeNotificacao.ShowBalloonTip(1000, "Endereço Inválido", "Caminho do programa da Steam ou do Afterburner inválido.", ToolTipIcon.Error);
            }
        }
        private void BT_Buscar_MSI_Click(object sender, EventArgs e) {
            if (FileDialog.ShowDialog().ToString() == "OK") Text_MSI_Diretorio.Text = FileDialog.FileName;
        }

        public void Enviar(String Comando) => TCPPrintLine(Comando);
        public void Enviar(String Variavel, String Texto) => TCPPrintLine(Variavel + ".txt=\"" + Texto + "\"");
        public void Enviar(String Variavel, int Valor) => TCPPrintLine(Variavel + ".val=" + Valor);
        public void Enviar(String Variavel, int Chanel, int Valor, int In_min, int In_max, int Out_min, int Out_max) => TCPPrintLine("add " + Variavel + ".id" + "," + Chanel.ToString() + "," + ((Valor - In_min) * (Out_max - Out_min) / (In_max - In_min) + Out_min).ToString("N0"));
        public void Enviar(String Variavel, Color c) => TCPPrintLine(Variavel + ".pco=" + (((c.R >> 3) << 11) + ((c.G >> 2) << 5) + (c.B >> 3)).ToString("N0").Replace(".", ""));
    }
}

//TODO: Mudar o enviar Page 0 para outra page, jaque esta será a de conexao com roteador
