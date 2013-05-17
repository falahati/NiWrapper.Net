#include <stdio.h>
#include "Array.cpp"
#include "OpenNI.h"
#include "NiTE.h"
using namespace nite;

extern "C"
{
	__declspec(dllexport) SkeletonJoint* Skeleton_getJoint(Skeleton* sk, JointType type)
	{
		 return const_cast<SkeletonJoint*>(&sk->getJoint(type));
	}
	__declspec(dllexport) SkeletonState Skeleton_getState(Skeleton* sk)
	{
		 return sk->getState();
	}
};