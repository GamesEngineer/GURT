//#define DBG_IMAGE
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;

namespace GURT
{
    public class RayTracer
    {
        public List<ISceneObject> sceneObjects = new List<ISceneObject>();
        public List<ILight> lights = new List<ILight>();

        public void RenderImage(Camera camera, Image image)
        {
            float near = 1f / MathF.Tan(camera.fov * MathF.PI / 180f);
            float aspect = (float)image.width / (float)image.height;
            Vector2 size = new Vector2(image.width - 1, image.height - 1);

            for (int y = 0; y < image.height; y++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    Vector3 uvw = new Vector3(x / size.X - 0.5f, y / size.Y - 0.5f, near);
                    uvw.X *= aspect;
                    Vector3 screenPoint = Vector3.Transform(uvw, camera.lookAtMatrix);
                    var ray = Ray.CreateFromLine(camera.position, screenPoint);
                    foreach (var so in sceneObjects)
                    {
                        Color c;
                        if (so.Hit(ray, out RayHit hit, out Material mat))
                        {
                            c = mat.Shade(hit, lights);
                        }
                        else
                        {
                            c = Color.Black;
                        }

#if DBG_IMAGE
                        float red = 255.999f * (uvw.X + 0.5f);
                        float green = 255.999f * (uvw.Y + 0.5f);
                        c = Color.FromArgb((int)red, (int)green, 0);
#endif

                        image.pixels[y, x] = c;
                    }
                }
            }
        }
    }
}
