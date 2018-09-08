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
NITE_WRAPPER_API int UserMap_getStride(UserMap* um)
{
	return um->getStride();
}

NITE_WRAPPER_API void UserMap_getSize(UserMap* um, int* w, int* h)
{
	*w = um->getWidth();
	*h = um->getHeight();
}

NITE_WRAPPER_API UserId* UserMap_getPixels(UserMap* um)
{
	return const_cast<UserId*>(um->getPixels());
}
};
