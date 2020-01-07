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

        public static IFigure3D Substract(this IFigure3D figure, IFigure3D figure2)
        {
            return new SubstractionOp { Object2 = figure, Object1 = figure2 };
        }
    }
}
