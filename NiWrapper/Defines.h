#ifndef NI_WRAPPER_DEFINES_H
#define NI_WRAPPER_DEFINES_H

#if defined(_MSC_VER) || defined(__MINGW32__)
#define ONI_WRAPPER_API __declspec(dllexport)
#else
    #define ONI_WRAPPER_API
#endif

#endif /* NI_WRAPPER_DEFINES_H */
