using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsRays.Cameras;
using WindowsFormsRays.Lights;
using WindowsFormsRays.Materials;
using WindowsFormsRays.RayMarchings;

namespace WindowsFormsRays
{
    public class Tracer
    {
        private volatile Canvas canvas;
        private readonly ICamera camera;
        private readonly List<IRayMarching> rayMarchings;
        private readonly List<ILight> lights;
        public event Action EndSempl;
        private readonly Random r = new Random();

        public Tracer(Canvas canvas, ICamera camera, List<IRayMarching> rayMarchings,
            List<ILight> lights, int seed)
        {
            this.canvas = canvas;
            this.camera = camera;
            this.rayMarchings = rayMarchings;
            this.lights = lights;
            r = new Random(seed);
        }

        public DateTime startTime;
        public TimeSpan timeSpan;

        public async void AsyncRun(CancellationToken cancellationToken)
        {
            await Task.Run(Run, cancellationToken);
        }

        public void Run()
        {
            startTime = DateTime.Now;
            int samplesCount = 2000;
            for (int p = 0; p < samplesCount; p++)
            {
                for (int y = 0; y < canvas.h; y++)
                    for (int x = 0; x < canvas.w; x++)
                    {
                        float cameraX = (x - canvas.w / 2 + RandomVal()) / canvas.w;
                        float cameraY = (y - canvas.h / 2 + RandomVal()) / canvas.w;
                        Vector position = camera.GetPosition(cameraX, cameraY);
                        Vector direction = camera.GetDirection(cameraX, cameraY);
                        Vector color = Trace(position, direction);
                        canvas.AddPixel(x, y, (int)color.x, (int)color.y, (int)color.z);
                    }

                timeSpan = DateTime.Now - startTime;
                EndSempl?.Invoke();
            }
        }

        float RandomVal() { return (float)r.NextDouble(); }

        Vector Trace(Vector origin, Vector direction)
        {
            Vector color = new Vector();
            Vector colorFilter = new Vector(1, 1, 1);
            foreach(var rayMarching in rayMarchings)
            {
                var hitType = rayMarching.RayMarching(origin, direction, out var sampledPosition, out var normal);
                if (hitType == null || !hitType.ApplyColor(sampledPosition, normal, RandomVal,
                    ref direction, ref origin, ref color, ref colorFilter))
                    break;

                if (hitType is DifuseMaterial)
                    foreach (var light in lights)
                        light.ApplyColor(sampledPosition, normal, RandomVal,
                            rayMarching, ref colorFilter, ref color);
            }
            return color;
        }
    }
}
