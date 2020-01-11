using System;

namespace WindowsFormsRays.Materials
{
    public class LightMaterial : IMaterial
    {
        public Vector Color;

        public bool ApplyColor(Vector position, Vector normal, Func<float> rand,
            ref Vector direction, ref Vector origin, ref Vector color, ref Vector colorFilter)
        {
            color += Color * colorFilter;
            return false;
        }
    }
}
