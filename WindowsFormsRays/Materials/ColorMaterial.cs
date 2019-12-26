using System;

namespace WindowsFormsRays.Materials
{
    public class ColorMaterial : IMaterial
    {
        public Vector Color;

        public bool ApplyColor(Vector position, Vector normal, Func<float> rand,
            ref Vector direction, ref Vector origin, ref float attenuation, ref Vector color)
        {
            color += Color * attenuation;
            return false;
        }
    }
}
