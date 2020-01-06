using System.Collections.Generic;
using System.Linq;
using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class InvertOp : IObject3D
    {
        public IMaterial Material => Objects[0].Material; //TODO_deg подумать

        public List<IObject3D> Objects = new List<IObject3D>();

        public float GetDistance(Vector position)
        {
            //-min carves an empty space
            return -Objects.Min(x => x.GetDistance(position));
        }
    }
}
