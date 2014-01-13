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
                if (currentDevice.HasSensor(Device.SensorType.COLOR)) cb_sensor.Items.Add("Color");
                if (currentDevice.HasSensor(Device.SensorType.DEPTH)) cb_sensor.Items.Add("Depth");
                if (currentDevice.HasSensor(Device.SensorType.IR)) cb_sensor.Items.Add("IR");
            }
        }
        VideoStream currentSensor;
        private void cb_sensor_SelectedIndexChanged(object sender, EventArgs e)
        {
            cb_videomode.Items.Clear();
            if (cb_sensor.SelectedItem != null && currentDevice != null)
            {
                if (currentSensor != null && currentSensor.IsValid) currentSensor.Stop();
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
                VideoMode[] videoModes = currentSensor.SensorInfo.GetSupportedVideoModes();
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
            if (currentSensor != null && currentSensor.IsValid && cb_videomode.SelectedItem != null)
            {
                currentSensor.Stop();
                currentSensor.onNewFrame -= currentSensor_onNewFrame;
                currentSensor.VideoMode = (VideoMode)cb_videomode.SelectedItem;
                try // Not supported by Kinect yet
                {
                    currentSensor.Mirroring = cb_mirrorHard.Checked;
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
            if (vStream.IsValid && vStream.isFrameAvailable())
            {
                using (VideoFrameRef frame = vStream.readFrame())
                {
                    if (frame.IsValid)
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
                            /////////////////////// Instead of creating a bitmap object for each frame, you can simply
                            /////////////////////// update one you have. Please note that you must be very careful 
                            /////////////////////// with multi-thread situations.
                            try
                            {
                                frame.UpdateBitmap(bitmap, options);
                            }
                            catch (Exception) // Happens when our Bitmap object is not compatible with returned Frame
                            {
                                bitmap = frame.ToBitmap(options);
                            }
                            /////////////////////// END NOTE

                            /////////////////////// You can always use .toBitmap() if you dont want to
                            /////////////////////// clone image later and be safe when using it in multi-thread situations
                            /////////////////////// This is little slower, but easier to handle
                            //bitmap = frame.toBitmap(options);
                            /////////////////////// END NOTE

                            if (cb_mirrorSoft.Checked)
                                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        
                        ///////////////////// You can simply pass the newly created/updated image to a
                        ///////////////////// PictureBox right here instead of drawing it with Graphic object
                        //this.BeginInvoke(new MethodInvoker(delegate()
                        //{
                        //    if (!pb_image.Visible)
                        //        pb_image.Visible = true;
                        //    if (bitmap == null)
                        //        return;
                        //    lock (bitmap) // this.BeginInvoke happens on UI Thread so it is better to always keep this lock in place
                        //    {
                        //        if (pb_image.Image != null)
                        //            pb_image.Image.Dispose();

                        //        /////////////////////// If you want to use one bitmap object for all frames, the 
                        //        /////////////////////// best way to prevent and multi-thread access problems
                        //        /////////////////////// is to clone the bitmap each time you want to send it to PictureBox 
                        //        pb_image.Image = new Bitmap(bitmap, bitmap.Size);
                        //        /////////////////////// END NOTE

                        //        /////////////////////// If you only use toBitmap() method. you can simply skip the
                        //        /////////////////////// cloning process. It is perfectly thread-safe.
                        //        //pb_image.Image = bitmap;
                        //        /////////////////////// END NOTE

                        //        pb_image.Refresh();
                        //    }
                        //}));
                        ///////////////////// END NOTE
                        if (!pb_image.Visible)
                            this.Invalidate();
                    }
                }
            }
        }

        /////////////////////// You can use one Bitmap object and update it for each frame 
        /////////////////////// or one Bitmap object per each frame. Either way you can use
        /////////////////////// OnPaint method to prevent multi-thread access problems
        /////////////////////// without creating a new Bitmap object each time.
        /////////////////////// In other word, you can skip Cloning even when using updateBitmap().
        protected override void OnPaint(PaintEventArgs e)
        {
            if (pb_image.Visible)
                pb_image.Visible = false;
            if (bitmap == null)
                return;
            lock (bitmap) // OnPaint happens on UI Thread so it is better to always keep this lock in place
            {
                Size canvasSize = pb_image.Size; // Even though we dont use PictureBox, we use it as a placeholder
                Point canvasPosition = pb_image.Location;

                double ratioX = (double)canvasSize.Width / (double)bitmap.Width;
                double ratioY = (double)canvasSize.Height / (double)bitmap.Height;
                double ratio = Math.Min(ratioX, ratioY);

                int drawWidth = Convert.ToInt32(bitmap.Width * ratio);
                int drawHeight = Convert.ToInt32(bitmap.Height * ratio);

                int drawX = canvasPosition.X + Convert.ToInt32((canvasSize.Width - drawWidth) / 2);
                int drawY = canvasPosition.Y + Convert.ToInt32((canvasSize.Height - drawHeight) / 2);

                e.Graphics.DrawImage(bitmap, drawX, drawY, drawWidth, drawHeight);
                /////////////////////// If we do create a new Bitmap object per each frame we must
                /////////////////////// make sure to DISPOSE it after using.
                //bitmap.Dispose();
                /////////////////////// END NOTE
            }
        }
        /////////////////////// END NOTE

        private void frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            OpenNI.Shutdown();
        }
    }
}
