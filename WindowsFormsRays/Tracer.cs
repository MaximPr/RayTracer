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
        private ICamera camera;
        private List<IRayMarching> rayMarchings;
        private List<ILight> lights;
        public event Action EndSempl;

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
                        float cameraX = (x - canvas.w / 2 + randomVal()) / canvas.w;
                        float cameraY = (y - canvas.h / 2 + randomVal()) / canvas.w;
                        Vector position = camera.GetPosition(cameraX, cameraY);
                        Vector direction = camera.GetDirection(cameraX, cameraY);
                        Vector color = Trace(position, direction);
                        canvas.AddPixel(x, y, (int)color.x, (int)color.y, (int)color.z);
                    }

                timeSpan = DateTime.Now - startTime;
                EndSempl?.Invoke();
            }
        }

        Random r = new Random();

        float randomVal() { return (float)r.NextDouble(); }

        public TimeSpan timeSpan;

        Vector Trace(Vector origin, Vector direction)
        {
            Vector color = new Vector();
            float attenuation = 1;
            foreach(var rayMarching in rayMarchings)
            {
                var hitType = rayMarching.RayMarching(origin, direction, out var sampledPosition, out var normal);
                if (hitType == null || !hitType.ApplyColor(sampledPosition, normal, randomVal,
                    ref direction, ref origin, ref attenuation, ref color))
                    break;

                if (hitType is WallMaterial)
                    foreach (var light in lights)
                        light.ApplyColor(sampledPosition, normal, randomVal,
                            rayMarching, attenuation, ref color);
            }
            return color;
        }
    }
}
