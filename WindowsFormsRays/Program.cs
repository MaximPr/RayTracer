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
            ChacheData chacheData = new ChacheData(scene);

            Tracer tracer = null;
            List<Tracer> tracers = new List<Tracer>();
            for (int i = 0; i < 16; i++)
                tracers.Add(tracer = new Tracer(canvas, scene, chacheData, i));

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            AsyncRun(chacheData, tracers, token);
      
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
                source.Cancel();
            }
        }

        public static async void AsyncRun(ChacheData chacheData, List<Tracer> tracers, CancellationToken cancellationToken)
        {
            await Task.Run(chacheData.InitData, cancellationToken);

            Thread.Sleep(1000);
            
            foreach(var tracer in tracers)
                tracer.AsyncRun(cancellationToken);
        }
    }
}
