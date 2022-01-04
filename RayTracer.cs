using System.Collections.Generic;
using System.Numerics;

namespace GURT
{
    public class RayTracer
    {
        public List<ISceneObject> sceneObjects = new List<ISceneObject>();

        public void RenderImage(Camera camera, Image image)
        {
            const float DEG2RAD = 0.017453286279f;
            const float near = 1f;
            const float far = 1000f;
            float aspect = (float)image.width / (float)image.height;
            // TODO - FIXME!
            Matrix4x4 xform = Matrix4x4.CreatePerspectiveFieldOfView(camera.fov * DEG2RAD, aspect, near, far);
            Vector2 size = new Vector2(image.width - 1, image.height - 1);

            for (int v = 0; v < image.height; v++)
            {
                for (int h = 0; h < image.width; h++)
                {
                    Vector3 uvw = new Vector3(h / size.X - 0.5f, v / size.Y - 0.5f, near);
                    uvw = Vector3.Transform(uvw, xform);
                    Vector3 screenPoint = camera.position + uvw;
                    var ray = Ray.FromLine(camera.position, screenPoint);
                }
            }
        }
    }
}
