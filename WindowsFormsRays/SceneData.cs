using System;
using System.Collections.Generic;
using WindowsFormsRays.SceneObjects;

namespace WindowsFormsRays
{
    public class SceneData
    {
        public Vector lightDirection = new Vector(.6f, .6f, 1).Normal();// Directional light

        private List<IObject3D> objects = new List<IObject3D>();

        public SceneData()
        {
            objects.Add(new Letter3D("PIXAR"));
            objects.Add(new Room3D());
            objects.Add(new Sun3D());
        }

        // Sample the world using Signed Distance Fields.
        public float QueryDatabase(Vector position, out int hitType)
        {
            hitType = (int)HitType.HIT_NONE;
            float distance = 1e9f;
            foreach (var obj in objects)
            {
                var dist = obj.GetDistance(position);
                if (distance > dist)
                {
                    distance = dist;
                    hitType = (int)obj.HitType;
                }
            }

            return distance;
        }

        public Vector QueryDatabaseNorm(Vector hitPos)
        {
            float d = QueryDatabase(hitPos, out var _);
            return QueryDatabaseNorm(hitPos, d);
        }

        public Vector QueryDatabaseNorm(Vector hitPos, float d)
        {
            return new Vector(QueryDatabase(hitPos + new Vector(.01f, 0), out _) - d,
                QueryDatabase(hitPos + new Vector(0, .01f), out _) - d,
                QueryDatabase(hitPos + new Vector(0, 0, .01f), out _) - d).Normal();
        }
    }
}
