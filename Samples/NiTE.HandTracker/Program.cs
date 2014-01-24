namespace NiTEHandTracker
{
    #region

    using System;
    using System.Windows.Forms;

    #endregion

    internal static class Program
    {
        #region Methods

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frm_Main());
        }

        #endregion
    }
}