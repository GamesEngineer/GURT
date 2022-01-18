using System.Numerics;

namespace GURT
{
    public interface ISceneObject
    {
        Vector3 Position { get; }
        Material Material { get; }
        bool Hit(Ray ray, out RayHit hit);
    }
}
