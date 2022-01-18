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
        public float roughness = 1f; // microfacet roughness (affects reflection and transmission)

        public static readonly Color SkyColor = Color.Cyan * 0.125f;
        public static readonly Color GroundColor = Color.Yellow * 0.025f;
        public static Color AmbientLightFromSkyAndGround(Vector3 viewDir) => Color.Lerp(GroundColor, SkyColor, MathF.Pow((viewDir.Y + 1f) / 2f, 2f));

        public Color Shade(RayHit hit, Vector3 viewDir, RayTracer tracer)
        {
            // Use a basic light transport algorithm to determine the color at the hit point from the viewing direction
            Color ambientLight = AmbientLightFromSkyAndGround(hit.normal);
            Color diffuseLight = Color.Black;
            Color specularLight = Color.Black;
            Vector3 roughNormal = RandomlyPerturbDirection(hit.normal, roughness);

            // Transmitted light
            Color transmittedLight = Color.Black;
            if (baseColor.A < 1f) // material must be somewhat translucent in order to transmit light
            {
                float cosTheta = Vector3.Dot(-roughNormal, viewDir);
                float r = cosTheta > 0f ? 1f / refractiveIndex : refractiveIndex; // TODO - get ratio of indices of refacation
                float q = 1f - r * r * (1f - cosTheta * cosTheta);
                if (q >= 0f)
                {
                    Vector3 T = viewDir * r + roughNormal * (cosTheta * r - MathF.Sqrt(q));
                    Ray transmissionDir = new() { origin = hit.point, direction = Vector3.Normalize(T) };
                    // HACK - nudge the ray's origin so that the object won't immediately hit the same surface
                    transmissionDir.NudgeForward();
                    transmittedLight += tracer.TraceRay(transmissionDir);
                }
#if false
                else 
                {
                    // Total internal reflection
                    Vector3 R = Reflection(hit.normal, viewDir);
                    Ray reflectedRay = new() { origin = hit.point, direction = R };
                    // HACK - nudge the ray's origin so that the object won't immediately reflect itself
                    reflectedRay.NudgeForward();
                    transmittedLight += tracer.TraceRay(reflectedRay);
                }
#endif
            }

#if false
            // Collect ambient light from nearby emmisive objects
            foreach (var so in tracer.sceneObjects)
            {
                if (so.Material.emissionColor.A <= 0.001f || hit.sceneObject == so) continue;

                PointLight fakeAmbientLight = new PointLight
                {
                    center = so.Position,
                    color = so.Material.emissionColor,
                    intensity = so.Material.emissionColor.A,
                };

                Color light = fakeAmbientLight.Sample(hit.point, tracer.sceneObjects, so, out Vector3 pointToLight);
                if (light.Value <= 0f) continue;
                Vector3 dirToLight = Vector3.Normalize(pointToLight);
                float diffusion = Vector3.Dot(dirToLight, hit.normal); // Lambertian reflection
                if (diffusion <= 0f) continue; // handle self shadowing
                ambientLight += light * diffusion;
            }
#endif

            // Add contributions from each light source
            foreach (var lightSource in tracer.lights)
            {
                Color light = lightSource.Sample(hit.point, tracer.sceneObjects, ignore: null, out Vector3 pointToLight);
                if (light.Value <= 0f) continue;
                Vector3 dirToLight = Vector3.Normalize(pointToLight);
                float diffusion = Vector3.Dot(dirToLight, hit.normal); // Lambertian reflection
                if (diffusion <= 0f) continue; // handle self shadowing
                diffuseLight += light * diffusion;

                // HACK - create specular glints for point/spot lights
                if (specularity <= 0f) continue;
                float specularGlint = SpecularGlint(viewDir, dirToLight, roughNormal);
                specularLight += light * specularGlint;
            }

            if (specularity > 0f)
            {
                Vector3 R = Reflection(roughNormal, viewDir);
                Ray reflectedRay = new() { origin = hit.point, direction = R };
                // HACK - nudge the ray's origin so that the object won't immediately reflect itself
                reflectedRay.NudgeForward();
                Color reflection = tracer.TraceRay(reflectedRay);
                specularLight += reflection * specularity;
            }

            Color finalColor =
                emissionColor * emissionColor.A +
                (ambientLight + diffuseLight) * (1f - specularity) * baseColor * baseColor.A +
                transmittedLight * baseColor * (1f - baseColor.A) +
                specularLight * Color.Lerp(Color.White, baseColor, metallicity);

            return finalColor;
        }

        Random rng = new Random((int)DateTime.Now.Ticks);

        public float SpecularGlint(Vector3 V, Vector3 L, Vector3 N)
        {
            Vector3 R = Reflection(N, L);
            return MathF.Pow(Clamp01(Vector3.Dot(R, Vector3.Normalize(V))), (1f - roughness) * 1000f);
        }

        private static Vector3 Reflection(Vector3 N, Vector3 V) => Vector3.Normalize(2f * Vector3.Dot(N, -V) * N + V);

        private static float Clamp01(float x) => MathF.Max(0f, MathF.Min(x, 1f));

        private Vector3 RandomlyPerturbDirection(Vector3 direction, float amount)
        {
            if (amount == 0f) return direction;
            Vector3 phi = Vector3.Normalize(Vector3.Cross(direction, Vector3.UnitY)); // TODO - select "best up" vector based on direction
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
