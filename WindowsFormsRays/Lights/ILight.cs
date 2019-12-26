using System;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.Lights
{
    public interface ILight
    {
        void ApplyColor(Vector position, Vector normal, Func<float> rand,
            Func<Vector, Vector, IMaterial> RayMarching,
            float attenuation, ref Vector color);
    }
}
