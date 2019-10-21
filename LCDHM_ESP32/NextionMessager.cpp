#include "NextionMessager.h"

void NextionMessager::WriteNextion(String Mensagem){
	if(Serial){
		Serial.print((Mensagem.endsWith("\n") ? Mensagem.substring(0, Mensagem.length() - 1) : Mensagem));
		Serial.write(0xFF);
		Serial.write(0xFF);
		Serial.write(0xFF);
	}
}

void NextionMessager::SetPagina(String nomePagina){
	WriteNextion("page " + nomePagina);
}

void NextionMessager::SetTexto(String id, String texto){
	WriteNextion(id + ".txt=\"" + texto + "\"");
}

void NextionMessager::SetValor(String id, int valor){
	WriteNextion(id + ".val=" + String(valor));
}

void NextionMessager::SetImagem(String id, int idImagem){
	WriteNextion(id + ".pic=" + String(idImagem));
}

void NextionMessager::Reset(){
	WriteNextion("rest");
}
