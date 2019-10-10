#include <Wire.h>
#include <SPI.h>
#include <heltec.h>
#include <WiFiServer.h>
#include <WiFiClient.h>
#include <WiFi.h>
#include <ETH.h>
#include <EEPROM.h>

#define WIFI_TIMEOUT 30
#define SERIAL_TIMEOUT 50
#define PORTA 1199

bool AUTOCONECTAR = true, ClienteChanged = false;
WiFiServer Servidor(PORTA);
WiFiClient Cliente;

void setup(){
	Heltec.begin(false, false, true);
	Serial.setTimeout(SERIAL_TIMEOUT);
	pinMode(25, OUTPUT);
	digitalWrite(25, AUTOCONECTAR);
	attachInterrupt(digitalPinToInterrupt(GPIO_NUM_0), changeAutoconectar, FALLING);
}

void changeAutoconectar(){
	AUTOCONECTAR = !AUTOCONECTAR;
	digitalWrite(25, AUTOCONECTAR);
	Serial.println("A");
}

void loop(){
	if(Serial.available() > 0){
		String entrada = Serial.readStringUntil(':');
		if(entrada.startsWith("conectar")){
			if(Serial.available() > 0){
				String SSID = Serial.readStringUntil(':');
				if(SSID.equals(getEEPROM_SSID())){
					ConectarWIFI(getEEPROM_SSID(), getEEPROM_PASS());
				} else{
					ConectarWIFI(SSID, Serial.readStringUntil(':'));
				}
			} else{
				ConectarWIFI(getEEPROM_SSID(), getEEPROM_PASS());
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
			WriteNextion(Recebido);
			Recebido = "";
		}
	}
	if(Recebido.length() > 0) WriteNextion(Recebido);
}

void ScanWIFI(){
	if(WiFi.isConnected()){
		if(ClienteChanged){
			WriteNextion("page Principal");
		} else{
			WriteNextion("page CONNECT");
			WriteNextion("j0.val=50");
			WriteNextion("CONNECT.t.txt=\"Aguardando " + WiFi.localIP().toString() + "\"");
		}
	} else{
		WriteNextion("t.txt=\"Buscando...\"");
		for(int i = 0; i <= 5; i++){
			WriteNextion("s" + String(i) + ".pic=22");
			WriteNextion("t" + String(i) + ".txt=\"\"");
			WriteNextion("p" + String(i) + ".pic=22");
		}
		int Qtd_Networks = WiFi.scanNetworks();
		WriteNextion("t.txt=\"" + String(Qtd_Networks) + " redes\"");
		for(int i = 0; i < Qtd_Networks; i++){
			String Potencia_Sinal = "19";
			if(WiFi.RSSI(i) <= -67) Potencia_Sinal = "20";
			if(WiFi.RSSI(i) <= -80) Potencia_Sinal = "21";
			WriteNextion("s" + String(i) + ".pic=" + Potencia_Sinal);
			String SSID = WiFi.SSID(i);
			WriteNextion("t" + String(i) + ".txt=\"" + SSID + "\"");
			if(SSID.equals(getEEPROM_SSID())){
				WriteNextion("p" + String(i) + ".pic=23");
				if(AUTOCONECTAR) ConectarWIFI(getEEPROM_SSID(), getEEPROM_PASS());
			} else{
				WriteNextion("p" + String(i) + ".pic=" + (WiFi.encryptionType(i) == WIFI_AUTH_OPEN ? "18" : "17"));
			}
		}
	}
	Serial.flush();
}

void ConectarWIFI(String SSID, String PASS){
	if(WiFi.isConnected()){
	} else{
		WriteNextion("page CONNECT");
		WriteNextion("CONNECT.t.txt=\"Conectando a " + SSID + "\"");
		if(PASS.length() > 0)	WiFi.begin(SSID.c_str(), PASS.c_str()); else WiFi.begin(SSID.c_str());

		int WiFi_Timeout = WIFI_TIMEOUT;
		int Progresso = 0;
		while(!WiFi.isConnected() && WiFi_Timeout > 0){
			WiFi_Timeout--;

			WriteNextion("j0.val=" + String((int)Progresso / 2));
			if(Progresso < 70) Progresso += 2;
			delay(100);
		}
		bool WiFiStatus = WiFi.status() == WL_CONNECTED;
		WriteNextion("t.txt=\"" + String((WiFiStatus) ? "Aguardando " + WiFi.localIP().toString() : "Falha ao conectar") + "\"");
		WriteNextion("j0.val=50");
		if(WiFiStatus){
			setEEPROM_SSID(SSID);
			setEEPROM_PASS(PASS);
			Servidor.begin();
		} else{
			WiFi.disconnect();
			delay(2000);
			WriteNextion("page Splash");
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
}

void clear_EEPROM(){
	EEPROM.begin(64);
	for(int i = 0; i < 64; i++) EEPROM.writeChar(i + 1, 255);
	EEPROM.commit();
	EEPROM.end();
}
void setEEPROM_SSID(String SSID){
	EEPROM.begin(64);
	for(int i = 0; i < 32; i++) EEPROM.writeChar(i + 1, 255);
	for(int i = 0; i < SSID.length(); i++) if(i + 1 < 32)EEPROM.writeChar(i + 1, SSID.charAt(i));
	EEPROM.commit();
	EEPROM.end();
}
void setEEPROM_PASS(String PASS){
	EEPROM.begin(64);
	for(int i = 33; i < 64; i++) EEPROM.writeChar(i, 255);
	for(int i = 0; i < PASS.length(); i++) if(i < 32)EEPROM.writeChar(i + 33, PASS.charAt(i));
	EEPROM.commit();
	EEPROM.end();
}
void setEEPROM_AUTOCONNECT(bool AUTOCONNECT){
	EEPROM.begin(64);
	EEPROM.write(0, (AUTOCONNECT ? '1' : '0'));
	EEPROM.commit();
	EEPROM.end();
}
bool getEEPROM_AUTOCONNECT(){
	bool AUTOCONNECT;
	EEPROM.begin(64);
	AUTOCONNECT = EEPROM.read(0) == '1';
	EEPROM.end();
	return AUTOCONNECT;
}
String getEEPROM_PASS(){
	String PASS = "";
	EEPROM.begin(64);
	for(int i = 33; i < 64; i++) if(EEPROM.read(i) != 255)PASS += (char)EEPROM.read(i);
	EEPROM.end();
	return PASS;
}
String getEEPROM_SSID(){
	String SSID = "";
	EEPROM.begin(64);
	for(int i = 1; i < 32; i++) if(EEPROM.read(i) != 255)SSID += (char)EEPROM.read(i);
	EEPROM.end();
	return SSID;
}

void WriteNextion(String Mensagem){
	Serial.print((Mensagem.endsWith("\n") ? Mensagem.substring(0, Mensagem.length() - 1) : Mensagem));
	Serial.write(0xFF);
	Serial.write(0xFF);
	Serial.write(0xFF);
}