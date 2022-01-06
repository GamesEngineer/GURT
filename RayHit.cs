using System.Numerics;

namespace GURT
{
    public class RayHit
    {
        public Vector3 point = Vector3.Zero;
        public Vector3 normal = Vector3.UnitX;
        public float distance = float.PositiveInfinity;
        public ISceneObject sceneObject = null;
    }
}
