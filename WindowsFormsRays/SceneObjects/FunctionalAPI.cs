using WindowsFormsRays.Materials;

namespace WindowsFormsRays.SceneObjects
{
    public static class FunctionalAPI
    {
        public static Object3D SetMaterial(this IFigure3D figure, IMaterial material)
        {
            return new Object3D(figure, material);
        }

        public static IFigure3D RepeatX(this IFigure3D figure, int period)
        {
            return new RepetitionOp { X = period, Object = figure };
        }

        public static IFigure3D Union(this IFigure3D figure, IFigure3D figure2)
        {
            return new UnionOp { Object1 = figure, Object2 = figure2 };
        }

        public static IFigure3D Substract(this IFigure3D figure, IFigure3D figure2)
        {
            return new SubstractionOp { Object1 = figure, Object2 = figure2 };
        }

        public static IFigure3D Intersection(this IFigure3D figure, IFigure3D figure2)
        {
            return new IntersectionOp { Object1 = figure, Object2 = figure2 };
        }
    }
}
