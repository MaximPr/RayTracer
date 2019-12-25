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
    }
}
