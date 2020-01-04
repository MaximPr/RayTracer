using System;

namespace WindowsFormsRays.Materials
{
    public interface IMaterial
    {
        bool ApplyColor(Vector position, Vector normal, Func<float> rand,
            ref Vector direction, ref Vector origin, ref float attenuation, ref Vector color, ref Vector colorFilter);
    }
}
