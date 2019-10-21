using MSI.Afterburner;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace LCDHM {

    public partial class FormPrincipal : Form {
        private float
                CPU_CLOCK = 0;
        private uint
                GPU_VRAM = 0,
                PAGINA = 0,
                ANALISE_LINHA = 0,
                ANALISE_MIN_FPS = 60,
                FPS_MIN = 0,
                FPS_MAX = 0,
                CPU_CORES = 0,
                CPU_THREADS = 0,
                RAM_TOTAL = 0,
                HDD_INDEX = 1,
                NET_INDEX = 1;

        private int
                CORE_BOOST = 0,
                MEM_BOOST = 0,
                FAN_BOOST = 30;

        private readonly int
                TCP_PORTA = 1199,
                FAN_SENSIBILIDADE = 5,
                CLOCK_SENSIBILIDADE = 25;

        private MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG
                FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO;

        private MSIAfterburner
                MSI;

        private TCPESP32
                Cliente;

        public FormPrincipal() {
            InitializeComponent();
            this.MenuIPs.Renderer = new ToolStripProfessionalRenderer(new TemaDarkRendererToolStrip());
            this.MenuContexto.Renderer = new ToolStripProfessionalRenderer(new TemaDarkRendererToolStrip());
        }

        private void FormPrincipalLoad(object sender, EventArgs e) {
            Ocultar_Configuracoes();
            Properties.Settings.Default.Reload();
            Text_MSI_Diretorio.Text = Properties.Settings.Default.MSI_Directory;
            Text_Steam_Diretorio.Text = Properties.Settings.Default.Steam_Directory;
            Menu_IP.Text = Properties.Settings.Default.IP_Favorito;
            if (!(Text_MSI_Diretorio.Text.EndsWith("MSIAfterburner.exe") && Text_Steam_Diretorio.Text.EndsWith("Steam.exe"))) Mostrar_Configuracoes();
            IconeNotificacao.Text = "LCDHM - Desconectado";
        }

        private void TCP_Listener_Tick(object sender, EventArgs e) {
            if (Cliente.Connected && Cliente.GetStream().DataAvailable) {
                String entrada = Cliente.TCP_ReadLine();
                if (entrada.Length > 1) {
                    if (entrada.Contains("init")) MSI_Conectar();
                    if (entrada.Contains("desconectar")) TCP_Desconectar();
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
                    if (entrada.Contains("analise_auto")) ANALISE_MIN_FPS = Convert.ToUInt32(entrada.Replace("analise_auto", ""));
                    if (entrada.Contains("fps_reset")) { FPS_MAX = 0; FPS_MIN = 0; Cliente.TCP_Enviar("GPU.t6", "-"); Cliente.TCP_Enviar("GPU.t7", "-"); }
                    if (entrada.Contains("core_min") && CORE_BOOST > -200) { CORE_BOOST -= CLOCK_SENSIBILIDADE; MSI_AtualizarClock(); Cliente.TCP_Enviar("Overclock.tcore", Color.Red); }
                    if (entrada.Contains("core_max") && CORE_BOOST < 200) { CORE_BOOST += CLOCK_SENSIBILIDADE; MSI_AtualizarClock(); Cliente.TCP_Enviar("Overclock.tcore", Color.Red); }
                    if (entrada.Contains("core_rst")) { CORE_BOOST = 0; MSI_AtualizarClock(); Cliente.TCP_Enviar("Overclock.tcore", Color.Red); }
                    if (entrada.Contains("mem_min") && MEM_BOOST > -200) { MEM_BOOST -= CLOCK_SENSIBILIDADE; MSI_AtualizarClock(); Cliente.TCP_Enviar("Overclock.tmem", Color.Red); }
                    if (entrada.Contains("mem_max") && MEM_BOOST < 200) { MEM_BOOST += CLOCK_SENSIBILIDADE; MSI_AtualizarClock(); Cliente.TCP_Enviar("Overclock.tmem", Color.Red); }
                    if (entrada.Contains("mem_rst")) { MEM_BOOST = 0; MSI_AtualizarClock(); Cliente.TCP_Enviar("Overclock.tmem", Color.Red); }
                    if (entrada.Contains("fan_min") && FAN_BOOST > 31 + FAN_SENSIBILIDADE) { FAN_BOOST -= FAN_SENSIBILIDADE; FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None; MSI_AtualizarClock(); Cliente.TCP_Enviar("Overclock.tfan", Color.Red); }
                    if (entrada.Contains("fan_max") && FAN_BOOST < 100 - FAN_SENSIBILIDADE) { FAN_BOOST += FAN_SENSIBILIDADE; FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None; MSI_AtualizarClock(); Cliente.TCP_Enviar("Overclock.tfan", Color.Red); }
                    if (entrada.Contains("fan_auto")) { FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO; FAN_BOOST = 30; MSI_AtualizarClock(); Cliente.TCP_Enviar("Overclock.tfan", Color.Red); }
                    if (entrada.Contains("oc_aplicar")) {
                        MSI.GetGPUEntidade(0).CoreClockBoostCur = CORE_BOOST * 1000;
                        MSI.GetGPUEntidade(0).MemoryClockBoostCur = MEM_BOOST * 1000;
                        MSI.GetGPUEntidade(0).FanFlagsCur = FAN_FLAG;
                        if (FAN_FLAG == MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None) MSI.GetGPUEntidade(0).FanSpeedCur = uint.Parse(FAN_BOOST.ToString());
                        Cliente.TCP_Enviar("Overclock.tcore", Color.FromArgb(255, 0, 184, 192));
                        Cliente.TCP_Enviar("Overclock.tmem", Color.FromArgb(255, 0, 184, 192));
                        Cliente.TCP_Enviar("Overclock.tfan", Color.FromArgb(255, 0, 184, 192));
                        MSI.OverclockAplicar();
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

        private void MudarPagina(uint PG_Index) {
            PAGINA = PG_Index;
            TCP_EnviarAutomatico();
        }

        private void TCP_EnviarAutomatico() {
            uint fps;
            switch (PAGINA) {
                default:
                    break;

                case 1:
                    fps = uint.Parse(MSI.GetMSIEntidade("Framerate").Data.ToString("N0"));
                    if (fps == 0) {
                        Cliente.TCP_Enviar("GPU.t5", "-");
                        Cliente.TCP_Enviar("GPU.t6", "-");
                        Cliente.TCP_Enviar("GPU.t7", "-");
                        FPS_MAX = 0;
                        FPS_MIN = 0;
                    } else {
                        if (FPS_MIN == 0) FPS_MIN = fps; else if (fps < FPS_MIN) FPS_MIN = fps;
                        if (FPS_MAX == 0) FPS_MAX = fps; else if (fps > FPS_MAX) FPS_MAX = fps;
                        Cliente.TCP_Enviar("GPU.t5", fps.ToString());
                        Cliente.TCP_Enviar("GPU.t6", FPS_MIN.ToString());
                        Cliente.TCP_Enviar("GPU.t7", FPS_MAX.ToString());
                    }
                    Cliente.TCP_Enviar("GPU.t0", MSI.GetMSIEntidade("GPU usage").Data.ToString());
                    Cliente.TCP_Enviar("GPU.t1", MSI.GetMSIEntidade("Core clock").Data.ToString());
                    Cliente.TCP_Enviar("GPU.t2", MSI.GetMSIEntidade("Memory clock").Data.ToString());
                    Cliente.TCP_Enviar("GPU.t3", MSI.GetMSIEntidade("Memory usage").Data.ToString("N0"));
                    Cliente.TCP_Enviar("GPU.t4", MSI.GetMSIEntidade("GPU temperature").Data.ToString());
                    Cliente.TCP_Enviar("GPU.t8", MSI.GetMSIEntidade("Fan tachometer").Data.ToString());
                    Cliente.TCP_Enviar("GPU.t9", MSI.GetMSIEntidade("Power").Data.ToString());
                    break;

                case 2:
                    fps = uint.Parse(MSI.GetMSIEntidade("Framerate").Data.ToString("N0"));
                    Cliente.TCP_Enviar("gGPU", 0, (int)fps, 0, 100, 0, 100);
                    Cliente.TCP_Enviar("gGPU", 1, int.Parse(MSI.GetMSIEntidade("GPU temperature").Data.ToString("N0")), 0, 100, 0, 100);
                    Cliente.TCP_Enviar("Overclock.t11", MSI.GetMSIEntidade("GPU temperature").Data.ToString() + " C");
                    Cliente.TCP_Enviar("Overclock.t10", fps.ToString() + " FPS");
                    Cliente.TCP_Enviar("gGPU", 2, 60, 0, 100, 0, 100);
                    break;

                case 3:
                    Cliente.TCP_Enviar("CPU.t12", MSI.GetMSIEntidade("CPU usage").Data.ToString("N0"));
                    Cliente.TCP_Enviar("CPU.t13", MSI.GetMSIEntidade("CPU fan speed").Data.ToString());
                    Cliente.TCP_Enviar("CPU.t14", MSI.GetMSIEntidade("CPU temperature").Data.ToString());
                    Cliente.TCP_Enviar("CPU.t0", MSI.GetMSIEntidade("CPU clock").Data.ToString("N0"));
                    Console.WriteLine(MSI.GetMSIEntidade("CPU clock").Data);
                    Cliente.TCP_Enviar("CPU.t15", CPU_CORES.ToString());
                    Cliente.TCP_Enviar("CPU.t16", CPU_THREADS.ToString());
                    String cpus = "|";
                    for (int i = 1; i <= CPU_THREADS; i++) {
                        String porcentagem = MSI.GetMSIEntidade("CPU" + i.ToString() + " usage").Data.ToString("N0");
                        switch (porcentagem.Length) {
                            case 1: porcentagem = porcentagem.Equals("0") ? "   " : " " + porcentagem + "%"; break;
                            case 2: porcentagem += "%"; break;
                            case 3: porcentagem = "MAX"; break;
                        }
                        cpus += " " + porcentagem + " |";
                    }
                    Cliente.TCP_Enviar("CPU.t17", cpus);
                    Cliente.TCP_Enviar("gCPU", 0, int.Parse(MSI.GetMSIEntidade("CPU usage").Data.ToString("N0")), 0, 100, 0, 81);
                    Cliente.TCP_Enviar("gCPU", 1, int.Parse(MSI.GetMSIEntidade("CPU temperature").Data.ToString("N0")), 0, 100, 0, 81);
                    break;

                case 4:
                    float RAM_Uso = MSI.GetMSIEntidade("RAM usage").Data;
                    float RAM_Porcentagem = (RAM_Uso / RAM_TOTAL) * 100;
                    Cliente.TCP_Enviar("MEM.t18", RAM_Porcentagem.ToString("N1"));
                    Cliente.TCP_Enviar("MEM.j1", int.Parse(RAM_Porcentagem.ToString("N0")));
                    Cliente.TCP_Enviar("MEM.t19", RAM_Uso.ToString("N0"));
                    Cliente.TCP_Enviar("MEM.t20", MSI.GetMSIEntidade("HDD" + HDD_INDEX.ToString() + " read rate").Data.ToString("N3"));
                    Cliente.TCP_Enviar("MEM.t21", MSI.GetMSIEntidade("HDD" + HDD_INDEX.ToString() + " write rate").Data.ToString("N3"));
                    Cliente.TCP_Enviar("MEM.t22", MSI.GetMSIEntidade("HDD" + HDD_INDEX.ToString() + " temperature").Data.ToString("N1"));
                    Cliente.TCP_Enviar("MEM.t0", MSI.GetMSIEntidade("HDD" + HDD_INDEX.ToString() + " usage").Data.ToString("N1"));
                    Cliente.TCP_Enviar("MEM.t23", HDD_INDEX.ToString());
                    Cliente.TCP_Enviar("MEM.t25", MSI.GetMSIEntidade("NET" + NET_INDEX.ToString() + " download rate").Data.ToString("N3"));
                    Cliente.TCP_Enviar("MEM.t26", MSI.GetMSIEntidade("NET" + NET_INDEX.ToString() + " download rate").SrcUnits.ToString());
                    Cliente.TCP_Enviar("MEM.t27", MSI.GetMSIEntidade("NET" + NET_INDEX.ToString() + " upload rate").Data.ToString("N3"));
                    Cliente.TCP_Enviar("MEM.t28", MSI.GetMSIEntidade("NET" + NET_INDEX.ToString() + " upload rate").SrcUnits.ToString());
                    Cliente.TCP_Enviar("MEM.t29", NET_INDEX.ToString());
                    Cliente.TCP_Enviar("gMEM", 0, int.Parse(RAM_Porcentagem.ToString("N0")), 0, 100, 0, 81);
                    break;

                case 5:
                    float RAM_Uso2 = MSI.GetMSIEntidade("RAM usage").Data;
                    float RAM_Porcentagem2 = (RAM_Uso2 / RAM_TOTAL) * 100;
                    Cliente.TCP_Enviar("GRAFICO.t30", MSI.GetMSIEntidade("GPU usage").Data.ToString("N0") + " %");
                    Cliente.TCP_Enviar("GRAFICO.t31", MSI.GetMSIEntidade("CPU usage").Data.ToString("N0") + " %");
                    Cliente.TCP_Enviar("GRAFICO.t32", MSI.GetMSIEntidade("framerate").Data.ToString("N0") + " fps");
                    Cliente.TCP_Enviar("s0", 0, int.Parse(MSI.GetMSIEntidade("GPU usage").Data.ToString("N0")), 0, 100, 0, 71);
                    Cliente.TCP_Enviar("s0", 1, int.Parse(MSI.GetMSIEntidade("CPU usage").Data.ToString("N0")), 0, 100, 0, 71);
                    Cliente.TCP_Enviar("s0", 2, int.Parse(MSI.GetMSIEntidade("framerate").Data.ToString("N0")), 0, 71, 0, 71);
                    Cliente.TCP_Enviar("GRAFICO.t33", MSI.GetMSIEntidade("Memory usage").Data.ToString("N0") + " MB");
                    Cliente.TCP_Enviar("GRAFICO.t34", MSI.GetMSIEntidade("GPU temperature").Data.ToString() + " C");
                    Cliente.TCP_Enviar("s1", 0, int.Parse(((MSI.GetMSIEntidade("Memory usage").Data / GPU_VRAM) * 100).ToString("N0")), 0, 100, 0, 51);
                    Cliente.TCP_Enviar("GRAFICO.t35", MSI.GetMSIEntidade("CPU clock").Data.ToString("N0") + " MHz");
                    Cliente.TCP_Enviar("GRAFICO.t36", MSI.GetMSIEntidade("CPU temperature").Data.ToString() + " C");
                    float CPU_CLOCK_MSI = MSI.GetMSIEntidade("CPU clock").Data;
                    if (CPU_CLOCK_MSI > CPU_CLOCK) CPU_CLOCK = CPU_CLOCK_MSI;
                    Cliente.TCP_Enviar("s2", 0, int.Parse(((CPU_CLOCK_MSI / CPU_CLOCK) * 100).ToString("N0")), 0, 100, 0, 51);
                    Cliente.TCP_Enviar("GRAFICO.t37", RAM_Uso2.ToString("N0") + " MB");
                    Cliente.TCP_Enviar("GRAFICO.t38", (RAM_TOTAL - RAM_Uso2).ToString("N0") + " MB");
                    Cliente.TCP_Enviar("s3", 0, int.Parse(RAM_Porcentagem2.ToString("N0")), 0, 100, 0, 51);
                    break;

                case 7:
                    fps = Convert.ToUInt32(MSI.GetMSIEntidade("Framerate").Data);
                    Cliente.TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "0", fps.ToString("N0"));
                    Cliente.TCP_Enviar("t" + ANALISE_LINHA + "0", fps < ANALISE_MIN_FPS && fps != 0 ? Color.Red : Color.FromArgb(255, 0, 184, 192));
                    Cliente.TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "1", MSI.GetMSIEntidade("GPU usage").Data.ToString("N0"));
                    Cliente.TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "2", MSI.GetMSIEntidade("Memory usage").Data.ToString("N0").Replace(".", ""));
                    Cliente.TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "3", MSI.GetMSIEntidade("GPU Temperature").Data.ToString("N0"));
                    Cliente.TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "4", MSI.GetMSIEntidade("CPU usage").Data.ToString("N0"));
                    Cliente.TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "5", MSI.GetMSIEntidade("CPU temperature").Data.ToString("N0"));
                    Cliente.TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "6", MSI.GetMSIEntidade("RAM usage").Data.ToString("N0").Replace(".", ""));
                    Cliente.TCP_Enviar("ANALISE.t" + ANALISE_LINHA + "7", HDD_INDEX + ":" + MSI.GetMSIEntidade("HDD" + HDD_INDEX.ToString() + " usage").Data.ToString("N0"));
                    if (fps < ANALISE_MIN_FPS && ANALISE_LINHA < 7 && fps != 0) ANALISE_LINHA++;
                    break;

                case 6:
                    Cliente.TCP_Enviar("PSU.t0", MSI.GetMSIEntidade("Power").Data.ToString());
                    Cliente.TCP_Enviar("PSU.t1", MSI.GetMSIEntidade("CPU voltage").Data.ToString());
                    Cliente.TCP_Enviar("PSU.t2", MSI.GetMSIEntidade("PSU + 3.3V voltage").Data.ToString());
                    Cliente.TCP_Enviar("PSU.t4", MSI.GetMSIEntidade("PSU + 5V voltage").Data.ToString());
                    Cliente.TCP_Enviar("PSU.t6", MSI.GetMSIEntidade("PSU + 12V voltage").Data.ToString());
                    break;
            }
        }

        private void TCP_Conectar(String IP) {
            try {
                Cliente = new TCPESP32();
                if (Cliente.ConnectAsync(IP, TCP_PORTA).Wait(2000)) {
                    menu_Conectar.Visible = false;
                    menu_Desconectar.Visible = true;
                    Properties.Settings.Default.IP_Favorito = IP;
                    Properties.Settings.Default.Save();
                    Cliente.ReceiveTimeout = 2000;
                    Cliente.SendTimeout = 2000;
                    IconeNotificacao.ShowBalloonTip(1000, "LCDHM", "Conectado", ToolTipIcon.Info);
                    IconeNotificacao.Text = "LCDHM - Conectado a " + IP;
                    TCP_Listener.Start();
                    timer.Start();
                } else {
                    IconeNotificacao.ShowBalloonTip(1000, "LCDHM", "Erro ao tentar se conectar a " + IP + ":" + TCP_PORTA, ToolTipIcon.Error);
                    Properties.Settings.Default.Reload();
                    Menu_IP.Text = Properties.Settings.Default.IP_Favorito;
                }
            } catch (Exception ex) {
                IconeNotificacao.ShowBalloonTip(1000, "LCDHM", ex.Message, ToolTipIcon.Error);
                Properties.Settings.Default.Reload();
                Menu_IP.Text = Properties.Settings.Default.IP_Favorito;
            }
        }

        private void TCP_Desconectar() {
            timer.Stop();
            TCP_Listener.Stop();
            menu_Conectar.Visible = true;
            menu_Desconectar.Visible = false;
            MSI.DisconnectAll();
            if (Cliente != null && Cliente.Connected) { Cliente.Close(); Cliente.Dispose(); }
            IconeNotificacao.ShowBalloonTip(1000, "LCDHM", "Desconectado", ToolTipIcon.Info);
        }

        private void MSI_Conectar() {
            Cliente.TCP_Enviar("CONNECT.titulo", "Conectando ao MSI");
            Cliente.TCP_Enviar("page CONNECT");
            Thread.Sleep(300);
            Cliente.TCP_Enviar("j0", 10);
            Cliente.TCP_Enviar("t", "Inicializando MSI");
            while (Process.GetProcessesByName("MSIAfterburner").Length == 0) {
                try {
                    Process.Start(Properties.Settings.Default.MSI_Directory);
                } catch (Exception) {
                    Mostrar_Configuracoes();
                }
            }
            Cliente.TCP_Enviar("j0", 20);
            Cliente.TCP_Enviar("t", "Criando Conectividade");
            MSI = new MSIAfterburner();

            Cliente.TCP_Enviar("j0", 30);
            Cliente.TCP_Enviar("t", "Conectando ao MSI");
            MSI.Conectar();

            Cliente.TCP_Enviar("j0", 45);
            Cliente.TCP_Enviar("t", "Definindo Constantes");
            CPU_THREADS = (uint)Environment.ProcessorCount;
            CPU_CORES = 0;
            System.Management.ManagementObjectSearcher managementObjectSearcher = new System.Management.ManagementObjectSearcher("Select * from Win32_Processor");
            foreach (System.Management.ManagementBaseObject item in managementObjectSearcher.Get()) this.CPU_CORES += uint.Parse(item["NumberOfCores"].ToString());
            managementObjectSearcher.Dispose();
            Cliente.TCP_Enviar("j0", 50);
            managementObjectSearcher = new System.Management.ManagementObjectSearcher("select MaxClockSpeed from Win32_Processor");
            foreach (var item in managementObjectSearcher.Get()) CPU_CLOCK = (uint)item["MaxClockSpeed"];
            managementObjectSearcher.Dispose();
            Cliente.TCP_Enviar("j0", 55);
            managementObjectSearcher = new System.Management.ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (System.Management.ManagementObject item in managementObjectSearcher.Get()) GPU_VRAM = uint.Parse(item["AdapterRAM"].ToString()) / 1024 / 1024;
            managementObjectSearcher.Dispose();

            Cliente.TCP_Enviar("j0", 65);
            RAM_TOTAL = Convert.ToUInt32(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024));

            Cliente.TCP_Enviar("j0", 90);
            Cliente.TCP_Enviar("t", "Definindo Clocks");
            FAN_BOOST = int.Parse(MSI.GetGPUEntidade(0).FanSpeedCur.ToString("N0"));

            Cliente.TCP_Enviar("j0", 100);
            Cliente.TCP_Enviar("t", "Conectado");
            Thread.Sleep(600);
            Cliente.TCP_Enviar("page Principal");
        }

        private void MSI_EnviarTimer(object sender, EventArgs e) => TCP_EnviarAutomatico();

        private void MSI_AtualizarClock() {
            MSI.ReloadControlMemory();
            Cliente.TCP_Enviar("Overclock.tcore", (MSI.GetMSIEntidade("Core clock").Data + MSI.GetGPUEntidade(0).CoreClockBoostCur / 1000).ToString("N0").Replace(".", "") + " (" + (CORE_BOOST >= 0 ? "+" : "") + CORE_BOOST + ")");
            Cliente.TCP_Enviar("Overclock.tmem", (MSI.GetMSIEntidade("Memory clock").Data + MSI.GetGPUEntidade(0).MemoryClockBoostCur / 1000).ToString("N0").Replace(".", "") + " (" + (MEM_BOOST >= 0 ? "+" : "") + MEM_BOOST + ")");
            Cliente.TCP_Enviar("Overclock.tfan", FAN_FLAG == MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO ? "AUTO" : FAN_BOOST.ToString());
        }

        private void Menu_Conectar_Click(object sender, EventArgs e) {
            MenuContexto.Hide();
            Properties.Settings.Default.Reload();
            if (IPAddress.TryParse(Properties.Settings.Default.IP_Favorito, out _)) TCP_Conectar(Properties.Settings.Default.IP_Favorito);
        }

        private void Menu_IP_EnterClick(object sender, KeyEventArgs e) {
            int pontos = 0;
            foreach (char c in Menu_IP.Text) if (c == '.') pontos++;
            if (e.KeyCode == Keys.Enter && IPAddress.TryParse(Menu_IP.Text, out _) && pontos > 2) {
                MenuContexto.Hide();
                TCP_Conectar(Menu_IP.Text);
            }
        }

        private void Menu_IP_TextChanged(object sender, EventArgs e) {
            String IP = Menu_IP.Text;
            int pontos = 0;
            foreach (char c in IP) if (c == '.') pontos++;
            if (IPAddress.TryParse(IP, out _) && pontos > 2) {
                Menu_IP.ForeColor = Color.FromArgb(255, 0, 184, 192);
            } else {
                Menu_IP.ForeColor = Color.FromArgb(255, 200, 50, 50);
            }
        }

        private void Menu_Desconectar_Click(object sender, EventArgs e) => TCP_Desconectar();

        private void Menu_Configurar_Click(object sender, EventArgs e) => Mostrar_Configuracoes();

        private void Menu_Sobre_Click(object sender, EventArgs e) {
            new SobreForm().Show();
        }

        private void Menu_Sair_Click(object sender, EventArgs e) {
            if (Cliente != null && Cliente.Connected) TCP_Desconectar();
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
    }
}

//TODO: Melhorar Listener TCP
//TODO: Melhorar Listener de perda de conexão