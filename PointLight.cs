﻿#define ENABLE_SHADOWS
using System.Collections.Generic;
using System.Numerics;

namespace GURT
{
    public class PointLight : ILight
    {
        public Vector3 position;
        public Color color = Color.White;
        public float intensity = 100f;

        public Color Sample(Vector3 point, List<ISceneObject> sceneObjects, ISceneObject ignore, out Vector3 pointToLight)
        {
            pointToLight = position - point;
            float transmission = 1f;
#if ENABLE_SHADOWS
            // Check for scene objects blocking the light
            var lightRay = new Ray
            {
                origin = point,
                direction = Vector3.Normalize(pointToLight),
            };
            // HACK - nudge the ray's origin so that the object won't immediately shadow itself
            lightRay.NudgeForward();
            foreach (var so in sceneObjects)
            {
                if (so == ignore) continue;
                if (so.Hit(lightRay, out RayHit hit))
                {
                    transmission *= (1f - hit.sceneObject.Material.baseColor.A);
                    if (transmission < 0.001f)
                    {
                        return Color.Black; // Point is completely shadowed
                    }
                }
            }
#endif
            // Compute the amount of light reaching the point
            float distanceSquared = Vector3.Dot(pointToLight, pointToLight);
            distanceSquared += 1f;// HACK - prevent divide by zero and attenuate blowout
            float lux = intensity / distanceSquared;
            return color * lux * transmission;
        }
    }
}
