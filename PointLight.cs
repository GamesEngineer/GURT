#define ENABLE_SHADOWS
using System.Collections.Generic;
using System.Numerics;

namespace GURT
{
    public class PointLight : ILight
    {
        public Vector3 center;
        public Color color = Color.White;
        public float intensity = 100f;

        public Color Sample(Vector3 point, List<ISceneObject> sceneObjects, out Vector3 pointToLight)
        {
            pointToLight = center - point;
#if ENABLE_SHADOWS
            // Check for scene objects blocking the light
            var lightRay = new Ray
            {
                origin = point,
                direction = Vector3.Normalize(pointToLight),
            };
            // HACK - nudge the ray's origin so that the object won't immediately shadow itself
            lightRay.MoveOrigin(0.001f);
            foreach (var so in sceneObjects)
            {
                if (so.Hit(lightRay, out RayHit hit))
                {
                    return Color.Black; // Point is shadowed
                }
            }
#endif
            // Compute the amount of light reaching the point
            float d2 = Vector3.Dot(pointToLight, pointToLight);
            float lux = intensity / (d2 + 1f); // HACK - prevent divide by zero and attenuate blowout
            return color * lux;
        }
    }
}
