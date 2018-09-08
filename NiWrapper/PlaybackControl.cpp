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

#include "Defines.h"
#include "OpenNI.h"

using namespace openni;

extern "C" {

ONI_WRAPPER_API bool PlaybackControl_isValid(PlaybackControl* pbc)
{
	return pbc->isValid();
}

ONI_WRAPPER_API int PlaybackControl_getNumberOfFrames(PlaybackControl* pbc, VideoStream* vs)
{
	return pbc->getNumberOfFrames(*vs);
}

ONI_WRAPPER_API Status PlaybackControl_seek(PlaybackControl* pbc, VideoStream* vs, int frameIndex)
{
	return pbc->seek(*vs, frameIndex);
}

ONI_WRAPPER_API bool PlaybackControl_getRepeatEnabled(PlaybackControl* pbc)
{
	return pbc->getRepeatEnabled();
}

ONI_WRAPPER_API Status PlaybackControl_setRepeatEnabled(PlaybackControl* pbc, bool repeat)
{
	return pbc->setRepeatEnabled(repeat);
}

ONI_WRAPPER_API float PlaybackControl_getSpeed(PlaybackControl* pbc)
{
	return pbc->getSpeed();
}

ONI_WRAPPER_API Status PlaybackControl_setSpeed(PlaybackControl* pbc, float speed)
{
	return pbc->setSpeed(speed);
}
};
