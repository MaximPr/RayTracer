using System;

namespace WindowsFormsRays.Materials
{
    public class DifuseMaterial : IMaterial
    {
        public Vector Color = new Vector(1, 1, 1);

        public bool ApplyColor(Vector position, Vector normal, Func<float> rand,
            ref Vector direction, ref Vector origin, ref float attenuation, ref Vector color, ref Vector colorFilter)
        {
            float p = 6.283185f * rand();
            float c = rand();
            float s = (float)Math.Sqrt(1 - c);
            float g = normal.z < 0 ? -1 : 1;
            float u = -1 / (g + normal.z);
            float v = normal.x * normal.y * u;
            direction = new Vector(v,
                            g + normal.y * normal.y * u,
                            -normal.y) * ((float)Math.Cos(p) * s)
                        +
                        new Vector(1 + g * normal.x * normal.x * u,
                            g * v,
                            -g * normal.x) * ((float)Math.Sin(p) * s) + normal * (float)Math.Sqrt(c);
            origin = position + direction * .1f;
            attenuation = attenuation * 0.2f;
            colorFilter *= Color;
            return true;
        }
    }
}
