﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsRays
{
    struct MyPixel
    {
        public int c, r, g, b;

        public Color GetColor()
        {
            // Reinhard tone mapping
            var color = new Vec(r, g, b) * (1f / c) + 14f / 241;
            Vec o = color + 1;
            color = new Vec(color.x / o.x, color.y / o.y, color.z / o.z) * 255;

            return Color.FromArgb((int)color.x, (int)color.y, (int)color.z);
        }
    }
}
