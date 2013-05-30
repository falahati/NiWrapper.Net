using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NiTEWrapper;
namespace NiTEHandTracker
{
    public partial class frm_Main : Form
    {
        Bitmap image = new Bitmap(640, 480);
        public frm_Main()
        {
            InitializeComponent();
        }

        static bool HandleError(NiTE.Status status)
        {
            if (status == NiTE.Status.OK)
                return true;
            MessageBox.Show("Error: " + status.ToString());
            return false;
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            if (HandleError(NiTE.Initialize()))
            {
                this.Text = "Running by NiTE v" + NiTE.Version.ToString();
            }
            else
            {
                Environment.Exit(1);
            }
        }

        HandTracker hTracker;
        private void button1_Click(object sender, EventArgs e)
        {
            hTracker = HandTracker.Create();
            btn_start.Enabled = false;
            HandleError(hTracker.StartGestureDetection(GestureData.GestureType.HAND_RAISE));
            //hTracker.onNewData += new HandTracker.HandTrackerListener(hTracker_onNewData);

            /* Because of incompatibility between current version of OpenNI and NiTE,
             * we can't use event based reading. So we put our sample in a loop.
             * You can copy OpenNI.dll from version 2.0 to solve this problem.
             * Then you can uncomment above line of code and comment below ones.
             */
            while (this.IsHandleCreated)
            {
                hTracker_onNewData(hTracker);
                Application.DoEvents();
            }
        }

        ulong lastTime;
        ulong fps;
        void hTracker_onNewData(HandTracker hTracker)
        {
            if (!hTracker.isValid)
                return;
            using (HandTrackerFrameRef frame = hTracker.readFrame())
            {
                if (!frame.isValid)
                    return;
                lock (image)
                {
                    /* Because of incompatibility between current version of OpenNI and NiTE,
                     * we can't use VideoFrameRef methods when working with 
                     * UserTrackerFrameRef.DepthFrame/HandTrackerFrameRef.DepthFrame
                     * There is no workaround for this problem yet and we must wait for official update of NiTE
                     * Then you can uncomment below lines.
                     */
                    //if (image.Width != frame.DepthFrame.FrameSize.Width || image.Height != frame.DepthFrame.FrameSize.Height)
                    //    image = new Bitmap(frame.DepthFrame.FrameSize.Width, frame.DepthFrame.FrameSize.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        g.FillRectangle(Brushes.Black, new Rectangle(new Point(0, 0), image.Size));
                        foreach (GestureData gesture in frame.Gestures)
                            if (gesture.isComplete)
                                hTracker.startHandTracking(gesture.CurrentPosition);
                        if (frame.Hands.Length == 0)
                            g.DrawString("Raise your hand", SystemFonts.DefaultFont, Brushes.White, 10, 10);
                        else
                            foreach (HandData hand in frame.Hands)
                            {
                                if (hand.isTracking)
                                {
                                    Point HandPosEllipse = new Point();
                                    PointF HandPos = hTracker.ConvertHandCoordinatesToDepth(hand.Position);
                                    HandPosEllipse.X = (int)HandPos.X - 5;
                                    HandPosEllipse.Y = (int)HandPos.Y - 5;
                                    g.DrawEllipse(new Pen(Brushes.White, 5), new Rectangle(HandPosEllipse, new Size(5, 5)));
                                }
                            }

                        g.Save();
                    }
                }
                this.Invoke(new MethodInvoker(delegate()
                {
                    fps = ((1000000 / (frame.Timestamp - lastTime)) + (fps * 4)) / 5;
                    lastTime = frame.Timestamp;
                    this.Text = "Frame #" + frame.FrameIndex.ToString() + " - Time: " + frame.Timestamp.ToString() + " - FPS: " + fps.ToString();
                    pb_preview.Image = image.Clone(new Rectangle(new Point(0, 0), image.Size), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                }));
            }
        }

        private void frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            NiTE.Shutdown();
            OpenNIWrapper.OpenNI.Shutdown();
        }
    }
}
