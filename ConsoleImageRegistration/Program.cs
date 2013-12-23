namespace ConsoleImageRegistration
{
    using System;
    using System.Drawing;
    using OpenNIWrapper;

    class Program
    {
        private static Device device;

        private static VideoStream depthStream, colorStream;
        static void Main(string[] args)
        {
            try
            {
                OpenNI.Initialize();
                device = Device.Open(Device.ANY_DEVICE);

                depthStream = device.CreateVideoStream(Device.SensorType.DEPTH);
                depthStream.VideoMode = new VideoMode
                {
                    DataPixelFormat = VideoMode.PixelFormat.DEPTH_1MM,
                    FPS = 30,
                    Resolution = new Size(640, 480)
                };

                colorStream = device.CreateVideoStream(Device.SensorType.COLOR);
                colorStream.VideoMode = new VideoMode
                {
                    DataPixelFormat = VideoMode.PixelFormat.RGB888,
                    FPS = 30,
                    Resolution = new Size(640, 480)
                };
                device.DepthColorSyncEnabled = true;
                depthStream.Start();
                colorStream.Start();
                device.ImageRegistration = Device.ImageRegistrationMode.DEPTH_TO_COLOR;
                Console.WriteLine("Image registration is active and working well.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
            if (device != null)
                device.Close();
            OpenNI.Shutdown();
        }
    }
}
