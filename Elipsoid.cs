#define ENABLE_SPHERES_ONLY
using System;
using System.Numerics;

namespace GURT
{
    public class Elipsoid : ISceneObject
    {
        public Vector3 center;
        public Vector3 radii;
        public Material material;

        public Material Material => material;

        public bool Hit(Ray ray, out RayHit hit)
        {
            hit = new RayHit();
            // Solve quadratic equation to find instersection of ray with elipsoid
#if true
            // https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
            Vector3 start = ray.origin - center;
            float a = Vector3.Dot(ray.direction, ray.direction); // NOTE: should always be 1!
            float b = 2f * Vector3.Dot(start, ray.direction);
            float c = Vector3.Dot(start, start) - radii.X * radii.X;
            if (!SolveQuadratic(a, b, c, out float t0, out float t1))
            {
                //no intersection
                return false;
            }
            //if (t0 < 0f)
            //{
            //    t0 = t1;
            //    if (t0 < 0f) return false;
            //}
            hit.distance = t0;
            hit.point = ray.GetPoint(hit.distance);
            hit.normal = Vector3.Normalize(hit.point - center);

#else
            // https://www.iquilezles.org/www/articles/intersectors/intersectors.htm
#if ENABLE_SPHERES_ONLY
            Vector3 start = ray.origin - center;
            float b = Vector3.Dot(start, ray.direction);
            float c = Vector3.Dot(start, start) - radii.X * radii.X;
            float h = b * b - c;
            if (h < 0f)
            {
                //no intersection
                return false;
            }
            h = MathF.Sqrt(h);
            hit.distance = -b - h;
            //if (hit.distance < 0f)
            //{
            //    //no intersection
            //    return false;
            //}
#else
            Vector3 start = (ray.origin - center) / radii;
            Vector3 dir = ray.direction / radii;
            float a = Vector3.Dot(dir, dir);
            float b = Vector3.Dot(start, dir);
            float c = Vector3.Dot(start, start);
            float h = b * b - a * (c - 1f);
            if (h < 0f)
            {
                //no intersection
                return false;
            }

            h = MathF.Sqrt(h);
            hit.distance = (-b - h) / a; // TODO - handle ray origin starting inside elipsoid
#endif
            hit.point = ray.origin + ray.direction * hit.distance;
            hit.normal = Vector3.Normalize(hit.point - center);
#endif
            return true;
        }

        public static bool SolveQuadratic(float a, float b, float c, out float x0, out float x1)
        { 
            float discr = b * b - 4f * a * c;
            if (discr < 0f)
            {
                x0 = float.NegativeInfinity;
                x1 = float.PositiveInfinity;
                return false;
            }
            else if (discr == 0f)
            {
                x0 = -0.5f * b / a;
                x1 = x0;
            }
            else
            {
                float q = -0.5f * (b + MathF.Sign(b) * MathF.Sqrt(discr));
                x0 = q / a;
                x1 = c / q;
            }

            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
            }
 
            return true;
        } 
    }
}
