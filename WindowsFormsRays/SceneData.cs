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
        private List<Object3D> objects = new List<Object3D>();

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
            var roomMaterial = new DifuseMaterial() { Color = new Vector(0.2f, 0.2f, 0.2f) };
            //objects.Add(
            //    new Box3D { LowerLeft = new Vector(-14f, 1f, -4f), UpperRight = new Vector(-6f, 9f, 4f) }
            //    .Substract( new Sphere3D()
            //        {
            //            Radius = 5,
            //            Center = new Vector(-10, 5, 0)
            //        })
            //    .SetMaterial(roomMaterial)//new MirrorMaterial() { Color = new Vector(1, 1, 1) })
            //);

            //objects.Add(new Sphere3D()
            //{
            //    Radius = 3,
            //    Center = new Vector(-10, 5, 0)
            //}.SetMaterial(new MirrorMaterial() { Color = new Vector(1f, 0.1f, 0.1f) }));

            objects.Add(new Text3D("PIXAR").SetMaterial(new MirrorMaterial()));
            //Room
            objects.Add(new InvertOp()
            {
                Objects = new List<IFigure3D> {
                    new Box3D {LowerLeft = new Vector(-30, -.5f, -30), UpperRight = new Vector(30, 18, 30) }, //LowerRoom
                    new Box3D {LowerLeft = new Vector(-25, 17, -25), UpperRight = new Vector(25, 20, 25) }, //UpperRoom
                }
            }.SetMaterial(roomMaterial));
            objects.Add(new Box3D() { LowerLeft = new Vector(1.5f, 18.5f, -25), UpperRight = new Vector(6.5f, 20, 25) }
                .RepeatX(8)
                .SetMaterial(roomMaterial));
            objects.Add(new Sun3D().SetMaterial(lightMaterial));
        }

        // Sample the world using Signed Distance Fields.
        public float QueryDatabase(Vector position, out Object3D hitObj)
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

        public Vector QueryDatabaseNorm(Vector hitPos, float d, Object3D obj)
        {
            return new Vector(obj.GetDistance(hitPos + new Vector(.01f, 0)) - d,
                obj.GetDistance(hitPos + new Vector(0, .01f)) - d,
                obj.GetDistance(hitPos + new Vector(0, 0, .01f)) - d).Normal();
        }
    }
}
