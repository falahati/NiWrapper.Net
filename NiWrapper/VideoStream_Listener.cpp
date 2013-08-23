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

class VideoStream_Listener : public VideoStream::NewFrameListener
{
	public:
		typedef void (*del_NewFrame)(VideoStream* vs);
		VideoStream_Listener() : event_NewFrame( NULL ) { }
		void SetNewFrameCallback(del_NewFrame callback)
		{	event_NewFrame = callback;	}
		void onNewFrame(VideoStream& vs){
			if (event_NewFrame != NULL)
				event_NewFrame(&vs);
		}
	private:
		del_NewFrame event_NewFrame;
};
