using System;
using System.Threading;
using OpenNIWrapper;

namespace ConsoleTest
{
    #region

    #endregion

    public static class Program
    {
        #region Public Methods and Operators

        public static bool HandleError(OpenNI.Status status)
        {
            if (status == OpenNI.Status.Ok)
            {
                return true;
            }

            Console.WriteLine("Error: " + status + " - " + OpenNI.LastError);
            Console.ReadLine();

            return false;
        }

        #endregion

        #region Static Fields

        private static int eventColor;

        private static int eventDepth;

        private static int inlineColor;

        private static int inlineDepth;

        private static long lastUpdate;

        #endregion

        #region Methods

        private static void DisplayInfo()
        {
            while (!Console.KeyAvailable)
            {
                if (lastUpdate == 0)
                {
                    lastUpdate = DateTime.Now.Ticks;

                    continue;
                }

                if (DateTime.Now.Ticks - lastUpdate > 1000)
                {
                    lastUpdate = Environment.TickCount;
                    //Console.Clear();
                    Console.WriteLine(
                        "Inline Depth: " +
                        inlineDepth +
                        " - Inline Color: " +
                        inlineColor +
                        " - Event Depth: " +
                        eventDepth +
                        " - Event Color: " +
                        eventColor);
                    inlineDepth = inlineColor = eventDepth = eventColor = 0;
                }
                else
                {
                    continue;
                }

                Thread.Sleep(100);
            }
        }

        private static void Main()
        {
            Console.WriteLine(OpenNI.Version.ToString());
            var status = OpenNI.Initialize();

            if (!HandleError(status))
            {
                Environment.Exit(0);
            }

            OpenNI.OnDeviceConnected += OpenNiOnDeviceConnected;
            OpenNI.OnDeviceDisconnected += OpenNiOnDeviceDisconnected;
            var devices = OpenNI.EnumerateDevices();

            if (devices.Length == 0)
            {
                return;
            }

            Device device;

            // lean init and no reset flags
            using (device = Device.Open(null, "lr"))
            {
                if (device.HasSensor(Device.SensorType.Depth) && device.HasSensor(Device.SensorType.Color))
                {
                    var depthStream = device.CreateVideoStream(Device.SensorType.Depth);
                    var colorStream = device.CreateVideoStream(Device.SensorType.Color);

                    if (depthStream.IsValid && colorStream.IsValid)
                    {
                        if (!HandleError(depthStream.Start()))
                        {
                            OpenNI.Shutdown();

                            return;
                        }

                        if (!HandleError(colorStream.Start()))
                        {
                            OpenNI.Shutdown();

                            return;
                        }

                        var workThread = new Thread(DisplayInfo);
                        workThread.Start();
                        depthStream.OnNewFrame += DepthStreamOnNewFrame;
                        colorStream.OnNewFrame += ColorStreamOnNewFrame;
                        VideoStream[] array = {depthStream, colorStream};

                        while (!Console.KeyAvailable)
                        {
                            VideoStream aS;

                            if (OpenNI.WaitForAnyStream(array, out aS) == OpenNI.Status.Ok)
                            {
                                if (aS.Equals(colorStream))
                                {
                                    inlineColor++;
                                }
                                else
                                {
                                    inlineDepth++;
                                }

                                aS.ReadFrame().Dispose();
                            }
                        }
                    }
                }

                Console.ReadLine();
            }

            OpenNI.Shutdown();
            Environment.Exit(0);
        }

        private static void OpenNiOnDeviceConnected(DeviceInfo device)
        {
            Console.WriteLine(device.Name + " Connected ...");
        }

        private static void OpenNiOnDeviceDisconnected(DeviceInfo device)
        {
            Console.WriteLine(device.Name + " Disconnected ...");
        }

        private static void ColorStreamOnNewFrame(VideoStream videoStream)
        {
            eventColor++;
        }

        private static void DepthStreamOnNewFrame(VideoStream videoStream)
        {
            eventDepth++;
        }

        #endregion
    }
}