﻿using System;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class UnionOp : IFigure3D
    {
        public IFigure3D Object1;
        public IFigure3D Object2;

        public float GetDistance(Vector position)
        {
             return Math.Min(Object1.GetDistance(position), Object2.GetDistance(position));
        }
    }
}
