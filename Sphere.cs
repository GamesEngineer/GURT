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

            // Calculate intersections of the ray with this sphere via the geometric solution (projection).
            // https://www.geogebra.org/m/uxv5kfum
            // This solution can also be interpreted as an optimized version the analytical solution of
            // the quadratic equation that results from substituting the parametric point-on-ray equation
            // into the implicit points-on-sphere equation.
            // https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
            //   point-on-ray:        ray.origin + ray.direction * t;
            //   points-on-sphere:    |point - sphere.position|^2 - radius^2 = 0
            //   substitution and rearangement of terms gives:
            //      |(ray.origin + ray.direction * t) - sphere.position|^2 - radius^2 = 0
            //      |(ray.origin - sphere.position) + ray.direction * t|^2 - radius^2 = 0
            //      |relativePosition + ray.direction * t|^2 - radius^2 = 0
            //      relativePosition^2 + 2 * relativePosition * ray.direction * t + ray.direction^2 * t^2 - radius^2 = 0
            //      ray.direction^2 * t^2 + 2 * relativePosition * ray.direction * t + relativePosition^2 − radius^2 = 0
            //   which is a quadratic equation of the form Ax^2 + Bx + C = 0 where:
            //      A = ray.direction^2
            //      B = 2 * relativePosition * ray.direction
            //      C = relativePosition^2 − radius ^2
            //      x = t
            //   and this can be solved using the quadratic formula:
            //      x0 = (-B + Sqrt(B^2 - 4*A*C)) / 2*A
            //      x1 = (-B - Sqrt(B^2 - 4*A*C)) / 2*A
            // This can be optimized by factoring out constants. For example, A is always 1 because 'ray.direction' is a unit vector.
            // And the 2 in B can be dropped because it cancels out with the Sqrt(4) in the discriminant and the 2 in the divisor.
            // The optimized analytic solution becomes the same as the geometric solution.
            // https://www.iquilezles.org/www/articles/intersectors/intersectors.htm

            Vector3 relativePosition = position - ray.origin; // reversed to avoid negation of 'b' when calculating distance
            ///// a = Vector3.Dot(ray.direction, ray.direction) which is always 1, because 'direction' is a unit vector
            float b = Vector3.Dot(relativePosition, ray.direction); // length of the projection of the relative position onto the ray
            float c = Vector3.Dot(relativePosition, relativePosition) - radius * radius;
            float d = b * b - c; // Optimized quadratic discriminant
            if (d < 0f)
            {
                return false; //no intersection
            }
            d = MathF.Sqrt(d); // 'd' is now the distance from the projection point (sphere center onto ray) to the ray-sphere intersection points
            hit.distance = b - d;
            bool isInside = hit.distance < 0f;
            if (isInside)
            {
                hit.distance = b + d;
                if (hit.distance < 0f) return false; // both intersection points are "behind" the start of the ray
            }
            hit.point = ray.GetPoint(hit.distance);
            hit.normal = Vector3.Normalize(hit.point - position);
            if (isInside) hit.normal = Vector3.Negate(hit.normal);
            hit.sceneObject = this;
            return true;
        }

    }
}
