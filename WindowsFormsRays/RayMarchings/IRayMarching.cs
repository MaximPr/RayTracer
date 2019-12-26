using WindowsFormsRays.Materials;

namespace WindowsFormsRays.RayMarchings
{
    public interface IRayMarching
    {
        IMaterial RayMarching(Vector origin, Vector direction);
        IMaterial RayMarching(Vector origin, Vector direction, out Vector hitPos, out Vector hitNorm);
    }
}
