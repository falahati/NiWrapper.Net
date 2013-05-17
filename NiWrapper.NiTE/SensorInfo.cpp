#include <stdio.h>
#include "OpenNI.h"
#include "Array.cpp"
using namespace openni;

extern "C"
{

	__declspec(dllexport) SensorType SensorInfo_getSensorType(SensorInfo* si)
	{
		 return si->getSensorType();
	}

	__declspec(dllexport) WrapperArray SensorInfo_getSupportedVideoModes(SensorInfo* si){
		WrapperArray* csarray = new WrapperArray();
		const Array<VideoMode>& dIArray = si->getSupportedVideoModes();
		csarray->Size = dIArray.getSize();
		VideoMode** dP = new VideoMode*[255];
		for (int i = 0; i < dIArray.getSize(); i++)
			dP[i] = const_cast<VideoMode*>(&(dIArray[i]));
		csarray->Data = dP;
		return *csarray;
	}

	__declspec(dllexport) void SensorInfo_destroyVideoModesArray(WrapperArray p){
		delete[] p.Data;
	}
};