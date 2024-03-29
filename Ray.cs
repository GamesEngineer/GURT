﻿using System.Numerics;

namespace GURT
{
    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public static Ray CreateFromLine(Vector3 start, Vector3 end)
        {
            return new Ray { origin = start, direction = Vector3.Normalize(end - start) };
        }

        public static Ray CreateFromLine(Vector3 start, Vector3 end, out float distance)
        {
            Vector3 delta = end - start;
            distance = delta.Length();
            return new Ray { origin = start, direction = delta / distance };
        }

        public Vector3 GetPoint(float t) => origin + direction * t;

        public void MoveOrigin(float amount) => origin += direction * amount;

        public void NudgeForward() => MoveOrigin(0.005f);
    }
}
