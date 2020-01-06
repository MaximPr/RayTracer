using System;
using System.Collections.Generic;
using WindowsFormsRays.Lights;
using WindowsFormsRays.Materials;
using WindowsFormsRays.SceneObjects;

namespace WindowsFormsRays
{
    public class SceneData
    {
        public List<ILight> lights = new List<ILight>();
        private List<IObject3D> objects = new List<IObject3D>();

        public SceneData()
        {
            IMaterial lightMaterial = new LightMaterial { Color = new Vector(50, 80, 100) }; //небо
            lights.Add(new DirectionLight
            {
                Color = new Vector(500, 400, 100),
                Direction = new Vector(.6f, .6f, 1).Normal(),
                Material = lightMaterial,
                randomCoef = 0.2f
            });

            //objects.Add(new Sphere() {
            //    Material = new MirrorMaterial() { Color = new Vector(1, 1, 1) },
            //    Radius = 5, 
            //    Center = new Vector(0, 5, 0) 
            //});

            //objects.Add(new Sphere() {
            //    Material = new MirrorMaterial() { Color = new Vector(1f, 0.1f, 0.1f) }, 
            //    Radius = 2, 
            //    Center = new Vector(-10, 3, 0) 
            //});
            objects.Add(new Text3D("PIXAR") { Material = new MirrorMaterial() });
            //Room
            var roomMaterial = new DifuseMaterial() { Color = new Vector(1f, 1f, 1f) };
            objects.Add(new InvertOp() {
                Objects = new List<IObject3D> {
                    new Box3D { Material = roomMaterial, LowerLeft = new Vector(-30, -.5f, -30), UpperRight = new Vector(30, 18, 30) }, //LowerRoom
                    new Box3D { Material = roomMaterial, LowerLeft = new Vector(-25, 17, -25), UpperRight = new Vector(25, 20, 25) }, //UpperRoom
                }
            });
            objects.Add(new RepetitionOp
            {
                X = 8, // Ceiling "planks" spaced 8 units apart.
                Object = new Box3D() { Material = roomMaterial, LowerLeft = new Vector(1.5f, 18.5f, -25), UpperRight = new Vector(6.5f, 20, 25) }
            });
            objects.Add(new Sun3D() { Material = lightMaterial });
        }

        // Sample the world using Signed Distance Fields.
        public float QueryDatabase(Vector position, out IObject3D hitObj)
        {
            hitObj = null;
            float distance = 1e9f;
            foreach (var obj in objects)
            {
                var dist = obj.GetDistance(position);
                if (distance > dist)
                {
                    distance = dist;
                    hitObj = obj;
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

        public Vector QueryDatabaseNorm(Vector hitPos, float d, IObject3D obj)
        {
            return new Vector(obj.GetDistance(hitPos + new Vector(.01f, 0)) - d,
                obj.GetDistance(hitPos + new Vector(0, .01f)) - d,
                obj.GetDistance(hitPos + new Vector(0, 0, .01f)) - d).Normal();
        }
    }
}
