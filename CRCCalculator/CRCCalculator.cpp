// CRCCalculator.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#ifdef __cplusplus
extern "C" {
#endif

	__declspec(dllexport) void CalculateCRC(char input[], char crc[])
	{
		crc[0] = 1;
		crc[1] = 2;
	}

#ifdef __cplusplus
	}
#endif
