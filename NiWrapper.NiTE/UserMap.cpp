#include <stdio.h>
#include "Array.cpp"
#include "OpenNI.h"
#include "NiTE.h"
using namespace nite;

extern "C"
{
	__declspec(dllexport) int UserMap_getStride(UserMap* um)
	{
		 return um->getStride();
	}
	__declspec(dllexport) void UserMap_getSize(UserMap* um, int* w, int* h)
	{
		 *w = um->getWidth();
		 *h = um->getHeight();
	}
	__declspec(dllexport) UserId* UserMap_getPixels(UserMap* um)
	{
		 return const_cast<UserId*>(um->getPixels());
	}
};