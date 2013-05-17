#include <stdio.h>
#include "OpenNI.h"
using namespace openni;

extern "C"
{

	__declspec(dllexport) Status CoordinateConverter_convertDepthToColor (
		VideoStream* depthStream, VideoStream* colorStream, int depthX, int depthY, DepthPixel depthZ,
		int *pColorX, int *pColorY)
	{
		 return CoordinateConverter::convertDepthToColor(*depthStream, *colorStream, depthX, depthY, depthZ, pColorX, pColorY);
	}

	__declspec(dllexport) Status CoordinateConverter_convertDepthToWorld (
		VideoStream* depthStream, int depthX, int depthY, DepthPixel depthZ,
		float *pWorldX, float *pWorldY, float *pWorldZ)
	{
		 return CoordinateConverter::convertDepthToWorld(*depthStream, depthX, depthY, depthZ, pWorldX, pWorldY, pWorldZ);
	}

	__declspec(dllexport) Status CoordinateConverter_convertDepthToWorld_Float (
		VideoStream* depthStream, float depthX, float depthY, float depthZ,
		float *pWorldX, float *pWorldY, float *pWorldZ)
	{
		 return CoordinateConverter::convertDepthToWorld(*depthStream, depthX, depthY, depthZ, pWorldX, pWorldY, pWorldZ);
	}

	__declspec(dllexport) Status CoordinateConverter_convertWorldToDepth (
		VideoStream* depthStream, float worldX, float worldY, float worldZ,
		int *pDepthX, int *pDepthY, DepthPixel *pDepthZ)
	{
		return CoordinateConverter::convertWorldToDepth(*depthStream, worldX, worldY, worldZ, pDepthX, pDepthY, pDepthZ);
	}

	__declspec(dllexport) Status CoordinateConverter_convertWorldToDepth_Float (
		VideoStream* depthStream, float worldX, float worldY, float worldZ,
		float *pDepthX, float *pDepthY, float *pDepthZ)
	{
		return CoordinateConverter::convertWorldToDepth(*depthStream, worldX, worldY, worldZ, pDepthX, pDepthY, pDepthZ);
	}

};