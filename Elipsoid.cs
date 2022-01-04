using System;
using System.Drawing;
using System.Numerics;

namespace GURT
{
    public class Elipsoid : ISceneObject
    {
        //public Matrix4x4 transform;
        public Vector3 center;
        public Vector3 radii;
        public Material material;

        public bool Hit(Ray ray, out RayHit hit, out Material material)
        {
            hit = new RayHit();
            material = this.material;
            //bool okay = Matrix4x4.Decompose(transform, out Vector3 radii, out Quaternion rotation, out Vector3 center);
            //if (!okay) throw new InvalidOperationException();

            // Solve quadratic equation to find instersection of ray with elipsoid
            // https://www.iquilezles.org/www/articles/intersectors/intersectors.htm
            Vector3 o = ray.origin / radii;
            Vector3 d = ray.direction / radii;
            float a = Vector3.Dot(d, d);
            float b = Vector3.Dot(o, d);
            float c = Vector3.Dot(o, o);
            float h = b * b - a * (c - 1f);
            if (h < 0.0)
            {
                //no intersection
                return false;
            }

            h = (float)Math.Sqrt(h);
            hit.distance = (-b - h) / a; // TODO - handle ray origin starting inside elipsoid
            hit.point = ray.origin + ray.direction * hit.distance;
            hit.normal = Vector3.Normalize(hit.point - center);

            return true;
        }
    }
}
