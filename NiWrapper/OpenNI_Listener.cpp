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

class OpenNI_Listener :	public OpenNI::DeviceConnectedListener,
							public OpenNI::DeviceDisconnectedListener,
							public OpenNI::DeviceStateChangedListener
{
	public:
		typedef void (*del_StateChanged)(DeviceInfo*, DeviceState);
		typedef void (*del_ConnectionChanged)(DeviceInfo*);
		OpenNI_Listener() : event_Connect( NULL ),
							event_Disconnect( NULL ),
							event_StateChanged( NULL ) { }
		void SetConnectCallback(del_ConnectionChanged callback)
		{	event_Connect = callback;	}
		void SetDisconnectCallback(del_ConnectionChanged callback)
		{	event_Disconnect = callback;	}
		void SetStateChangedCallback(del_StateChanged callback)
		{	event_StateChanged = callback;	}
		void onDeviceConnected(const DeviceInfo* device){
			if (event_Connect != NULL)
				event_Connect(const_cast<DeviceInfo*>(device));
		}
		void onDeviceDisconnected(const DeviceInfo* device){
			if (event_Disconnect != NULL)
				event_Disconnect(const_cast<DeviceInfo*>(device));
		}
		void onDeviceStateChanged(const DeviceInfo* device, DeviceState state){
			if (event_StateChanged != NULL)
				event_StateChanged(const_cast<DeviceInfo*>(device), state);
		}
	private:
		del_ConnectionChanged event_Connect;
		del_ConnectionChanged event_Disconnect;
		del_StateChanged event_StateChanged;
};
