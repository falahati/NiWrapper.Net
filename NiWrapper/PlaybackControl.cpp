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

	__declspec(dllexport) bool PlaybackControl_isValid(PlaybackControl* pbc)
	{
		 return pbc->isValid();
	}

	__declspec(dllexport) int PlaybackControl_getNumberOfFrames(PlaybackControl* pbc, VideoStream* vs)
	{
		 return pbc->getNumberOfFrames(*vs);
	}

	__declspec(dllexport) Status PlaybackControl_seek(PlaybackControl* pbc, VideoStream* vs, int frameIndex)
	{
		 return pbc->seek(*vs, frameIndex);
	}

	__declspec(dllexport) bool PlaybackControl_getRepeatEnabled(PlaybackControl* pbc)
	{
		 return pbc->getRepeatEnabled();
	}
	__declspec(dllexport) Status PlaybackControl_setRepeatEnabled(PlaybackControl* pbc, bool repeat)
	{
		 return pbc->setRepeatEnabled(repeat);
	}

	__declspec(dllexport) float PlaybackControl_getSpeed(PlaybackControl* pbc)
	{
		 return pbc->getSpeed();
	}
	__declspec(dllexport) Status PlaybackControl_setSpeed(PlaybackControl* pbc, float speed)
	{
		 return pbc->setSpeed(speed);
	}
};