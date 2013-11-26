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
#include "OpenNI.h"
using namespace openni;

extern "C"
{
	enum copyBitmapOptions
	{
		None = 0,
		Force24BitRGB = 1,
		DepthFillLeftBlack = 2,
		DepthFillRigthBlack = 4,
		DepthHistogramEqualize = 8,
		DepthInvert = 16,
		DepthFillShadow = 32,
	};

	__declspec(dllexport) void VideoFrameRef_release(VideoFrameRef* vf)
	{
		 vf->release();
	}
	
	__declspec(dllexport) VideoMode* VideoFrameRef_getVideoMode(VideoFrameRef* vf)
	{
		 return const_cast<VideoMode*>(&vf->getVideoMode());
	}

	__declspec(dllexport) bool VideoFrameRef_isValid(VideoFrameRef* vf)
	{
		 return vf->isValid();
	}

	__declspec(dllexport) bool VideoFrameRef_getCroppingOrigin(VideoFrameRef* vf, int* pOriginX, int* pOriginY)
	{
		if (!vf->getCroppingEnabled())
			return false;
		 *pOriginX = vf->getCropOriginX();
		 *pOriginY = vf->getCropOriginY();
		 return true;
	}

	__declspec(dllexport) void* VideoFrameRef_getData(VideoFrameRef* vf)
	{
		return const_cast<void*>(vf->getData());
	}

	__declspec(dllexport) void VideoFrameRef_copyDataTo(VideoFrameRef* vf, void* dstData, int dstStride, copyBitmapOptions options)
	{
		int width = vf->getWidth();
		int height = vf->getHeight();
		int stride = vf->getStrideInBytes();
		const void* data = vf->getData();
		PixelFormat pixelFormat = vf->getVideoMode().getPixelFormat();
		if (pixelFormat == PIXEL_FORMAT_RGB888){
			// BGR24 TO RGB24
			for	(int y = 0; y < height; ++y)
			{
				OniRGB888Pixel* destPixel = (OniRGB888Pixel*)((char*)dstData + (y * dstStride));
				OniRGB888Pixel* srcPixel = (OniRGB888Pixel*)((char*)data + (y * stride));
				for	(int x = 0; x < width;	
					++x, ++destPixel, ++srcPixel)
				{
					destPixel->b = srcPixel->r;
					destPixel->g = srcPixel->g;
					destPixel->r = srcPixel->b;
				}
			}
		}else if ((options & Force24BitRGB) == Force24BitRGB && 
					pixelFormat == PIXEL_FORMAT_GRAY8){
			// GRAY8 TO RGB24
			for	(int y = 0; y < height; ++y)
			{
				OniRGB888Pixel* destPixel = (OniRGB888Pixel*)((char*)dstData + (y * dstStride));
				char* srcPixel = ((char*)data + (y * stride));
				for	(int x = 0; x < width;	
					++x, ++destPixel, ++srcPixel)
					destPixel->b = destPixel->g = destPixel->r = *srcPixel;
			}
		}else if ((options & Force24BitRGB) == Force24BitRGB && 
					pixelFormat == PIXEL_FORMAT_GRAY16){
			// GRAY16 TO RGB24
			uint16_t maxValue = 0;
			uint16_t minValue = 0xFFFF;
			int dataSize = vf->getDataSize();
			for	(int y = 0; y < height; ++y)
			{
				uint16_t* srcPixel = (uint16_t*)((char*)data + (y * stride));
				for	(int x = 0; x < width;	
					++x, ++srcPixel)
				{
					if (*srcPixel > maxValue)
						maxValue = *srcPixel;
					if (*srcPixel > 0 && *srcPixel < minValue)
						minValue = *srcPixel;
				}
			}
			double pixelRate = (double)255 / (maxValue - minValue);
			for	(int y = 0; y < height; ++y)
			{
				OniRGB888Pixel* destPixel = (OniRGB888Pixel*)((char*)dstData + (y * dstStride));
				uint16_t* srcPixel = (uint16_t*)((char*)data + (y * stride));
				for	(int x = 0; x < width;	
					++x, ++destPixel, ++srcPixel)
					destPixel->b = destPixel->g = destPixel->r = (char)((*srcPixel - minValue) * pixelRate);
			}
		}else if (	pixelFormat == PIXEL_FORMAT_GRAY8 ||
					pixelFormat == PIXEL_FORMAT_GRAY16){
			// GRAY8 TO GRAY8 or
			// GRAY16 TO GRAY16
			for	(int y = 0; y < height; ++y)
			{
				char* destPixel = ((char*)dstData + (y * dstStride));
				char* srcPixel = ((char*)data + (y * stride));
				memcpy(destPixel, srcPixel, stride);
			}
		}else if (pixelFormat == PIXEL_FORMAT_DEPTH_1_MM || pixelFormat == PIXEL_FORMAT_DEPTH_100_UM)
		{
			DepthPixel maxDepth = 0;
			DepthPixel minDepth = 0xFFFF;
			int dataSize = vf->getDataSize();
			for	(int y = 0; y < height; ++y)
			{
				DepthPixel* srcPixel = (DepthPixel*)((char*)data + (y * stride));
				for	(int x = 0; x < width;	
					++x, ++srcPixel)
				{
					if (*srcPixel > maxDepth)
						maxDepth = *srcPixel;
					if (*srcPixel > 0 && *srcPixel < minDepth)
						minDepth = *srcPixel;
				}
			}

			bool histogram = ((options & DepthHistogramEqualize) == DepthHistogramEqualize);
			int depthHistogram[65536];
			int numberOfPoints = 0;
			if (histogram)
			{
				memset(depthHistogram, 0,
					sizeof(depthHistogram));
				for	(int y = 0; y < height; ++y)
				{
					DepthPixel* depthCell = (DepthPixel*)((char*)data + (y * stride));
					for	(int x = 0; x < width; ++x, ++depthCell)
					{
						if (*depthCell != 0)
						{
							depthHistogram[*depthCell]++;
							numberOfPoints++;
						}
					}
				}

				for (int nIndex=1;
				nIndex < sizeof(depthHistogram) / sizeof(int);
				nIndex++)
					depthHistogram[nIndex] += depthHistogram[nIndex-1];
			}

			bool invert = ((options & DepthInvert) == DepthInvert);
			bool shadow = ((options & DepthFillShadow) == DepthFillShadow);
			bool fillShadow = shadow && ((options & DepthFillLeftBlack) == DepthFillLeftBlack || (options & DepthFillRigthBlack) == DepthFillRigthBlack);
			bool reverse = fillShadow && ((options & DepthFillRigthBlack) == DepthFillRigthBlack);
			if ((options & Force24BitRGB) == Force24BitRGB){
				// Depth16 TO RGB24
				double pixelRate = (double)255 / (maxDepth - minDepth);
				for	(int y = 0; y < height; ++y)
				{
					int rPow = (reverse) ? y + 1 : y;
					RGB888Pixel* destPixel = (RGB888Pixel*)((char*)dstData + (rPow * dstStride));
					DepthPixel* srcPixel = (DepthPixel*)((char*)data + (rPow * stride));
					char lastPixel = 0;
					for	(int x = 0; x < width; ++x)
					{
						if (reverse)
						{
							--destPixel;
							--srcPixel;
						}
						if (*srcPixel > 0)
						{
							char depthValue = 0;
							if (histogram)
								if (invert)
									depthValue = (char)((1 - ((float)depthHistogram[*srcPixel]  / numberOfPoints)) * 255);
								else
									depthValue = (char)(((float)depthHistogram[*srcPixel]  / numberOfPoints) * 255);
							else
								if (invert)
									depthValue = (char)((maxDepth - *srcPixel) * pixelRate);
								else
									depthValue = (char)((*srcPixel - minDepth) * pixelRate);
							lastPixel = destPixel->b = destPixel->g = destPixel->r = depthValue;
						}else if (shadow)
							destPixel->b = destPixel->g = destPixel->r = (fillShadow) ? lastPixel : 0;
						if (!reverse)
						{
							++destPixel;
							++srcPixel;
						}
					}
				}
			}else{
				// Depth16 TO Gray16
				double pixelRate = (double)65535 / (maxDepth - minDepth);
				for	(int y = 0; y < height; ++y)
				{
					int rPow = (reverse) ? y + 1 : y;
					uint16_t* destPixel = (uint16_t*)((char*)dstData + (rPow * dstStride));
					DepthPixel* srcPixel = (DepthPixel*)((char*)data + (rPow * stride));
					uint16_t lastPixel = 0;
					for	(int x = 0; x < width;	
						++x, ++destPixel, ++srcPixel)
					{
						if (reverse)
						{
							--destPixel;
							--srcPixel;
						}
						if (*srcPixel > 0)
						{
							uint16_t depthValue = 0;
							if (histogram)
								if (invert)
									depthValue = (uint16_t)((1 - ((float)depthHistogram[*srcPixel]  / numberOfPoints)) * 65535);
								else
									depthValue = (uint16_t)(((float)depthHistogram[*srcPixel]  / numberOfPoints) * 65535);
							else
								if (invert)
									depthValue = (uint16_t)((maxDepth - *srcPixel) * pixelRate);
								else
									depthValue = (uint16_t)((*srcPixel - minDepth) * pixelRate);
							lastPixel = *destPixel = depthValue;
						}
						else if (shadow)
							*destPixel = (fillShadow) ? lastPixel : 0;
						if (!reverse)
						{
							++destPixel;
							++srcPixel;
						}
					}
				}
			}
		}
	}

	__declspec(dllexport) int VideoFrameRef_getDataSize(VideoFrameRef* vf)
	{
		 return vf->getDataSize();
	}

	__declspec(dllexport) int VideoFrameRef_getFrameIndex(VideoFrameRef* vf)
	{
		 return vf->getFrameIndex();
	}
	__declspec(dllexport) void VideoFrameRef_getSize(VideoFrameRef* vf, int* width, int* height)
	{
		*height = vf->getHeight();
		*width = vf->getWidth();
	}
	__declspec(dllexport) SensorType VideoFrameRef_getSensorType(VideoFrameRef* vf)
	{
		 return vf->getSensorType();
	}
	__declspec(dllexport) int VideoFrameRef_getStrideInBytes(VideoFrameRef* vf)
	{
		 return vf->getStrideInBytes();
	}
	__declspec(dllexport) uint64_t VideoFrameRef_getTimestamp(VideoFrameRef* vf)
	{
		 return vf->getTimestamp();
	}
};