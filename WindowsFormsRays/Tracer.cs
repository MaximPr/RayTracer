using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsRays.Lights;
using WindowsFormsRays.Materials;
using WindowsFormsRays.RayMarchings;

namespace WindowsFormsRays
{
    public class Tracer
    {
        private volatile Canvas canvas;
        private List<IRayMarching> rayMarchings;
        private List<ILight> lights;
        public event Action EndSempl;

        public Tracer(Canvas canvas, List<IRayMarching> rayMarchings, List<ILight> lights, int seed)
        {
            this.canvas = canvas;
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
                        Vector position = new Vector(-22, 5, 25);// + new Vector(randomVal(), randomVal(), randomVal()) * 0.7f;
                        //Vec position = new Vec(-20, 5, 18);

                        Vector goal = (new Vector(-3, 4, 0) - position).Normal();
                        Vector left = -(new Vector(goal.z, 0, -goal.x)).Normal() * (1.0f / canvas.w);

                        // Cross-product to get the up vector
                        Vector up = (new Vector(goal.y * left.z - goal.z * left.y,
                               goal.z * left.x - goal.x * left.z,
                               goal.x * left.y - goal.y * left.x));

                        //Vec target = (goal + left * (x - w / 2 + randomVal())*0.1f + up * (y - h / 2 + randomVal())*0.1f ).Normal();
                        Vector target = (goal + left * (x - canvas.w / 2 + randomVal()) + up * (y - canvas.h / 2 + randomVal())).Normal();
                        Vector color = Trace(position, target);
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
                            rayMarching.RayMarching, attenuation, ref color);
            }
            return color;
        }
    }
}
