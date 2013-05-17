#include <stdio.h>
#include "NITE.h"
using namespace nite;

extern "C"
{
	__declspec(dllexport) Version NiTE_getVersion()
	{
		 return NiTE::getVersion();
	}
	__declspec(dllexport) Status NiTE_initialize()
	{
		 return NiTE::initialize();
	}
	__declspec(dllexport) void NiTE_shutdown()
	{
		 NiTE::shutdown();
	}
};
