using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.Lights
{
    public class DirectionLight : ILight
    {
        public Vector Color;
        public Vector Direction;
        public IMaterial Material;
        public float randomCoef;

        public void ApplyColor(Vector position, Vector normal, Func<float> rand,
            Func<Vector, Vector, IMaterial> RayMarching,
            float attenuation, ref Vector color)
        {
            float incidence = normal % Direction;
            if (incidence > 0)
            {
                var ldir = (Direction + new Vector(rand(), rand(), rand()) * randomCoef).Normal();
                if (RayMarching(position + normal * .1f, ldir) == Material)
                    color += Color * (attenuation * incidence * 0.5f);
            }
        }
    }
}
