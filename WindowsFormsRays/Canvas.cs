using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsRays
{
    public class Canvas
    {
        public readonly int w;
        public readonly int h;
        public Canvas(int w, int h)
        {
            this.w = w;
            this.h = h;
            sourceBmp = new MyPixel[w, h];
            bmp = new Bitmap(w, h);
        }

        private volatile MyPixel[,] sourceBmp;
        public volatile Bitmap bmp;
        public void AddPixel(int x, int y, int r, int g, int b)
        {
            lock (bmp)
            {
                sourceBmp[x, y].c++;
                sourceBmp[x, y].r += r;
                sourceBmp[x, y].g += g;
                sourceBmp[x, y].b += b;
                bmp.SetPixel(x, y, sourceBmp[x, y].GetColor());
            }
        }


    }
}
