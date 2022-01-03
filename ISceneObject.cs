namespace GURT
{
    public interface ISceneObject
    {
        bool Hit(Ray ray, out RayHit hit, out Material material);
    }
}
