﻿using System;
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
        string labelText;
        Canvas canvas;

        public MainForm(Canvas canvas)
        {
            InitializeComponent();

            this.canvas = canvas;
            pictureBox1.Size = new Size { Height = canvas.h, Width = canvas.w};
            pictureBox1.Image = canvas.bmp;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (labelText != null)
            {
                label1.Text = labelText;
                labelText = null;
            }

            canvas.UpdateBitmap();
            pictureBox1.Refresh();
        }

        public void Print(string str)
        {
			labelText = str;
        }
    }
}
