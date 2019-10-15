#include <Wire.h>
#include <SPI.h>
#include <WiFiServer.h>
#include <WiFiClient.h>
#include <WiFi.h>
#include <ETH.h>
#include "EEPROM_ESP32.h"
#include "NextionMessager.h"

#define WIFI_TIMEOUT 30
#define SERIAL_TIMEOUT 50
#define PORTA 1199

bool AUTOCONECTAR = true, ClienteChanged = false;
WiFiServer Servidor(PORTA);
WiFiClient Cliente;
EEPROM_ESP32 Flash;
NextionMessager Nextion;

void setup(){
	Serial.begin(115200);
	Serial.setTimeout(SERIAL_TIMEOUT);
}

void loop(){
	if(Serial.available() > 0){
		String entrada = Serial.readStringUntil(':');
		if(entrada.startsWith("conectar")){
			if(Serial.available() > 0){
				String SSID = Serial.readStringUntil(':');
				if(SSID.equals(Flash.getEEPROM_SSID())){
					ConectarWIFI(Flash.getEEPROM_SSID(), Flash.getEEPROM_PASS());
				} else{
					ConectarWIFI(SSID, Serial.readStringUntil(':'));
				}
			} else{
				ConectarWIFI(Flash.getEEPROM_SSID(), Flash.getEEPROM_PASS());
			}
		} else if(entrada.startsWith("scan")) ScanWIFI();
		else if(entrada.startsWith("desconectar")) DesconectarWIFI();
		else EnviarTCP(entrada);
	}
	if(Cliente.available() > 0) ReceberTCP();
	if(WiFi.isConnected() && !Cliente.connected()){

		if(ClienteChanged) DesconectarWIFI();
		if(Servidor.hasClient()){
			ClienteChanged = true;
			Cliente = Servidor.available();
			while(!Cliente.connected());
			EnviarTCP("init");
		}
	}
}

void EnviarTCP(String e){
	if(Cliente.connected()) Cliente.println(e);
}

void ReceberTCP(){
	String Recebido = "";
	while(Cliente.available() > 0){
		if((Recebido += (char)Cliente.read()).endsWith("\n")){
			Nextion.WriteNextion(Recebido);
			Recebido = "";
		}
	}
	if(Recebido.length() > 0) Nextion.WriteNextion(Recebido);
}

void ScanWIFI(){
	if(WiFi.isConnected()){
		if(ClienteChanged){
			Nextion.WriteNextion("page Principal");
		} else{
			Nextion.WriteNextion("page CONNECT");
			Nextion.WriteNextion("j0.val=50");
			Nextion.WriteNextion("CONNECT.t.txt=\"Aguardando " + WiFi.localIP().toString() + "\"");
		}
	} else{
		Nextion.WriteNextion("t.txt=\"Buscando...\"");
		for(int i = 0; i <= 5; i++){
			Nextion.WriteNextion("s" + String(i) + ".pic=22");
			Nextion.WriteNextion("t" + String(i) + ".txt=\"\"");
			Nextion.WriteNextion("p" + String(i) + ".pic=22");
		}
		int Qtd_Networks = WiFi.scanNetworks();
		Nextion.WriteNextion("t.txt=\"" + String(Qtd_Networks) + " redes\"");
		for(int i = 0; i < Qtd_Networks; i++){
			String Potencia_Sinal = "19";
			if(WiFi.RSSI(i) <= -67) Potencia_Sinal = "20";
			if(WiFi.RSSI(i) <= -80) Potencia_Sinal = "21";
			Nextion.WriteNextion("s" + String(i) + ".pic=" + Potencia_Sinal);
			String SSID = WiFi.SSID(i);
			Nextion.WriteNextion("t" + String(i) + ".txt=\"" + SSID + "\"");
			if(SSID.equals(Flash.getEEPROM_SSID())){
				Nextion.WriteNextion("p" + String(i) + ".pic=23");
				if(AUTOCONECTAR) ConectarWIFI(Flash.getEEPROM_SSID(), Flash.getEEPROM_PASS());
			} else{
				Nextion.WriteNextion("p" + String(i) + ".pic=" + (WiFi.encryptionType(i) == WIFI_AUTH_OPEN ? "18" : "17"));
			}
		}
	}
	Serial.flush();
}

void ConectarWIFI(String SSID, String PASS){
	if(!WiFi.isConnected()){
		Nextion.WriteNextion("page CONNECT");
		Nextion.WriteNextion("CONNECT.t.txt=\"Conectando a " + SSID + "\"");
		if(PASS.length() > 0) WiFi.begin(SSID.c_str(), PASS.c_str()); else WiFi.begin(SSID.c_str());
		int WiFi_Timeout = WIFI_TIMEOUT;
		int Progresso = 0;
		while(!WiFi.isConnected() && WiFi_Timeout > 0){
			WiFi_Timeout--;
			Nextion.WriteNextion("j0.val=" + String((int)Progresso / 2));
			if(Progresso < 70) Progresso += 2;
			delay(100);
		}
		bool WiFiStatus = WiFi.status() == WL_CONNECTED;
		Nextion.WriteNextion("t.txt=\"" + String((WiFiStatus) ? "Aguardando " + WiFi.localIP().toString() : "Falha ao conectar") + "\"");
		Nextion.WriteNextion("j0.val=50");
		if(WiFiStatus){
			Flash.setEEPROM_SSID(SSID);
			Flash.setEEPROM_PASS(PASS);
			Servidor.begin();
		} else{
			WiFi.disconnect();
			delay(2000);
			Nextion.WriteNextion("page Splash");
		}
	}
	Serial.flush();
}

void DesconectarWIFI(){
	ClienteChanged = false;
	if(Cliente.connected()){
		EnviarTCP("desconectar");
		while(Cliente.connected());
		Cliente.stop();
	}
	if(WiFi.isConnected()){
		Servidor.end();
		WiFi.disconnect();
	}
	while(WiFi.isConnected());
	Serial.flush();
	Nextion.WriteNextion("page Splash");
}