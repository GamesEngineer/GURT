#define OPTIMIZE_QUADRATIC_SOLUTION
using System;
using System.Numerics;

namespace GURT
{
    public class Sphere : ISceneObject
    {
        public Vector3 position;
        public float radius;
        public Material material;

        Vector3 ISceneObject.Position => position;
        Material ISceneObject.Material => material;

        bool ISceneObject.Hit(Ray ray, out RayHit hit)
        {
            hit = new RayHit();
            // Solve quadratic equation to find instersection of ray with sphere
#if OPTIMIZE_QUADRATIC_SOLUTION
            // https://www.iquilezles.org/www/articles/intersectors/intersectors.htm
            Vector3 start = ray.origin - position;
            float b = Vector3.Dot(start, ray.direction);
            float c = Vector3.Dot(start, start) - radius * radius;
            float h = b * b - c; // Optimized quadratic discriminent
            if (h < 0f)
            {
                //no intersection
                return false;
            }
            h = MathF.Sqrt(h);
            hit.distance = -b - h;
            bool isInside = hit.distance < 0f;
            if (isInside)
            {
                hit.distance = -b + h;
                if (hit.distance < 0f) return false;
            }
#else
            // https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
            Vector3 start = ray.origin - center;
            float a = Vector3.Dot(ray.direction, ray.direction); // NOTE: should always be 1!
            float b = 2f * Vector3.Dot(start, ray.direction);
            float c = Vector3.Dot(start, start) - radius * radius;
            if (!SolveQuadratic(a, b, c, out float t0, out float t1))
            {
                //no intersection
                return false;
            }
            bool isInside = t0 < 0f;
            if (t0 < 0f)
            {
                t0 = t1;
                if (t0 < 0f) return false;
            }
            hit.distance = t0;
#endif
            hit.point = ray.GetPoint(hit.distance);
            hit.normal = Vector3.Normalize(hit.point - position);
            if (isInside) hit.normal = Vector3.Negate(hit.normal);
            hit.sceneObject = this;
            return true;
        }

#if !OPTIMIZE_QUADRATIC_SOLUTION
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
#endif
    }
}
