using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public interface IObject3D
    {
        IMaterial Material { get; }
        float GetDistance(Vector position);
    }
}
