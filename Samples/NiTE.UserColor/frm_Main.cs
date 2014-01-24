namespace NiTEUserColor
{
    #region

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    using NiTEWrapper;

    using OpenNIWrapper;

    #endregion

    // ReSharper disable once InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    public partial class frm_Main : Form
    {
        #region Fields

        private ulong fps;

        private Bitmap image;

        private ulong lastTime;

        private UserTracker userTracker;

        #endregion

        #region Constructors and Destructors

        public frm_Main()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        private static bool HandleError(NiTE.Status status)
        {
            if (status == NiTE.Status.Ok)
            {
                return true;
            }

            MessageBox.Show(string.Format(@"Error: {0}{1}{2}", status, Environment.NewLine, OpenNI.LastError));
            return false;
        }

        private unsafe void FillImageFromUserMap(UserMap um)
        {
            int[] colors = { 16777215, 14565387, 32255, 7996159, 16530175, 8373026, 14590399, 7062435, 13951499, 55807 };
            if (this.image == null || this.image.Width != um.FrameSize.Width || this.image.Height != um.FrameSize.Height)
            {
                this.image = new Bitmap(um.FrameSize.Width, um.FrameSize.Height, PixelFormat.Format24bppRgb);
            }

            BitmapData bd = this.image.LockBits(
                new Rectangle(new Point(0, 0), this.image.Size), 
                ImageLockMode.WriteOnly, 
                PixelFormat.Format24bppRgb);
            for (int y = 0; y < um.FrameSize.Height; y++)
            {
                ushort* dataPos = (ushort*)((byte*)um.Pixels.ToPointer() + (y * um.DataStrideBytes));
                byte* imagePos = (byte*)bd.Scan0.ToPointer() + (y * bd.Stride);
                for (int x = 0; x < um.FrameSize.Width; x++)
                {
                    int color = colors[*dataPos % colors.Length];
                    *imagePos = (byte)(((color / 1) % 256) * *dataPos); // R
                    imagePos++;
                    *imagePos = (byte)(((color / 256) % 256) * *dataPos); // G
                    imagePos++;
                    *imagePos = (byte)(((color / 65536) % 256) * *dataPos); // B
                    imagePos++;
                    dataPos++;
                }
            }

            this.image.UnlockBits(bd);
        }

        private void BtnStartClick(object sender, EventArgs e)
        {
            this.userTracker = UserTracker.Create();
            this.btn_start.Enabled = false;
            this.userTracker.OnNewData += this.UserTrackerOnNewData;

            // FIXED Jun 2013
            // * Because of incompatibility between current version of OpenNI and NiTE,
            // * we can't use event based reading. So we put our sample in a loop.
            // * You can copy OpenNI.dll from version 2.0 to solve this problem.
            // * Then you can uncomment above line of code and comment below ones.
            // */
            // while (this.IsHandleCreated)
            // {
            // uTracker_onNewData(uTracker);
            // Application.DoEvents();
            // }
        }

        private void FrmMainFormClosing(object sender, FormClosingEventArgs e)
        {
            NiTE.Shutdown();
            OpenNI.Shutdown();
        }

        private void FrmMainLoad(object sender, EventArgs e)
        {
            if (HandleError(NiTE.Initialize()))
            {
                this.Text = @"Running by NiTE v" + NiTE.Version;
            }
            else
            {
                Environment.Exit(1);
            }
        }

        // ReSharper disable once ParameterHidesMember
        private void UserTrackerOnNewData(UserTracker userTracker)
        {
            if (!userTracker.IsValid)
            {
                return;
            }

            UserTrackerFrameRef frame = userTracker.ReadFrame();

            if (frame == null || !frame.IsValid)
            {
                return;
            }

            this.FillImageFromUserMap(frame.UserMap);

            using (Graphics g = Graphics.FromImage(this.image))
            {
                foreach (UserData user in frame.Users)
                {
                    if (user.CenterOfMass.Z > 0)
                    {
                        Point p = new Point();
                        PointF pf = userTracker.ConvertJointCoordinatesToDepth(user.CenterOfMass);
                        p.X = (int)pf.X - 5;
                        p.Y = (int)pf.Y - 5;
                        g.DrawEllipse(new Pen(Brushes.White, 5), new Rectangle(p, new Size(5, 5)));
                        g.DrawString("Center Of Mass", SystemFonts.DefaultFont, Brushes.White, p.X - 40, p.Y - 20);
                    }
                }

                g.Save();
            }

            this.Invoke(
                new MethodInvoker(
                    delegate
                        {
                            this.fps = ((1000000 / (frame.Timestamp - this.lastTime)) + (this.fps * 4)) / 5;
                            this.lastTime = frame.Timestamp;
                            this.Text = string.Format(
                                "Frame #{0} - Time: {1} - FPS: {2}",
                                frame.FrameIndex,
                                frame.Timestamp,
                                this.fps);
                            this.pb_preview.Image = this.image.Clone(
                                new Rectangle(new Point(0, 0), this.image.Size),
                                PixelFormat.Format24bppRgb);
                            frame.Release();
                        }));
        }

        #endregion
    }
}