using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays
{
    public class Tracer
    {
        private volatile Canvas canvas;
        private volatile SceneData scene;
        private volatile CacheData chacheData;
        public event Action EndSempl;

        public Tracer(Canvas canvas, SceneData scene, CacheData chacheData, int seed)
        {
            this.chacheData = chacheData;
            this.canvas = canvas;
            this.scene = scene;
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


        IMaterial RayMarching(Vector origin, Vector direction)
        {
            float[,,] data = chacheData.GetDataDist(direction);

            Vector hitPos = origin;
            int noHitCount = 0;
            float d = chacheData.FastFastQueryDatabase(hitPos, data); // distance from closest object in world.
            float prev_d;
            // Signed distance marching
            for (float total_d = d; total_d < 100; total_d += d)
            {
                //Vec prev = hitPos;
                hitPos = origin + direction * total_d;
                prev_d = d;
                d = chacheData.FastFastQueryDatabase(hitPos, data);
                //if (prev_d >= accuracy + 0.01 && d < accuracy//тут было 0.01
                if (d < accuracy/*тут было 0.01*/ || ++noHitCount > 99 || d < 0)
                    return chacheData.FastFastQueryDatabaseHitType(hitPos); // Weird return statement where a variable is also updated.
            }

            return null;
        }

        // Perform signed sphere marching
        // Returns IMaterial and update hit position/normal
        IMaterial RayMarching(Vector origin, Vector direction, out Vector hitPos, out Vector hitNorm)
        {
            float[,,] data = chacheData.GetDataDist(direction);

            hitPos = origin;
            hitNorm = direction;
            int noHitCount = 0;
            float d = chacheData.FastFastQueryDatabase(hitPos, data); // distance from closest object in world.
            float prev_d;
            //bool outside = false;
            // Signed distance marching
            for (float total_d = d; total_d < 100; total_d += d)
            {
                //Vec prev = hitPos;
                hitPos = origin + direction * total_d;
                prev_d = d;
                d = chacheData.FastFastQueryDatabase(hitPos, data);
                //if (prev_d >= accuracy + 0.01 && d < accuracy//тут было 0.01
                if (d < accuracy//тут было 0.01
                            || ++noHitCount > 99 || d < 0)
                {
                    var hitType = chacheData.FastFastQueryDatabaseHitType(hitPos);

                    if (noHitCount < 99)
                    {
                        if (hitType is MirrorMaterial)
                        {
                            //Считаем по-честному, чтоб точность побольше была
                            d = scene.QueryDatabase(hitPos, out _);
                            hitNorm = scene.QueryDatabaseNorm(hitPos, d);
                        }
                        else
                        {
                            hitNorm = chacheData.FastFastQueryDatabaseNorm(hitPos);
                        }
                    }

                    if (float.IsNaN(hitNorm.x))
                    {
                        //hitNorm = hitNorm;
                        hitNorm = direction;
                    }

                    //hitPos = prev;
                    //hitPos = (direction%hitNorm)
                    return hitType; // Weird return statement where a variable is also updated.
                }
            }

            return null;
        }

        public TimeSpan timeSpan;

        private float accuracy;

        Vector Trace(Vector origin, Vector direction)
        {
            Vector color = new Vector();
            float attenuation = 1;
            accuracy = 0.02f;
            for (int bounceCount = 0; bounceCount < 3; bounceCount++)
            {
                var hitType = RayMarching(origin, direction, out var sampledPosition, out var normal);
                accuracy *= 2;
                if (hitType == null || !hitType.ApplyColor(sampledPosition, normal, randomVal,
                    ref direction, ref origin, ref attenuation, ref color))
                    break;

                if (hitType is WallMaterial)
                    foreach (var light in scene.lights)
                        light.ApplyColor(sampledPosition, normal, randomVal,
                            RayMarching, attenuation, ref color);
            }
            return color;
        }
    }
}
