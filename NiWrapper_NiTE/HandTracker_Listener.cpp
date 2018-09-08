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

#include "NiTE.h"
using namespace nite;

class HandTracker_Listener : public HandTracker::NewFrameListener
{
public:
	typedef void (*del_NewFrame)(HandTracker* ut);

	HandTracker_Listener() : event_NewFrame(nullptr)
	{
	}

	void SetNewFrameCallback(del_NewFrame callback)
	{
		event_NewFrame = callback;
	}

	void onNewFrame(HandTracker& ut) override
	{
		if (event_NewFrame != nullptr)
			event_NewFrame(&ut);
	}

private:
	del_NewFrame event_NewFrame;
};
