using System;
using System.Collections.Generic;

namespace WindowsFormsRays.SceneObjects
{
    public class Letter3D : IObject3D
    {
        private List<IObject3D> objects = new List<IObject3D>();

        public Letter3D(string text)
        {
            // 15 two points lines
            const string letters = "5O5_5W9W5_9_AOEOCOC_A_E_IOQ_I_QOUOY_Y_]OWW[WaOa_aWeWa_e_cWiO"; // R (without curve)

            for (int i = 0; i < letters.Length; i += 4)
            {
                var begin = new Vector(letters[i] - 79, letters[i + 1] - 79, 0.0f) * .5f;
                var end = new Vector(letters[i + 2] - 79, letters[i + 3] - 79, 0.0f) * .5f - begin;
                objects.Add(new LetterLine3D { begin = begin, end = end });
            }

            // Two curves (for P and R in PixaR) with hard-coded locations.
            objects.Add(new LetterCurve3D { center = new Vector(11, -6) });
            objects.Add(new LetterCurve3D { center = new Vector(-11, -6) });
        }

        public HitType HitType => HitType.HIT_LETTER;

        public float GetDistance(Vector position)
        {
            float w = 2.0f;
            if (position.z >= w || position.z <= -w)
                return Utils3D.BoxTest(position, new Vector(-15, -10, -w / 2), new Vector(15, 10, w / 2));
            
            Vector f = position; // Flattened position (z=0)
            f.z = 0;

            float distance = float.MaxValue;
            foreach (var obj in objects)
                distance = Utils3D.min(distance, obj.GetDistance(f));

            return (float)Math.Pow(Utils3D.Pow8(distance) + Utils3D.Pow8(position.z), 0.125f) - 0.5f;
        }
    }

    public class LetterLine3D: IObject3D
    {
        public Vector begin, end;
        public HitType HitType => HitType.HIT_LETTER;

        public float GetDistance(Vector position)
        {
            Vector o = position - (begin + end * Utils3D.min(-Utils3D.min((begin - position) % end / (end % end), 0), 1));
            return (float)Math.Sqrt(o % o);
        }
    }

    public class LetterCurve3D : IObject3D
    {
        public Vector center;
        public HitType HitType => HitType.HIT_LETTER;

        public float GetDistance(Vector position)
        {
            Vector o = position + center;

            if (o.x > 0)
            {
                return (float)Math.Abs(Math.Sqrt(o % o) - 2);
            }
            else
            {
                o.y += o.y > 0 ? -2 : 2;
                return (float)Math.Sqrt(o % o);
            }
        }
    }
}
