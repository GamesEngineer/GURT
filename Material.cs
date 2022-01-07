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

        public static Color Ambient(Vector3 viewDir) => Color.Lerp(GroundColor, SkyColor, MathF.Pow((viewDir.Y + 1f) / 2f, 2f));

        public Color Shade(RayHit hit, Vector3 viewDir, RayTracer tracer)
        {
            Color totalLight = emissionColor;

            Vector3 roughNormal = PerturbDirection(hit.normal, roughness);

            if (baseColor.A < 1f)
            {
                float cosTheta = Vector3.Dot(-roughNormal, viewDir);
                float r = cosTheta > 0f ? 0.8f : 1f / 0.8f; // TODO - get ratio of indices of refacation
                float q = 1f - r * r * (1f - cosTheta * cosTheta);
                if (q >= 0f)
                {
                    Vector3 T = viewDir * r + roughNormal * (cosTheta * r - MathF.Sqrt(q));
                    T = Vector3.Normalize(T);
                    Ray transmissionDir = new() { origin = hit.point, direction = T };
                    // HACK - nudge the ray's origin so that the object won't immediately hit the same surface
                    transmissionDir.NudgeForward();
                    totalLight += tracer.TraceRay(transmissionDir) * (1f - baseColor.A);
                }
            }

            // Ambient light from sky and ground
            Color ambientLight = baseColor * Ambient(hit.normal) * (1f - metallicity) * baseColor.A;

            Color metallic = Color.White * (1f - metallicity) + baseColor * metallicity;

            // Add contributions from each light source
            foreach (var lightSource in tracer.lights)
            {
                Color light = lightSource.Sample(hit.point, tracer.sceneObjects, out Vector3 pointToLight);
                if (light.Value <= 0f) continue;
                Vector3 dirToLight = Vector3.Normalize(pointToLight);
                float diffusion = Vector3.Dot(dirToLight, hit.normal); // Lambertian reflection
                if (diffusion <= 0f) continue; // handle self shadowing
                float specular = Specular(viewDir, dirToLight, roughNormal);
                totalLight += light * baseColor * diffusion * (1f - metallicity) * baseColor.A + light * specular * metallic;
            }

            if (metallicity > 0f)
            {
                Vector3 R = Reflection(roughNormal, viewDir);
                Ray reflectedRay = new() { origin = hit.point, direction = R };
                // HACK - nudge the ray's origin so that the object won't immediately reflect itself
                reflectedRay.NudgeForward();
                Color reflection = tracer.TraceRay(reflectedRay);
                totalLight += baseColor * reflection * baseColor.A;
            }

            return emissionColor + ambientLight + totalLight;
        }

        Random rng = new Random((int)DateTime.Now.Ticks);

        public float Specular(Vector3 V, Vector3 L, Vector3 N)
        {
            if (specularity < 0.01f) return 0f;
            Vector3 R = Reflection(N, L);
            return MathF.Pow(Clamp01(Vector3.Dot(R, Vector3.Normalize(V))), specularity * 100f);
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
