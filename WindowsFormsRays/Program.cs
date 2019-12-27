using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsRays.Cameras;
using WindowsFormsRays.RayMarchings;

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

                // new Vec(-20, 5, 18);
                var camera = new PerspectiveCamera(new Vector(-22, 5, 25), new Vector(-3, 4, 0));
                //var camera = new OrtogonalCamera(new Vector(-22+10, 5+4, 25-10), new Vector(-3+10, 4+4, 0-10));

                List<Tracer> tracers = new List<Tracer>();
                for (int i = 0; i < 8; i++)
                {
                    var rayMarchings = new List<IRayMarching>()
                    {
                        //new RayMarchingScene(scene, 0.02f),
                        //new RayMarchingScene(scene, 0.04f),
                        //new RayMarchingScene(scene, 0.08f),
                        new RayMarchingCache(scene, cacheData, 0.02f),
                        new RayMarchingCache(scene, cacheData, 0.04f),
                        new RayMarchingCache(scene, cacheData, 0.08f),
                    };
                    tracers.Add(new Tracer(canvas, camera, rayMarchings, scene.lights, i));
                }

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
