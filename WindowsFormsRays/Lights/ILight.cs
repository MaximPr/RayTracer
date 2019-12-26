using System;
using WindowsFormsRays.Materials;
using WindowsFormsRays.RayMarchings;

namespace WindowsFormsRays.Lights
{
    public interface ILight
    {
        void ApplyColor(Vector position, Vector normal, Func<float> rand,
            IRayMarching rayMarching,
            float attenuation, ref Vector color);
    }
}
