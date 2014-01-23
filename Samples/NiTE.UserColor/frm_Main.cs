using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NiTEWrapper;
using System.Drawing.Imaging;

namespace NiTEUserColor
{
    public partial class frm_Main : Form
    {
        public frm_Main()
        {
            InitializeComponent();
        }

        static bool HandleError(NiTE.Status status)
        {
            if (status == NiTE.Status.Ok)
                return true;
            MessageBox.Show("Error: " + status.ToString() + Environment.NewLine + OpenNIWrapper.OpenNI.LastError);
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
        UserTracker uTracker;
        private void btn_start_Click(object sender, EventArgs e)
        {
            uTracker = UserTracker.Create();
            btn_start.Enabled = false;
            uTracker.OnNewData += new UserTracker.UserTrackerListenerDelegate(uTracker_onNewData);

            //  FIXED Jun 2013
            ///* Because of incompatibility between current version of OpenNI and NiTE,
            // * we can't use event based reading. So we put our sample in a loop.
            // * You can copy OpenNI.dll from version 2.0 to solve this problem.
            // * Then you can uncomment above line of code and comment below ones.
            // */
            //while (this.IsHandleCreated)
            //{
            //    uTracker_onNewData(uTracker);
            //    Application.DoEvents();
            //}
        }

        ulong lastTime = 0;
        ulong fps;
        void uTracker_onNewData(UserTracker uTracker)
        {
            if (!uTracker.IsValid)
                return;
            using (UserTrackerFrameRef frame = uTracker.ReadFrame())
            {
                if (!frame.IsValid)
                    return;
                UserMap um = frame.UserMap;
                FillImageFromUserMap(frame.UserMap);

                UserData[] ud = frame.Users;
                using (Graphics g = Graphics.FromImage(image))
                {
                    foreach (UserData user in frame.Users)
                    {
                        if (user.CenterOfMass.Z > 0)
                        {
                            Point p = new Point();
                            PointF pf = uTracker.ConvertJointCoordinatesToDepth(user.CenterOfMass);
                            p.X = (int)pf.X - 5;
                            p.Y = (int)pf.Y - 5;
                            g.DrawEllipse(new Pen(Brushes.White, 5), new Rectangle(p, new Size(5, 5)));
                            g.DrawString("Center Of Mass", SystemFonts.DefaultFont, Brushes.White, p.X - 40, p.Y - 20);
                        }
                    }
                    g.Save();
                }
                this.Invoke(new MethodInvoker(delegate()
                {
                    fps = ((1000000 / (frame.Timestamp - lastTime)) + (fps * 4)) / 5;
                    lastTime = frame.Timestamp;
                    this.Text = "Frame #" + frame.FrameIndex.ToString() + " - Time: " + frame.Timestamp.ToString() + " - FPS: " + fps.ToString();
                    pb_preview.Image = image.Clone(new Rectangle(new Point(0,0), image.Size), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                }));
                
            }
        }

        Bitmap image;
        private unsafe void FillImageFromUserMap(UserMap um)
        {
            int[] colors = new int[] { 16777215, 14565387, 32255, 7996159, 16530175, 8373026, 14590399, 7062435, 13951499, 55807 };
            if (image == null || image.Width != um.FrameSize.Width || image.Height != um.FrameSize.Height)
                image = new Bitmap(um.FrameSize.Width, um.FrameSize.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData bd = image.LockBits(new Rectangle(new Point(0, 0), image.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            for (int y = 0; y < um.FrameSize.Height; y++)
            {
                ushort* dataPos = (ushort*)((byte*)um.Pixels.ToPointer() + (y * um.DataStrideBytes));
                byte* imagePos = (byte*)bd.Scan0.ToPointer() + (y * bd.Stride);
                for (int x = 0; x < um.FrameSize.Width; x++)
                {
                    int color = colors[*dataPos % colors.Length];
                    *imagePos = (byte)(((color / 1) % 256) * (*dataPos)); // R
                    imagePos++;
                    *imagePos = (byte)(((color / 256) % 256) * (*dataPos)); // G
                    imagePos++;
                    *imagePos = (byte)(((color / 65536) % 256) * (*dataPos)); // B
                    imagePos++;
                    dataPos++;
                }
            }
            image.UnlockBits(bd);
        }

        private void frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            NiTE.Shutdown();
            OpenNIWrapper.OpenNI.Shutdown();
        }
    }
}
