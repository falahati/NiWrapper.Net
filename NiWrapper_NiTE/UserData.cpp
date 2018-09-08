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
#include "Defines.h"
#include "NiTE.h"
using namespace nite;

extern "C" {
NITE_WRAPPER_API void UserData_getBoundingBox(UserData* ud,
                                              float* minX, float* minY, float* minZ, float* maxX, float* maxY,
                                              float* maxZ)
{
	BoundingBox bb = ud->getBoundingBox();
	*minX = bb.min.x;
	*minY = bb.min.y;
	*minZ = bb.min.z;
	*maxX = bb.max.x;
	*maxY = bb.max.y;
	*maxZ = bb.max.z;
}

NITE_WRAPPER_API void UserData_getCenterOfMass(UserData* ud,
                                               float* X, float* Y, float* Z)
{
	Point3f p = ud->getCenterOfMass();
	*X = p.x;
	*Y = p.y;
	*Z = p.z;
}

NITE_WRAPPER_API UserId UserData_getId(UserData* ud)
{
	return ud->getId();
}

NITE_WRAPPER_API PoseData* UserData_getPose(UserData* ud, PoseType type)
{
	return const_cast<PoseData*>(&ud->getPose(type));
}

NITE_WRAPPER_API Skeleton* UserData_getSkeleton(UserData* ud)
{
	return const_cast<Skeleton*>(&ud->getSkeleton());
}

NITE_WRAPPER_API bool UserData_isLost(UserData* ud)
{
	return ud->isLost();
}

NITE_WRAPPER_API bool UserData_isNew(UserData* ud)
{
	return ud->isNew();
}

NITE_WRAPPER_API bool UserData_isVisible(UserData* ud)
{
	return ud->isVisible();
}

};
