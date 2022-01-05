//#define DBG_IMAGE
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GURT
{
    public class RayTracer
    {
        public List<ISceneObject> sceneObjects = new();
        public List<ILight> lights = new();

        public void RenderImage(Camera camera, Image image)
        {
            float near = 1f / MathF.Tan(camera.fov * MathF.PI / 180f);
            float aspect = (float)image.width / (float)image.height;
            Vector2 size = new(image.width - 1, image.height - 1);

            for (int y = 0; y < image.height; y++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    Vector3 uvw = new(x / size.X - 0.5f, y / size.Y - 0.5f, near);
                    uvw.X *= aspect;
                    Vector3 screenPoint = Vector3.Transform(uvw, camera.lookAtMatrix);
                    var ray = Ray.CreateFromLine(camera.position, screenPoint);
                    Color color = Color.Black;
#if DBG_IMAGE
                    float red = 255.999f * (uvw.X + 0.5f);
                    float green = 255.999f * (uvw.Y + 0.5f);
                    c = Color.FromArgb((int)red, (int)green, 0);
#endif
                    float closestHitDistance = float.PositiveInfinity;
                    foreach (var so in sceneObjects)
                    {
                        if (!so.Hit(ray, out RayHit hit)) continue;
                        if (hit.distance > closestHitDistance) continue;
                        closestHitDistance = hit.distance;
                        color = so.Material.Shade(hit, lights, sceneObjects);
                        // TODO - handle recursive reflections and translucency
                    }
                    image.pixels[y, x] = color;
                }
            }
        }
    }
}
