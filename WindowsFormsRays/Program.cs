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
            SceneData scene = new SceneData();
            
            var tracer = new Tracer(canvas, scene);
            var tracer2 = new Tracer(canvas, scene);

            Thread t = new Thread(() =>
            {
                tracer.InitData();
                tracer.Run();
            });

            Thread t2 = new Thread(() =>
            {
                tracer2.InitData();
                tracer2.Run();
            });
            t.Start();
            t2.Start();

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
                    form.Print($"{iter} iter; {tracer.timeSpan.TotalSeconds} sec; {tracer.timeSpan.TotalSeconds / iter} sec/iter;  {tracer.steps}");
                };
                Application.Run(form);
            }
            finally
            {
                t.Abort();
                t2.Abort();
            }
        }
    }
}
