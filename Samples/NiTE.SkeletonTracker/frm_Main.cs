using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using NiTEWrapper;
using OpenNIWrapper;
using Size = System.Drawing.Size;

namespace NiTESkeletonTracker
{
    #region

    #endregion

    // ReSharper disable once InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
        Justification = "Reviewed. Suppression is OK here.")]
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

        private Bitmap image;

        private ulong lastTime;

        private UserTracker userTracker;

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
                    var joint1 = skel.GetJoint(j1);
                    var joint2 = skel.GetJoint(j2);

                    if (joint1.Position.Z > 0 && joint2.Position.Z > 0)
                    {
                        var joint1PosEllipse = new Point();
                        var joint2PosEllipse = new Point();
                        var joint1PosLine = userTracker.ConvertJointCoordinatesToDepth(joint1.Position);
                        var joint2PosLine = userTracker.ConvertJointCoordinatesToDepth(joint2.Position);
                        joint1PosEllipse.X = (int) joint1PosLine.X - 5;
                        joint1PosEllipse.Y = (int) joint1PosLine.Y - 5;
                        joint2PosEllipse.X = (int) joint2PosLine.X - 5;
                        joint2PosEllipse.Y = (int) joint2PosLine.Y - 5;
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
            userTracker = UserTracker.Create();
            btn_start.Enabled = false;
            userTracker.OnNewData += UserTrackerOnNewData;
        }

        private void FrmMainFormClosing(object sender, FormClosingEventArgs e)
        {
            userTracker?.Dispose();
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
        private void UserTrackerOnNewData(UserTracker userTracker)
        {
            if (!userTracker.IsValid)
            {
                return;
            }

            using (var frame = userTracker.ReadFrame())
            {
                if (frame == null || !frame.IsValid)
                {
                    return;
                }

                lock (this)
                {
                    if (image?.Width != frame.UserMap.FrameSize.Width ||
                        image?.Height != frame.UserMap.FrameSize.Height)
                    {
                        image = new Bitmap(
                            frame.UserMap.FrameSize.Width,
                            frame.UserMap.FrameSize.Height,
                            PixelFormat.Format24bppRgb);
                    }

                    using (var g = Graphics.FromImage(image))
                    {
                        g.FillRectangle(Brushes.Black, new Rectangle(new Point(0, 0), image.Size));

                        foreach (var user in frame.Users)
                        {
                            if (user.IsNew && user.IsVisible)
                            {
                                userTracker.StartSkeletonTracking(user.UserId);
                            }

                            if (user.IsVisible && user.Skeleton.State == Skeleton.SkeletonState.Tracked)
                            {
                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.RightHand,
                                    SkeletonJoint.JointType.RightElbow);
                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.LeftHand,
                                    SkeletonJoint.JointType.LeftElbow);

                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.RightElbow,
                                    SkeletonJoint.JointType.RightShoulder);
                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.LeftElbow,
                                    SkeletonJoint.JointType.LeftShoulder);

                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.RightFoot,
                                    SkeletonJoint.JointType.RightKnee);
                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.LeftFoot,
                                    SkeletonJoint.JointType.LeftKnee);

                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.RightKnee,
                                    SkeletonJoint.JointType.RightHip);
                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.LeftKnee,
                                    SkeletonJoint.JointType.LeftHip);

                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.RightShoulder,
                                    SkeletonJoint.JointType.LeftShoulder);
                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.RightHip,
                                    SkeletonJoint.JointType.LeftHip);

                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.RightShoulder,
                                    SkeletonJoint.JointType.RightHip);
                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.LeftShoulder,
                                    SkeletonJoint.JointType.LeftHip);

                                DrawLineBetweenJoints(
                                    g,
                                    user.Skeleton,
                                    SkeletonJoint.JointType.Head,
                                    SkeletonJoint.JointType.Neck);
                            }
                        }

                        g.Save();
                    }
                }

                Invoke(
                    new MethodInvoker(
                        delegate
                        {
                            // ReSharper disable AccessToDisposedClosure
                            fps = (1000000 / (frame.Timestamp - lastTime) + fps * 4) / 5;
                            lastTime = frame.Timestamp;
                            Text = string.Format(
                                "Frame #{0} - Time: {1} - FPS: {2}",
                                frame.FrameIndex,
                                frame.Timestamp,
                                fps);
                            // ReSharper restore AccessToDisposedClosure
                            pb_preview.Image?.Dispose();
                            pb_preview.Image = image.Clone(
                                new Rectangle(new Point(0, 0), image.Size),
                                PixelFormat.Format24bppRgb);
                        }));
            }
        }

        #endregion
    }
}