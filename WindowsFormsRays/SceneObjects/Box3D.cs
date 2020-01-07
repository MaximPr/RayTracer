using System;

namespace WindowsFormsRays.SceneObjects
{
    public class Box3D : IFigure3D
    {
        public Vector LowerLeft, UpperRight;

        public float GetDistance(Vector position)
        {
            return Utils3D.BoxTest(position, LowerLeft, UpperRight);
        }
    }
}
