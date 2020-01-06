using System;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.RayMarchings
{
    public class RayMarchingScene : IRayMarching
    {
        public float accuracy;
        private SceneData sceneData;

        public RayMarchingScene(SceneData sceneData, float accuracy)
        {
            this.sceneData = sceneData;
            this.accuracy = accuracy;
        }

        public IMaterial RayMarching(Vector origin, Vector direction)
        {
            Vector hitPos = origin;
            int noHitCount = 0;
            float d = sceneData.QueryDatabase(hitPos, out _); // distance from closest object in world.
            // Signed distance marching
            for (float total_d = d; total_d < 100; total_d += d)
            {
                hitPos = origin + direction * total_d;
                d = sceneData.QueryDatabase(hitPos, out var hitObj);
                if (d < accuracy || ++noHitCount > 99 || d < 0)
                    return hitObj.Material; // Weird return statement where a variable is also updated.
            }

            return null;
        }

        // Perform signed sphere marching
        // Returns IMaterial and update hit position/normal
        public IMaterial RayMarching(Vector origin, Vector direction, out Vector hitPos, out Vector hitNorm)
        {
            hitPos = origin;
            hitNorm = direction;
            int noHitCount = 0;
            float d = sceneData.QueryDatabase(hitPos, out _); // distance from closest object in world.
            // Signed distance marching
            for (float total_d = d; total_d < 100; total_d += d)
            {
                hitPos = origin + direction * total_d;
                d = sceneData.QueryDatabase(hitPos, out var hitObj);
                if (d < accuracy || ++noHitCount > 99 || d < 0)
                {
                    if (noHitCount < 99)
                        hitNorm = sceneData.QueryDatabaseNorm(hitPos, d, hitObj);

                    if (float.IsNaN(hitNorm.x))
                        hitNorm = direction;

                    return hitObj.Material; // Weird return statement where a variable is also updated.
                }
            }

            return null;
        }
    }
}
