#include "NextionMessager.h"

void NextionMessager::WriteNextion(String Mensagem){
	if(Serial){
		Serial.print((Mensagem.endsWith("\n") ? Mensagem.substring(0, Mensagem.length() - 1) : Mensagem));
		Serial.write(0xFF);
		Serial.write(0xFF);
		Serial.write(0xFF);
	}
}
