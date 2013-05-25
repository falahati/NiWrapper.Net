using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        UserTracker uTracker;
        private void button1_Click(object sender, EventArgs e)
        {
            uTracker = UserTracker.Create();
            btn_start.Enabled = false;
            //uTracker.onNewData += new UserTracker.UserTrackerListener(uTracker_onNewData);

            /* Because of incompatibility between current version of OpenNI and NiTE,
             * we can't use event based reading. So we put our sample in a loop.
             * You can copy OpenNI.dll from version 2.0 to solve this problem.
             * Then you can uncomment above line of code and comment below ones.
             */
            while (true)
            {
                uTracker_onNewData(uTracker);
                Application.DoEvents();
            }
        }

        ulong lastTime;
        ulong fps;
        void uTracker_onNewData(UserTracker uTracker)
        {
            using (UserTrackerFrameRef frame = uTracker.readFrame())
            {
                lock (image)
                {
                    if (image.Width != frame.UserMap.FrameSize.Width || image.Height != frame.UserMap.FrameSize.Height)
                        image = new Bitmap(frame.UserMap.FrameSize.Width, frame.UserMap.FrameSize.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        g.FillRectangle(Brushes.Black, new Rectangle(new Point(0, 0), image.Size));
                        foreach (UserData user in frame.Users)
                        {
                            if (user.isNew && user.isVisible)
                                uTracker.StartSkeletonTracking(user.UserId);
                            if (user.isVisible && user.Skeleton.State == Skeleton.SkeletonState.TRACKED)
                            {
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RIGHT_HAND, SkeletonJoint.JointType.RIGHT_ELBOW);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.LEFT_HAND, SkeletonJoint.JointType.LEFT_ELBOW);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RIGHT_ELBOW, SkeletonJoint.JointType.RIGHT_SHOULDER);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.LEFT_ELBOW, SkeletonJoint.JointType.LEFT_SHOULDER);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RIGHT_FOOT, SkeletonJoint.JointType.RIGHT_KNEE);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.LEFT_FOOT, SkeletonJoint.JointType.LEFT_KNEE);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RIGHT_KNEE, SkeletonJoint.JointType.RIGHT_HIP);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.LEFT_KNEE, SkeletonJoint.JointType.LEFT_HIP);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RIGHT_SHOULDER, SkeletonJoint.JointType.LEFT_SHOULDER);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RIGHT_HIP, SkeletonJoint.JointType.LEFT_HIP);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.RIGHT_SHOULDER, SkeletonJoint.JointType.RIGHT_HIP);
                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.LEFT_SHOULDER, SkeletonJoint.JointType.LEFT_HIP);

                                DrawLineBetweenJoints(g, user.Skeleton, SkeletonJoint.JointType.HEAD, SkeletonJoint.JointType.NECK);
                            }
                        }
                        g.Save();
                    }
                }
                this.Invoke(new Action(delegate()
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
                if (skel.State == Skeleton.SkeletonState.TRACKED)
                {
                    SkeletonJoint joint1 = skel.getJoint(j1);
                    SkeletonJoint joint2 = skel.getJoint(j2);
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
        }
    }
}
