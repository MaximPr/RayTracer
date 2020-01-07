using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public class Object3D
    {
        public Object3D(IFigure3D figure, IMaterial material)
        {
            this.Figure = figure;
            this.Material = material;
        }

        public IMaterial Material { get; }
        public float GetDistance(Vector position)
        {
            return Figure.GetDistance(position);
        }
        public IFigure3D Figure { get;}
    }
}
