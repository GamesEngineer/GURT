using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Color Shade(RayHit hit, List<ILight> lights)
        {
            float alpha = 1f; // TODO handle translucency
            float red = emissionColor.R / 255f;
            float green = emissionColor.G / 255f;
            float blue = emissionColor.B / 255f;

            foreach (var light in lights)
            {
                Color lux = light.Sample(hit.point, out Vector3 pointToLight);
                Vector3 dirToLight = Vector3.Normalize(pointToLight);
                float ldn = Vector3.Dot(dirToLight, hit.normal);
                if (ldn <= 0f) continue; // handle self shadowing
                red += baseColor.R / 255f * lux.R * ldn;
                green += baseColor.G / 255f * lux.G * ldn;
                blue += baseColor.B / 255f * lux.B * ldn;
            }

            red = Remap255(red);
            green = Remap255(green);
            blue = Remap255(blue);

            return Color.FromArgb((int)alpha, (int)red, (int)green, (int)blue);
        }

        private static float Remap255(float x) => MathF.Max(0f, MathF.Min(x, 1f)) * 255f;

    }
}
