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
            sourceBmp = new MyPixel[h][];
            for (int y = 0; y < h; y++)
                sourceBmp[y] = new MyPixel[w];
            sourceChange = new bool[h];
            bmp = new Bitmap(w, h);
            line = new Color[w];
        }

        private volatile MyPixel[][] sourceBmp;
        private volatile bool[] sourceChange;

        public void AddPixel(int x, int y, int r, int g, int b)
        {
            lock (sourceBmp[y])
            {
                sourceBmp[y][x].c++;
                sourceBmp[y][x].r += r;
                sourceBmp[y][x].g += g;
                sourceBmp[y][x].b += b;
                sourceChange[y] = true;
            }
        }

        public void AddLine(int y, int[] r, int[] g, int[] b)
        {
            lock (sourceBmp[y])
            {
                for (int x = 0; x < w; x++)
                {
                    sourceBmp[y][x].c++;
                    sourceBmp[y][x].r += r[x];
                    sourceBmp[y][x].g += g[x];
                    sourceBmp[y][x].b += b[x];
                }
                sourceChange[y] = true;
            }
        }

        public Bitmap bmp;
        Color[] line;

        public void UpdateBitmap()
        {
            for (int y = 0; y < h; y++)
                if (sourceChange[y])
                {
                    lock (sourceBmp[y])
                    {
                        sourceChange[y] = false;
                        for (int x = 0; x < w; x++)
                            line[x] = sourceBmp[y][x].GetColor();
                    }

                    for (int x = 0; x < w; x++)
                        bmp.SetPixel(x, y, line[x]);
                }
        }
    }
}
