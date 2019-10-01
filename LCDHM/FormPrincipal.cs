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
        }
        
        private void TCP_Listener_Tick(object sender, EventArgs e) {
            if (Cliente.Connected && Cliente.GetStream().DataAvailable) {
                String entrada = TCP_ReadLine();
                if (entrada.Length > 1) {
                    Console.WriteLine(entrada);
                    if (entrada.Contains("init")) MSI_Conectar();
                    if (entrada.Contains("MENU") || entrada.Contains("p0")) MudarPagina(0);
                    if (entrada.Contains("p1")) MudarPagina(1);
                    if (entrada.Contains("p2")) { MSI_AtualizarClock(); MudarPagina(2); }
                    if (entrada.Contains("p3")) MudarPagina(3);
                    if (entrada.Contains("p4")) MudarPagina(4);
                    if (entrada.Contains("p5")) MudarPagina(5);
                    if (entrada.Contains("p6")) MudarPagina(6);
                    if (entrada.Contains("p7")) { ANALISE_LINHA = 0; MudarPagina(7); }
                    if (entrada.Contains("analise_gravar") && ANALISE_LINHA < 7) ANALISE_LINHA++;
                    if (entrada.Contains("analise_limpar")) ANALISE_LINHA = 0;
                    if (entrada.Contains("analise_auto")) ANALISE_MIN_FPS = Convert.ToInt32(entrada.Replace("analise_auto", ""));
                    if (entrada.Contains("fps_reset")) { FPS_MAX = 0; FPS_MIN = 0; TCP_Enviar("GPU.t6", "-"); TCP_Enviar("GPU.t7", "-"); }
                    if (entrada.Contains("core_min") && CORE_BOOST > -200) { CORE_BOOST -= CLOCK_SENSIBILIDADE; MSI_AtualizarClock(); TCP_Enviar("Overclock.tcore", Color.Red); }
                    if (entrada.Contains("core_max") && CORE_BOOST < 200) { CORE_BOOST += CLOCK_SENSIBILIDADE; MSI_AtualizarClock(); TCP_Enviar("Overclock.tcore", Color.Red); }
                    if (entrada.Contains("core_rst")) { CORE_BOOST = 0; MSI_AtualizarClock(); TCP_Enviar("Overclock.tcore", Color.Red); }
                    if (entrada.Contains("mem_min") && MEM_BOOST > -200) { MEM_BOOST -= CLOCK_SENSIBILIDADE; MSI_AtualizarClock(); TCP_Enviar("Overclock.tmem", Color.Red); }
                    if (entrada.Contains("mem_max") && MEM_BOOST < 200) { MEM_BOOST += CLOCK_SENSIBILIDADE; MSI_AtualizarClock(); TCP_Enviar("Overclock.tmem", Color.Red); }
                    if (entrada.Contains("mem_rst")) { MEM_BOOST = 0; MSI_AtualizarClock(); TCP_Enviar("Overclock.tmem", Color.Red); }
                    if (entrada.Contains("fan_min") && FAN_BOOST > 31 + FAN_SENSIBILIDADE) { FAN_BOOST -= FAN_SENSIBILIDADE; FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None; MSI_AtualizarClock(); TCP_Enviar("Overclock.tfan", Color.Red); }
                    if (entrada.Contains("fan_max") && FAN_BOOST < 100 - FAN_SENSIBILIDADE) { FAN_BOOST += FAN_SENSIBILIDADE; FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None; MSI_AtualizarClock(); TCP_Enviar("Overclock.tfan", Color.Red); }
                    if (entrada.Contains("fan_auto")) { FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO; FAN_BOOST = 30; MSI_AtualizarClock(); TCP_Enviar("Overclock.tfan", Color.Red); }
                    if (entrada.Contains("oc_aplicar")) {
                        CM.GpuEntries[0].CoreClockBoostCur = CORE_BOOST * 1000;
                        CM.GpuEntries[0].MemoryClockBoostCur = MEM_BOOST * 1000;
                        CM.GpuEntries[0].FanFlagsCur = FAN_FLAG;
                        if (FAN_FLAG == MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None) CM.GpuEntries[0].FanSpeedCur = uint.Parse(FAN_BOOST.ToString());
                        TCP_Enviar("Overclock.tcore", Color.FromArgb(255, 0, 184, 192));
                        TCP_Enviar("Overclock.tmem", Color.FromArgb(255, 0, 184, 192));
                        TCP_Enviar("Overclock.tfan", Color.FromArgb(255, 0, 184, 192));
                        CM.CommitChanges();
                        MSI_AtualizarClock();
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
        }
        private void MudarPagina(int PG_Index) {
            PAGINA = PG_Index;
            TCP_EnviarAutomatico();
        }        
        private void TCP_EnviarAutomatico() {
            int fps;
            switch (PAGINA) {
                default:
                    break;
                case 1:
                    fps = int.Parse(GetMSIEntidade("Framerate").Data.ToString("N0"));
                    if (fps == 0) {
                        TCP_Enviar("GPU.t5", "-");
                        TCP_Enviar("GPU.t6", "-");
                        TCP_Enviar("GPU.t7", "-");
                        FPS_MAX = 0;
                        FPS_MIN = 0;
                    } else {
                        if (FPS_MIN == 0) FPS_MIN = fps; else if (fps < FPS_MIN) FPS_MIN = fps;
                        if (FPS_MAX == 0) FPS_MAX = fps; else if (fps > FPS_MAX) FPS_MAX = fps;
                        TCP_Enviar("GPU.t5", fps.ToString());
                        TCP_Enviar("GPU.t6", FPS_MIN.ToString());
                        TCP_Enviar("GPU.t7", FPS_MAX.ToString());
                    }
                    TCP_Enviar("GPU.t0", GetMSIEntidade("GPU usage").Data.ToString());
                    TCP_Enviar("GPU.t1", GetMSIEntidade("Core clock").Data.ToString());
                    TCP_Enviar("GPU.t2", GetMSIEntidade("Memory clock").Data.ToString());
                    TCP_Enviar("GPU.t3", GetMSIEntidade("Memory usage").Data.ToString("N0"));
                    TCP_Enviar("GPU.t4", GetMSIEntidade("GPU temperature").Data.ToString());
                    TCP_Enviar("GPU.t8", GetMSIEntidade("Fan tachometer").Data.ToString());
                    TCP_Enviar("GPU.t9", GetMSIEntidade("Power").Data.ToString());
                    break;
                case 2:
                    fps = int.Parse(GetMSIEntidade("Framerate").Data.ToString("N0"));
                    TCP_Enviar("gGPU", 0, fps, 0, 100, 0, 100);
                    TCP_Enviar("gGPU", 1, int.Parse(GetMSIEntidade("GPU temperature").Data.ToString("N0")), 0, 100, 0, 100);
                    TCP_Enviar("Overclock.t11", GetMSIEntidade("GPU temperature").Data.ToString() + " C");
                    TCP_Enviar("Overclock.t10", fps.ToString() + " FPS");
                    TCP_Enviar("gGPU", 2, 60, 0, 100, 0, 100);
                    break;
                case 3:
                    TCP_Enviar("CPU.t12", GetMSIEntidade("CPU usage").Data.ToString("N0"));
                    TCP_Enviar("CPU.t13", GetMSIEntidade("CPU fan speed").Data.ToString());
                    TCP_Enviar("CPU.t14", GetMSIEntidade("CPU temperature").Data.ToString());
                    TCP_Enviar("CPU.t0", GetMSIEntidade("CPU clock").Data.ToString("N0"));
                    TCP_Enviar("CPU.t15", CPU_CORES.ToString());
                    TCP_Enviar("CPU.t16", CPU_THREADS.ToString());
                    String cpus = "|";
                    for (int i = 1; i <= CPU_THREADS; i++) {
                        String porcentagem = GetMSIEntidade("CPU" + i.ToString() + " usage").Data.ToString("N0");
                        switch (porcentagem.Length) {
                            case 1: porcentagem = porcentagem.Equals("0") ? "   " : " " + porcentagem + "%"; break;
                            case 2: porcentagem += "%"; break;
                            case 3: porcentagem = "MAX"; break;
                        }
                        cpus += " " + porcentagem + " |";
                    }
                    TCP_Enviar("CPU.t17", cpus);
                    TCP_Enviar("gCPU", 0, int.Parse(GetMSIEntidade("CPU usage").Data.ToString("N0")), 0, 100, 0, 81);
                    TCP_Enviar("gCPU", 1, int.Parse(GetMSIEntidade("CPU temperature").Data.ToString("N0")), 0, 100, 0, 81);
                    break;
                case 4:
                    float RAM_Uso = GetMSIEntidade("RAM usage").Data;
                    float RAM_Porcentagem = (RAM_Uso / RAM_TOTAL) * 100;
                    TCP_Enviar("MEM.t18", RAM_Porcentagem.ToString("N1"));
                    TCP_Enviar("MEM.j1", int.Parse(RAM_Porcentagem.ToString("N0")));
                    TCP_Enviar("MEM.t19", RAM_Uso.ToString("N0"));
                    TCP_Enviar("MEM.t20", GetMSIEntidade("HDD" + HDD_INDEX.ToString() + " read rate").Data.ToString("N3"));
                    TCP_Enviar("MEM.t21", GetMSIEntidade("HDD" + HDD_INDEX.ToString() + " write rate").Data.ToString("N3"));
                    TCP_Enviar("MEM.t22", GetMSIEntidade("HDD" + HDD_INDEX.ToString() + " temperature").Data.ToString("N1"));
                    TCP_Enviar("MEM.t0", GetMSIEntidade("HDD" + HDD_INDEX.ToString() + " usage").Data.ToString("N1"));
                    TCP_Enviar("MEM.t23", HDD_INDEX.ToString());
                    TCP_Enviar("MEM.t25", GetMSIEntidade("NET" + NET_INDEX.ToString() + " download rate").Data.ToString("N3"));
                    TCP_Enviar("MEM.t26", GetMSIEntidade("NET" + NET_INDEX.ToString() + " download rate").SrcUnits.ToString());
                    TCP_Enviar("MEM.t27", GetMSIEntidade("NET" + NET_INDEX.ToString() + " upload rate").Data.ToString("N3"));
                    TCP_Enviar("MEM.t28", GetMSIEntidade("NET" + NET_INDEX.ToString() + " upload rate").SrcUnits.ToString());
                    TCP_Enviar("MEM.t29", NET_INDEX.ToString());
                    TCP_Enviar("gMEM", 0, int.Parse(RAM_Porcentagem.ToString("N0")), 0, 100, 0, 81);
                    break;
                case 5:
                    float RAM_Uso2 = GetMSIEntidade("RAM usage").Data;
                    float RAM_Porcentagem2 = (RAM_Uso2 / RAM_TOTAL) * 100;
                    TCP_Enviar("GRAFICO.t30", GetMSIEntidade("GPU usage").Data.ToString("N0") + " %");
                    TCP_Enviar("GRAFICO.t31", GetMSIEntidade("CPU usage").Data.ToString("N0") + " %");
                    TCP_Enviar("GRAFICO.t32", GetMSIEntidade("framerate").Data.ToString("N0") + " fps");
                    TCP_Enviar("s0", 0, int.Parse(GetMSIEntidade("GPU usage").Data.ToString("N0")), 0, 100, 0, 71);
                    TCP_Enviar("s0", 1, int.Parse(GetMSIEntidade("CPU usage").Data.ToString("N0")), 0, 100, 0, 71);
                    TCP_Enviar("s0", 2, int.Parse(GetMSIEntidade("framerate").Data.ToString("N0")), 0, 71, 0, 71);
                    TCP_Enviar("GRAFICO.t33", GetMSIEntidade("Memory usage").Data.ToString("N0") + " MB");
                    TCP_Enviar("GRAFICO.t34", GetMSIEntidade("GPU temperature").Data.ToString() + " C");
                    //TODO: Obter MB da GPU
                    TCP_Enviar("s1", 0, int.Parse(((GetMSIEntidade("Memory usage").Data / 4096) * 100).ToString("N0")), 0, 100, 0, 51);
                    TCP_Enviar("GRAFICO.t35", GetMSIEntidade("CPU clock").Data.ToString("N0") + " MHz");
                    TCP_Enviar("GRAFICO.t36", GetMSIEntidade("CPU temperature").Data.ToString() + " C");
                    //TODO: Obter Clock da CPU
                    TCP_Enviar("s2", 0, int.Parse(((GetMSIEntidade("CPU clock").Data / 3900) * 100).ToString("N0")), 0, 100, 0, 51);
                    TCP_Enviar("GRAFICO.t37", RAM_Uso2.ToString("N0") + " MB");
                    TCP_Enviar("GRAFICO.t38", (RAM_TOTAL - RAM_Uso2).ToString("N0") + " MB");
                    TCP_Enviar("s3", 0, int.Parse(RAM_Porcentagem2.ToString("N0")), 0, 100, 0, 51);
                    break;
                case 7:
                    fps = Convert.ToInt32(GetMSIEntidade("Framerate").Data);
                    TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "0", fps.ToString("N0"));
                    //if (fps < ANALISE_MIN_FPS && fps != 0) Enviar("t" + ANALISE_LINHA + "0", Color.Red); else Enviar("t" + ANALISE_LINHA + "0", Color.FromArgb(255, 0, 184, 192));
                    TCP_Enviar("t" + ANALISE_LINHA + "0", fps < ANALISE_MIN_FPS && fps != 0 ? Color.Red : Color.FromArgb(255, 0, 184, 192));
                    TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "1", GetMSIEntidade("GPU usage").Data.ToString("N0"));
                    TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "2", GetMSIEntidade("Memory usage").Data.ToString("N0").Replace(".", ""));
                    TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "3", GetMSIEntidade("GPU Temperature").Data.ToString("N0"));
                    TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "4", GetMSIEntidade("CPU usage").Data.ToString("N0"));
                    TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "5", GetMSIEntidade("CPU temperature").Data.ToString("N0"));
                    TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "6", GetMSIEntidade("RAM usage").Data.ToString("N0").Replace(".", ""));
                    TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "7", HDD_INDEX + ":" + GetMSIEntidade("HDD" + HDD_INDEX.ToString() + " usage").Data.ToString("N0"));
                    if (fps < ANALISE_MIN_FPS && ANALISE_LINHA < 7 && fps != 0) ANALISE_LINHA++;
                    break;

                case 6:
                    TCP_Enviar("PSU.t0", GetMSIEntidade("Power").Data.ToString());
                    TCP_Enviar("PSU.t1", GetMSIEntidade("CPU voltage").Data.ToString());

                    TCP_Enviar("PSU.t2", GetMSIEntidade("PSU + 3.3V voltage").Data.ToString());
                    TCP_Enviar("PSU.t4", GetMSIEntidade("PSU + 5V voltage").Data.ToString());
                    TCP_Enviar("PSU.t6", GetMSIEntidade("PSU + 12V voltage").Data.ToString());



                    break;
            }
        }

        private void TCP_WriteLine(String MensagemTCP) {
            if (Cliente.Connected) {
                if (!MensagemTCP.EndsWith("\n")) MensagemTCP += '\n';
                Byte[] MensagemTCPByte = Encoding.ASCII.GetBytes(MensagemTCP);
                Cliente.GetStream().Write(MensagemTCPByte, 0, MensagemTCPByte.Length);
            }
        }
        private String TCP_ReadLine() {
            String LinhaLida = "";
            while (Cliente.Connected && Cliente.GetStream().DataAvailable) {
                int b = Cliente.GetStream().ReadByte();
                if ((char)b == '\n' || (char)b == 13) break;
                LinhaLida += (char)b;
            }
            return LinhaLida;
        }
        private void TCP_Conectar(String IP) {
            try {
                Cliente = new TcpClient();
                if (Cliente.ConnectAsync(IP, TCP_PORTA).Wait(2000)) {
                    menu_Conectar.Visible = false;
                    menu_Desconectar.Visible = true;
                    Properties.Settings.Default.IP_Favorito = IP;
                    Properties.Settings.Default.Save();
                    IconeNotificacao.ShowBalloonTip(1000, "LCDHM", "Conectado", ToolTipIcon.Info);
                    TCP_Listener.Start();
                    timer.Start();
                } else {
                    IconeNotificacao.ShowBalloonTip(1000, "LCDHM", "Erro ao tentar se conectar a " + IP + ":" + TCP_PORTA, ToolTipIcon.Error);
                    AtualizarIPLocal();
                }
            } catch (Exception ex) {
                IconeNotificacao.ShowBalloonTip(1000, "LCDHM", ex.Message, ToolTipIcon.Error);
                AtualizarIPLocal();
            }
        }
        private void TCP_Desconectar() {
            if (Cliente.Connected) {
                timer.Stop();
                TCP_Listener.Stop();
                menu_Conectar.Visible = true;
                menu_Desconectar.Visible = false;
                if (HM != null) HM.Disconnect();
                if (CM != null) CM.Disconnect();
                Cliente.Close();
                Cliente.Dispose();
                IconeNotificacao.ShowBalloonTip(1000, "LCDHM", "Desconectado", ToolTipIcon.Info);
            }
        }

        private void MSI_Conectar() {
            TCP_Enviar("j0", 10);
            TCP_Enviar("t", "Inicializando MSI");
            while (Process.GetProcessesByName("MSIAfterburner").Length == 0) {
                try {
                    Process.Start(Properties.Settings.Default.MSI_Directory);
                } catch (Exception) {
                    Mostrar_Configuracoes();
                }
            }
            TCP_Enviar("j0", 25);
            TCP_Enviar("t", "Criando Conectividade");
            while (true) {
                try {
                    HM = new HardwareMonitor();
                    CM = new ControlMemory();
                    break;
                } catch (Exception) {
                    TCP_Enviar("j0", 30);
                    Thread.Sleep(500);
                }
            }

            TCP_Enviar("j0", 35);
            TCP_Enviar("t", "Conectando ao MSI");
            while (true) {
                try {
                    HM.Connect();
                    CM.Connect();
                    break;
                } catch (Exception) {
                    TCP_Enviar("j0", 40);
                    Thread.Sleep(500);
                }
            }

            TCP_Enviar("j0", 50);
            TCP_Enviar("t", "Definindo Constantes");
            CPU_THREADS = Environment.ProcessorCount;
            CPU_CORES = 0;
            System.Management.ManagementObjectSearcher managementObjectSearcher = new System.Management.ManagementObjectSearcher("Select * from Win32_Processor");
            foreach (System.Management.ManagementBaseObject item in managementObjectSearcher.Get()) this.CPU_CORES += int.Parse(item["NumberOfCores"].ToString());
            managementObjectSearcher.Dispose();
            TCP_Enviar("j0", 60);
            RAM_TOTAL = Convert.ToInt32(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024));

            TCP_Enviar("j0", 70);
            TCP_Enviar("t", "Definindo Clocks");
            FAN_BOOST = int.Parse(CM.GpuEntries[0].FanSpeedCur.ToString("N0"));

            TCP_Enviar("j0", 100);
            TCP_Enviar("t", "Conectado");
            Thread.Sleep(600);
            TCP_Enviar("page Principal");
        }
        private void MSI_EnviarTimer(object sender, EventArgs e) => TCP_EnviarAutomatico();
        private void MSI_AtualizarClock() {
            CM.ReloadAll();
            TCP_Enviar("Overclock.tcore", (GetMSIEntidade("Core clock").Data + CM.GpuEntries[0].CoreClockBoostCur / 1000).ToString("N0").Replace(".", "") + " (" + (CORE_BOOST >= 0 ? "+" : "") + CORE_BOOST + ")");
            TCP_Enviar("Overclock.tmem", (GetMSIEntidade("Memory clock").Data + CM.GpuEntries[0].MemoryClockBoostCur / 1000).ToString("N0").Replace(".", "") + " (" + (MEM_BOOST >= 0 ? "+" : "") + MEM_BOOST + ")");
            TCP_Enviar("Overclock.tfan", FAN_FLAG == MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO ? "AUTO" : FAN_BOOST.ToString());
        }
        private HardwareMonitorEntry GetMSIEntidade(String nome) {
            foreach (HardwareMonitorEntry e in HM.Entries) {
                if (e.SrcName.Equals(nome)) {
                    HM.ReloadEntry(e);
                    return e;
                }
            }
            return new HardwareMonitorEntry();
        }

        private void Menu_Conectar_Click(object sender, EventArgs e) {
            MenuContexto.Hide();
            Properties.Settings.Default.Reload();
            String IP = Properties.Settings.Default.IP_Favorito;
            if (IPAddress.TryParse(IP, out _)) TCP_Conectar(IP);

        }
        private void Menu_IP_EnterClick(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && IPAddress.TryParse(Menu_IP.Text, out _)) {
                MenuContexto.Hide();
                TCP_Conectar(Menu_IP.Text);

            }

        }
        private void Menu_IP_TextChanged(object sender, EventArgs e) {
            if (!Menu_IP.Text.StartsWith(IPLocal.Substring(0, IPLocal.Length - 3))) {
                Menu_IP.Text = IPLocal.Substring(0, IPLocal.Length - 3);
            }
            String IP = Menu_IP.Text;
            if (IPAddress.TryParse(IP, out _)) {
                Menu_IP.ForeColor = Color.FromArgb(255, 0, 184, 192);
            } else {
                Menu_IP.ForeColor = Color.FromArgb(255, 100, 0, 0);
            }
        }
        private void Menu_Desconectar_Click(object sender, EventArgs e) => TCP_Desconectar();
        private void Menu_Configurar_Click(object sender, EventArgs e) => Mostrar_Configuracoes();
        private void Menu_Sobre_Click(object sender, EventArgs e) => new SobreForm().Show();
        private void Menu_Sair_Click(object sender, EventArgs e) {
            TCP_Desconectar();
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
            FileDialog.FileName = "Steam.exe";
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
            FileDialog.FileName = "MSIAfterburner.exe";
            if (FileDialog.ShowDialog().ToString() == "OK") Text_MSI_Diretorio.Text = FileDialog.FileName;
        }

        public void TCP_Enviar(String Comando) => TCP_WriteLine(Comando);
        public void TCP_Enviar(String Variavel, String Texto) => TCP_WriteLine(Variavel + ".txt=\"" + Texto + "\"");
        public void TCP_Enviar(String Variavel, int Valor) => TCP_WriteLine(Variavel + ".val=" + Valor);
        public void TCP_Enviar(String Variavel, int Chanel, int Valor, int In_min, int In_max, int Out_min, int Out_max) => TCP_WriteLine("add " + Variavel + ".id" + "," + Chanel.ToString() + "," + ((Valor - In_min) * (Out_max - Out_min) / (In_max - In_min) + Out_min).ToString("N0"));
        public void TCP_Enviar(String Variavel, Color c) => TCP_WriteLine(Variavel + ".pco=" + (((c.R >> 3) << 11) + ((c.G >> 2) << 5) + (c.B >> 3)).ToString("N0").Replace(".", ""));
    }
}

