using System;

namespace GURT
{
    public struct Color
    {
        public float R; // red
        public float G; // green
        public float B; // blue
        public float A; // alpha

        public Color Clamp01 => new Color
        {
            R = MathF.Max(0f, MathF.Min(R, 1f)),
            G = MathF.Max(0f, MathF.Min(G, 1f)),
            B = MathF.Max(0f, MathF.Min(B, 1f)),
            A = MathF.Max(0f, MathF.Min(A, 1f)),
        };

        public float Value => MathF.Max(R, MathF.Max(G, B));

        // Additive blend
        public static Color operator +(Color x, Color y) => new Color
        {
            R = x.R + y.R,
            G = x.G + y.G,
            B = x.B + y.B,
            A = 1f,
        };
        
        // Multiplicative blend
        public static Color operator *(Color x, Color y) => new Color
        {
            R = x.R * y.R,
            G = x.G * y.G,
            B = x.B * y.B,
            A = 1f,
        };
        
        // Scale
        public static Color operator *(Color x, float y) => new Color
        {
            R = x.R * y,
            G = x.G * y,
            B = x.B * y,
            A = 1f,
        };

        public static Color Lerp(Color x, Color y, float t) => x * (1f - t) + y * t;

        public void ApplyGammaCorrection(float invGamma)
        {
            R = MathF.Pow(R, invGamma);
            G = MathF.Pow(G, invGamma);
            B = MathF.Pow(B, invGamma);
        }

        public static readonly Color White      = new Color { R = 1f, G = 1f, B = 1f, A = 1f };
        public static readonly Color Black      = new Color { R = 0f, G = 0f, B = 0f, A = 1f };
        public static readonly Color Gray       = new Color { R = 0.5f, G = 0.5f, B = 0.5f, A = 1f };
        public static readonly Color Red        = new Color { R = 1f, G = 0f, B = 0f, A = 1f };
        public static readonly Color Green      = new Color { R = 0f, G = 1f, B = 0f, A = 1f };
        public static readonly Color Blue       = new Color { R = 0f, G = 0f, B = 1f, A = 1f };
        public static readonly Color Cyan       = new Color { R = 0f, G = 1f, B = 1f, A = 1f };
        public static readonly Color Yellow     = new Color { R = 1f, G = 1f, B = 0f, A = 1f };
        public static readonly Color Magenta    = new Color { R = 1f, G = 0f, B = 1f, A = 1f };
    }
}
