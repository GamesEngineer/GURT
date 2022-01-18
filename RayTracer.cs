using System.Collections.Generic;
using System.Numerics;

namespace GURT
{
    public class RayTracer
    {
        public List<ISceneObject> sceneObjects = new();
        public List<ILight> lights = new();
        public enum Quality { Fast = 1, Good = 4, Great = 8 }
        public Quality quality = Quality.Fast;
        public int recursionDepth = 0;

        public void RenderImage(Camera camera, Image outputImage)
        {
            float stepSize = 1f / (float)quality;
            // For each pixel in the output image...
            for (int y = 0; y < outputImage.height; y++)
            {
                for (int x = 0; x < outputImage.width; x++)
                {
                    RenderPixel(x, y, camera, outputImage, stepSize);
                }
            }
        }

        private Color RenderPixel(int x, int y, Camera camera, Image outputImage, float stepSize)
        {
            Color pixelColor = Color.Black;
            float halfStep = stepSize / 2f;
            for (float a = halfStep; a < 1f; a += stepSize)
            {
                for (float b = halfStep; b < 1f; b += stepSize)
                {
                    Vector3 screenPoint = MakePointOnScreen(x + a, y + b, camera, outputImage);
                    var ray = Ray.CreateFromLine(camera.positionWS, screenPoint);
                    Color rayColor = TraceRay(ray);
                    pixelColor += rayColor;
                }
            }
            pixelColor *= stepSize * stepSize;
            outputImage.pixels[y, x] = pixelColor;
            return pixelColor;
        }

        private static Vector3 MakePointOnScreen(float x, float y, Camera camera, Image image)
        {
            Vector2 imageSizeMinusOne = new(image.width - 1, image.height - 1);
            Vector3 screenPoint = new(x / imageSizeMinusOne.X - 0.5f, y / imageSizeMinusOne.Y - 0.5f, camera.screenDistance);
            screenPoint.X *= image.aspectRatio;
            screenPoint = Vector3.Transform(screenPoint, camera.viewMatrixL2W);
            return screenPoint;
        }
        
        public Color TraceRay(Ray ray)
        {
            if (recursionDepth > 8) return Color.Black;
            recursionDepth++;

            RayHit closestHit = null;
            foreach (var so in sceneObjects)
            {
                if (!so.Hit(ray, out RayHit hit)) continue;
                if (closestHit != null && hit.distance > closestHit.distance) continue;
                closestHit = hit;
            }
            Color color = (closestHit != null) ?
                closestHit.sceneObject.Material.Shade(closestHit, ray.direction, this) :
                Material.AmbientLightFromSkyAndGround(ray.direction);

            recursionDepth--;

            return color;
        }
    }
}
