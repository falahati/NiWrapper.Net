cmake_minimum_required(VERSION 3.1)

project(NiWrapper.NiTE)

set(CMAKE_MODULE_PATH ${CMAKE_SOURCE_DIR}/cmake ${CMAKE_MODULE_PATH})

find_package(NiTE2 REQUIRED)

set(NITEWRAPPER_SRC
  Array.cpp
  Common.cpp
  GestureData.cpp
  HandData.cpp
  HandTracker.cpp
  HandTrackerFrameRef.cpp
  HandTracker_Listener.cpp
  NiTE.cpp
  PoseData.cpp
  Skeleton.cpp
  SkeletonJoint.cpp
  UserData.cpp
  UserMap.cpp
  UserTracker.cpp
  UserTrackerFrameRef.cpp
  UserTracker_Listener.cpp
  )

# Changed to underscore, because library names with a "dot"
# in them will not have .dll appended on Windows for DllImport()
# decorators
add_library(NiWrapper_NiTE SHARED ${NITEWRAPPER_SRC})
target_link_libraries(NiWrapper_NiTE OpenNI2 NiTE2)
