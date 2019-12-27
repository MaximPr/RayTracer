using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsRays.Cameras
{
    public class PerspectiveCamera : ICamera
    {
        Vector position;
        Vector goal;
        Vector left;
        Vector up;

        public PerspectiveCamera(Vector position, Vector target)
        {
            this.position = position;
            goal = (target - position).Normal();
            left = -(new Vector(goal.z, 0, -goal.x)).Normal();
            // Cross-product to get the up vector
            up = (new Vector(goal.y * left.z - goal.z * left.y,
                   goal.z * left.x - goal.x * left.z,
                   goal.x * left.y - goal.y * left.x));
        }

        public Vector GetPosition(float x, float y)
        {
            return position;
        }

        public Vector GetDirection(float x, float y)
        {
            return (goal + left * x + up * y).Normal();
        }
    }
}
