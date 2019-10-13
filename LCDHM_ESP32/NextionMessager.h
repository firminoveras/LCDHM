
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
};
#endif

