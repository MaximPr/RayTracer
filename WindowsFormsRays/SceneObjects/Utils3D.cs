namespace WindowsFormsRays.SceneObjects
{
    public static class Utils3D
    {
        public static float Pow8(float d)
        {
            var d2 = d * d;
            var d4 = d2 * d2;
            return d4 * d4;
        }

        public static float min(float l, float r) { return l < r ? l : r; }

        // Rectangle CSG equation. Returns minimum signed distance from 
        // space carved by lowerLeft vertex and opposite rectangle
        // vertex upperRight.
        public static float BoxTest(Vector position, Vector lowerLeft, Vector upperRight)
        {
            lowerLeft = position - lowerLeft;
            upperRight = upperRight - position;
            return -min(
              min(
                min(lowerLeft.x, upperRight.x),
                min(lowerLeft.y, upperRight.y)
              ),
              min(lowerLeft.z, upperRight.z));
        }
    }
}
