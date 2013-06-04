#include <stdio.h>
#include "Array.cpp"
#include "OpenNI.h"
#include "NiTE.h"
using namespace nite;

extern "C"
{
	__declspec(dllexport) void UserTrackerFrameRef_release(UserTrackerFrameRef* vf)
	{
		vf->release();
		delete vf;
	}

	__declspec(dllexport) openni::VideoFrameRef* UserTrackerFrameRef_getDepthFrame(UserTrackerFrameRef* vf)
	{
		openni::VideoFrameRef* vp = new openni::VideoFrameRef(vf->getDepthFrame());
		return vp;
	}

	__declspec(dllexport) void UserTrackerFrameRef_getFloor(UserTrackerFrameRef* vf,
		float *Px, float *Py, float *Pz, float *Nx, float *Ny, float *Nz)
	{
		 Plane pl = vf->getFloor();
		 *Px = pl.point.x; *Py = pl.point.y; *Pz = pl.point.z;
		 *Nx = pl.normal.x; *Ny = pl.normal.y; *Nz = pl.normal.z;
	}

	__declspec(dllexport) float UserTrackerFrameRef_getFloorConfidence(UserTrackerFrameRef* vf)
	{
		 return vf->getFloorConfidence();
	}

	__declspec(dllexport) int  UserTrackerFrameRef_getFrameIndex(UserTrackerFrameRef* vf)
	{
		 return vf->getFrameIndex();
	}

	__declspec(dllexport) uint64_t UserTrackerFrameRef_getTimestamp(UserTrackerFrameRef* vf)
	{
		 return vf->getTimestamp();
	}

	__declspec(dllexport) bool UserTrackerFrameRef_isValid(UserTrackerFrameRef* vf)
	{
		 return vf->isValid();
	}

	__declspec(dllexport) UserData* UserTrackerFrameRef_getUserById(UserTrackerFrameRef* vf, UserId id)
	{
		 return const_cast<UserData*>(vf->getUserById(id));
	}

	__declspec(dllexport) UserMap* UserTrackerFrameRef_getUserMap(UserTrackerFrameRef* vf)
	{
		 return const_cast<UserMap*>(&(vf->getUserMap()));
	}

	__declspec(dllexport) WrapperArray UserTrackerFrameRef_getUsers(UserTrackerFrameRef* vf){
		WrapperArray* csarray = new WrapperArray();
		Array<UserData>* dIArray = const_cast<Array<UserData>*>(&(vf->getUsers()));
		csarray->Handle = &dIArray;
		csarray->Size = dIArray->getSize();
		UserData** dP = new UserData*[255];
		for (int i = 0; i < dIArray->getSize(); i++)
			dP[i] = const_cast<UserData*>(&((*dIArray)[i]));
		csarray->Data = dP;
		return *csarray;
	}

	__declspec(dllexport) void UserTrackerFrameRef_destroyUsersArray(WrapperArray p){
		delete[] p.Data;
		//delete &p;
	}

};