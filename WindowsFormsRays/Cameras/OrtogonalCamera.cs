using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsRays.Cameras
{
    public class OrtogonalCamera : ICamera
    {
        Vector position;
        Vector goal;
        Vector left;
        Vector up;
        float zoom;

        public OrtogonalCamera(Vector position, Vector target)
        {
            zoom = (float)Math.Sqrt((target - position) % (target - position));

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
            return position + left * x* zoom + up * y* zoom;
        }

        public Vector GetDirection(float x, float y)
        {
            return goal;
        }
    }
}
