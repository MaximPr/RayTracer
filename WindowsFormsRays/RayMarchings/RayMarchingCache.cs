using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.RayMarchings
{
    public class RayMarchingCache : IRayMarching
    {
        public float accuracy;
        private SceneData sceneData;
        private CacheData chacheData;

        public RayMarchingCache(SceneData sceneData, CacheData chacheData, float accuracy)
        {
            this.sceneData = sceneData;
            this.chacheData = chacheData;
            this.accuracy = accuracy;
        }

        public IMaterial RayMarching(Vector origin, Vector direction)
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
        public IMaterial RayMarching(Vector origin, Vector direction, out Vector hitPos, out Vector hitNorm)
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
                            d = sceneData.QueryDatabase(hitPos, out _);
                            hitNorm = sceneData.QueryDatabaseNorm(hitPos, d);
                        }
                        else
                        {
                            hitNorm = chacheData.FastFastQueryDatabaseNorm(hitPos);
                        }
                    }

                    if (float.IsNaN(hitNorm.x))
                        hitNorm = direction;

                    //hitPos = prev;
                    //hitPos = (direction%hitNorm)
                    return hitType; // Weird return statement where a variable is also updated.
                }
            }

            return null;
        }
    }
}
