namespace WindowsFormsRays.SceneObjects
{
    public interface IObject3D
    {
        HitType HitType { get; }
        float GetDistance(Vector position);
    }
}
