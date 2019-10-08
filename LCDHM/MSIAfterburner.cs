using MSI.Afterburner;
using System;
using System.Threading;

namespace LCDHM {
    class MSIAfterburner {
        public ControlMemory CM;
        public HardwareMonitor HM;

        public MSIAfterburner() {
            while (true) {
                try {
                    HM = new HardwareMonitor();
                    CM = new ControlMemory();
                    break;
                } catch (Exception) {
                    Thread.Sleep(500);
                }
            }
        }

        public void Conectar() {
            while (true) {
                try {
                    HM.Connect();
                    CM.Connect();
                    break;
                } catch (Exception) {
                    Thread.Sleep(500);
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
