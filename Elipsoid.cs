using System;
using System.Numerics;

namespace GURT
{
    public class Elipsoid : ISceneObject
    {
        public Matrix4x4 transform;
        public Material material;

        public bool Hit(Ray ray, out RayHit hit, out Material material)
        {
            throw new NotImplementedException();
        }
    }
}
