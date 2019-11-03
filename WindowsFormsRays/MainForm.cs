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
        private readonly Thread t;
        Tracer tracer;
        public MainForm()
        {
            InitializeComponent();
            //int w = 960, h = 540;
            int w = pictureBox1.Width, h = pictureBox1.Height;

            SceneData scene = new SceneData();

            tracer = new Tracer(w, h, scene);

            t = new Thread(() =>
            {
                tracer.InitData();
                tracer.startTime = DateTime.Now;

                tracer.Run();

                stop = true;
            });
            t.Start();

            pictureBox1.Image = tracer.bmp;
        }

        bool stop = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (tracer == null)
                return;

            lock (tracer.bmp)
            {
                label1.Text = tracer.p + " iter " + tracer.timeSpan.TotalSeconds + " sec " + tracer.timeSpan.TotalSeconds / tracer.p + " spf " + tracer.steps;
                Refresh();
            }

            if (stop)
                timer1.Enabled = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            t.Abort();
        }
    }
}
