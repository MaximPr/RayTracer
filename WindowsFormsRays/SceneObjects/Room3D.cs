using System;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class Room3D : IObject3D
    {
        public IMaterial Material { get; set; }

        public float GetDistance(Vector position)
        {
            return //-min carves an empty space
                          -Utils3D.min(// Lower room
                               Utils3D.BoxTest(position, new Vector(-30, -.5f, -30), new Vector(30, 18, 30)),
                               // Upper room
                               Utils3D.BoxTest(position, new Vector(-25, 17, -25), new Vector(25, 20, 25))
                          );
        }
    }
}
