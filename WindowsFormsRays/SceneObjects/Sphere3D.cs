using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class Sphere3D : IFigure3D
    {
        public Vector Center { get; set; }

        public double Radius { get; set; }

        public float GetDistance(Vector position)
        {
            return (float)((position - Center).Magnitude() - Radius);
        }
    }
}
