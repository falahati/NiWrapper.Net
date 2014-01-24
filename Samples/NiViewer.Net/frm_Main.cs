namespace NiViewer.Net
{
    #region

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Windows.Forms;

    using OpenNIWrapper;

    #endregion

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    // ReSharper disable once InconsistentNaming
    public partial class frm_Main : Form
    {
        #region Fields

        private Bitmap bitmap;

        private Device currentDevice;

        private VideoStream currentSensor;

        #endregion

        #region Constructors and Destructors

        public frm_Main()
        {
            this.InitializeComponent();
            this.bitmap = new Bitmap(1, 1);
        }

        #endregion

        #region Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.pb_image.Visible)
            {
                this.pb_image.Visible = false;
            }

            if (this.bitmap == null)
            {
                return;
            }

            lock (this.bitmap)
            {
                // OnPaint happens on UI Thread so it is better to always keep this lock in place
                Size canvasSize = this.pb_image.Size; // Even though we dont use PictureBox, we use it as a placeholder
                Point canvasPosition = this.pb_image.Location;

                double ratioX = canvasSize.Width / (double)this.bitmap.Width;
                double ratioY = canvasSize.Height / (double)this.bitmap.Height;
                double ratio = Math.Min(ratioX, ratioY);

                int drawWidth = Convert.ToInt32(this.bitmap.Width * ratio);
                int drawHeight = Convert.ToInt32(this.bitmap.Height * ratio);

                int drawX = canvasPosition.X + Convert.ToInt32((canvasSize.Width - drawWidth) / 2);
                int drawY = canvasPosition.Y + Convert.ToInt32((canvasSize.Height - drawHeight) / 2);

                e.Graphics.DrawImage(this.bitmap, drawX, drawY, drawWidth, drawHeight);

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
            this.BeginInvoke(new MethodInvoker(this.UpdateDevicesList));
        }

        private void UpdateDevicesList()
        {
            DeviceInfo[] devices = OpenNI.EnumerateDevices();
            this.cb_devices.Items.Clear();
            foreach (DeviceInfo device in devices)
            {
                this.cb_devices.Items.Add(device);
            }
        }

        private void BtnSubmitClick(object sender, EventArgs e)
        {
            if (this.currentSensor != null && this.currentSensor.IsValid && this.cb_videomode.SelectedItem != null)
            {
                this.currentSensor.Stop();
                this.currentSensor.OnNewFrame -= this.CurrentSensorOnNewFrame;
                this.currentSensor.VideoMode = (VideoMode)this.cb_videomode.SelectedItem;
                try
                {
                    // Not supported by Kinect yet
                    this.currentSensor.Mirroring = this.cb_mirrorHard.Checked;
                    this.currentDevice.ImageRegistration = this.cb_tir.Checked ? Device.ImageRegistrationMode.DepthToColor : Device.ImageRegistrationMode.Off;
                }
                catch (Exception)
                {
                }

                if (this.currentSensor.Start() == OpenNI.Status.Ok)
                {
                    this.currentSensor.OnNewFrame += this.CurrentSensorOnNewFrame;
                }
                else
                {
                    MessageBox.Show(@"Failed to start stream.");
                }
            }
        }

        private void CbDevicesSelectedIndexChanged(object sender, EventArgs e)
        {
            this.cb_sensor.Items.Clear();
            if (this.cb_devices.SelectedItem != null)
            {
                if (this.currentDevice != null)
                {
                    this.currentDevice.Dispose();
                }

                this.currentDevice = ((DeviceInfo)this.cb_devices.SelectedItem).OpenDevice();
                if (this.currentDevice.HasSensor(Device.SensorType.Color))
                {
                    this.cb_sensor.Items.Add("Color");
                }

                if (this.currentDevice.HasSensor(Device.SensorType.Depth))
                {
                    this.cb_sensor.Items.Add("Depth");
                }

                if (this.currentDevice.HasSensor(Device.SensorType.Ir))
                {
                    this.cb_sensor.Items.Add("IR");
                }
            }
        }

        private void CbSensorSelectedIndexChanged(object sender, EventArgs e)
        {
            this.cb_videomode.Items.Clear();
            if (this.cb_sensor.SelectedItem != null && this.currentDevice != null)
            {
                if (this.currentSensor != null && this.currentSensor.IsValid)
                {
                    this.currentSensor.Stop();
                }

                switch ((string)this.cb_sensor.SelectedItem)
                {
                    case "Color":
                        this.currentSensor = this.currentDevice.CreateVideoStream(Device.SensorType.Color);
                        break;
                    case "Depth":
                        this.currentSensor = this.currentDevice.CreateVideoStream(Device.SensorType.Depth);
                        break;
                    case "IR":
                        this.currentSensor = this.currentDevice.CreateVideoStream(Device.SensorType.Ir);
                        break;
                }

                if (this.currentSensor != null)
                {
                    VideoMode[] videoModes = this.currentSensor.SensorInfo.GetSupportedVideoModes();
                    foreach (VideoMode mode in videoModes)
                    {
                        if (mode.DataPixelFormat == VideoMode.PixelFormat.Gray16
                            || mode.DataPixelFormat == VideoMode.PixelFormat.Gray8
                            || mode.DataPixelFormat == VideoMode.PixelFormat.Rgb888
                            || mode.DataPixelFormat == VideoMode.PixelFormat.Depth1Mm)
                        {
                            this.cb_videomode.Items.Add(mode);
                        }
                    }
                }
            }
        }

        private void CurrentSensorOnNewFrame(VideoStream videoStream)
        {
            if (videoStream.IsValid && videoStream.IsFrameAvailable())
            {
                using (VideoFrameRef frame = videoStream.ReadFrame())
                {
                    if (frame.IsValid)
                    {
                        VideoFrameRef.CopyBitmapOptions options = VideoFrameRef.CopyBitmapOptions.Force24BitRgb
                                                                  | VideoFrameRef.CopyBitmapOptions.DepthFillShadow;
                        if (this.cb_invert.Checked)
                        {
                            options |= VideoFrameRef.CopyBitmapOptions.DepthInvert;
                        }

                        if (this.cb_equal.Checked)
                        {
                            options |= VideoFrameRef.CopyBitmapOptions.DepthHistogramEqualize;
                        }

                        if (this.cb_fill.Checked)
                        {
                            options |= videoStream.Mirroring
                                           ? VideoFrameRef.CopyBitmapOptions.DepthFillRigthBlack
                                           : VideoFrameRef.CopyBitmapOptions.DepthFillLeftBlack;
                        }

                        lock (this.bitmap)
                        {
                            /////////////////////// Instead of creating a bitmap object for each frame, you can simply
                            /////////////////////// update one you have. Please note that you must be very careful 
                            /////////////////////// with multi-thread situations.
                            try
                            {
                                frame.UpdateBitmap(this.bitmap, options);
                            }
                            catch (Exception)
                            {
                                // Happens when our Bitmap object is not compatible with returned Frame
                                this.bitmap = frame.ToBitmap(options);
                            }

                            /////////////////////// END NOTE

                            /////////////////////// You can always use .toBitmap() if you dont want to
                            /////////////////////// clone image later and be safe when using it in multi-thread situations
                            /////////////////////// This is little slower, but easier to handle
                            // bitmap = frame.toBitmap(options);
                            /////////////////////// END NOTE
                            if (this.cb_mirrorSoft.Checked)
                            {
                                this.bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
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
                        if (!this.pb_image.Visible)
                        {
                            this.Invalidate();
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
            this.HandleError(OpenNI.Initialize());
            OpenNI.OnDeviceConnected += this.OpenNiOnDeviceConnectionStateChanged;
            OpenNI.OnDeviceDisconnected += this.OpenNiOnDeviceConnectionStateChanged;
            this.UpdateDevicesList();
        }

        #endregion
    }
}