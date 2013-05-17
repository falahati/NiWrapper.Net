using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NiTEWrapper;
namespace NiTEHandTracker
{
    public partial class frm_Main : Form
    {
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
                this.Text = NiTE.Version.ToString();
                //NiTE.Shutdown();
            }
            else
            {
                Environment.Exit(1);
            }
        }
    }
}
