using System;
using System.Collections.Generic;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class Text3D : IFigure3D
    {
        private Vector boxMin, boxMax;
        private List<Letter3D> objects = new List<Letter3D>();

        //public IMaterial Material { get; set; }

        private float sizeBetween = 2;

        public Text3D(string text)
        {
            float size = 0;
            foreach (var c in text)
            {
                var letter = new Letter3D(c, size);
                size += letter.size + sizeBetween;
                objects.Add(letter);
            }

            size = (size - sizeBetween) / 2;
            foreach (var letter in objects)
                letter.posX -= size;

            boxMin = new Vector(-size, 0, -1f);
            boxMax = new Vector(size, 8, 1f);
        }

        public float GetDistance(Vector position)
        {
            var boxDist = Utils3D.BoxTest(position, boxMin, boxMax);
            if (boxDist > 1f)
                return boxDist;

            Vector f = position; // Flattened position (z=0)
            f.z = 0;

            float distance = float.MaxValue;
            foreach (var obj in objects)
                distance = Utils3D.min(distance, obj.GetDistance(f));

            return (float)Math.Pow(Utils3D.Pow8(distance) + Utils3D.Pow8(position.z), 0.125f) - 0.5f;
        }
    }

    public class Letter3D : IFigure3D
    {
        public float size;
        public float posX;

        private Vector boxMin, boxMax;
        private List<IFigure3D> objects = new List<IFigure3D>();

        public Letter3D(char c, float posX)
        {
            this.posX = posX;

            if (c == 'A')
            {
                size = 4;
                objects.Add(new LetterLine3D { begin = new Vector(0, 0), end = new Vector(2, 8) });
                objects.Add(new LetterLine3D { begin = new Vector(2, 8), end = new Vector(2, -8) });
                objects.Add(new LetterLine3D { begin = new Vector(1, 4), end = new Vector(2, 0) });
            }
            if (c == 'B')
            {
                size = 4;
                objects.Add(new LetterLine3D { begin = new Vector(0, 0), end = new Vector(0, 8) });
                objects.Add(new LetterLine3D { begin = new Vector(0, 0), end = new Vector(2, 0) });
                objects.Add(new LetterLine3D { begin = new Vector(0, 4), end = new Vector(2, 0) });
                objects.Add(new LetterLine3D { begin = new Vector(0, 8), end = new Vector(2, 0) });
                objects.Add(new LetterCurve3D { center = new Vector(2, 6), r = 2 });
                objects.Add(new LetterCurve3D { center = new Vector(2, 2), r = 2 });
            }
            if (c == 'I')
            {
                size = 2;
                objects.Add(new LetterLine3D { begin = new Vector(0, 0), end = new Vector(2, 0) });
                objects.Add(new LetterLine3D { begin = new Vector(1, 0), end = new Vector(0, 8) });
                objects.Add(new LetterLine3D { begin = new Vector(0, 8), end = new Vector(2, 0) });
            }
            if (c == 'P')
            {
                size = 4;
                objects.Add(new LetterLine3D { begin = new Vector(0, 0), end = new Vector(0, 8) });
                objects.Add(new LetterLine3D { begin = new Vector(0, 4), end = new Vector(2, 0) });
                objects.Add(new LetterLine3D { begin = new Vector(0, 8), end = new Vector(2, 0) });
                objects.Add(new LetterCurve3D { center = new Vector(2, 6), r = 2 });
            }
            if (c == 'R')
            {
                size = 4;
                objects.Add(new LetterLine3D { begin = new Vector(0, 0), end = new Vector(0, 8) });
                objects.Add(new LetterLine3D { begin = new Vector(0, 4), end = new Vector(2, 0) });
                objects.Add(new LetterLine3D { begin = new Vector(0, 8), end = new Vector(2, 0) });
                objects.Add(new LetterLine3D { begin = new Vector(1, 4), end = new Vector(3, -4) });
                objects.Add(new LetterCurve3D { center = new Vector(2, 6), r = 2 });
            }
            if (c == 'X')
            {
                size = 4;
                objects.Add(new LetterLine3D { begin = new Vector(0, 0), end = new Vector(4, 8) });
                objects.Add(new LetterLine3D { begin = new Vector(0, 8), end = new Vector(4, -8) });
            }

            boxMin = new Vector(0, 0, -1f);
            boxMax = new Vector(size, 8, 1f);
        }

        //public IMaterial Material { get; set; }

        public float GetDistance(Vector position)
        {
            position.x -= this.posX;

            var boxDist = Utils3D.BoxTest(position, boxMin, boxMax);
            if (boxDist > 1f)
                return boxDist;

            Vector f = position; // Flattened position (z=0)
            f.z = 0;

            float distance = float.MaxValue;
            foreach (var obj in objects)
                distance = Utils3D.min(distance, obj.GetDistance(f));

            return distance;
        }
    }

    public class LetterLine3D : IFigure3D
    {
        public Vector begin, end;
        public IMaterial Material { get; set; }

        public float GetDistance(Vector position)
        {
            Vector o = position - (begin + end * Utils3D.min(-Utils3D.min((begin - position) % end / (end % end), 0), 1));
            return (float)Math.Sqrt(o % o);
        }
    }

    public class LetterCurve3D : IFigure3D
    {
        public float r = 2;
        public Vector center;
        public IMaterial Material { get; set; }

        public float GetDistance(Vector position)
        {
            Vector o = position - center;

            if (o.x > 0)
            {
                return (float)Math.Abs(Math.Sqrt(o % o) - r);
            }
            else
            {
                o.y += o.y > 0 ? -r : r;
                return (float)Math.Sqrt(o % o);
            }
        }
    }
}
