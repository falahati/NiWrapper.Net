using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using OpenNIWrapper;

namespace NiViewer.Net
{
    #region

    #endregion

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification =
        "Reviewed. Suppression is OK here.")]
    // ReSharper disable once InconsistentNaming
    public partial class frm_Main : Form
    {
        #region Constructors and Destructors

        public frm_Main()
        {
            InitializeComponent();
            bitmap = new Bitmap(1, 1);
        }

        #endregion

        #region Fields

        private Bitmap bitmap;

        private Device currentDevice;

        private VideoStream currentSensor;

        #endregion

        #region Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            if (pb_image.Visible)
            {
                pb_image.Visible = false;
            }

            if (bitmap == null)
            {
                return;
            }

            lock (bitmap)
            {
                // OnPaint happens on UI Thread so it is better to always keep this lock in place
                var canvasSize = pb_image.Size; // Even though we dont use PictureBox, we use it as a placeholder
                var canvasPosition = pb_image.Location;

                var ratioX = canvasSize.Width / (double) bitmap.Width;
                var ratioY = canvasSize.Height / (double) bitmap.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var drawWidth = Convert.ToInt32(bitmap.Width * ratio);
                var drawHeight = Convert.ToInt32(bitmap.Height * ratio);

                var drawX = canvasPosition.X + Convert.ToInt32((canvasSize.Width - drawWidth) / 2);
                var drawY = canvasPosition.Y + Convert.ToInt32((canvasSize.Height - drawHeight) / 2);

                e.Graphics.DrawImage(bitmap, drawX, drawY, drawWidth, drawHeight);

                /////////////////////// If we do create a new Bitmap object per each frame we must
                /////////////////////// make sure to DISPOSE it after using.
                // bitmap.Dispose();
                /////////////////////// END NOTE
            }
        }

        private void HandleError(OpenNI.Status status)
        {
            if (status == OpenNI.Status.Ok)
            {
                return;
            }

            MessageBox.Show(
                string.Format(@"Error: {0} - {1}", status, OpenNI.LastError),
                @"Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk);
        }

        private void OpenNiOnDeviceConnectionStateChanged(DeviceInfo device)
        {
            BeginInvoke(new MethodInvoker(UpdateDevicesList));
        }

        private void UpdateDevicesList()
        {
            var devices = OpenNI.EnumerateDevices();
            cb_devices.Items.Clear();

            foreach (var device in devices)
            {
                cb_devices.Items.Add(device);
            }
        }

        private void BtnSubmitClick(object sender, EventArgs e)
        {
            if (currentSensor != null && currentSensor.IsValid && cb_videomode.SelectedItem != null)
            {
                currentSensor.Stop();
                currentSensor.OnNewFrame -= CurrentSensorOnNewFrame;
                currentSensor.VideoMode = (VideoMode) cb_videomode.SelectedItem;

                try
                {
                    // Not supported by Kinect yet
                    currentSensor.Mirroring = cb_mirrorHard.Checked;
                    currentDevice.ImageRegistration = cb_tir.Checked
                        ? Device.ImageRegistrationMode.DepthToColor
                        : Device.ImageRegistrationMode.Off;
                }
                catch (Exception)
                {
                }

                if (currentSensor.Start() == OpenNI.Status.Ok)
                {
                    currentSensor.OnNewFrame += CurrentSensorOnNewFrame;
                }
                else
                {
                    MessageBox.Show(@"Failed to start stream.");
                }
            }
        }

        private void CbDevicesSelectedIndexChanged(object sender, EventArgs e)
        {
            cb_sensor.Items.Clear();

            if (cb_devices.SelectedItem != null)
            {
                if (currentDevice != null)
                {
                    currentDevice.Dispose();
                }

                currentDevice = ((DeviceInfo) cb_devices.SelectedItem).OpenDevice();

                if (currentDevice.HasSensor(Device.SensorType.Color))
                {
                    cb_sensor.Items.Add("Color");
                }

                if (currentDevice.HasSensor(Device.SensorType.Depth))
                {
                    cb_sensor.Items.Add("Depth");
                }

                if (currentDevice.HasSensor(Device.SensorType.Ir))
                {
                    cb_sensor.Items.Add("IR");
                }
            }
        }

        private void CbSensorSelectedIndexChanged(object sender, EventArgs e)
        {
            cb_videomode.Items.Clear();

            if (cb_sensor.SelectedItem != null && currentDevice != null)
            {
                if (currentSensor != null && currentSensor.IsValid)
                {
                    currentSensor.Stop();
                }

                switch ((string) cb_sensor.SelectedItem)
                {
                    case "Color":
                        currentSensor = currentDevice.CreateVideoStream(Device.SensorType.Color);

                        break;
                    case "Depth":
                        currentSensor = currentDevice.CreateVideoStream(Device.SensorType.Depth);

                        break;
                    case "IR":
                        currentSensor = currentDevice.CreateVideoStream(Device.SensorType.Ir);

                        break;
                }

                if (currentSensor != null)
                {
                    var videoModes = currentSensor.SensorInfo.GetSupportedVideoModes();

                    foreach (var mode in videoModes)
                    {
                        if (mode.DataPixelFormat == VideoMode.PixelFormat.Gray16 ||
                            mode.DataPixelFormat == VideoMode.PixelFormat.Gray8 ||
                            mode.DataPixelFormat == VideoMode.PixelFormat.Rgb888 ||
                            mode.DataPixelFormat == VideoMode.PixelFormat.Depth1Mm)
                        {
                            cb_videomode.Items.Add(mode);
                        }
                    }
                }
            }
        }

        private void CurrentSensorOnNewFrame(VideoStream videoStream)
        {
            if (videoStream.IsValid && videoStream.IsFrameAvailable())
            {
                using (var frame = videoStream.ReadFrame())
                {
                    if (frame.IsValid)
                    {
                        var options = VideoFrameRef.CopyBitmapOptions.Force24BitRgb |
                                      VideoFrameRef.CopyBitmapOptions.DepthFillShadow;

                        if (cb_invert.Checked)
                        {
                            options |= VideoFrameRef.CopyBitmapOptions.DepthInvert;
                        }

                        if (cb_equal.Checked)
                        {
                            options |= VideoFrameRef.CopyBitmapOptions.DepthHistogramEqualize;
                        }

                        if (cb_fill.Checked)
                        {
                            options |= videoStream.Mirroring
                                ? VideoFrameRef.CopyBitmapOptions.DepthFillRigthBlack
                                : VideoFrameRef.CopyBitmapOptions.DepthFillLeftBlack;
                        }

                        lock (bitmap)
                        {
                            /////////////////////// Instead of creating a bitmap object for each frame, you can simply
                            /////////////////////// update one you have. Please note that you must be very careful 
                            /////////////////////// with multi-thread situations.
                            try
                            {
                                frame.UpdateBitmap(bitmap, options);
                            }
                            catch (Exception)
                            {
                                // Happens when our Bitmap object is not compatible with returned Frame
                                bitmap = frame.ToBitmap(options);
                            }

                            /////////////////////// END NOTE

                            /////////////////////// You can always use .toBitmap() if you dont want to
                            /////////////////////// clone image later and be safe when using it in multi-thread situations
                            /////////////////////// This is little slower, but easier to handle
                            // bitmap = frame.toBitmap(options);
                            /////////////////////// END NOTE
                            if (cb_mirrorSoft.Checked)
                            {
                                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            }
                        }

                        ///////////////////// You can simply pass the newly created/updated image to a
                        ///////////////////// PictureBox right here instead of drawing it with Graphic object
                        // this.BeginInvoke(new MethodInvoker(delegate()
                        // {
                        // if (!pb_image.Visible)
                        // pb_image.Visible = true;
                        // if (bitmap == null)
                        // return;
                        // lock (bitmap) // this.BeginInvoke happens on UI Thread so it is better to always keep this lock in place
                        // {
                        // if (pb_image.Image != null)
                        // pb_image.Image.Dispose();

                        // /////////////////////// If you want to use one bitmap object for all frames, the 
                        // /////////////////////// best way to prevent and multi-thread access problems
                        // /////////////////////// is to clone the bitmap each time you want to send it to PictureBox 
                        // pb_image.Image = new Bitmap(bitmap, bitmap.Size);
                        // /////////////////////// END NOTE

                        // /////////////////////// If you only use toBitmap() method. you can simply skip the
                        // /////////////////////// cloning process. It is perfectly thread-safe.
                        // //pb_image.Image = bitmap;
                        // /////////////////////// END NOTE

                        // pb_image.Refresh();
                        // }
                        // }));
                        ///////////////////// END NOTE
                        if (!pb_image.Visible)
                        {
                            Invalidate();
                        }
                    }
                }
            }
        }

        /////////////////////// You can use one Bitmap object and update it for each frame 
        /////////////////////// or one Bitmap object per each frame. Either way you can use
        /////////////////////// OnPaint method to prevent multi-thread access problems
        /////////////////////// without creating a new Bitmap object each time.
        /////////////////////// In other word, you can skip Cloning even when using updateBitmap().

        /////////////////////// END NOTE
        private void FrmMainFormClosing(object sender, FormClosingEventArgs e)
        {
            OpenNI.Shutdown();
        }

        private void FrmMainLoad(object sender, EventArgs e)
        {
            HandleError(OpenNI.Initialize());
            OpenNI.OnDeviceConnected += OpenNiOnDeviceConnectionStateChanged;
            OpenNI.OnDeviceDisconnected += OpenNiOnDeviceConnectionStateChanged;
            UpdateDevicesList();
        }

        #endregion
    }
}