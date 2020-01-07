using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class Sun3D : IFigure3D
    {
        public float GetDistance(Vector position)
        {
            return 19.9f - position.y; // Everything above 19.9 is light source.
        }
    }
}
