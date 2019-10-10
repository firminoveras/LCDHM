using MSI.Afterburner;
using System;
using System.Threading;

namespace LCDHM {
    class MSIAfterburner {
        public ControlMemory CM;
        public HardwareMonitor HM;
        private uint TimeoutInstanciaSegundos = 10, TimeoutConectarSegundos = 10;


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
