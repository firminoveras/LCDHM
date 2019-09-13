using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCDHM {
    public partial class Nextion : SerialPort {
        public Nextion() {
            InitializeComponent();
        }

        public Nextion(IContainer container) {
            container.Add(this);

            InitializeComponent();
        }

        public void Enviar(String Comando) => WriteLine(Comando);

        public void Enviar(String Variavel, String Texto) => WriteLine(Variavel + ".txt=\"" + Texto + "\"");

        public void Enviar(String Variavel, int Valor) => WriteLine(Variavel + ".val=" + Valor);

        public void Enviar(String Variavel, int Chanel, int Valor, int In_min, int In_max, int Out_min, int Out_max) {
            int div = (In_max - In_min) + Out_min;
            if (div != 0) {
                WriteLine("add " + Variavel + ".id" + "," + Chanel.ToString() + "," + ((Valor - In_min) * (Out_max - Out_min) / div).ToString("N0"));
            } else {
                WriteLine("add " + Variavel + ".id" + ",0");
            }
        }
    }
}
