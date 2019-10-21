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

bool ClienteConectado = false;
WiFiServer Servidor(PORTA);
WiFiClient Cliente;
EEPROM_ESP32 Flash;
NextionMessager Nextion;

void setup(){
	Serial.begin(115200);
	Serial.setTimeout(SERIAL_TIMEOUT);
	delay(2000);
	ConectarWIFI(Flash.getEEPROM_SSID(), Flash.getEEPROM_PASS());
	while(!WiFi.isConnected()){
		if(Serial.available() > 0){
			String entrada = Serial.readStringUntil(':');
			if(entrada.startsWith("conectar")) ConectarWIFI(Serial.readStringUntil(':'), Serial.readStringUntil(':'));
		}
	}
}

void loop(){
	ArduinoOTA.handle();
	if(Serial.available() > 0){
		String entrada = Serial.readStringUntil(':');
		if(entrada.startsWith("desconectar")) DesconectarWIFI();
		else if(entrada.startsWith("scan")) ConectarWIFI(Flash.getEEPROM_SSID(), Flash.getEEPROM_PASS());
		else EnviarTCP(entrada);
	}
	if(Cliente.available() > 0) ReceberTCP();
	if(WiFi.isConnected() && !Cliente.connected()){
		if(ClienteConectado) DesconectarWIFI();
		if(Servidor.hasClient()){
			ClienteConectado = true;
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
			if(WiFi.SSID(i).equals(Flash.getEEPROM_SSID())){
				Nextion.SetImagem("p" + String(i), 23);
			} else{
				Nextion.SetImagem("p" + String(i), (WiFi.encryptionType(i) == WIFI_AUTH_OPEN ? 18 : 17));
			}
		}
	}
	Serial.flush();
}

void ConectarWIFI(String SSID, String PASS){
	if(!WiFi.isConnected()){
		Nextion.SetTexto("Splash.t0", "Desconectado");
		Nextion.SetPagina("CONNECT");
		Nextion.SetTexto("t", "Conectando a " + SSID);
		if(PASS.length() > 0) WiFi.begin(SSID.c_str(), PASS.c_str()); else WiFi.begin(SSID.c_str());
		int WiFi_Timeout = WIFI_TIMEOUT;
		int Progresso = 0;
		while(!WiFi.isConnected() && WiFi_Timeout > 0){
			WiFi_Timeout--;
			Nextion.SetValor("j0", Progresso);
			if(Progresso < 90) Progresso += 3;
			delay(100);
		}
		bool WiFiStatus = WiFi.status() == WL_CONNECTED;
		Nextion.SetTexto("t", String((WiFiStatus) ? "Conectado" : "Falha ao Conectar"));
		Nextion.SetValor("j0", 100);
		if(WiFiStatus){
			Flash.setEEPROM_SSID(SSID);
			Flash.setEEPROM_PASS(PASS);
			Servidor.begin();
			Nextion.SetTexto("Splash.t0", "Aguardando em " + WiFi.localIP().toString() + ":" + String(PORTA));
			StartOTA();
		} else{
			WiFi.disconnect();
		}
		delay(2000);
		Nextion.SetPagina("Splash");
	}
	Serial.flush();
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
	Nextion.SetTexto("Splash.t0", "Desconectado");
	Nextion.SetPagina("Splash");
}

void StartOTA(){
	ArduinoOTA.onStart([](){
		Nextion.SetPagina("CONNECT");
		Nextion.SetTexto("Titulo", "Atualizacao via OTA");
		});
	ArduinoOTA.onEnd([](){ Nextion.Reset(); });
	ArduinoOTA.onProgress(
		[](unsigned int progress, unsigned int total){
			Nextion.SetValor("j0", (progress / (total / 100)));
			Nextion.SetTexto("t", "Progresso: " + String((progress / (total / 100))) + "%");
		});

	ArduinoOTA.onError(
		[](ota_error_t error){
			String erro = "";
			if(error == OTA_AUTH_ERROR) erro = "Licenca Invalida";
			else if(error == OTA_BEGIN_ERROR) erro = "Erro de Inicializacao";
			else if(error == OTA_CONNECT_ERROR) erro = "Erro de Conexao";
			else if(error == OTA_RECEIVE_ERROR) erro = "Erro de Recebimento";
			else if(error == OTA_END_ERROR) erro = "Erro na Finalizacao";
			Nextion.SetTexto("t", "ERRO: " + erro);
			
		});
	ArduinoOTA.begin();
	delay(500);
}