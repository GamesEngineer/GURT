using System;
using System.Drawing;
using System.Numerics;

namespace GURT
{
    class PointLight : ILight
    {
        public Vector3 center;
        public Color color = Color.White;

        public Color Sample(Vector3 point, out Vector3 pointToLight)
        {
            pointToLight = center - point;
            float d2 = Vector3.Dot(pointToLight, pointToLight);
            float lux = 1f / (d2 + 1f); // HACK - prevent divide by zero and attenuate blowout
            float red = Remap255(color.R * lux);
            float green = Remap255(color.G * lux);
            float blue = Remap255(color.B * lux);
            return Color.FromArgb((int)red, (int)green, (int)blue);
        }

        private static float Remap255(float x) => MathF.Max(0f, MathF.Min(x, 1f)) * 255f;
    }
}
