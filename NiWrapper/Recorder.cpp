#include <stdio.h>
#include "OpenNI.h"
using namespace openni;

extern "C"
{
	__declspec(dllexport) void Recorder_destroy(Recorder* rec)
	{
		 rec->destroy();
	}

	__declspec(dllexport) Status Recorder_create(Recorder*& rec, const char* fileName)
	{
		 Recorder* vsl = new Recorder();
		 Status status = vsl->create(fileName);
		 if (status == STATUS_OK)
			 rec = vsl;
		 else
			 Recorder_destroy(vsl);
		 return status;
	}

	__declspec(dllexport) Status Recorder_attach(Recorder* rec, VideoStream* vs, bool allowLossyCompression = false)
	{
		 return rec->attach(*vs, allowLossyCompression);
	}

	__declspec(dllexport) bool Recorder_isValid(Recorder* rec)
	{
		 return rec->isValid();
	}

	__declspec(dllexport) Status Recorder_start(Recorder* rec)
	{
		 return rec->start();
	}
	__declspec(dllexport) void Recorder_stop(Recorder* rec)
	{
		 rec->stop();
	}
};