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
#include "NiTE.h"
using namespace nite;

extern "C" {
NITE_WRAPPER_API void HandData_getPosition(HandData* hd,
                                           float* X, float* Y, float* Z)
{
	Point3f p = hd->getPosition();
	*X = p.x;
	*Y = p.y;
	*Z = p.z;
}

NITE_WRAPPER_API HandId HandData_getId(HandData* hd)
{
	return hd->getId();
}

NITE_WRAPPER_API bool HandData_isLost(HandData* hd)
{
	return hd->isLost();
}

NITE_WRAPPER_API bool HandData_isNew(HandData* hd)
{
	return hd->isNew();
}

NITE_WRAPPER_API bool HandData_isTracking(HandData* hd)
{
	return hd->isTracking();
}

NITE_WRAPPER_API bool HandData_isTouchingFov(HandData* hd)
{
	return hd->isTouchingFov();
}

};
