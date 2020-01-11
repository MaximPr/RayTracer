using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsRays.Materials;
using WindowsFormsRays.RayMarchings;

namespace WindowsFormsRays.Lights
{
    public class DirectionLight : ILight
    {
        public Vector Color;
        public Vector Direction;
        public IMaterial Material;
        public float randomCoef;

        public void ApplyColor(Vector position, Vector normal, Func<float> rand,
            IRayMarching rayMarching,
            ref Vector colorFilter, ref Vector color)
        {
            float incidence = normal % Direction;
            if (incidence > 0)
            {
                var ldir = (Direction + new Vector(rand(), rand(), rand()) * randomCoef).Normal();
                if (rayMarching.RayMarching(position + normal * .1f, ldir) == Material)
                    color += Color * colorFilter * (incidence * 0.5f);
            }
        }
    }
}
