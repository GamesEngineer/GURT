using System.Numerics;

namespace GURT
{
    public class Camera
    {
        public Vector3 position;
        public Vector3 direction;
        public float fov;
        public Matrix4x4 lookAtMatrix;

        public Camera(Vector3 position, Vector3 lookAtTarget, float fov = 35f)
        {
            this.position = position;
            this.direction = Vector3.Normalize(lookAtTarget - position);
            this.fov = fov;
            this.lookAtMatrix = Matrix4x4.CreateLookAt(position, lookAtTarget, Vector3.UnitY);
        }
    }
}
