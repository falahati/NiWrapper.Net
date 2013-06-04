#include <stdio.h>
#include "NiTE.h"
using namespace nite;

class HandTracker_Listener : public HandTracker::NewFrameListener
{
	public:
		typedef void (*del_NewFrame)(HandTracker* ut);
		HandTracker_Listener() : event_NewFrame( NULL ) { }
		void SetNewFrameCallback(del_NewFrame callback)
		{	event_NewFrame = callback;	}
		void onNewFrame(HandTracker& ut){
			if (event_NewFrame != NULL)
				event_NewFrame(&ut);
		}
	private:
		del_NewFrame event_NewFrame;
};
