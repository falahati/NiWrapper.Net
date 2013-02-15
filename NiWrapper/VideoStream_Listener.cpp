#include <stdio.h>
#include "OpenNI.h"
using namespace openni;

class VideoStream_Listener : public VideoStream::NewFrameListener
{
	public:
		typedef void (*del_NewFrame)(VideoStream* vs);
		VideoStream_Listener() : event_NewFrame( NULL ) { }
		void SetNewFrameCallback(del_NewFrame callback)
		{	event_NewFrame = callback;	}
		void onNewFrame(VideoStream& vs){
			if (event_NewFrame != NULL)
				event_NewFrame(&vs);
		}
	private:
		del_NewFrame event_NewFrame;
};
