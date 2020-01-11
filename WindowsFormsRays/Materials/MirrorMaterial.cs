using System;

namespace WindowsFormsRays.Materials
{
    public class MirrorMaterial : IMaterial
    {
        public Vector Color = new Vector(0.2f, 0.2f, 0.2f);
        public bool ApplyColor(Vector position, Vector normal, Func<float> rand,
            ref Vector direction, ref Vector origin, ref Vector color, ref Vector colorFilter)
        {
            // Specular bounce on a letter. No color acc.
            direction = direction + normal * (normal % direction * -2);
            origin = position + direction * 0.1f;
            colorFilter *= Color;// Attenuation via distance traveled.

            return true;
        }
    }
}
