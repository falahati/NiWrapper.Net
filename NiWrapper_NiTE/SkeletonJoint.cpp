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
NITE_WRAPPER_API void SkeletonJoint_getOrientation(SkeletonJoint* sj,
                                                   float* x, float* y, float* z, float* w)
{
	Quaternion q = sj->getOrientation();
	*x = q.x;
	*y = q.y;
	*z = q.z;
	*w = q.w;
}

NITE_WRAPPER_API void SkeletonJoint_getPosition(SkeletonJoint* sj,
                                                float* x, float* y, float* z)
{
	Point3f q = sj->getPosition();
	*x = q.x;
	*y = q.y;
	*z = q.z;
}

NITE_WRAPPER_API float SkeletonJoint_getOrientationConfidence(SkeletonJoint* sj)
{
	return sj->getOrientationConfidence();
}

NITE_WRAPPER_API float SkeletonJoint_getPositionConfidence(SkeletonJoint* sj)
{
	return sj->getPositionConfidence();
}

NITE_WRAPPER_API JointType SkeletonJoint_getType(SkeletonJoint* sj)
{
	return sj->getType();
}
};
