using System;

namespace WindowsFormsRays.Materials
{
    public class MirrorMaterial : IMaterial
    {
        public bool ApplyColor(Vector position, Vector normal, Func<float> rand,
            ref Vector direction, ref Vector origin, ref float attenuation, ref Vector color, ref Vector colorFilter)
        {
            // Specular bounce on a letter. No color acc.
            direction = direction + normal * (normal % direction * -2);
            origin = position + direction * 0.1f;
            attenuation = attenuation * 0.2f; // Attenuation via distance traveled.
            return true;
        }
    }
}
