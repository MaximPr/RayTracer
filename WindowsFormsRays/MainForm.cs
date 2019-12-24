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
        Canvas canvas;

        public MainForm(Canvas canvas)
        {
            InitializeComponent();

            this.canvas = canvas;
            pictureBox1.Size = new Size { Height = canvas.h, Width = canvas.w};
            pictureBox1.Image = new Bitmap(canvas.w, canvas.h);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            canvas.CopyTo(pictureBox1.Image);
            pictureBox1.Refresh();
        }

        public void Print(string str)
        {
            Invoke(new Action(() => label1.Text = str));
        }
    }
}
