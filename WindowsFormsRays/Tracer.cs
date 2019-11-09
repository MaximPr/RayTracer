using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public int p;

        public async void AsyncRun(CancellationToken cancellationToken)
        {
            await Task.Run(Run, cancellationToken);
        }

        public void Run()
        {
            startTime = DateTime.Now;
            int samplesCount = 2000;
            for (p = 0; p < samplesCount; p++)
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


        int RayMarching(Vector origin, Vector direction, float accuracy)
        {
            float[,,] data = chacheData.GetDataDist(direction);

            Vector hitPos = origin;
            int hitType = (int)HitType.HIT_NONE;
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
                if (d < accuracy//тут было 0.01
                            || ++noHitCount > 99 || d < 0)
                {
                    hitType = chacheData.FastFastQueryDatabaseHitType(hitPos);

                    return hitType; // Weird return statement where a variable is also updated.
                }
            }
            return 0;
        }

        // Perform signed sphere marching
        // Returns hitType 0, 1, 2, or 3 and update hit position/normal
        int RayMarching(Vector origin, Vector direction, float accuracy, out Vector hitPos, out Vector hitNorm)
        {
            float[,,] data = chacheData.GetDataDist(direction);

            hitPos = origin;
            hitNorm = direction;
            int hitType = (int)HitType.HIT_NONE;
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
                    hitType = chacheData.FastFastQueryDatabaseHitType(hitPos);

                    if (noHitCount < 99)
                    {
                        if (hitType == (int)HitType.HIT_LETTER)
                        {
                            d = scene.QueryDatabase(hitPos, out _);
                            hitNorm = new Vector(scene.QueryDatabase(hitPos + new Vector(.01f, 0), out _) - d,
                               scene.QueryDatabase(hitPos + new Vector(0, .01f), out _) - d,
                               scene.QueryDatabase(hitPos + new Vector(0, 0, .01f), out _) - d).Normal();

                            //hitNorm = chacheData.FastFastQueryDatabaseNorm(hitPos);
                        }
                        else
                        {
                            hitNorm = chacheData.FastFastQueryDatabaseNorm(hitPos);
                            //hitNorm = new Vec(chacheData.FastFastQueryDatabase(hitPos + new Vec(.01f, 0)) - d,
                            //  chacheData.FastFastQueryDatabase(hitPos + new Vec(0, .01f)) - d,
                            //  chacheData.FastFastQueryDatabase(hitPos + new Vec(0, 0, .01f)) - d).Normal();
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
            return 0;
        }

        public TimeSpan timeSpan;

        Vector Trace(Vector origin, Vector direction)
        {
            Vector color = new Vector();
            float attenuation = 1;
            //Vec lightDirection = (new Vec(.6f, .6f, 1)).Normal(); 
            float accuracy = 0.02f;
            for (int bounceCount = 3; bounceCount-- > 0;)
            {
                int hitType = RayMarching(origin, direction, accuracy, out var sampledPosition, out var normal);
                accuracy *= 2;
                if (hitType == (int)HitType.HIT_NONE)
                    break; // No hit. This is over, return color.
                if (hitType == (int)HitType.HIT_LETTER)
                { // Specular bounce on a letter. No color acc.
                    direction = direction + normal * (normal % direction * -2);
                    origin = sampledPosition + direction * 0.1f;
                    attenuation = attenuation * 0.2f; // Attenuation via distance traveled.
                }
                if (hitType == (int)HitType.HIT_WALL)
                { // Wall hit uses color yellow?
                    float incidence = normal % scene.lightDirection;
                    float p = 6.283185f * randomVal();
                    float c = randomVal();
                    float s = (float)Math.Sqrt(1 - c);
                    float g = normal.z < 0 ? -1 : 1;
                    float u = -1 / (g + normal.z);
                    float v = normal.x * normal.y * u;
                    direction = new Vector(v,
                                    g + normal.y * normal.y * u,
                                    -normal.y) * ((float)Math.Cos(p) * s)
                                +
                                new Vector(1 + g * normal.x * normal.x * u,
                                    g * v,
                                    -g * normal.x) * ((float)Math.Sin(p) * s) + normal * (float)Math.Sqrt(c);
                    origin = sampledPosition + direction * .1f;
                    attenuation = attenuation * 0.2f;
                    if (incidence > 0)
                    {
                        //тут было 20 вместо 5
                        var ldir = (scene.lightDirection * 5 + new Vector(randomVal(), randomVal(), randomVal())).Normal();
                        if (RayMarching(sampledPosition + normal * .1f,
                                    ldir, accuracy) == (int)HitType.HIT_SUN)

                            color = color + new Vector(500, 400, 100) * (attenuation * incidence * 0.5f);
                    }
                }
                if (hitType == (int)HitType.HIT_SUN)
                { //
                    color = color + new Vector(50, 80, 100) * attenuation;
                    break; // Sun Color
                }
            }
            return color;
        }
    }
}
