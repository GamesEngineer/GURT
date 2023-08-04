using System;
using System.Numerics;

namespace GURT
{
    public class Camera
    {
        public Vector3 positionWS;
        public Vector3 viewDirectionWS;
        public float screenDistance;
        public Matrix4x4 viewMatrixW2L; // transforms world space to local space
        public Matrix4x4 viewMatrixL2W; // transforms local space to world space

        public Camera(Vector3 positionWS, Vector3 lookAtTargetWS, float fieldOfViewAngle = 35f)
        {
            this.positionWS = positionWS;
            this.viewDirectionWS = Vector3.Normalize(lookAtTargetWS - positionWS);
            this.viewMatrixW2L = Matrix4x4.CreateLookAt(positionWS, lookAtTargetWS, Vector3.UnitY);
            Matrix4x4.Invert(this.viewMatrixW2L, out this.viewMatrixL2W);
            this.screenDistance = -1f / MathF.Tan(fieldOfViewAngle * MathF.PI / 180f);
        }
    }
}
