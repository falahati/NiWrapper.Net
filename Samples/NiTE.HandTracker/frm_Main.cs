namespace NiTEHandTracker
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

        private HandTracker handTracker;

        private Bitmap image = new Bitmap(640, 480);

        private ulong lastTime;

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

            MessageBox.Show(@"Error: " + status);
            return false;
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            this.handTracker = HandTracker.Create();
            this.btn_start.Enabled = false;
            HandleError(this.handTracker.StartGestureDetection(GestureData.GestureType.HandRaise));
            this.handTracker.OnNewData += this.HandTrackerOnNewData;

            // FIXED Jun 2013
            // * Because of incompatibility between current version of OpenNI and NiTE,
            // * we can't use event based reading. So we put our sample in a loop.
            // * You can copy OpenNI.dll from version 2.0 to solve this problem.
            // * Then you can uncomment above line of code and comment below ones.
            // */
            // while (this.IsHandleCreated)
            // {
            // hTracker_onNewData(hTracker);
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
        private void HandTrackerOnNewData(HandTracker handTracker)
        {
            if (!handTracker.IsValid)
            {
                return;
            }

            HandTrackerFrameRef frame = handTracker.ReadFrame();

            if (frame == null || !frame.IsValid)
            {
                return;
            }

            lock (this.image)
            {
                using (VideoFrameRef depthFrame = frame.DepthFrame)
                {
                    if (this.image.Width != depthFrame.FrameSize.Width
                        || this.image.Height != depthFrame.FrameSize.Height)
                    {
                        this.image = new Bitmap(
                            depthFrame.FrameSize.Width,
                            depthFrame.FrameSize.Height,
                            PixelFormat.Format24bppRgb);
                    }
                }

                using (Graphics g = Graphics.FromImage(this.image))
                {
                    g.FillRectangle(Brushes.Black, new Rectangle(new Point(0, 0), this.image.Size));
                    foreach (GestureData gesture in frame.Gestures)
                    {
                        if (gesture.IsComplete)
                        {
                            handTracker.StartHandTracking(gesture.CurrentPosition);
                        }
                    }

                    if (frame.Hands.Length == 0)
                    {
                        g.DrawString("Raise your hand", SystemFonts.DefaultFont, Brushes.White, 10, 10);
                    }
                    else
                    {
                        foreach (HandData hand in frame.Hands)
                        {
                            if (hand.IsTracking)
                            {
                                Point handPosEllipse = new Point();
                                PointF handPos = handTracker.ConvertHandCoordinatesToDepth(hand.Position);
                                handPosEllipse.X = (int)handPos.X - 5;
                                handPosEllipse.Y = (int)handPos.Y - 5;
                                g.DrawEllipse(new Pen(Brushes.White, 5), new Rectangle(handPosEllipse, new Size(5, 5)));
                            }
                        }
                    }

                    g.Save();
                }
            }

            this.Invoke(
                new MethodInvoker(
                    delegate
                        {
                            this.fps = ((1000000 / (frame.Timestamp - this.lastTime)) + (this.fps * 4)) / 5;
                            this.lastTime = frame.Timestamp;
                            this.Text = @"Frame #" + frame.FrameIndex + @" - Time: " + frame.Timestamp + @" - FPS: "
                                        + this.fps;
                            this.pb_preview.Image = this.image.Clone(
                                new Rectangle(new Point(0, 0), this.image.Size),
                                PixelFormat.Format24bppRgb);
                            frame.Release();
                        }));
        }

        #endregion
    }
}