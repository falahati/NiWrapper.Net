#include <stdio.h>

extern "C"
{
	__declspec(dllexport) void Common_Delete(void* vf)
	{
		delete vf;
	}
	__declspec(dllexport) void Common_DeleteArray(void* vf)
	{
		delete[] vf;
	}
};