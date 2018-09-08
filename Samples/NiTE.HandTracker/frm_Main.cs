using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using NiTEWrapper;
using OpenNIWrapper;
using Size = System.Drawing.Size;

namespace NiTEHandTracker
{
    #region

    #endregion

    // ReSharper disable once InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification =
        "Reviewed. Suppression is OK here.")]
    public partial class frm_Main : Form
    {
        #region Constructors and Destructors

        public frm_Main()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields

        private ulong fps;

        private HandTracker handTracker;

        private Bitmap image;

        private ulong lastTime;

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
            handTracker = HandTracker.Create();
            btn_start.Enabled = false;
            HandleError(handTracker.StartGestureDetection(GestureData.GestureType.HandRaise));
            handTracker.OnNewData += HandTrackerOnNewData;
        }

        private void FrmMainFormClosing(object sender, FormClosingEventArgs e)
        {
            handTracker?.StopGestureDetection(GestureData.GestureType.HandRaise);
            handTracker?.Dispose();
            NiTE.Shutdown();
            OpenNI.Shutdown();
        }

        private void FrmMainLoad(object sender, EventArgs e)
        {
            if (HandleError(NiTE.Initialize()))
            {
                Text = @"Running by NiTE v" + NiTE.Version;
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

            using (var frame = handTracker.ReadFrame())
            {
                if (frame == null || !frame.IsValid)
                {
                    return;
                }

                lock (this)
                {
                    using (var depthFrame = frame.DepthFrame)
                    {
                        if (image?.Width != depthFrame.FrameSize.Width ||
                            image?.Height != depthFrame.FrameSize.Height)
                        {
                            image?.Dispose();
                            image = new Bitmap(
                                depthFrame.FrameSize.Width,
                                depthFrame.FrameSize.Height,
                                PixelFormat.Format24bppRgb);
                        }
                    }

                    using (var g = Graphics.FromImage(image))
                    {
                        g.FillRectangle(Brushes.Black, new Rectangle(new Point(0, 0), image.Size));

                        foreach (var gesture in frame.Gestures)
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
                            foreach (var hand in frame.Hands)
                            {
                                if (hand.IsTracking)
                                {
                                    var handPosEllipse = new Point();
                                    var handPos = handTracker.ConvertHandCoordinatesToDepth(hand.Position);

                                    if (!handPos.IsEmpty && !float.IsNaN(handPos.X) && !float.IsNaN(handPos.Y))
                                    {
                                        handPosEllipse.X = (int) handPos.X - 5;
                                        handPosEllipse.Y = (int) handPos.Y - 5;
                                        g.DrawEllipse(new Pen(Brushes.White, 5),
                                            new Rectangle(handPosEllipse, new Size(5, 5)));
                                    }
                                }
                            }
                        }

                        g.Save();
                    }
                }

                Invoke(
                    new MethodInvoker(
                        delegate
                        {
                            try
                            {
                                // ReSharper disable AccessToDisposedClosure
                                fps = (1000000 / (frame.Timestamp - lastTime) + fps * 4) / 5;
                                lastTime = frame.Timestamp;
                                Text = @"Frame #" +
                                       frame.FrameIndex +
                                       @" - Time: " +
                                       frame.Timestamp +
                                       @" - FPS: " +
                                       fps;
                                // ReSharper restore AccessToDisposedClosure
                                pb_preview.Image?.Dispose();
                                pb_preview.Image = image.Clone(
                                    new Rectangle(new Point(0, 0), image.Size),
                                    PixelFormat.Format24bppRgb);
                            }
                            catch
                            {
                            }
                        }));
            }
        }

        #endregion
    }
}