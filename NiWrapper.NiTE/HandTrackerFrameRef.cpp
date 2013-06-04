#include <stdio.h>
#include "Array.cpp"
#include "OpenNI.h"
#include "NiTE.h"
using namespace nite;

extern "C"
{
	__declspec(dllexport) void HandTrackerFrameRef_release(HandTrackerFrameRef* vf)
	{
		vf->release();
		delete vf;
	}

	__declspec(dllexport) openni::VideoFrameRef* HandTrackerFrameRef_getDepthFrame(HandTrackerFrameRef* vf)
	{
		openni::VideoFrameRef* vp = new openni::VideoFrameRef(vf->getDepthFrame());
		return vp;
	}

	__declspec(dllexport) int  HandTrackerFrameRef_getFrameIndex(HandTrackerFrameRef* vf)
	{
		 return vf->getFrameIndex();
	}

	__declspec(dllexport) uint64_t HandTrackerFrameRef_getTimestamp(HandTrackerFrameRef* vf)
	{
		 return vf->getTimestamp();
	}

	__declspec(dllexport) bool HandTrackerFrameRef_isValid(HandTrackerFrameRef* vf)
	{
		 return vf->isValid();
	}

	__declspec(dllexport) WrapperArray HandTrackerFrameRef_getGestures(HandTrackerFrameRef* vf){
		WrapperArray* csarray = new WrapperArray();
		Array<GestureData>* dIArray = const_cast<Array<GestureData>*>(&(vf->getGestures()));
		csarray->Handle = &dIArray;
		csarray->Size = dIArray->getSize();
		GestureData** dP = new GestureData*[255];
		for (int i = 0; i < dIArray->getSize(); i++)
			dP[i] = const_cast<GestureData*>(&((*dIArray)[i]));
		csarray->Data = dP;
		return *csarray;
	}

	__declspec(dllexport) void HandTrackerFrameRef_destroyGesturesArray(WrapperArray p){
		delete[] p.Data;
		//delete &p;
	}

	__declspec(dllexport) WrapperArray HandTrackerFrameRef_getHands(HandTrackerFrameRef* vf){
		WrapperArray* csarray = new WrapperArray();
		Array<HandData>* dIArray = const_cast<Array<HandData>*>(&(vf->getHands()));
		csarray->Handle = &dIArray;
		csarray->Size = dIArray->getSize();
		HandData** dP = new HandData*[255];
		for (int i = 0; i < dIArray->getSize(); i++)
			dP[i] = const_cast<HandData*>(&((*dIArray)[i]));
		csarray->Data = dP;
		return *csarray;
	}

	__declspec(dllexport) void HandTrackerFrameRef_destroyHandsArray(WrapperArray p){
		delete[] p.Data;
		//delete &p;
	}

};