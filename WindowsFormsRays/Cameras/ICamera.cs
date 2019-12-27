namespace WindowsFormsRays.Cameras
{
    public interface ICamera
    {
        Vector GetPosition(float x, float y);
        Vector GetDirection(float x, float y);
    }
}
