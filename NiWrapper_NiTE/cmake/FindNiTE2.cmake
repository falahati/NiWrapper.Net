# Simple NiTE2 find module

include(FindPackageHandleStandardArgs)

find_package(OpenNI2 REQUIRED)

find_library(NiTE2_LIBRARY NiTE2 HINTS ENV NITE2_REDIST)
find_path(NiTE2_INCLUDE NiTE.h HINTS ENV NITE2_INCLUDE PATH_SUFFIXES NiTE2)

find_package_handle_standard_args(
  NiTE2
  DEFAULT_MSG
  NiTE2_LIBRARY
  NiTE2_INCLUDE
  OpenNI2_FOUND)

if (NiTE2_FOUND AND NOT TARGET NiTE2)
  add_library(NiTE2 SHARED IMPORTED)
  set_property(TARGET NiTE2 PROPERTY INTERFACE_INCLUDE_DIRECTORIES ${NiTE2_INCLUDE})
  if (WIN32)
    set_property(TARGET NiTE2 PROPERTY IMPORTED_IMPLIB ${NiTE2_LIBRARY})
  else()
    set_property(TARGET NiTE2 PROPERTY IMPORTED_LOCATION ${NiTE2_LIBRARY})
  endif()
endif()
