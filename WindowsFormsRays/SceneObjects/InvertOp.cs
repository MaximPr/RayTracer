using System.Collections.Generic;
using System.Linq;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class InvertOp : IFigure3D
    {
        public List<IFigure3D> Objects = new List<IFigure3D>();

        public float GetDistance(Vector position)
        {
            //-min carves an empty space
            return -Objects.Min(x => x.GetDistance(position));
        }
    }
}
