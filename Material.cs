using System;
using System.Numerics;

namespace GURT
{
    public class Material
    {
        public Color baseColor = Color.Gray; // alpha is translucency
        public Color emissionColor = Color.Black; // alpha is strength of emission
        public float refractiveIndex = 1f; // air = 1, water = 1.3333, glass = 1.52, diamond = 2.42
        public float metallicity = 0f; // 0 = dialectric, 1 = metallic
        public float specularity = 0f; // strength of specular reflections
        public float roughness = 0.5f; // microfacet roughness (for both diffuse and specular reflections)

        public static readonly Color SkyColor = Color.Cyan * 0.05f;
        public static readonly Color GroundColor = Color.Yellow * 0.005f;

        public static Color Ambient(Vector3 viewDir) => Color.Lerp(GroundColor, SkyColor, MathF.Pow((viewDir.Y + 1f) / 2f, 2f));

        public Color Shade(RayHit hit, Vector3 viewDir, RayTracer tracer)
        {
            Color totalLight = emissionColor;

            Vector3 roughNormal = PerturbDirection(hit.normal, roughness);

            if (baseColor.A < 1f)
            {
                // Handle Translucency
                float cosTheta = Vector3.Dot(-roughNormal, viewDir);
                float r = cosTheta > 0f ? 1f / refractiveIndex : refractiveIndex; // TODO - get ratio of indices of refacation
                float q = 1f - r * r * (1f - cosTheta * cosTheta);
                if (q >= 0f)
                {
                    Vector3 T = viewDir * r + roughNormal * (cosTheta * r - MathF.Sqrt(q));
                    T = Vector3.Normalize(T);
                    Ray transmissionDir = new() { origin = hit.point, direction = T };
                    // HACK - nudge the ray's origin so that the object won't immediately hit the same surface
                    transmissionDir.NudgeForward();
                    totalLight += tracer.TraceRay(transmissionDir) * baseColor * (1f - baseColor.A);
                }
#if false
                else 
                {
                    // Total internal reflection
                    Vector3 R = Reflection(hit.normal, viewDir);
                    Ray reflectedRay = new() { origin = hit.point, direction = R };
                    // HACK - nudge the ray's origin so that the object won't immediately reflect itself
                    reflectedRay.NudgeForward();
                    totalLight += tracer.TraceRay(reflectedRay) * baseColor * (1f - baseColor.A);
                }
#endif
            }

            // Ambient light from sky and ground
            Color ambientLight = Ambient(hit.normal) * (1f - specularity) * baseColor * baseColor.A;

            // Add contributions from each light source
            foreach (var lightSource in tracer.lights)
            {
                Color light = lightSource.Sample(hit.point, tracer.sceneObjects, out Vector3 pointToLight);
                if (light.Value <= 0f) continue;
                Vector3 dirToLight = Vector3.Normalize(pointToLight);
                float diffusion = Vector3.Dot(dirToLight, hit.normal); // Lambertian reflection
                if (diffusion <= 0f) continue; // handle self shadowing
                totalLight += light * diffusion * (1f - specularity) * baseColor * baseColor.A;
                // HACK - create specular glints for point/spot lights
                if (specularity <= 0f) continue;
                float specularGlint = SpecularGlint(viewDir, dirToLight, roughNormal);
                totalLight += light * specularGlint * Color.Lerp(Color.White, baseColor, metallicity);
            }

            if (specularity > 0f)
            {
                Vector3 R = Reflection(roughNormal, viewDir);
                Ray reflectedRay = new() { origin = hit.point, direction = R };
                // HACK - nudge the ray's origin so that the object won't immediately reflect itself
                reflectedRay.NudgeForward();
                Color reflection = tracer.TraceRay(reflectedRay);
                totalLight += reflection * specularity * Color.Lerp(Color.White, baseColor, metallicity);
            }

            return emissionColor + ambientLight + totalLight;
        }

        Random rng = new Random((int)DateTime.Now.Ticks);

        public float SpecularGlint(Vector3 V, Vector3 L, Vector3 N)
        {
            Vector3 R = Reflection(N, L);
            return MathF.Pow(Clamp01(Vector3.Dot(R, Vector3.Normalize(V))), (1f - roughness) * 1000f);
        }

        private static Vector3 Reflection(Vector3 N, Vector3 V) => Vector3.Normalize(2f * Vector3.Dot(N, -V) * N + V);

        private static float Clamp01(float x) => MathF.Max(0f, MathF.Min(x, 1f));

        private Vector3 PerturbDirection(Vector3 direction, float amount)
        {
            if (amount == 0f) return direction;
            Vector3 phi = Vector3.Normalize(Vector3.Cross(direction, Vector3.UnitY));
            Vector3 psi = Vector3.Normalize(Vector3.Cross(direction, phi));
            float roughnessAngle = rng.Next() / (float)int.MaxValue * MathF.Tau;
            float roughnessMag = rng.Next() / (float)int.MaxValue * amount;
            float sa = MathF.Sin(roughnessAngle);
            float ca = MathF.Cos(roughnessAngle);
            direction += phi * sa * roughnessMag;
            direction += psi * ca * roughnessMag;
            direction = Vector3.Normalize(direction);
            return direction;
        }
    }
}
