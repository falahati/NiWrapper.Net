/*
    Copyright (C) 2013 Soroush Falahati - soroush@falahati.net

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
	*/

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