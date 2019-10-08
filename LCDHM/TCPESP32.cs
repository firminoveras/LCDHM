using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Drawing;

namespace LCDHM {
    class TCPESP32 : TcpClient {

        public void TCP_WriteLine(String MensagemTCP) {
            try {
                if (Connected) {
                    if (!MensagemTCP.EndsWith("\n")) MensagemTCP += '\n';
                    Byte[] MensagemTCPByte = Encoding.ASCII.GetBytes(MensagemTCP);
                    GetStream().Write(MensagemTCPByte, 0, MensagemTCPByte.Length);
                }
            } catch (Exception) { };
            
        }
        public String TCP_ReadLine() {
            String LinhaLida = "";
            try {
                while (Connected && GetStream().DataAvailable) {
                    int b = GetStream().ReadByte();
                    if ((char)b == '\n' || (char)b == 13) break;
                    LinhaLida += (char)b;
                }
            }  catch (Exception) { };
            return LinhaLida;
        }

        public void TCP_Enviar(String Comando) => TCP_WriteLine(Comando);
        public void TCP_Enviar(String Variavel, String Texto) => TCP_WriteLine(Variavel + ".txt=\"" + Texto + "\"");
        public void TCP_Enviar(String Variavel, int Valor) => TCP_WriteLine(Variavel + ".val=" + Valor);
        public void TCP_Enviar(String Variavel, int Chanel, int Valor, int In_min, int In_max, int Out_min, int Out_max) => TCP_WriteLine("add " + Variavel + ".id" + "," + Chanel.ToString() + "," + ((Valor - In_min) * (Out_max - Out_min) / (In_max - In_min) + Out_min).ToString("N0"));
        public void TCP_Enviar(String Variavel, Color c) => TCP_WriteLine(Variavel + ".pco=" + (((c.R >> 3) << 11) + ((c.G >> 2) << 5) + (c.B >> 3)).ToString("N0").Replace(".", ""));

    }
}
