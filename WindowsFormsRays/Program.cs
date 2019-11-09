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

            CancellationTokenSource source = new CancellationTokenSource();
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var form = new MainForm(canvas);

                SceneData scene = new SceneData();
                CacheData cacheData = new CacheData(scene);

                List<Tracer> tracers = new List<Tracer>();
                for (int i = 0; i < 8; i++)
                    tracers.Add(new Tracer(canvas, scene, cacheData, i));

                AsyncRun(form, cacheData, tracers, source.Token);

                Application.Run(form);
            }
            finally
            {
                source.Cancel();
            }
        }

        public static async void AsyncRun(MainForm form, CacheData cacheData, List<Tracer> tracers, CancellationToken cancellationToken)
        {
            await Task.Run(cacheData.InitData, cancellationToken);

            form.Print("start");

            var firstTracer = tracers.FirstOrDefault();
            int iter = 0;
            firstTracer.EndSempl += () =>
            {
                iter++;
                form.Print($"{iter} iter; {firstTracer.timeSpan.TotalSeconds} sec; {firstTracer.timeSpan.TotalSeconds / iter} sec/iter;  {cacheData.steps}");
            };
            
            foreach(var tracer in tracers)
                tracer.AsyncRun(cancellationToken);
        }
    }
}
