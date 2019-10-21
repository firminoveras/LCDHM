
#ifndef _NEXTIONMESSAGER_h
#define _NEXTIONMESSAGER_h
#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif
class NextionMessager{
public:
	void WriteNextion(String mensagem);
	void SetPagina(String nomePagina);
	void SetTexto(String id, String texto);
	void SetValor(String id, int valor);
	void SetImagem(String id, int idImagem);
	void Reset();
};
#endif

