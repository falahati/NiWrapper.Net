using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using NiTEWrapper;
using OpenNIWrapper;
using Size = System.Drawing.Size;

namespace NiTEUserColor
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

            MessageBox.Show(string.Format(@"Error: {0}{1}{2}", status, Environment.NewLine, OpenNI.LastError));

            return false;
        }

        private unsafe void FillImageFromUserMap(UserMap um)
        {
            int[] colors = {16777215, 14565387, 32255, 7996159, 16530175, 8373026, 14590399, 7062435, 13951499, 55807};

            if (image == null || image.Width != um.FrameSize.Width || image.Height != um.FrameSize.Height)
            {
                image = new Bitmap(um.FrameSize.Width, um.FrameSize.Height, PixelFormat.Format24bppRgb);
            }

            var bd = image.LockBits(
                new Rectangle(new Point(0, 0), image.Size),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);

            for (var y = 0; y < um.FrameSize.Height; y++)
            {
                var dataPos = (ushort*) ((byte*) um.Pixels.ToPointer() + y * um.DataStrideBytes);
                var imagePos = (byte*) bd.Scan0.ToPointer() + y * bd.Stride;

                for (var x = 0; x < um.FrameSize.Width; x++)
                {
                    var color = colors[*dataPos % colors.Length];
                    *imagePos = (byte) (color / 1 % 256 * *dataPos); // R
                    imagePos++;
                    *imagePos = (byte) (color / 256 % 256 * *dataPos); // G
                    imagePos++;
                    *imagePos = (byte) (color / 65536 % 256 * *dataPos); // B
                    imagePos++;
                    dataPos++;
                }
            }

            image.UnlockBits(bd);
        }

        private void BtnStartClick(object sender, EventArgs e)
        {
            userTracker = UserTracker.Create();
            btn_start.Enabled = false;
            userTracker.OnNewData += UserTrackerOnNewData;
        }

        private void FrmMainFormClosing(object sender, FormClosingEventArgs e)
        {
            userTracker.Dispose();
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

                FillImageFromUserMap(frame.UserMap);

                using (var g = Graphics.FromImage(image))
                {
                    foreach (var user in frame.Users)
                    {
                        if (user.CenterOfMass.Z > 0)
                        {
                            var p = new Point();
                            var pf = userTracker.ConvertJointCoordinatesToDepth(user.CenterOfMass);
                            p.X = (int) pf.X - 5;
                            p.Y = (int) pf.Y - 5;
                            g.DrawEllipse(new Pen(Brushes.White, 5), new Rectangle(p, new Size(5, 5)));
                            g.DrawString("Center Of Mass", SystemFonts.DefaultFont, Brushes.White, p.X - 40, p.Y - 20);
                        }
                    }

                    g.Save();
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