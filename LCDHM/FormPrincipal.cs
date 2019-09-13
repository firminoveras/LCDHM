using MSI.Afterburner;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace LCDHM {
    public partial class FormPrincipal : Form {
        private bool
                Conectado = false;

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

        private MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG
                FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO;

        private ControlMemory
                CM;

        private HardwareMonitor
                HM;

        public FormPrincipal() {
            InitializeComponent();
            AtualizarPortas();
        }

        private void Serial_Recebeu(object sender, SerialDataReceivedEventArgs e) {
            if (Conectado) {
                String entrada = serial.ReadLine();
                entrada = entrada.Substring(0, entrada.Length - 1);
                Console.WriteLine(entrada);
                if (entrada.Contains("init")) ConectarMSI();
                if (entrada.Contains("MENU")) MudarPagina(0);
                if (entrada.Contains("p1")) MudarPagina(1);
                if (entrada.Contains("p2")) { AtualizarClocks(); MudarPagina(2); }
                if (entrada.Contains("p3")) MudarPagina(3);
                if (entrada.Contains("p4")) MudarPagina(4);
                if (entrada.Contains("p5")) MudarPagina(5);
                if (entrada.Contains("p6")) MudarPagina(6);
                if (entrada.Contains("p7")) {
                    ANALISE_LINHA = 0;
                    MudarPagina(7);
                }
                if (entrada.Contains("analise_gravar") && ANALISE_LINHA < 7) ANALISE_LINHA++;
                if (entrada.Contains("analise_limpar")) ANALISE_LINHA = 0;
                if (entrada.Contains("analise_auto")) ANALISE_MIN_FPS = Convert.ToInt32(entrada.Replace("analise_auto", ""));

                if (entrada.Contains("fps_reset")) {
                    FPS_MAX = 0;
                    FPS_MIN = 0;
                    Enviar("GPU.t6", "-");
                    Enviar("GPU.t7", "-");
                }
                if (entrada.Contains("core_min") && CORE_BOOST > -200) { CORE_BOOST -= 25; AtualizarClocks(); }
                if (entrada.Contains("core_max") && CORE_BOOST < 200) { CORE_BOOST += 25; AtualizarClocks(); Console.WriteLine(CM.GpuEntries[0].CoreClockBoostCur); }
                if (entrada.Contains("core_rst")) { CORE_BOOST = 0; AtualizarClocks(); }
                if (entrada.Contains("mem_min") && MEM_BOOST > -200) { MEM_BOOST -= 25; AtualizarClocks(); }
                if (entrada.Contains("mem_max") && MEM_BOOST < 200) { MEM_BOOST += 25; AtualizarClocks(); }
                if (entrada.Contains("mem_rst")) { MEM_BOOST = 0; AtualizarClocks(); }
                if (entrada.Contains("fan_min") && FAN_BOOST > 39) {
                    FAN_BOOST -= 10;
                    FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None;
                    AtualizarClocks();
                }
                if (entrada.Contains("fan_max") && FAN_BOOST < 91) {
                    FAN_BOOST += 10;
                    FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None;
                    AtualizarClocks();
                }
                if (entrada.Contains("fan_auto")) {
                    FAN_FLAG = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO;
                    FAN_BOOST = 30;
                    AtualizarClocks();
                }
                if (entrada.Contains("oc_aplicar")) {
                    CM.GpuEntries[0].CoreClockBoostCur = CORE_BOOST * 1000;
                    CM.GpuEntries[0].MemoryClockBoostCur = MEM_BOOST * 1000;
                    CM.GpuEntries[0].FanFlagsCur = FAN_FLAG;
                    if (FAN_FLAG == MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None) CM.GpuEntries[0].FanSpeedCur = uint.Parse(FAN_BOOST.ToString());
                    CM.CommitChanges();
                    AtualizarClocks();
                }
                if (entrada.Contains("gerenciador")) Process.Start("Taskmgr.exe");
                if (entrada.Contains("HD_max")) if (HDD_INDEX < 9) HDD_INDEX++; else HDD_INDEX = 1;
                if (entrada.Contains("HD_min")) if (HDD_INDEX > 1) HDD_INDEX--; else HDD_INDEX = 9;
                if (entrada.Contains("NET_max")) if (NET_INDEX < 9) NET_INDEX++; else NET_INDEX = 1;
                if (entrada.Contains("NET_min")) if (NET_INDEX > 1) NET_INDEX--; else NET_INDEX = 9;
                if (entrada.Contains("Steam")) {
                    try {
                        Process.Start(Properties.Settings.Default.Steam_Directory);
                    } catch (Exception) {
                        Mostrar_Configuracoes();
                    }
                }
                //TODO: Não funciona.
                if (entrada.Contains("delay")) {
                    Console.WriteLine(timer.Interval);
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

        private void Enviar_Automatico() {
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
                    Enviar("Overclock.t10", fps.ToString() + " fps");
                    Enviar("line 11,160,311,160,1024");
                    break;
                case 3:
                    Enviar("CPU.t12", GetEntidade("CPU usage").Data.ToString("N0"));
                    Enviar("CPU.t13", GetEntidade("CPU fan speed").Data.ToString());
                    Enviar("CPU.t14", GetEntidade("CPU temperature").Data.ToString());
                    Enviar("CPU.t0", GetEntidade("CPU clock").Data.ToString("N0"));
                    Enviar("CPU.t15", CPU_CORES.ToString());
                    Enviar("CPU.t16", CPU_THREADS.ToString());
                    String cpus = "";
                    //MONOSPAÇAR
                    for (int i = 1; i < CPU_THREADS; i++) {
                        String porcentagem = GetEntidade("CPU" + i.ToString() + " usage").Data.ToString("N0");
                        switch (porcentagem.Length) {
                            case 1: porcentagem = "0" + porcentagem + "%"; break;
                            case 2: porcentagem += "%"; break;
                            case 3: porcentagem = "MAX"; break;
                        }
                        cpus += porcentagem + "  ";
                    }


                    Enviar("CPU.t17", cpus);
                    Enviar("gCPU", 0, int.Parse(GetEntidade("CPU usage").Data.ToString("N0")), 0, 100, 0, 81);
                    Enviar("gCPU", 1, int.Parse(GetEntidade("CPU temperature").Data.ToString("N0")), 0, 100, 0, 81);
                    break;
                case 4:
                    float RAM_Uso = GetEntidade("RAM usage").Data;
                    float RAM_Porcentagem = (RAM_Uso / RAM_TOTAL) * 100;
                    Enviar("MEM.t18", RAM_Porcentagem.ToString("N0"));
                    Enviar("MEM.j1", int.Parse(RAM_Porcentagem.ToString("N0")));
                    Enviar("MEM.t19", RAM_Uso.ToString("N0"));
                    Enviar("MEM.t20", GetEntidade("HDD" + HDD_INDEX.ToString() + " read rate").Data.ToString("N0"));
                    Enviar("MEM.t21", GetEntidade("HDD" + HDD_INDEX.ToString() + " write rate").Data.ToString("N0"));
                    Enviar("MEM.t22", GetEntidade("HDD" + HDD_INDEX.ToString() + " temperature").Data.ToString("N0"));
                    Enviar("MEM.t0", GetEntidade("HDD" + HDD_INDEX.ToString() + " usage").Data.ToString("N0"));
                    Enviar("MEM.t23", HDD_INDEX.ToString());
                    Enviar("MEM.t25", GetEntidade("NET" + NET_INDEX.ToString() + " download rate").Data.ToString("N0"));
                    Enviar("MEM.t26", GetEntidade("NET" + NET_INDEX.ToString() + " download rate").SrcUnits.ToString());
                    Enviar("MEM.t27", GetEntidade("NET" + NET_INDEX.ToString() + " upload rate").Data.ToString("N0"));
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
                    Enviar("s1", 0, int.Parse(GetEntidade("Memory usage").Data.ToString("N0")), 0, Convert.ToInt32(GetEntidade("Total Committed").Data), 0, 51);
                    Enviar("GRAFICO.t35", GetEntidade("CPU clock").Data.ToString("N0") + " MHz");
                    Enviar("GRAFICO.t36", GetEntidade("CPU temperature").Data.ToString() + " C");
                    //FAZER SOZINHO
                    Enviar("s2", 0, int.Parse(((GetEntidade("CPU clock").Data / 3900) * 100).ToString("N0")), 0, 100, 0, 51);
                    Enviar("GRAFICO.t37", RAM_Uso2.ToString("N0") + " MB");
                    Enviar("GRAFICO.t38", (RAM_TOTAL - RAM_Uso2).ToString("N0") + " MB");
                    Enviar("s3", 0, int.Parse(RAM_Porcentagem2.ToString("N0")), 0, 100, 0, 51);
                    break;
                case 7:
                    fps = Convert.ToInt32(GetEntidade("Framerate").Data);
                    if (fps <= ANALISE_MIN_FPS && ANALISE_LINHA < 7 && fps != 0) ANALISE_LINHA++;
                    Enviar("ANALISE.t" + ANALISE_LINHA + "0", fps.ToString("N0"));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "1", GetEntidade("GPU usage").Data.ToString("N0"));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "2", GetEntidade("Memory usage").Data.ToString("N0").Replace(".", ""));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "3", GetEntidade("GPU Temperature").Data.ToString("N0"));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "4", GetEntidade("CPU usage").Data.ToString("N0"));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "5", GetEntidade("CPU temperature").Data.ToString("N0"));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "6", GetEntidade("RAM usage").Data.ToString("N0").Replace(".", ""));
                    Enviar("ANALISE.t" + ANALISE_LINHA + "7", GetEntidade("HDD" + HDD_INDEX.ToString() + " usage").Data.ToString("N0"));
                    break;


                //SrcName = CPU voltage; SrcUnits = V; LocalizedSourceName = Tensão da CPU; LocalizedSrcUnits = V; RecommendedFormat = % .2f; Data = 0; MinLimit = 0; MaxLimit = 2; Flags = None; GPU = 4294967295; SrcId = 241
                //SrcName = PSU + 3.3V voltage; SrcUnits = V; LocalizedSourceName = Tensão da Fonte +3.3V; LocalizedSrcUnits = V; RecommendedFormat = % .2f; Data = 0; MinLimit = 0; MaxLimit = 5; Flags = None; GPU = 0; SrcId = 246
                //SrcName = PSU + 5V voltage; SrcUnits = V; LocalizedSourceName = Tensão da Fonte +5V; LocalizedSrcUnits = V; RecommendedFormat = % .2f; Data = 0; MinLimit = 0; MaxLimit = 10; Flags = None; GPU = 0; SrcId = 246
                //SrcName = PSU + 12V voltage; SrcUnits = V; LocalizedSourceName = Tensão da Fonte +12V; LocalizedSrcUnits = V; RecommendedFormat = % .2f; Data = 0; MinLimit = 0; MaxLimit = 15; Flags = None; GPU = 0; SrcId = 246
                case 6:
                    Enviar("PSU.t0", GetEntidade("Power").Data.ToString());
                    Enviar("PSU.t1", GetEntidade("CPU voltage").Data.ToString());

                    Enviar("PSU.t2", GetEntidade("PSU + 3.3V voltage").Data.ToString());
                    Enviar("PSU.t4", GetEntidade("PSU + 5V voltage").Data.ToString());
                    Enviar("PSU.t6", GetEntidade("PSU + 12V voltage").Data.ToString());



                    break;
            }
        }

        private void ConectarCORE(object sender, EventArgs e) {            
            serial.PortName = ((ToolStripItem)sender).Text;
            serial.Open();
            serial.DiscardInBuffer();
            serial.DiscardOutBuffer();
            Thread.Sleep(1000);
            serial.WriteLine("INIT");
            Thread.Sleep(1000);
            if (serial.BytesToRead > 0 && serial.ReadLine().Contains("Conectar")) {
                icone.ShowBalloonTip(1000, "LCDHM", "Conectado a porta " + ((ToolStripItem)sender).Text + "...", ToolTipIcon.Warning);
                Conectado = true;
                menu_Conectar.Visible = false;
                menu_Atualizar.Visible = false;
                menu_Desconectar.Visible = true;
                serial.WriteLine("page 0");
                timer.Start();
            } else {
                icone.ShowBalloonTip(1000, "LCDHM", "Erro ao tentar se conectar.", ToolTipIcon.Error);
                serial.Close();
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
            RAM_TOTAL = (int)(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024));

            Enviar("j0", 70);
            Enviar("t", "Definindo Clocks");
            FAN_BOOST = int.Parse(CM.GpuEntries[0].FanSpeedCur.ToString("N0"));

            Enviar("j0", 100);
            Enviar("t", "Conectado");
            Thread.Sleep(600);
            Enviar("page Principal");
        }

        private void DesconectarCORE() {
            if (Conectado) {
                Enviar("page 0");
                timer.Stop();
                Conectado = false;
                menu_Conectar.Visible = true;
                menu_Atualizar.Visible = true;
                menu_Desconectar.Visible = false;
                HM.Disconnect();
                CM.Disconnect();                
                serial.Close();
                icone.ShowBalloonTip(1000, "LCDHM", "Desconectado", ToolTipIcon.Info);
            }
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
                icone.ShowBalloonTip(1000, "Endereço Inválido", "Caminho do programa da Steam ou do Afterburner inválido.", ToolTipIcon.Error);
            }
        }

        private void BT_Buscar_MSI_Click(object sender, EventArgs e) {
            if (FileDialog.ShowDialog().ToString() == "OK") Text_MSI_Diretorio.Text = FileDialog.FileName;
        }

        private void MudarPagina(int PG_Index) {
            PAGINA = PG_Index;
            Enviar_Automatico();
        }

        private void AtualizarClocks() {
            CM.ReloadAll();
            String sinalCore = "", sinalMem = ""; ;
            if (CORE_BOOST >= 0) sinalCore = "+";
            if (MEM_BOOST >= 0) sinalMem = "+";
            Enviar("Overclock.tcore", (GetEntidade("Core clock").Data + CM.GpuEntries[0].CoreClockBoostCur / 1000).ToString("N0").Replace(".", "") + sinalCore + CORE_BOOST);
            Enviar("Overclock.tmem", (GetEntidade("Memory clock").Data + CM.GpuEntries[0].MemoryClockBoostCur / 1000).ToString("N0").Replace(".", "") + sinalMem + MEM_BOOST);
            if (FAN_FLAG == MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO) Enviar("Overclock.tfan", "AUTO"); else Enviar("Overclock.tfan", FAN_BOOST.ToString());
        }

        private void AtualizarPortas() {
            menu_portas.Items.Clear();
            foreach (String port in SerialPort.GetPortNames()) menu_portas.Items.Add(port, null, ConectarCORE);
        }

        private void Menu_Atualizar_Click(object sender, EventArgs e) {
            AtualizarPortas();
            icone.ShowBalloonTip(1000, "LCDHM", SerialPort.GetPortNames().Length + " porta(s) encontradas.", ToolTipIcon.Info);
        }

        private void Menu_Desconectar_Click(object sender, EventArgs e) => DesconectarCORE();

        private void Menu_Configurar_Click(object sender, EventArgs e) {
            Mostrar_Configuracoes();
        }

        private void Mostrar_Configuracoes() {
            CenterToScreen();
            this.Opacity = 100;
            this.ShowInTaskbar = true;
        }

        private void Ocultar_Configuracoes() {
            this.Location = new Point(-10000, -10000);
            this.Opacity = 0;
            this.ShowInTaskbar = false;
        }

        private void Menu_Sair_Click(object sender, EventArgs e) {
            DesconectarCORE();
            Application.Exit();
        }

        private void TimerTick(object sender, EventArgs e) => Enviar_Automatico();

        private void Form1_Load(object sender, EventArgs e) {
            Ocultar_Configuracoes();
            Properties.Settings.Default.Reload();
            Text_MSI_Diretorio.Text = Properties.Settings.Default.MSI_Directory;
            Text_Steam_Diretorio.Text = Properties.Settings.Default.Steam_Directory;
            if (!(Text_MSI_Diretorio.Text.EndsWith("MSIAfterburner.exe") && Text_Steam_Diretorio.Text.EndsWith("Steam.exe"))) Mostrar_Configuracoes();

        }








        public void Enviar(String Comando) => serial.WriteLine(Comando);

        public void Enviar(String Variavel, String Texto) => serial.WriteLine(Variavel + ".txt=\"" + Texto + "\"");

        public void Enviar(String Variavel, int Valor) => serial.WriteLine(Variavel + ".val=" + Valor);

        public void Enviar(String Variavel, int Chanel, int Valor, int In_min, int In_max, int Out_min, int Out_max) {
            int div = (In_max - In_min) + Out_min;
            if (div != 0) {
                serial.WriteLine("add " + Variavel + ".id" + "," + Chanel.ToString() + "," + ((Valor - In_min) * (Out_max - Out_min) / div).ToString("N0"));
            } else {
                serial.WriteLine("add " + Variavel + ".id" + ",0");
            }
        }





    }

}