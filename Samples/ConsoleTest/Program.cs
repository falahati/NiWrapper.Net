using System;
using OpenNIWrapper;

namespace ConsoleTest
{
    class Program
    {
        static int eventDepth = 0, eventColor = 0, inlineDepth = 0, inlineColor = 0;
        static bool HandleError(OpenNI.Status status)
        {
            if (status == OpenNI.Status.Ok)
                return true;
            Console.WriteLine("Error: " + status.ToString() + " - " + OpenNI.LastError);
            Console.ReadLine();
            return false;
        }
        static void Main(string[] args)
        {
            OpenNI.Status status;
            Console.WriteLine(OpenNI.Version.ToString());
            status = OpenNI.Initialize();
            if (!HandleError(status)) { Environment.Exit(0); }
            OpenNI.OnDeviceConnected += new OpenNI.DeviceConnectionStateChanged(OpenNI_onDeviceConnected);
            OpenNI.OnDeviceDisconnected += new OpenNI.DeviceConnectionStateChanged(OpenNI_onDeviceDisconnected);
            DeviceInfo[] devices = OpenNI.EnumerateDevices();
            if (devices.Length == 0)
                return;
            Device device;
            using (device = Device.Open(null,"lr")) // lean init and no reset flags
            {	
                VideoStream depth;
                SensorInfo sensorInfo = device.GetSensorInfo(Device.SensorType.Depth);
	            if (sensorInfo != null)
	            {
		            depth = VideoStream.Create(device, OpenNIWrapper.Device.SensorType.Depth);
	            }


                if (device.HasSensor(Device.SensorType.Depth) &&
                    device.HasSensor(Device.SensorType.Color))
                {
                    VideoStream depthStream = device.CreateVideoStream(Device.SensorType.Depth);
                    VideoStream colorStream = device.CreateVideoStream(Device.SensorType.Color);
                    if (depthStream.IsValid && colorStream.IsValid)
                    {
                        if (!HandleError(depthStream.Start())) { OpenNI.Shutdown(); return; }
                        if (!HandleError(colorStream.Start())) { OpenNI.Shutdown(); return; }
                        new System.Threading.Thread(new System.Threading.ThreadStart(DisplayInfo)).Start();
                        depthStream.OnNewFrame += new VideoStream.VideoStreamNewFrame(depthStream_onNewFrame);
                        colorStream.OnNewFrame += new VideoStream.VideoStreamNewFrame(colorStream_onNewFrame);
                        VideoStream[] array = new VideoStream[] { depthStream, colorStream };
                        while (!Console.KeyAvailable)
                        {
                            VideoStream aS;
                            if (OpenNI.WaitForAnyStream(array, out aS) == OpenNI.Status.Ok)
                            {
                                if (aS.Equals(colorStream))
                                    inlineColor++;
                                else
                                    inlineDepth++;
                                aS.ReadFrame().Release();
                            }
                        }
                        
                    }
                }
                Console.ReadLine();
            }
            OpenNI.Shutdown();
            Environment.Exit(0);
        }

        static void depthStream_onNewFrame(VideoStream vStream)
        {
            eventDepth++;
        }
        static void colorStream_onNewFrame(VideoStream vStream)
        {
            eventColor++;
        }

        static void OpenNI_onDeviceDisconnected(DeviceInfo Device)
        {
            Console.WriteLine(Device.Name + " Disconnected ...");
        }

        static void OpenNI_onDeviceConnected(DeviceInfo Device)
        {
            Console.WriteLine(Device.Name + " Connected ...");
        }

        static int lUpdate;
        static void DisplayInfo()
        {
            while (true)
            {
                if (lUpdate == 0)
                {
                    lUpdate = Environment.TickCount;
                    continue;
                }
                if (Environment.TickCount - lUpdate > 1000)
                {
                    lUpdate = Environment.TickCount;
                    Console.Clear();
                    Console.WriteLine("Inline Depth: " + inlineDepth.ToString() + " - Inline Color: " + inlineColor.ToString() +
                        " - Event Depth: " + eventDepth.ToString() + " - Event Color: " + eventColor.ToString());
                    inlineDepth = inlineColor = eventDepth = eventColor = 0;
                }
                else
                    continue;
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
