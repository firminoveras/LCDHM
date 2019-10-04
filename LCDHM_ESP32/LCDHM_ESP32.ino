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

int BytesIn = 0, BytesOut = 0;
bool ClienteChanged = false;
WiFiServer Servidor(PORTA);
WiFiClient Cliente;

void setup(){
	Heltec.begin(true, false, true);
	attachInterrupt(digitalPinToInterrupt(0), BT, FALLING);
	Serial.setTimeout(SERIAL_TIMEOUT);
	Status();
}

void loop(){
	if(Serial.available() > 0){
		String entrada = Serial.readStringUntil(':');
		if(entrada.startsWith("conectar")){
			if(Serial.available() > 0){
				String SSID = Serial.readStringUntil(':');
				if(SSID.equals(getEEPROM_SSID())) ConectarWIFI(getEEPROM_SSID(), getEEPROM_PASS()); else ConectarWIFI(SSID, Serial.readStringUntil(':'));
			} else{
				ConectarWIFI(getEEPROM_SSID(), getEEPROM_PASS());
			}
		} else if(entrada.startsWith("scan")) ScanWIFI();
		else if(entrada.startsWith("desconectar")) DesconectarWIFI();
		else EnviarTCP(entrada);
	}
	if(Cliente.available() > 0) ReceberTCP();
	if(WiFi.isConnected() && !Cliente.connected()){
		if(ClienteChanged){
			BytesIn = 0;
			BytesOut = 0;
			ClienteChanged = false;
			WriteNextion("page Splash");
			Status();
		}
		if(Servidor.hasClient()){
			ClienteChanged = true;
			Cliente = Servidor.available();
			while(!Cliente.connected());
			EnviarTCP("init");
			Status();
		}
	}
}

void EnviarTCP(String e){
	if(Cliente.connected()){
		digitalWrite(25, HIGH);
		Cliente.println(e);
		BytesOut += e.length() + 1;
		Status();
		digitalWrite(25, LOW);
	}
}
void ReceberTCP(){
	String Recebido = "";
	digitalWrite(25, HIGH);
	char c;
	while(Cliente.available() > 0){
		c = (char)Cliente.read();
		BytesIn++;
		Recebido += c;
		if(c == '\n'){
			WriteNextion(Recebido);
			Recebido = "";
		}
	}
	if(Recebido.length() > 0) WriteNextion(Recebido);
	digitalWrite(25, LOW);
	Status();
}
void Status(){
	Heltec.display->setTextAlignment(TEXT_ALIGN_CENTER);
	Heltec.display->clear();
	Heltec.display->drawRect(0, 0, Heltec.display->getWidth(), Heltec.display->getHeight());
	Heltec.display->setFont(ArialMT_Plain_10);
	Heltec.display->drawString(Heltec.display->getWidth() / 2, 2, "LCDHM - Status");
	Heltec.display->drawLine(0, 14, Heltec.display->getWidth(), 14);
	Heltec.display->drawString(Heltec.display->getWidth() / 2, 15, String(WiFi.isConnected() ? String(WiFi.SSID()) : "DESCONECTADO"));
	if(WiFi.isConnected()){
		if(Cliente.connected()){
			Heltec.display->drawString(Heltec.display->getWidth() / 2, 25, "In : " + (BytesIn > (1048576) ? String(BytesIn / 1048576) + "mB" : (BytesIn > 1024 ? String(BytesIn / 1024) + "kB" : String(BytesIn) + "Bytes")));
			Heltec.display->drawString(Heltec.display->getWidth() / 2, 35, "Out: " + (BytesOut > (1048576) ? String(BytesOut / 1048576) + "mB" : (BytesOut > 1024 ? String(BytesOut / 1024) + "kB" : String(BytesOut) + "Bytes")));
			Heltec.display->drawString(Heltec.display->getWidth() / 2, 45, "Cliente Conectado");
		} else{
			Heltec.display->drawString(Heltec.display->getWidth() / 2, 25, WiFi.macAddress());
			Heltec.display->drawString(Heltec.display->getWidth() / 2, 35, WiFi.localIP().toString() + ":" + String(PORTA));
			Heltec.display->drawString(Heltec.display->getWidth() / 2, 45, "Cliente Desconectado");
		}
	}
	Heltec.display->display();
}
void ScanWIFI(){
	if(WiFi.isConnected()){
		WriteNextion("page CONNECT");
		WriteNextion("j0.val=50");
		WriteNextion("CONNECT.t.txt=\"Aguardando " + WiFi.localIP().toString() + "\"");
	}else{
		for(int i = 0; i <= 5; i++){
			WriteNextion("s" + String(i) + ".pic=22");
			WriteNextion("t" + String(i) + ".txt=\"\"");
			WriteNextion("p" + String(i) + ".pic=22");
		}
		int Qtd_Networks = WiFi.scanNetworks();
		for(int i = 0; i < Qtd_Networks; i++){
			if(i <= 5){
				String Potencia_Sinal = "19";
				if(WiFi.RSSI(i) <= -67) Potencia_Sinal = "20";
				if(WiFi.RSSI(i) <= -80) Potencia_Sinal = "21";
				WriteNextion("s" + String(i) + ".pic=" + Potencia_Sinal);
				String SSID = WiFi.SSID(i);
				WriteNextion("t" + String(i) + ".txt=\"" + SSID + "\"");
				if(SSID.equals(getEEPROM_SSID())){
					WriteNextion("p" + String(i) + ".pic=18");
				} else{
					WriteNextion("p" + String(i) + ".pic=" + (WiFi.encryptionType(i) == WIFI_AUTH_OPEN ? "18" : "17"));
				}
			}
		}
	}
}
void ConectarWIFI(String SSID, String PASS){
	Serial.flush();
	WriteNextion("page CONNECT");
	WriteNextion("CONNECT.t.txt=\"Conectando a " + SSID + "\"");
	WiFi.begin(SSID.c_str(), PASS.c_str());
	int WiFi_Timeout = WIFI_TIMEOUT;
	int Progresso = 0;
	Heltec.display->setTextAlignment(TEXT_ALIGN_CENTER);
	Heltec.display->clear();
	Heltec.display->drawRect(0, 0, Heltec.display->getWidth(), Heltec.display->getHeight());
	Heltec.display->setFont(ArialMT_Plain_10);
	Heltec.display->drawString(Heltec.display->getWidth() / 2, 5, "Conectando a rede");
	Heltec.display->setFont(ArialMT_Plain_16);
	Heltec.display->drawString(Heltec.display->getWidth() / 2, 15, SSID);
	while(!WiFi.isConnected() && WiFi_Timeout > 0){
		WiFi_Timeout--;
		Heltec.display->drawProgressBar(10, 40, Heltec.display->getWidth() - 20, 5, Progresso);
		Heltec.display->display();
		WriteNextion("j0.val=" + String((int)Progresso/2));
		if(Progresso < 90) Progresso += 5;
		delay(100);
	}
	Heltec.display->drawProgressBar(10, 40, Heltec.display->getWidth() - 20, 5, 100);
	Heltec.display->setFont(ArialMT_Plain_10);
	Heltec.display->drawString(Heltec.display->getWidth() / 2, 50, (WiFi.status() == WL_CONNECTED) ? "SUCESSO" : "FALHA");
	Heltec.display->display();
	WriteNextion("t.txt=\"" + String((WiFi.status() == WL_CONNECTED) ? "Aguardando " + WiFi.localIP().toString() : "Falha ao conectar") + "\"");
	WriteNextion("j0.val=50");
	delay(2000);
	
	if(WiFi.status() == WL_CONNECTED){
		setEEPROM_SSID(SSID);
		setEEPROM_PASS(PASS);
		Servidor.begin();
	} else{
		WiFi.disconnect();
		WriteNextion("page keybdA");
		WriteNextion("show.pco=36864");
	}
	Status();
}
void DesconectarWIFI(){
	if(Cliente.connected()){
		EnviarTCP("desconectar");
		Cliente.stop();
	}
	if(WiFi.isConnected()){
		Servidor.end();		
		WiFi.disconnect();
	}
	while(WiFi.isConnected());
	WriteNextion("page Splash");
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

void BT(){
	WriteNextion("rest");
}
void WriteNextion(String Mensagem){
	Serial.print((Mensagem.endsWith("\n") ? Mensagem.substring(0, Mensagem.length() - 1) : Mensagem));
	Serial.write(0xFF);
	Serial.write(0xFF);
	Serial.write(0xFF);
}
