cmake_minimum_required(VERSION 3.1)

project(NiWrapper)

set(CMAKE_MODULE_PATH ${CMAKE_SOURCE_DIR}/cmake ${CMAKE_MODULE_PATH})
message(-- ${CMAKE_MODULE_PATH})
find_package(OpenNI2 REQUIRED)

set(NIWRAPPER_SRC
  Array.cpp
  CameraSettings.cpp
  Common.cpp
  CoordinateConverter.cpp
  Device.cpp
  DeviceInfo.cpp
  OpenNI.cpp
  OpenNI_Listener.cpp
  PlaybackControl.cpp
  Recorder.cpp
  SensorInfo.cpp
  VideoFrameRef.cpp
  VideoMode.cpp
  VideoStream.cpp
  VideoStream_Listener.cpp
  )

add_library(NiWrapper SHARED ${NIWRAPPER_SRC})
target_link_libraries(NiWrapper OpenNI2)
