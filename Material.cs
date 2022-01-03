using System.Drawing;

namespace GURT
{
    public struct Material
    {
        public Color baseColor; // alpha is translucency
        public Color emissionColor; // alpha is strength of emission
        public float indexOfRefraction;
        public float metallicity; // 0 = non-metallic, 1 = metallic
        public float specularity; // specular reflection
        public float roughness; // microfacet roughness (for both diffuse and specular reflections)
    }
}
