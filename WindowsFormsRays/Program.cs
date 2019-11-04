using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            int w = 622;
            int h = 381;
            Canvas canvas = new Canvas(w, h);
            //Canvas canvas2 = new Canvas(w, h);
            SceneData scene = new SceneData();
            ChacheData chacheData = new ChacheData(scene);

            chacheData.InitData();

            var tracer = new Tracer(canvas, scene, chacheData, 1);
            var tracer2 = new Tracer(canvas, scene, chacheData, 2);
            var tracer3 = new Tracer(canvas, scene, chacheData, 3);
            //var tracer4 = new Tracer(canvas, scene, chacheData, 4);

            Thread t = new Thread(() =>
            {
                Thread.Sleep(1000);
                tracer.Run();
            });

            Thread t2 = new Thread(() =>
            {
                Thread.Sleep(1000);
                tracer2.Run();
            });

            Thread t3 = new Thread(() =>
            {
                Thread.Sleep(1000);
                tracer3.Run();
            });

            //Thread t4 = new Thread(() =>
            //{
            //    Thread.Sleep(5000);
            //    tracer4.Run();
            //});

            t.Start();
            t2.Start();
            t3.Start();
            //t4.Start();

            //https://habr.com/post/434528/
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var form = new MainForm(canvas.bmp);

                int iter = 0;
                tracer.EndSempl += () =>
                {
                    iter++;
                    form.Print($"{iter} iter; {tracer.timeSpan.TotalSeconds} sec; {tracer.timeSpan.TotalSeconds / iter} sec/iter;  {chacheData.steps}");
                };
                Application.Run(form);
            }
            finally
            {
                t.Abort();
                t2.Abort();
                t3.Abort();
                //t4.Abort();
            }
        }
    }
}
