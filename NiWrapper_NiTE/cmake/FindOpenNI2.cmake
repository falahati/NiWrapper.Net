# Simple OpenNI2 find module

include(FindPackageHandleStandardArgs)

if(WIN32)
  if(CMAKE_SIZEOF_VOID_P EQUAL 8)
    find_library(OpenNI2_LIBRARY OpenNI2 HINTS ENV OPENNI2_LIB64 ENV OPENNI_REDIST64 PATH_SUFFIXES ni2.3-dev ni2 DOC "OpenNI2 library")
    find_path(OpenNI2_INCLUDE OpenNI.h HINTS ENV OPENNI2_INCLUDE64 PATH_SUFFIXES ni2.3-dev ni2 DOC "OpenNI2 include path")
  else()
    find_library(OpenNI2_LIBRARY OpenNI2 HINTS ENV OPENNI2_LIB ENV OPENNI_REDIST PATH_SUFFIXES ni2.3-dev ni2 DOC "OpenNI2 library")
    find_path(OpenNI2_INCLUDE OpenNI.h HINTS ENV OPENNI2_INCLUDE PATH_SUFFIXES ni2.3-dev ni2 DOC "OpenNI2 include path")
  endif()
else()
  find_library(OpenNI2_LIBRARY OpenNI2 HINTS ENV OPENNI_REDIST PATH_SUFFIXES ni2.3-dev ni2 DOC "OpenNI2 library")
  find_path(OpenNI2_INCLUDE OpenNI.h HINTS ENV OPENNI2_INCLUDE PATH_SUFFIXES ni2.3-dev ni2 DOC "OpenNI2 include path")
endif()

find_package_handle_standard_args(
  OpenNI2
  DEFAULT_MSG
  OpenNI2_LIBRARY
  OpenNI2_INCLUDE)

if (OPENNI2_FOUND AND NOT TARGET OpenNI2)
  add_library(OpenNI2 SHARED IMPORTED)
  set_property(TARGET OpenNI2 PROPERTY INTERFACE_INCLUDE_DIRECTORIES ${OpenNI2_INCLUDE})
  if (WIN32)
    set_property(TARGET OpenNI2 PROPERTY IMPORTED_IMPLIB ${OpenNI2_LIBRARY})
  else()
    set_property(TARGET OpenNI2 PROPERTY IMPORTED_LOCATION ${OpenNI2_LIBRARY})
  endif()
endif()
