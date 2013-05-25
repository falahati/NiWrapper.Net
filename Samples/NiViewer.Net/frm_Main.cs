using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenNIWrapper;

namespace NiViewer.Net
{
    public partial class frm_Main : Form
    {
        Bitmap bitmap;
        public frm_Main()
        {
            InitializeComponent();
            bitmap = new Bitmap(1, 1);
        }

        private bool HandleError(OpenNI.Status status)
        {
            if (status == OpenNI.Status.OK)
                return true;
            MessageBox.Show("Error: " + status.ToString() + " - " + OpenNI.LastError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return false;
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            HandleError(OpenNI.Initialize());
            OpenNI.onDeviceConnected += new OpenNI.DeviceConnectionStateChanged(OpenNI_onDeviceConnectionStateChanged);
            OpenNI.onDeviceDisconnected += new OpenNI.DeviceConnectionStateChanged(OpenNI_onDeviceConnectionStateChanged);
            UpdateDevicesList();
        }

        void OpenNI_onDeviceConnectionStateChanged(DeviceInfo Device)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                UpdateDevicesList();
            }));
        }

        private void UpdateDevicesList()
        {
            DeviceInfo[] devices = OpenNI.EnumerateDevices();
            cb_devices.Items.Clear();
            if (devices.Length == 0)
            {
                
            }
            for (int i = 0; i < devices.Length; i++)
			{
                cb_devices.Items.Add(devices[i]);
			}
        }

        Device currentDevice;
        private void cb_devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            cb_sensor.Items.Clear();
            if (cb_devices.SelectedItem != null)
            {
                if (currentDevice != null) currentDevice.Dispose();
                currentDevice = ((DeviceInfo)cb_devices.SelectedItem).OpenDevice();
                if (currentDevice.hasSensor(Device.SensorType.COLOR)) cb_sensor.Items.Add("Color");
                if (currentDevice.hasSensor(Device.SensorType.DEPTH)) cb_sensor.Items.Add("Depth");
                if (currentDevice.hasSensor(Device.SensorType.IR)) cb_sensor.Items.Add("IR");
            }
        }
        VideoStream currentSensor;
        private void cb_sensor_SelectedIndexChanged(object sender, EventArgs e)
        {
            cb_videomode.Items.Clear();
            if (cb_sensor.SelectedItem != null && currentDevice != null)
            {
                if (currentSensor != null && currentSensor.isValid) currentSensor.Stop();
                switch ((string)(cb_sensor.SelectedItem))
                {
                    case "Color":
                        currentSensor = currentDevice.CreateVideoStream(Device.SensorType.COLOR);
                        break;
                    case "Depth":
                        currentSensor = currentDevice.CreateVideoStream(Device.SensorType.DEPTH);
                        break;
                    case "IR":
                        currentSensor = currentDevice.CreateVideoStream(Device.SensorType.IR);
                        break;
                    default:
                        break;
                }
                VideoMode[] videoModes = currentSensor.SensorInfo.getSupportedVideoModes();
                for (int i = 0; i < videoModes.Length; i++)
                {
                    if (videoModes[i].DataPixelFormat == VideoMode.PixelFormat.GRAY16 ||
                        videoModes[i].DataPixelFormat == VideoMode.PixelFormat.GRAY8 ||
                        videoModes[i].DataPixelFormat == VideoMode.PixelFormat.RGB888 ||
                        videoModes[i].DataPixelFormat == VideoMode.PixelFormat.DEPTH_1MM)
                        cb_videomode.Items.Add(videoModes[i]);
                }
            }
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            if (currentSensor != null && currentSensor.isValid && cb_videomode.SelectedItem != null)
            {
                currentSensor.Stop();
                currentSensor.onNewFrame -= currentSensor_onNewFrame;
                currentSensor.VideoMode = (VideoMode)cb_videomode.SelectedItem;
                try // Not supported by Kinect yet
                {
                    currentSensor.Mirroring = cb_mirror.Checked;
                    if (cb_tir.Checked)
                        currentDevice.ImageRegistration = Device.ImageRegistrationMode.DEPTH_TO_COLOR;
                    else
                        currentDevice.ImageRegistration = Device.ImageRegistrationMode.OFF;
                }
                catch (Exception)
                {
                }

                if (currentSensor.Start() == OpenNI.Status.OK){
                    currentSensor.onNewFrame += currentSensor_onNewFrame;
                }else{
                    MessageBox.Show("Failed to start stream.");
                }
            }
        }
        
        void currentSensor_onNewFrame(VideoStream vStream)
        {
            if (vStream.isValid && vStream.isFrameAvailable())
            {
                using (VideoFrameRef frame = vStream.readFrame())
                {
                    if (frame.isValid)
                    {
                        VideoFrameRef.copyBitmapOptions options = VideoFrameRef.copyBitmapOptions.Force24BitRGB | VideoFrameRef.copyBitmapOptions.DepthFillShadow;
                        if (cb_invert.Checked)
                            options |= VideoFrameRef.copyBitmapOptions.DepthInvert;
                        if (cb_equal.Checked)
                            options |= VideoFrameRef.copyBitmapOptions.DepthHistogramEqualize;
                        if (cb_fill.Checked)
                            options |= (vStream.Mirroring) ? VideoFrameRef.copyBitmapOptions.DepthFillRigthBlack : VideoFrameRef.copyBitmapOptions.DepthFillLeftBlack;
                        lock (bitmap)
                        {
                            try
                            {
                                frame.updateBitmap(bitmap, options);
                            }
                            catch (Exception)
                            {
                                bitmap = frame.toBitmap(options);
                            }
                        }
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            lock (bitmap)
                            {
                                if (pb_image.Image != null)
                                    pb_image.Image.Dispose();
                                pb_image.Image = new Bitmap(bitmap, pb_image.Size);
                                pb_image.Refresh();
                            }
                        }));
                    }
                }
            }
        }

        private void frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            OpenNI.Shutdown();
        }
    }
}
