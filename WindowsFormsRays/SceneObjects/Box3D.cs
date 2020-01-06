using System;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class Box3D : IObject3D
    {
        public IMaterial Material { get; set; }

        public Vector LowerLeft, UpperRight;

        public float GetDistance(Vector position)
        {
            return Utils3D.BoxTest(position, LowerLeft, UpperRight);
        }
    }
}
