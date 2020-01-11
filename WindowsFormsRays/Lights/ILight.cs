using System;
using WindowsFormsRays.Materials;
using WindowsFormsRays.RayMarchings;

namespace WindowsFormsRays.Lights
{
    public interface ILight
    {
        void ApplyColor(Vector position, Vector normal, Func<float> rand,
            IRayMarching rayMarching,
             ref Vector colorFilter, ref Vector color);
    }
}
