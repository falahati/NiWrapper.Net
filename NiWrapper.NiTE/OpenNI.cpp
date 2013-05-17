#include <stdio.h>
#include "OpenNI.h"
#include "OpenNI_Listener.cpp"
#include "Array.cpp"
using namespace openni;

extern "C"
{
	__declspec(dllexport) Version OpenNI_getVersion()
	{
		 return OpenNI::getVersion();
		 
	}

	__declspec(dllexport) const char* OpenNI_getExtendedError(){
		return OpenNI::getExtendedError();
	}

	__declspec(dllexport) Status OpenNI_initialize(){
		return OpenNI::initialize();
	}

	__declspec(dllexport) void OpenNI_shutdown(){
		OpenNI::shutdown();
	}

	__declspec(dllexport) Status OpenNI_waitForAnyStream(VideoStream** vsArray, int vsArraySize, int* selectedStream, int timeOut){
		return OpenNI::waitForAnyStream(vsArray, vsArraySize, selectedStream, timeOut);
	}

	__declspec(dllexport) OpenNI_Listener* OpenNI_RegisterListener(
		void (*connect)(DeviceInfo*),
		void (*disconnect)(DeviceInfo*),
		void (*statechanged)(DeviceInfo*, DeviceState)){
		OpenNI_Listener* lis = new OpenNI_Listener();
		lis->SetConnectCallback(connect);
		lis->SetDisconnectCallback(disconnect);
		lis->SetStateChangedCallback(statechanged);
		Status ret = OpenNI::addDeviceConnectedListener(lis);
		if (ret != STATUS_OK)
			return NULL;
		ret = OpenNI::addDeviceDisconnectedListener(lis);
		if (ret != STATUS_OK)
			return NULL;
		ret = OpenNI::addDeviceStateChangedListener(lis);
		if (ret != STATUS_OK)
			return NULL;
		return lis;
	}

	__declspec(dllexport) WrapperArray OpenNI_enumerateDevices(){
		WrapperArray* csarray = new WrapperArray();
		Array<DeviceInfo>* dIArray = new Array<DeviceInfo>();
		OpenNI::enumerateDevices(dIArray);
		csarray->Handle = dIArray;
		csarray->Size = dIArray->getSize();
		DeviceInfo** dP = new DeviceInfo*[255];
		for (int i = 0; i < dIArray->getSize(); i++)
			dP[i] = const_cast<DeviceInfo*>(&((*dIArray)[i]));
		csarray->Data = dP;
		return *csarray;
	}

	__declspec(dllexport) void OpenNI_destroyDevicesArray(WrapperArray p){
		delete[] p.Data;
		delete p.Handle;
	}
};
