using System.Drawing;

namespace GURT
{
    public class Material
    {
        public Color baseColor; // alpha is translucency
        public Color emissionColor; // alpha is strength of emission
        public float indexOfRefraction;
        public float metallicity; // 0 = non-metallic, 1 = metallic
        public float specularity; // specular reflection
        public float roughness; // microfacet roughness (for both diffuse and specular reflections)

        public Material()
        {
            baseColor = Color.Gray;
            emissionColor = Color.Black;
            indexOfRefraction = 1f;
            metallicity = 0f;
            roughness = 0f;
            specularity = 0.8f;
        }
    }
}
