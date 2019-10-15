using MSI.Afterburner;
using System;
using System.Threading;

namespace LCDHM {

    internal class MSIAfterburner {
        private ControlMemory CM;
        private HardwareMonitor HM;
        private readonly uint TimeoutInstanciaSegundos = 10, TimeoutConectarSegundos = 10;

        public MSIAfterburner() {
            uint timeout = TimeoutInstanciaSegundos * 2;
            while (true) {
                try {
                    HM = new HardwareMonitor();
                    CM = new ControlMemory();
                    break;
                } catch (Exception) {
                    Thread.Sleep(500);
                    if (timeout > 0) timeout--; else break;
                }
            }
        }

        public void Conectar() {
            if (HM != null && CM != null) {
                uint timeout = TimeoutConectarSegundos * 2;
                while (true) {
                    try {
                        HM.Connect();
                        CM.Connect();
                        break;
                    } catch (Exception) {
                        Thread.Sleep(500);
                        if (timeout > 0) timeout--; else break;
                    }
                }
            }
        }

        public void OverclockAplicar() => CM.CommitChanges();

        public ControlMemoryGpuEntry GetGPUEntidade(int GPUIndex) => CM.GpuEntries[GPUIndex];

        public void ReloadControlMemory() {
            try {
                CM.ReloadAll();
            } catch (Exception) {
            }
        }

        public bool IsCMConnected() => CM != null;

        public bool IsHMConnected() => HM != null;

        public void DisconnectCM() {
            if (IsCMConnected()) CM.Disconnect();
        }

        public void DisconnectHM() {
            if (IsHMConnected()) HM.Disconnect();
        }

        public void DisconnectAll() {
            try { DisconnectCM(); DisconnectHM(); } catch (Exception) { }
        }

        public HardwareMonitorEntry GetMSIEntidade(String nome) {
            if (HM != null) {
                foreach (HardwareMonitorEntry e in HM.Entries) {
                    if (e.SrcName.Equals(nome)) {
                        HM.ReloadEntry(e);
                        return e;
                    }
                }
            }
            return new HardwareMonitorEntry();
        }
    }
}