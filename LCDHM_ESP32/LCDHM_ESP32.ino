#include <Update.h>
#include <ArduinoOTA.h>
#include <ESPmDNS.h>
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

bool ClienteConectado = false, StatusPendente = true;
WiFiServer Servidor(PORTA);
WiFiClient Cliente;

void setup(){
	Serial.begin(115200);
	Serial.setTimeout(SERIAL_TIMEOUT);
}

void loop(){
	if(Serial.available() > 0){
		String entrada = Serial.readStringUntil(':');
		if(entrada.startsWith("desconectar")){
			DesconectarWIFI();
		} else if(entrada.startsWith("conectar")){
			if(Flash.hasEEPROM_DATA() && !Serial.available() > 0) ConectarWIFI(Flash.getEEPROM_SSID(), Flash.getEEPROM_PASS());
			else if(Serial.available() > 0) ConectarWIFI(Serial.readStringUntil(':'), Serial.readStringUntil(':'));
			else if(!WiFi.isConnected())ScanWIFI();
		} else if(entrada.startsWith("limparEEPROM")){
			Flash.clear_EEPROM();
			Nextion.SetTexto("Splash.SSID", "");
		} else if(entrada.startsWith("scan")){
			ScanWIFI();
		} else{
			EnviarTCP(entrada);
		}
	}
	if(WiFi.isConnected()){
		ArduinoOTA.handle();
		if(Cliente.available() > 0) ReceberTCP();
		if(!Cliente.connected()){
			if(ClienteConectado) DesconectarWIFI();
			if(Servidor.hasClient()){
				ClienteConectado = true;
				Cliente = Servidor.available();
				while(!Cliente.connected());
				EnviarTCP("init");
			}
		}
	} else if(StatusPendente){
		StatusPendente = false;
		Nextion.SetTexto("Splash.SSID", (Flash.hasEEPROM_DATA() ? Flash.getEEPROM_SSID() : ""));
		Nextion.SetValor("Splash.j0", 0);
		Nextion.SetTexto("Splash.status", "Desconectado");
	}
}

void EnviarTCP(String Menssagem){
	if(Cliente.connected()) Cliente.println(Menssagem);
}

void ReceberTCP(){
	String Buffer = "";
	while(Cliente.available() > 0){
		if((Buffer += (char)Cliente.read()).endsWith("\n")){
			Nextion.WriteNextion(Buffer);
			Buffer = "";
		}
	}
	if(Buffer.length() > 0) Nextion.WriteNextion(Buffer);
}

void ScanWIFI(){
	if(WiFi.isConnected()){
		Nextion.SetPagina(ClienteConectado ? "Principal" : "Splash");
	} else{
		Nextion.SetPagina("WIFI");
		Nextion.SetTexto("t", "Buscando...");
		for(int i = 0; i <= 5; i++){
			Nextion.SetImagem("s" + String(i), 22);
			Nextion.SetImagem("p" + String(i), 22);
			Nextion.SetTexto("t" + String(i), "");
		}
		int Qtd_Networks = WiFi.scanNetworks();
		Nextion.SetTexto("t", String(Qtd_Networks) + " redes");
		for(int i = 0; i < Qtd_Networks; i++){
			int Potencia_Sinal = 19;
			if(WiFi.RSSI(i) <= -67) Potencia_Sinal = 20;
			if(WiFi.RSSI(i) <= -80) Potencia_Sinal = 21;
			Nextion.SetImagem("s" + String(i), Potencia_Sinal);
			Nextion.SetTexto("t" + String(i), WiFi.SSID(i));
			Nextion.SetImagem("p" + String(i), (WiFi.encryptionType(i) == WIFI_AUTH_OPEN ? 18 : 17));
		}
	}
}

void ConectarWIFI(String SSID, String PASS){
	if(!WiFi.isConnected()){
		Nextion.SetPagina("Splash");
		Nextion.SetTexto("Splash.status", "Conectando a " + SSID);
		if(PASS.length() > 0) WiFi.begin(SSID.c_str(), PASS.c_str()); else WiFi.begin(SSID.c_str());
		int WiFi_Timeout = WIFI_TIMEOUT;
		int Progresso = 0;
		while(!WiFi.isConnected() && WiFi_Timeout > 0){
			WiFi_Timeout--;
			Nextion.SetValor("Splash.j0", Progresso);
			if(Progresso < 90) Progresso += 3;
			delay(100);
		}
		bool WiFiStatus = WiFi.status() == WL_CONNECTED;
		Nextion.SetTexto("Splash.status", String((WiFiStatus) ? "Conectado" : "Falha ao Conectar"));
		Nextion.SetValor("Splash.j0", 100);
		if(WiFiStatus){
			Flash.setEEPROM_SSID(SSID);
			Flash.setEEPROM_PASS(PASS);
			Servidor.begin();
			Nextion.SetTexto("Splash.status", "Conectado - " + WiFi.localIP().toString());
			StatusPendente = true;
			StartOTA();
		} else{
			WiFi.disconnect();
			ScanWIFI();
		}
	}
}

void DesconectarWIFI(){
	ClienteConectado = false;
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
	Nextion.SetPagina("Splash");
	StatusPendente = true;
}

void StartOTA(){
	ArduinoOTA.onStart(
		[](){
			Nextion.SetPagina("Splash");
		});
	ArduinoOTA.onEnd([](){ Nextion.Reset(); });
	ArduinoOTA.onProgress(
		[](unsigned int progress, unsigned int total){
			Nextion.SetValor("Splash.j0", (progress / (total / 100)));
			Nextion.SetTexto("Splash.status", "Atualizacao OTA: " + String((progress / (total / 100))) + "%");
		});

	ArduinoOTA.onError(
		[](ota_error_t error){
			String erro = "";
			if(error == OTA_AUTH_ERROR) erro = "Licenca Invalida";
			else if(error == OTA_BEGIN_ERROR) erro = "Erro de Inicializacao";
			else if(error == OTA_CONNECT_ERROR) erro = "Erro de Conexao";
			else if(error == OTA_RECEIVE_ERROR) erro = "Erro de Recebimento";
			else if(error == OTA_END_ERROR) erro = "Erro na Finalizacao";
			Nextion.SetTexto("Splash.status", "ERRO: " + erro);
		});
	ArduinoOTA.begin();
}