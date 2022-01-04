//#define DBG_IMAGE
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;

namespace GURT
{
    public class RayTracer
    {
        public List<ISceneObject> sceneObjects = new List<ISceneObject>();

        public void RenderImage(Camera camera, Image image)
        {
            const float DEG2RAD = 0.017453286279f;
            const float near = 0.5f;
            const float far = 1000f;
            float aspect = (float)image.width / (float)image.height;
            // TODO - FIXME!
            //Vector3 right = Vector3.Transform(Vector3.UnitX, camera.lookAtMatrix);
            //Vector3 up = Vector3.Transform(Vector3.UnitY, camera.lookAtMatrix);
            //Matrix4x4 xform = Matrix4x4.CreatePerspectiveFieldOfView(camera.fov * DEG2RAD, aspect, near, far);
            Vector2 size = new Vector2(image.width - 1, image.height - 1);

            for (int y = 0; y < image.height; y++)
            {
                for (int x = 0; x < image.width; x++)
                {
                    Vector3 uvw = new Vector3(x / size.X - 0.5f, y / size.Y - 0.5f, near);
                    uvw = Vector3.Transform(uvw, camera.lookAtMatrix);
                    Vector3 screenPoint = camera.position + uvw;
                    var ray = Ray.CreateFromLine(camera.position, screenPoint);
                    foreach (var so in sceneObjects)
                    {
                        Color c;
                        if (so.Hit(ray, out RayHit hit, out Material mat))
                        {
                            c = mat.baseColor;
                            // TODO - do lighting and material effects
                        }
                        else
                        {
                            c = Color.Black;
                        }

#if DBG_IMAGE
                        float red = (255.999f * (float)x / (float)image.width);
                        float green = (255.999f * (float)y / (float)image.height);
                        c = Color.FromArgb((int)red, (int)green, 0);
#endif

                        image.pixels[y, x] = c;
                    }
                }
            }
        }
    }
}
