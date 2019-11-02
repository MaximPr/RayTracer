using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsRays
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
        //https://habr.com/post/434528/
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public struct Vec
    {
        public float x, y, z;

        public Vec(float v = 0) { x = y = z = v; }
        public Vec(float a, float b, float c = 0) { x = a; y = b; z = c; }

        public static Vec operator -(Vec k) { return new Vec(-k.x, -k.y , -k.z); }
        public static Vec operator +(Vec k, Vec r) { return new Vec(k.x + r.x, k.y + r.y, k.z + r.z); }
        public static Vec operator -(Vec k, Vec r) { return new Vec(k.x - r.x, k.y - r.y, k.z - r.z); }
        public static Vec operator +(Vec k, float r) { return new Vec(k.x + r, k.y + r, k.z + r); }
        public static Vec operator *(Vec k, Vec r) { return new Vec(k.x * r.x, k.y * r.y, k.z * r.z); }
        public static Vec operator *(Vec k, float r) { return new Vec(k.x * r, k.y * r, k.z * r); }
        // dot product
        public static float operator %(Vec k, Vec r) { return k.x * r.x + k.y * r.y + k.z * r.z; }
        // inverse square root
        public Vec Normal() { return this * (1 / (float)Math.Sqrt(this % this)); }
    }
}
