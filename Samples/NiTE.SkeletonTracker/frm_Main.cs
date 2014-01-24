namespace NiTESkeletonTracker
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
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
        Justification = "Reviewed. Suppression is OK here.")]
    public partial class frm_Main : Form
    {
        #region Fields

        private ulong fps;

        private Bitmap image = new Bitmap(1, 1);

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

            MessageBox.Show(@"Error: " + status);
            return false;
        }

        private void DrawLineBetweenJoints(
            Graphics g,
            Skeleton skel,
            SkeletonJoint.JointType j1,
            SkeletonJoint.JointType j2)
        {
            try
            {
                if (skel.State == Skeleton.SkeletonState.Tracked)
                {
                    SkeletonJoint joint1 = skel.GetJoint(j1);
                    SkeletonJoint joint2 = skel.GetJoint(j2);
                    if (joint1.Position.Z > 0 && joint2.Position.Z > 0)
                    {
                        Point joint1PosEllipse = new Point();
                        Point joint2PosEllipse = new Point();
                        PointF joint1PosLine = this.userTracker.ConvertJointCoordinatesToDepth(joint1.Position);
                        PointF joint2PosLine = this.userTracker.ConvertJointCoordinatesToDepth(joint2.Position);
                        joint1PosEllipse.X = (int)joint1PosLine.X - 5;
                        joint1PosEllipse.Y = (int)joint1PosLine.Y - 5;
                        joint2PosEllipse.X = (int)joint2PosLine.X - 5;
                        joint2PosEllipse.Y = (int)joint2PosLine.Y - 5;
                        joint1PosLine.X -= 2;
                        joint1PosLine.Y -= 2;
                        joint2PosLine.X -= 2;
                        joint2PosLine.Y -= 2;
                        g.DrawLine(new Pen(Brushes.White, 3), joint1PosLine, joint2PosLine);
                        g.DrawEllipse(new Pen(Brushes.White, 5), new Rectangle(joint1PosEllipse, new Size(5, 5)));
                        g.DrawEllipse(new Pen(Brushes.White, 5), new Rectangle(joint2PosEllipse, new Size(5, 5)));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void Button1Click(object sender, EventArgs e)
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

            lock (this.image)
            {
                if (this.image.Width != frame.UserMap.FrameSize.Width
                    || this.image.Height != frame.UserMap.FrameSize.Height)
                {
                    this.image = new Bitmap(
                        frame.UserMap.FrameSize.Width,
                        frame.UserMap.FrameSize.Height,
                        PixelFormat.Format24bppRgb);
                }

                using (Graphics g = Graphics.FromImage(this.image))
                {
                    g.FillRectangle(Brushes.Black, new Rectangle(new Point(0, 0), this.image.Size));
                    foreach (UserData user in frame.Users)
                    {
                        if (user.IsNew && user.IsVisible)
                        {
                            userTracker.StartSkeletonTracking(user.UserId);
                        }

                        if (user.IsVisible && user.Skeleton.State == Skeleton.SkeletonState.Tracked)
                        {
                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.RightHand,
                                SkeletonJoint.JointType.RightElbow);
                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.LeftHand,
                                SkeletonJoint.JointType.LeftElbow);

                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.RightElbow,
                                SkeletonJoint.JointType.RightShoulder);
                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.LeftElbow,
                                SkeletonJoint.JointType.LeftShoulder);

                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.RightFoot,
                                SkeletonJoint.JointType.RightKnee);
                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.LeftFoot,
                                SkeletonJoint.JointType.LeftKnee);

                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.RightKnee,
                                SkeletonJoint.JointType.RightHip);
                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.LeftKnee,
                                SkeletonJoint.JointType.LeftHip);

                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.RightShoulder,
                                SkeletonJoint.JointType.LeftShoulder);
                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.RightHip,
                                SkeletonJoint.JointType.LeftHip);

                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.RightShoulder,
                                SkeletonJoint.JointType.RightHip);
                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.LeftShoulder,
                                SkeletonJoint.JointType.LeftHip);

                            this.DrawLineBetweenJoints(
                                g,
                                user.Skeleton,
                                SkeletonJoint.JointType.Head,
                                SkeletonJoint.JointType.Neck);
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