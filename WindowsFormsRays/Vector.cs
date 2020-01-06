using System;
using System.Collections.Generic;

namespace WindowsFormsRays
{
    public struct Vector
    {
        public float x, y, z;

        public Vector(float v = 0) { x = y = z = v; }
        public Vector(float a, float b, float c = 0) { x = a; y = b; z = c; }

        public static Vector operator -(Vector k) { return new Vector(-k.x, -k.y, -k.z); }
        public static Vector operator +(Vector k, Vector r) { return new Vector(k.x + r.x, k.y + r.y, k.z + r.z); }
        public static Vector operator -(Vector k, Vector r) { return new Vector(k.x - r.x, k.y - r.y, k.z - r.z); }
        public static Vector operator +(Vector k, float r) { return new Vector(k.x + r, k.y + r, k.z + r); }
        public static Vector operator *(Vector k, Vector r) { return new Vector(k.x * r.x, k.y * r.y, k.z * r.z); }
        public static Vector operator *(Vector k, float r) { return new Vector(k.x * r, k.y * r, k.z * r); }
        // dot product
        public static float operator %(Vector k, Vector r) { return k.x * r.x + k.y * r.y + k.z * r.z; }
        // inverse square root
        public Vector Normal() { return this * (1 / (float)Math.Sqrt(this % this)); }

        public double Magnitude() { return Math.Sqrt(this % this); }
    }
}
