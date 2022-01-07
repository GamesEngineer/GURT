using System;
using System.Collections.Generic;
using System.Numerics;

namespace GURT
{
    public class Material
    {
        public Color baseColor = Color.Gray; // alpha is translucency
        public Color emissionColor = Color.Black; // alpha is strength of emission
        public float indexOfRefraction = 1f;
        public float metallicity = 0f; // 0 = non-metallic, 1 = metallic
        public float specularity = 0.8f; // specular reflection
        public float roughness = 0f; // microfacet roughness (for both diffuse and specular reflections)

        public static readonly Color SkyColor = Color.Cyan * 0.05f;
        public static readonly Color GroundColor = Color.Yellow * 0.005f;

        public Color Shade(RayHit hit, List<ILight> lights, List<ISceneObject> sceneObjects, Vector3 viewDir)
        {
            // TODO handle translucency and reflections
            Color totalLight = emissionColor;

            // Ambient light from sky and ground
            Color ambientLight = baseColor * Color.Lerp(GroundColor, SkyColor, MathF.Pow((hit.normal.Y + 1f) / 2f, 2f));

            Color metallic = Color.White * (1f - metallicity) + baseColor * metallicity;
#if true
            // Add contributions from each light source
            foreach (var lightSource in lights)
            {
                Color light = lightSource.Sample(hit.point, sceneObjects, out Vector3 pointToLight);
                if (light.Value <= 0f) continue;
                Vector3 dirToLight = Vector3.Normalize(pointToLight);
                float diffusion = Vector3.Dot(dirToLight, hit.normal); // Lambertian reflection
                if (diffusion <= 0f) continue; // handle self shadowing
                float specular = Specular(viewDir, dirToLight, hit.normal);
                totalLight += light * baseColor * diffusion + light * specular * metallic;
            }
#endif
            return emissionColor + ambientLight + totalLight;
        }

        public float Specular(Vector3 V, Vector3 L, Vector3 N)
        {
            Vector3 R = Vector3.Normalize(2f * Vector3.Dot(N, -L) * N + L);
            return MathF.Pow(Clamp01(Vector3.Dot(R, Vector3.Normalize(V))), specularity * 100f) * 2f;
        }

        private static float Clamp01(float x) => MathF.Max(0f, MathF.Min(x, 1f));
    }
}
