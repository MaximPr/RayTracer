using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsRays
{
    public partial class MainForm : Form
    {
        public MainForm(Bitmap bmp)
        {
            InitializeComponent();
            pictureBox1.Image = bmp;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (pictureBox1.Image)
            {
                Refresh();
            }
        }

        public void Print(string str)
        {
            Invoke(new Action(() => label1.Text = str));
        }
    }
}
