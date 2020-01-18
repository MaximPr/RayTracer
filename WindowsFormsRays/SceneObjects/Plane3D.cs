using System;

namespace WindowsFormsRays.SceneObjects
{
    public class Plane3D : IFigure3D
    {
        public static Plane3D XPlus(double value)
        {
            return new Plane3D(1, 0, 0, -value);
        }

        public static Plane3D XMinus(double value)
        {
            return new Plane3D(-1, 0, 0, value);
        }

        public static Plane3D YPlus(double value)
        {
            return new Plane3D(0, 1, 0, -value);
        }

        public static Plane3D YMinus(double value)
        {
            return new Plane3D(0, -1, 0, value);
        }

        public static Plane3D ZPlus(double value)
        {
            return new Plane3D(0, 0, 1, -value);
        }

        public static Plane3D ZMinus(double value)
        {
            return new Plane3D(0, 0, -1, value);
        }

        private double a, b, c, d;
        public Plane3D(double a, double b, double c, double d)
        {
            double norm = Math.Sqrt(a * a + b * b + c * c);

            this.a = a / norm;
            this.b = b / norm;
            this.c = c / norm;
            this.d = d / norm;
        }

        public float GetDistance(Vector position)
        {
            return (float)(position.x * a + position.y * b + position.z * c + d);
        }
    }
}
