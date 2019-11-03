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

            tracer = new Tracer(w, h, QueryDatabase);

            t = new Thread(() =>
            {
                InitLiterals();
                tracer.InitData();
                tracer.startTime = DateTime.Now;

                tracer.Run();

                stop = true;
            });
            t.Start();

            pictureBox1.Image = tracer.bmp;
        }

        bool stop = false;

        float min(float l, float r) { return l < r ? l : r; }

        // Rectangle CSG equation. Returns minimum signed distance from 
        // space carved by lowerLeft vertex and opposite rectangle
        // vertex upperRight.
        float BoxTest(Vector position, Vector lowerLeft, Vector upperRight)
        {
            lowerLeft = position - lowerLeft;
            upperRight = upperRight - position;
            return -min(
              min(
                min(lowerLeft.x, upperRight.x),
                min(lowerLeft.y, upperRight.y)
              ),
              min(lowerLeft.z, upperRight.z));
        }

        enum HitType
        {
            HIT_NONE = 0,
            HIT_LETTER = 1,
            HIT_WALL = 2,
            HIT_SUN = 3,
        }

        Vector[] begins;
        Vector[] es;
        void InitLiterals()
        {
            const string letters =               // 15 two points lines
              "5O5_5W9W5_9_AOEOCOC_A_E_IOQ_I_QOUOY_Y_]OWW[WaOa_aWeWa_e_cWiO"; // R (without curve)

            begins = new Vector[letters.Length / 4];
            es = new Vector[letters.Length / 4];
            for (int i = 0; i < letters.Length; i += 4)
            {
                begins[i / 4] = new Vector(letters[i] - 79, letters[i + 1] - 79, 0.0f) * .5f;
                es[i / 4] = new Vector(letters[i + 2] - 79, letters[i + 3] - 79, 0.0f) * .5f - begins[i / 4];
            }
        }

        float Pow8(float d)
        {
            var d2 = d * d;
            var d4 = d2 * d2;
            return d4 * d4;
        }

        // Sample the world using Signed Distance Fields.
        float QueryDatabase(Vector position, out int hitType)
        {

            float distance = 1e9f;
            Vector f = position; // Flattened position (z=0)
            f.z = 0;

            float w = 2.0f;
            if (position.z < w && position.z > -w)
            {
                for (int i = 0; i < begins.Length; i++)
                {
                    Vector begin = begins[i];
                    Vector e = es[i];
                    Vector o = f - (begin + e * min(-min((begin + f * -1) % e / (e % e), 0), 1));
                    distance = min(distance, o % o); // compare squared distance.
                }
                distance = (float)Math.Sqrt(distance); // Get real distance, not square distance.

                // Two curves (for P and R in PixaR) with hard-coded locations.
                Vector[] curves = { new Vector(11, -6), new Vector(-11, -6) };
                for (int i = 2; i-- > 0;)
                {
                    Vector o = f + curves[i];

                    if (o.x > 0)
                    {
                        distance = min(distance, (float)Math.Abs(Math.Sqrt(o % o) - 2));
                    }
                    else
                    {
                        o.y += o.y > 0 ? -2 : 2;
                        distance = min(distance, (float)Math.Sqrt(o % o));
                    }
                }
                distance = (float)Math.Pow(Pow8(distance) + Pow8(position.z), 0.125f) - 0.5f;
            }
            else
            {
                distance = BoxTest(position, new Vector(-15, -10, -w / 2), new Vector(15, 10, w / 2));
            }
            hitType = (int)HitType.HIT_LETTER;

            float roomDist = min(// min(A,B) = Union with Constructive solid geometry
                                 //-min carves an empty space
                          -min(// Lower room
                               BoxTest(position, new Vector(-30, -.5f, -30), new Vector(30, 18, 30)),
                               // Upper room
                               BoxTest(position, new Vector(-25, 17, -25), new Vector(25, 20, 25))
                          )
                          ,
                          BoxTest( // Ceiling "planks" spaced 8 units apart.
                            new Vector(Math.Abs(position.x) % 8,
                                position.y,
                                position.z),
                            new Vector(1.5f, 18.5f, -25),
                            new Vector(6.5f, 20, 25)
                          )
            );
            if (roomDist < distance)
            {
                distance = roomDist;
                hitType = (int)HitType.HIT_WALL;
            }

            float sun = 19.9f - position.y; // Everything above 19.9 is light source.
            if (sun < distance)
            {
                distance = sun;
                hitType = (int)HitType.HIT_SUN;
            }

            return distance;
        }


        Vector lightDirection = new Vector(.6f, .6f, 1).Normal();// Directional light

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
