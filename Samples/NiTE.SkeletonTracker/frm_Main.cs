using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NiTEWrapper;
namespace NiTESkeletonTracker
{
    public partial class frm_Main : Form
    {
        Bitmap image = new Bitmap(1, 1);
        public frm_Main()
        {
            InitializeComponent();
        }

        static bool HandleError(NiTE.Status status)
        {
            if (status == NiTE.Status.Ok)
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

        UserTracker uTracker;
        private void button1_Click(object sender, EventArgs e)
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

        ulong lastTime;
        ulong fps;
        void uTracker_onNewData(UserTracker uTracker)
        {
            if (!uTracker.IsValid)
                return;
            using (UserTrackerFrameRef frame = uTracker.ReadFrame())
            {
                if (!frame.IsValid)
                    return;
                lock (image)
                {
                    if (image.Width != frame.UserMap.FrameSize.Width || image.Height != frame.UserMap.FrameSize.Height)
                        image = new Bitmap(frame.UserMap.FrameSize.Width, frame.UserMap.FrameSize.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        g.FillRectangle(Brushes.Black, new Rectangle(new Point(0, 0), image.Size));
                        foreach (UserData user in frame.Users)
                        {
                            if (user.IsNew && user.IsVisible)
                                uTracker.StartSkeletonTracking(user.UserId);
                            if (user.IsVisible && user.Skeleton.State == Skeleton.SkeletonState.Tracked)
                            {
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RightHand, SkeletonJoint.JointType.RightElbow);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.LeftHand, SkeletonJoint.JointType.LeftElbow);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RightElbow, SkeletonJoint.JointType.RightShoulder);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.LeftElbow, SkeletonJoint.JointType.LeftShoulder);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RightFoot, SkeletonJoint.JointType.RightKnee);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.LeftFoot, SkeletonJoint.JointType.LeftKnee);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RightKnee, SkeletonJoint.JointType.RightHip);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.LeftKnee, SkeletonJoint.JointType.LeftHip);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RightShoulder, SkeletonJoint.JointType.LeftShoulder);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RightHip, SkeletonJoint.JointType.LeftHip);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RightShoulder, SkeletonJoint.JointType.RightHip);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.LeftShoulder, SkeletonJoint.JointType.LeftHip);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.Head, SkeletonJoint.JointType.Neck);
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

        private void DrawLineBetweenJoints(Graphics g, Skeleton skel, SkeletonJoint.JointType j1, SkeletonJoint.JointType j2)
        {
            try
            {
                if (skel.State == Skeleton.SkeletonState.Tracked)
                {
                    SkeletonJoint joint1 = skel.GetJoint(j1);
                    SkeletonJoint joint2 = skel.GetJoint(j2);
                    if (joint1.Position.Z > 0 && joint2.Position.Z > 0)
                    {
                        Point j1PosEllipse = new Point();
                        Point j2PosEllipse = new Point();
                        PointF j1PosLine = uTracker.ConvertJointCoordinatesToDepth(joint1.Position);
                        PointF j2PosLine = uTracker.ConvertJointCoordinatesToDepth(joint2.Position);
                        j1PosEllipse.X = (int)j1PosLine.X - 5;
                        j1PosEllipse.Y = (int)j1PosLine.Y - 5;
                        j2PosEllipse.X = (int)j2PosLine.X - 5;
                        j2PosEllipse.Y = (int)j2PosLine.Y - 5;
                        j1PosLine.X -= 2;
                        j1PosLine.Y -= 2;
                        j2PosLine.X -= 2;
                        j2PosLine.Y -= 2;
                        g.DrawLine(new Pen(Brushes.White, 3), j1PosLine, j2PosLine);
                        g.DrawEllipse(new Pen(Brushes.White, 5), new Rectangle(j1PosEllipse, new Size(5, 5)));
                        g.DrawEllipse(new Pen(Brushes.White, 5), new Rectangle(j2PosEllipse, new Size(5, 5)));
                    }
                }
            }
            catch (Exception) { }
        }

        private void frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            NiTE.Shutdown();
            OpenNIWrapper.OpenNI.Shutdown();
        }
    }
}
