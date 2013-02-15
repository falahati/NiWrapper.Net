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