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
#include "Array.cpp"
#include "OpenNI.h"
#include "NiTE.h"
using namespace nite;

extern "C" {
NITE_WRAPPER_API void UserTrackerFrameRef_release(UserTrackerFrameRef* vf)
{
	vf->release();
	delete vf;
}

NITE_WRAPPER_API openni::VideoFrameRef* UserTrackerFrameRef_getDepthFrame(UserTrackerFrameRef* vf)
{
	openni::VideoFrameRef* vp = new openni::VideoFrameRef(vf->getDepthFrame());
	return vp;
}

NITE_WRAPPER_API void UserTrackerFrameRef_getFloor(UserTrackerFrameRef* vf,
                                                   float* Px, float* Py, float* Pz, float* Nx, float* Ny, float* Nz)
{
	Plane pl = vf->getFloor();
	*Px = pl.point.x;
	*Py = pl.point.y;
	*Pz = pl.point.z;
	*Nx = pl.normal.x;
	*Ny = pl.normal.y;
	*Nz = pl.normal.z;
}

NITE_WRAPPER_API float UserTrackerFrameRef_getFloorConfidence(UserTrackerFrameRef* vf)
{
	return vf->getFloorConfidence();
}

NITE_WRAPPER_API int UserTrackerFrameRef_getFrameIndex(UserTrackerFrameRef* vf)
{
	return vf->getFrameIndex();
}

NITE_WRAPPER_API uint64_t UserTrackerFrameRef_getTimestamp(UserTrackerFrameRef* vf)
{
	return vf->getTimestamp();
}

NITE_WRAPPER_API bool UserTrackerFrameRef_isValid(UserTrackerFrameRef* vf)
{
	return vf->isValid();
}

NITE_WRAPPER_API UserData* UserTrackerFrameRef_getUserById(UserTrackerFrameRef* vf, UserId id)
{
	return const_cast<UserData*>(vf->getUserById(id));
}

NITE_WRAPPER_API UserMap* UserTrackerFrameRef_getUserMap(UserTrackerFrameRef* vf)
{
	return const_cast<UserMap*>(&(vf->getUserMap()));
}

NITE_WRAPPER_API WrapperArray UserTrackerFrameRef_getUsers(UserTrackerFrameRef* vf)
{
	WrapperArray csarray;
	Array<UserData>* dIArray = const_cast<Array<UserData>*>(&(vf->getUsers()));
	csarray.Handle = &dIArray;
	csarray.Size = dIArray->getSize();
	UserData** dP = new UserData*[255];
	for (int i = 0; i < dIArray->getSize(); i++)
		dP[i] = const_cast<UserData*>(&((*dIArray)[i]));
	csarray.Data = dP;
	return csarray;
}

NITE_WRAPPER_API void UserTrackerFrameRef_destroyUsersArray(WrapperArray p)
{
	UserData** array = reinterpret_cast<UserData**>(p.Data);
	delete [] array;
}

};
