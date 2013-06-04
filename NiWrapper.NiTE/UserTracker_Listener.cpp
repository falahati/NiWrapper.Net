#include <stdio.h>
#include "NiTE.h"
using namespace nite;

class UserTracker_Listener : public UserTracker::NewFrameListener
{
	public:
		typedef void (*del_NewFrame)(UserTracker* ut);
		UserTracker_Listener() : event_NewFrame( NULL ) { }
		void SetNewFrameCallback(del_NewFrame callback)
		{	event_NewFrame = callback;	}
		void onNewFrame(UserTracker& ut){
			if (event_NewFrame != NULL)
				event_NewFrame(&ut);
		}
	private:
		del_NewFrame event_NewFrame;
};
