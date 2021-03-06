﻿using System;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class RepetitionOp : IFigure3D
    {
        public int? X, Y, Z;
        public IFigure3D Object;

        public float GetDistance(Vector position)
        {
            if (X.HasValue)
                position.x = Math.Abs(position.x) % X.Value;
            if (Y.HasValue)
                position.y = Math.Abs(position.y) % Y.Value;
            if (Z.HasValue)
                position.z = Math.Abs(position.z) % Z.Value;
            return Object.GetDistance(position);
        }
    }
}
