using System.Numerics;

namespace GURT
{
    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public static Ray FromLine(Vector3 start, Vector3 end)
        {
            return new Ray { origin = start, direction = Vector3.Normalize(end - start) };
        }

        public Vector3 GetPoint(float t) => origin + direction * t;
    }
}
