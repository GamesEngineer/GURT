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
            material = new Material();
            //bool okay = Matrix4x4.Decompose(transform, out Vector3 radii, out Quaternion rotation, out Vector3 center);
            //if (!okay) throw new InvalidOperationException();

            // Solve quadratic equation to find instersection of ray with elipsoid
            // https://www.iquilezles.org/www/articles/intersectors/intersectors.htm
            Vector3 ocn = ray.origin / radii;
            Vector3 rdn = ray.direction / radii;
            float a = Vector3.Dot(rdn, rdn);
            float b = Vector3.Dot(ocn, rdn);
            float c = Vector3.Dot(ocn, ocn);
            float h = b * b - a * (c - 1f);
            if (h < 0.0)
            {
                //no intersection
                return false;
            }

            h = (float)Math.Sqrt(h);
            hit.distance = (-b - h) / a;
            hit.point = ray.origin + ray.direction * hit.distance;
            hit.normal = Vector3.Normalize(hit.point - center);
            material.baseColor = Color.Gray;
            material.emissionColor = Color.Black;
            material.indexOfRefraction = 1f;
            material.metallicity = 0f;
            material.roughness = 0f;
            material.specularity = 0.8f;

            return true;
        }
    }
}
